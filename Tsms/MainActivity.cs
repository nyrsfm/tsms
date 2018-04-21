using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;

namespace Tsms
{
    [Activity(Label = "Tsms", MainLauncher = false, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //var intent = new Intent(this,typeof(SMSContentProviderActivity));
            //StartActivity(intent);

            //var intent2 = new Intent(this, typeof(BroadcastReceiverActivity));
            //StartActivity(intent2);
            SetContentView(Resource.Layout.Main);
            Title = "短信转发";
            #region
            var helper = new ExtSQLiteHelper(this);
            var dic = helper.GetSendStatus();
            FindViewById<TextView>(Resource.Id.text_sendsuccessstatistics).Text = string.Format("发送成功{0}条", dic.ContainsKey("sendsuccess") ? dic["sendsuccess"] : "0");
            FindViewById<TextView>(Resource.Id.text_sendfailurestatistics).Text = string.Format("发送失败{0}条", dic.ContainsKey("sendfailure") ? dic["sendfailure"] : "0");
            var startbtn = FindViewById<Button>(Resource.Id.start_service_button);
            var endbtn = FindViewById<Button>(Resource.Id.stop_service_button);
           
            if (isServiceRunning(Android.OS.Process.MyPid()))
            {
                startbtn.Visibility = Android.Views.ViewStates.Gone;
            }
            else
            { 
                endbtn.Visibility = Android.Views.ViewStates.Gone;
            }
            #endregion

            #region
            startbtn.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(SMSReceiverService));
                StartService(intent);//启动服务
                helper.UpdateSendStatus(1);
                Toast.MakeText(this, "后台服务已启动", ToastLength.Short).Show();

                startbtn.Visibility = Android.Views.ViewStates.Gone;
                endbtn.Visibility = Android.Views.ViewStates.Visible;
            };
            endbtn.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(SMSReceiverService));
                StopService(intent);//停止服务
                helper.UpdateSendStatus(0);
                Toast.MakeText(this, "后台服务已停止", ToastLength.Short).Show();

                endbtn.Visibility = Android.Views.ViewStates.Gone;
                startbtn.Visibility = Android.Views.ViewStates.Visible;
            };

            FindViewById<Button>(Resource.Id.edit_smscontact_button).Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(SMSTransferContactActivity));
                StartActivity(intent);
            };
            FindViewById<Button>(Resource.Id.edit_smsconfig_button).Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(SMSTransferConfigActivity));
                StartActivity(intent);
            };
            FindViewById<Button>(Resource.Id.view_applog_button).Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(AppLogActivity));
                StartActivity(intent);
            };
            #endregion
        }

        private bool isServiceRunning(int pid)
        {

            ActivityManager manager = (ActivityManager)GetSystemService(Service.ActivityService);
            System.Collections.Generic.IList<ActivityManager.RunningServiceInfo> runningProcess = manager.GetRunningServices(int.MaxValue);
            if (runningProcess == null || runningProcess.Count == 0)
            {
                return false;
            }

            foreach (ActivityManager.RunningServiceInfo processInfo in runningProcess)
            {
                //processInfo.Process
                if (processInfo.Pid == pid)//IMPORTANCE_FOREGROUND
                {
                    return true;
                }
            }
            return false;
        }
    }
}

