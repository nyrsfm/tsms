using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Tsms
{
    // public delegate void SmsThreadCallbackDelegate(string to, string body);

    [Service(Exported = true, Name = "com.xamarin.example.SMSReceiverService")]
    public class SMSReceiverService : Service
    {
        ExtBinder binder = new ExtBinder();
        IntentFilter intentFilter;
        SMSBroadcastReceiver smsReceiver;
        ExtSQLiteHelper helper = null;

        public override IBinder OnBind(Intent intent)
        {
            return binder;
        }
        public override void OnCreate()
        {
            base.OnCreate();
            #region  --注册短信接收服务--
            intentFilter = new IntentFilter();
            intentFilter.AddAction("android.provider.Telephony.SMS_RECEIVED");
            smsReceiver = new SMSBroadcastReceiver();
            smsReceiver.MessageCallbackEvent += SmsReceiver_MessageCallbackEvent;

            RegisterReceiver(smsReceiver, intentFilter);
            #endregion

            #region
            helper = new ExtSQLiteHelper(this);
            #endregion
        }

        private void SmsReceiver_MessageCallbackEvent(String number, String body)
        {
            if (helper == null)
                return;
            var config = helper.GetConfig();
            if (config.Count == 0)
                return;
            helper.AddAppLog("[event]:" + (number + "->" + body));

            #region
            try
            {
                #region --代发--
                var controlphone = config["controlphone"] ?? "";//该手机号不转发
                if (!String.IsNullOrEmpty(controlphone) && controlphone.Length>7 && number.Trim('+').EndsWith(controlphone.Trim('+')))
                {
                    //if (body.StartsWith("#") && body.EndsWith("#"))
                    //{//指令区 
                    var cmd = body.Trim().Trim('#');
                    var param = cmd.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                    if (param.Length > 1)
                    {
                        SMSHelper.SendPhone(param[0].Trim(), param[1].Trim());
                        helper.UpdateSendStatistics("sendsuccess");
                        helper.AddAppLog("[controlcomplete]:" + body);
                    }
                    else
                    {
                        helper.AddAppLog("[controlerror]接收控制指令:" + cmd);
                    }
                    //}
                    return;
                }
                #endregion

                #region --转发--

                var contactList = helper.GetContact();
                var content = config["messagetemplate"].Replace("#from#", number).Replace("#body#", body);
                foreach (var contact in contactList)
                {
                    if (contact.Contains("@"))
                    {
                        #region email
                        EmailHelper.Init(config["smtpserver"], config["smtpaccount"], config["smtppassword"], config["smtpaccount"], Convert.ToInt32(config["smtpport"] ?? "0"));
                        var thread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                        {
                            try
                            {
                                EmailHelper.Send(string.Format("sms {0}", number), contact, content, null, false);

                                helper.UpdateSendStatistics("sendsuccess");
                                helper.AddAppLog("[emailcomplete]:" + (contact + "->" + content));

                            }
                            catch (Exception ex)
                            {
                                helper.UpdateSendStatistics("sendfailure");
                                helper.AddAppLog("[emailerror]:" + (contact + "->" + content + "->" + ex.Message));
                            }
                        }));
                        thread.Name = "sendmessagethread";
                        thread.IsBackground = true;
                        thread.Start();
                        #endregion
                    }
                    else
                    {
                        SMSHelper.SendPhone(contact, content);
                        helper.UpdateSendStatistics("sendsuccess");
                        helper.AddAppLog("[smscomplete]:" + (contact + "->" + content));
                    }
                    System.Threading.Thread.Sleep(100);
                }
                #endregion

            }
            catch (Exception ex)
            {
                helper.UpdateSendStatistics("sendfailure");
                helper.AddAppLog("[error]发送失败:" + ex.Message + "(" + body + ")");
            }
            #endregion
        }


        private void SendMessage(string to, string body)
        {

            SMSHelper.SendPhone(to, body);
            helper.UpdateSendStatistics("sendsuccess");
        }
    }
    public class ExtBinder : Binder
    {
        public String Test1()
        {
            return "当前北京时间:" + DateTime.Now.ToString();
        }
        public void Test2()
        {
        }
    }

    public class ServiceConnection : IServiceConnection
    {//用于建立与service进行联系的绑定
        #region --IServiceConnection接口--

        IntPtr IJavaObject.Handle
        {
            get
            {
                if (handle != null)
                    return handle.Handle;
                else
                    return (IntPtr)0;
            }
        }
        public Handler handle;
        public void OnServiceConnected(ComponentName name, IBinder service)
        {//活动与服务成功绑定时执行
            //var eb = (ExtBinder)service;
            if (ServiceConnectedEvent != null)
                ServiceConnectedEvent(service);
        }

        public void OnServiceDisconnected(ComponentName name)
        {//活动与服务解除绑定时执行

        }
        public void Dispose()
        {

        }

        #endregion

        //自定义一个事件与action交互
        public event Action<IBinder> ServiceConnectedEvent;
    }
}