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
    [Activity(Label = "SMSContentProviderActivity")]
    public class SMSContentProviderActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
           // SetContentView(Resource.Layout.SMSContentProviderView);
            // Create your application here

            //读取和操作相应程序中的数据
            FindViewById<Button>(Resource.Id.smsreadbotton).Click += delegate
            {
                #region --短信--
                //----低于Android6.0版本 
                //try
                //{
                //    //需要权限CALL_PHONE
                //    Intent intent = new Intent(Intent.ActionCall);
                //    intent.SetData(Android.Net.Uri.Parse("tel:10086"));
                //    StartActivity(intent);
                //}
                //catch (Java.Lang.SecurityException ex)
                //{
                //    ex.PrintStackTrace();
                //}

                //-----v6.0+ 
                //在配置清单里面勾选了CALLPHONE权限,这里的状态就会变为已授权(用户主动禁止权限这里依然是授权状态，只是拨打电话没有任何响应)
                //低于v6.0版本可使用CheckCallingOrSelfPermission等,v6.0+后可使用this.CheckSelfPermission方法;
                if (this.CheckSelfPermission(Android.Manifest.Permission.ReadSms) != Android.Content.PM.Permission.Granted)
                {//检查是否对指定的权限授权

                    //RequestPermissions权限请求仅在v6.0+版本编译有效,参数:需要的权限,唯一编号;
                    RequestPermissions(new string[] { Android.Manifest.Permission.ReadSms }, 1);

                    // EnforcePermission(Android.Manifest.Permission.CallPhone, Binder.CallingPid, Binder.CallingUid, "需要授权拨打电话权限!");//用途未知
                }
                else
                {

                    Android.Database.ICursor cursor = null;
                    try
                    {
                        //查询联系人数据(使用已封装好的uri)
                        cursor = ContentResolver.Query(Android.Net.Uri.Parse("content://sms"), new String[] { "_id", "address", "body", "date" }, null, null, "date desc");
                        if (cursor != null)
                        {
                            while (cursor.MoveToNext())
                            {
                               var body= cursor.GetString(cursor.GetColumnIndex("body"));
                                Toast.MakeText(this, body, ToastLength.Long).Show();
                                break;
                            }
                        }
                        Toast.MakeText(this,"操作完成", ToastLength.Long).Show();
                    }
                    catch (Java.Lang.Exception ex)
                    {
                        Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                        ex.PrintStackTrace();
                    }
                    finally
                    {
                        if (cursor != null)
                            cursor.Close();
                    }
                }
                // Android.Manifest.Permission.CallPhone

                #endregion
            };
        }
    }
}