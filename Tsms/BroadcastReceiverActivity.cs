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
    [Activity(Label = "BroadcastReceiverActivity")]
    public class BroadcastReceiverActivity : Activity
    {
        IntentFilter intentFilter;
        SMSBroadcastReceiver smsReceiver;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            intentFilter = new IntentFilter();
            intentFilter.AddAction("android.provider.Telephony.SMS_RECEIVED");
            smsReceiver = new SMSBroadcastReceiver();

            RegisterReceiver(smsReceiver, intentFilter);
        }

        protected override void OnDestroy()
        {//后退键时销毁资源
            base.OnDestroy();
            //取消注册广播接收
            if (smsReceiver != null)
                UnregisterReceiver(smsReceiver);

        }
    }

   
}