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
    [Activity(Label = "SMSTransferConfigActivity")]
    public class SMSTransferConfigActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SMSTransferConfigLayout);
            Title = "发送配置";
            // Create your application here
            var helper = new ExtSQLiteHelper(this);
            Dictionary<string, string> dic;
            var templatetext = FindViewById<EditText>(Resource.Id.text_template);
            var smtpservertext = FindViewById<EditText>(Resource.Id.text_smtpserver);
            var smtpporttext = FindViewById<EditText>(Resource.Id.text_smtpport);
            var smtpaccounttext = FindViewById<EditText>(Resource.Id.text_smtpaccount);
            var smtppasswordtext = FindViewById<EditText>(Resource.Id.text_smtppassword);
            var controlphonetext=FindViewById<EditText>(Resource.Id.text_controlphone);
            #region
            dic = helper.GetConfig();
            templatetext.Text = dic.ContainsKey("messagetemplate") ? dic["messagetemplate"] : "";
            smtpservertext.Text = dic.ContainsKey("smtpserver") ? dic["smtpserver"] : "";
            smtpporttext.Text = dic.ContainsKey("smtpport") ? dic["smtpport"] : "25";
            smtpaccounttext.Text = dic.ContainsKey("smtpaccount") ? dic["smtpaccount"] : "";
            smtppasswordtext.Text = dic.ContainsKey("smtppassword") ? dic["smtppassword"] : "";
            controlphonetext.Text = dic.ContainsKey("controlphone") ? dic["controlphone"] : "";
            #endregion

            #region
            FindViewById<Button>(Resource.Id.save_config_button).Click += (sender, e) =>
            {
                dic = new Dictionary<string, string>();
                dic.Add("messagetemplate", templatetext.Text);
                dic.Add("smtpserver", smtpservertext.Text);
                dic.Add("smtpport", smtpporttext.Text);
                dic.Add("smtpaccount", smtpaccounttext.Text);
                dic.Add("smtppassword", smtppasswordtext.Text); 
                dic.Add("controlphone", controlphonetext.Text.Trim().TrimStart('+'));
                helper.SaveConfig(dic);

                Toast.MakeText(this, "完成", ToastLength.Short).Show(); 
            };
            FindViewById<Button>(Resource.Id.cancel_config_button).Click += (sender, e) =>
            {
                this.Finish();
            };
            #endregion
        }
    }
}