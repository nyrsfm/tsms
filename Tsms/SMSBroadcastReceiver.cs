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
    /// <summary>
    /// 短信接收类
    /// </summary>
    public class SMSBroadcastReceiver : BroadcastReceiver
    {
        //private string SMS_RECEIVED_ACTION = "android.provider.Telephony.SMS_RECEIVED";
        public event Action<String, String> MessageCallbackEvent;
        public override void OnReceive(Context context, Intent intent)
        {
            if (!intent.HasExtra("pdus"))
            {
                return;
            }

            //pdus短信单位pdu
            //解析短信内容
            var pdus = (Java.Lang.Object[])intent.Extras.Get("pdus");
            String format = intent.GetStringExtra("format");
            foreach (var item in pdus)
            {
                //封装短信参数的对象 
                Android.Telephony.SmsMessage sms;
                if (Build.VERSION.SdkInt >= Build.VERSION_CODES.M)
                {
                    sms = Android.Telephony.SmsMessage.CreateFromPdu((byte[])item, format);
                }
                else
                {
                    sms = Android.Telephony.SmsMessage.CreateFromPdu((byte[])item);
                }
                String number = sms.OriginatingAddress;
                String body = sms.MessageBody;
                if (MessageCallbackEvent != null)
                {
                    MessageCallbackEvent(number, body);
                }
            }
            //if (intent.Action.Equals(SMS_RECEIVED_ACTION))
            //{
            //} 
        }
    }
    public class SMSHelper
    {
        public static void SendPhone(String to, String body)
        {
            try
            {
                SendSms1(to, body);
            }
            catch {
                SendSms2(to, body);
            }


        }
        public static void SendSms1(String to, String body)
        {
            Android.Telephony.SmsManager smsManager = Android.Telephony.SmsManager.Default;
            smsManager.SendTextMessage(to, null, body, null, null);
            //Intent sendIntent = new Intent(Intent.ActionSendto,Android.Net.Uri.Parse("smsto:" +to));
            //sendIntent.PutExtra("sms_body", body);
            //StartActivity(sendIntent);
        }
        public static void SendSms2(String to, String body)
        {
            //获取短信管理器   
            Android.Telephony.SmsManager smsManager = Android.Telephony.SmsManager.Default;
            //拆分短信内容（手机短信长度限制）    
            IList<String> divideContents = smsManager.DivideMessage(body);
            foreach (var msg in divideContents)
            {
                smsManager.SendTextMessage(to, null, msg, null, null);//android.permission.SEND_SMS
            } 
        }
    }
}