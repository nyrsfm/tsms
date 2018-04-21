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
    [Activity(Label = "SMSTransferContactActivity")]
    public class SMSTransferContactActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SMSTransferContactLayout);
            Title = "添加联系电话/邮箱";
            var helper = new ExtSQLiteHelper(this);
            var editText = FindViewById<EditText>(Resource.Id.contact_text);
            editText.Text = string.Join("\r\n", helper.GetContact());

            #region
            FindViewById<Button>(Resource.Id.save_contact_button).Click += (sender, e) =>
            {
                if (editText != null)
                {
                    var lines = editText.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    helper.SaveContact(lines);
                }
                Toast.MakeText(this, "完成", ToastLength.Short).Show();
            };
            FindViewById<Button>(Resource.Id.cancel_contact_button).Click += (sender, e) =>
            {
                this.Finish();
            };
            #endregion
        }
    }
}