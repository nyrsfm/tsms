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
    [Activity(Label = "AppLogActivity")]
    public class AppLogActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AppLogLayout);
            var helper = new ExtSQLiteHelper(this);
            var logtext = FindViewById<TextView>(Resource.Id.text_applog);
            logtext.Text = helper.GetAppLog();
            logtext.SetCursorVisible(false);//Edittext只读
            logtext.Focusable=false;
            logtext.FocusableInTouchMode=false;

            Title = "运行日志";
            #region
            FindViewById<Button>(Resource.Id.clear_applog_button).Click += (sender, e) =>
            {
                helper.ClearAppLog();
                logtext.Text = "";
                Toast.MakeText(this, "操作完成", ToastLength.Short).Show();
            };
            FindViewById<Button>(Resource.Id.cancel_applog_button).Click += (sender, e) =>
            {
                this.Finish();
            };
            #endregion
            // Create your application here

            
        }
    }
}