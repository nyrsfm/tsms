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
using Android.Database.Sqlite;

namespace Tsms
{
    public class ExtSQLiteOpenHelper : SQLiteOpenHelper
    {
        public Android.Content.Context context;
        public ExtSQLiteOpenHelper(Android.Content.Context context, string name, SQLiteDatabase.ICursorFactory factory, int version) : base(context, name, factory, version)
        {
            this.context = context;
        }
        public override void OnCreate(SQLiteDatabase db)
        {
            //var sql = new StringBuilder();
            //sql.AppendLine("create table receiver(receivetype text,receiveaddress text);");//接收者信息
            //sql.AppendLine("create table smsconfig(messagetemplate text,smtpserver text,smtpaccount text,smtppassword text);");//转移信息配置
            //sql.AppendLine("create table sendstatus(servicestatus INTEGER,sendsuccess INTEGER,sendfailure INTEGER);");//历史操作记录
            //sql.AppendLine("insert into smsconfig(messagetemplate) values('#from#:#body#');");
            //sql.AppendLine("insert into sendstatus(servicestatus,sendsuccess,sendfailure) values(0,0,0);");

            db.ExecSQL("create table receiver(receiveaddress text)");
            db.ExecSQL("create table smsconfig(messagetemplate text,smtpserver text,smtpaccount text,smtppassword text,smtpport text,controlphone text)");
            db.ExecSQL("create table sendstatus(servicestatus INTEGER,sendsuccess INTEGER,sendfailure INTEGER)");
            db.ExecSQL("create table applog(content text,updatetime datetime default (datetime('now', 'localtime')))");
            db.ExecSQL("insert into smsconfig(messagetemplate) values('#from#:#body#')");
            db.ExecSQL("insert into sendstatus(servicestatus,sendsuccess,sendfailure) values(0,0,0)");


            //db.ExecSQL("insert into smsconfig(messagetemplate) values(?);", new Java.Lang.Object[] {
            //   "#from#:#body#"
            //});
            //db.ExecSQL("insert into history(servicestatus,sendsuccess,sendfailure) values(0,0,0);");

        }
        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            if (newVersion > oldVersion)
            {
                if (newVersion == 3)
                    db.ExecSQL("alter table smsconfig add column smtpport text");
                else if (newVersion == 4)
                    db.ExecSQL("create table applog(content text,updatetime datetime default (datetime('now', 'localtime')))");
            }
        }
    }
    public class ExtSQLiteHelper
    {
        public static ExtSQLiteOpenHelper helper = null;

        public ExtSQLiteHelper(Context context)
        {
            if (helper == null)
            {
                helper = new ExtSQLiteOpenHelper(context, "smstransferdb", null, 4);
                var rd = helper.ReadableDatabase;
            }
        }
        public string[] GetContact()
        {
            var db = helper.ReadableDatabase;
            var lines = new List<string>();
            Android.Database.ICursor cursor = db.Query("receiver", null, null, null, null, null, null);
            if (cursor.MoveToFirst())
            {
                do
                {
                    var receiveAddress = cursor.GetString(cursor.GetColumnIndex("receiveaddress"));
                    lines.Add(receiveAddress);
                } while (cursor.MoveToNext());
            };
            db.Dispose();
            return lines.ToArray();
        }
        public long SaveContact(String[] array)
        {
            long result = 0;
            var db = helper.WritableDatabase;
            db.Delete("receiver", "", null);
            foreach (var txt in array)
            {
                if (Utility.IsDigit(txt) || Utility.IsEmail(txt))
                {
                    ContentValues values = new ContentValues();
                    values.Put("receiveaddress", txt.Trim());
                    result += db.Insert("receiver", null, values);
                }
            }
            db.Dispose();
            return result;
        }

        public long SaveConfig(Dictionary<string, string> dic)
        {
            if (dic == null || dic.Count == 0)
                return 0;

            var db = helper.WritableDatabase;
            db.Delete("smsconfig", "", null);
            ContentValues values = new ContentValues();
            if (dic.ContainsKey("messagetemplate"))
                values.Put("messagetemplate", dic["messagetemplate"].Trim());
            if (dic.ContainsKey("smtpserver"))
                values.Put("smtpserver", dic["smtpserver"].Trim());
            if (dic.ContainsKey("smtpaccount"))
                values.Put("smtpaccount", dic["smtpaccount"].Trim());
            if (dic.ContainsKey("smtppassword"))
                values.Put("smtppassword", dic["smtppassword"].Trim());
            if (dic.ContainsKey("controlphone"))
                values.Put("controlphone", dic["controlphone"].Trim());
            if (dic.ContainsKey("smtpport"))
                values.Put("smtpport", dic["smtpport"].Trim());
            long result = db.Insert("smsconfig", null, values);
            db.Dispose();
            return result;
        }
        public Dictionary<string, string> GetConfig()
        {
            var db = helper.ReadableDatabase;
            var dic = new Dictionary<string, string>();
            Android.Database.ICursor cursor = db.Query("smsconfig", null, null, null, null, null, null);
            if (cursor.MoveToFirst())
            {
                dic.Add("messagetemplate", cursor.GetString(cursor.GetColumnIndex("messagetemplate")));
                dic.Add("smtpserver", cursor.GetString(cursor.GetColumnIndex("smtpserver")));
                dic.Add("smtpaccount", cursor.GetString(cursor.GetColumnIndex("smtpaccount")));
                dic.Add("smtppassword", cursor.GetString(cursor.GetColumnIndex("smtppassword")));
                dic.Add("controlphone", cursor.GetString(cursor.GetColumnIndex("controlphone")));
                dic.Add("smtpport", cursor.GetString(cursor.GetColumnIndex("smtpport")));
            };
            db.Dispose();
            return dic;
        }

        public Dictionary<string, string> GetSendStatus()
        {
            var db = helper.ReadableDatabase;
            var dic = new Dictionary<string, string>();
            Android.Database.ICursor cursor = db.Query("sendstatus", null, null, null, null, null, null);
            if (cursor.MoveToFirst())
            {
                dic.Add("servicestatus", cursor.GetString(cursor.GetColumnIndex("servicestatus")));
                dic.Add("sendsuccess", cursor.GetString(cursor.GetColumnIndex("sendsuccess")));
                dic.Add("sendfailure", cursor.GetString(cursor.GetColumnIndex("sendfailure")));

            };
            db.Dispose();
            return dic;
        }
        public long UpdateSendStatus(int v)
        {
            var db = helper.WritableDatabase;
            ContentValues values = new ContentValues();
            values.Put("servicestatus", v);
            var r = db.Update("sendstatus", values, null, null);
            db.Dispose();
            return r;
        }
        public void UpdateSendStatistics(string column)
        {
            var db = helper.WritableDatabase;
            db.ExecSQL(string.Format("update sendstatus set {0}={0}+1", column));
            db.Dispose();
        }

        public string GetAppLog()
        {
            var db = helper.ReadableDatabase;
            var logstr = new StringBuilder();
            Android.Database.ICursor cursor = db.Query("applog", null, null, null, null, null, null);
            if (cursor.MoveToFirst())
            {
                do
                {
                    logstr.AppendLine(string.Format("{0}--{1}",
                    cursor.GetString(cursor.GetColumnIndex("content")),
                    cursor.GetString(cursor.GetColumnIndex("updatetime"))));
                } while (cursor.MoveToNext());
            };
            db.Dispose();
            return logstr.ToString();
        }
        public long AddAppLog(string content)
        {
            var db = helper.WritableDatabase;
            ContentValues values = new ContentValues();
            values.Put("content", content);
            var result = db.Insert("applog", null, values);
            db.Dispose();
            return result;
        }
        public void ClearAppLog()
        {
            var db = helper.WritableDatabase;
            db.Delete("applog", "", null);

            ContentValues values = new ContentValues();
            values.Put("sendsuccess", 0);
            values.Put("sendfailure", 0);
            db.Update("sendstatus", values, null, null);
            db.Dispose();
        }
    }
}