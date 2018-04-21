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
using System.Net.Mail;
using System.Net;

namespace Tsms
{
    public class EmailHelper
    {
        public static String SmtpHost { get; set; }
        public static String SmtpAccount { get; set; }
        public static String SmtpPassword { get; set; }
        public static String FromEmail { get; set; }
        public static Int32 Port { get; set; }
        public static Encoding encoding = Encoding.UTF8;

        #region
        /// <summary>
        /// 初始化(使用配置文件)
        /// </summary>
        /// <param name="emailConfig"></param>
        public static void Init(dynamic emailConfig)
        {
            Init(emailConfig.Smtp, emailConfig.Account, emailConfig.Password, emailConfig.MailAddress);
        }
        /// <summary>
        /// 初始化(使用参数,账号与邮箱同名)
        /// </summary>
        /// <param name="smtpHost"></param>
        /// <param name="smtpPassword"></param>
        /// <param name="fromEmail"></param>
        public static void Init(String smtpHost, String smtpPassword, String fromEmail)
        {
            if (String.IsNullOrEmpty(smtpHost))
                return;
            if (String.IsNullOrEmpty(fromEmail))
                return;
            String account = fromEmail.Substring(0, fromEmail.IndexOf("@"));
            Init(smtpHost, account, smtpPassword, fromEmail);
        }
        /// <summary>
        /// 初始化(使用参数)
        /// </summary>
        /// <param name="smtpHost"></param>
        /// <param name="smtpAccount"></param>
        /// <param name="smtpPassword"></param>
        /// <param name="fromEmail"></param>
        public static void Init(String smtpHost, String smtpAccount, String smtpPassword, String fromEmail, Int32 port = 0)
        {
            SmtpHost = smtpHost;
            SmtpAccount = smtpAccount;
            SmtpPassword = smtpPassword;
            FromEmail = fromEmail;
            encoding = Encoding.UTF8;
            Port = port;
        }
        #endregion

        #region
        /// <summary>
        /// 发送单封邮件
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="toEmail"></param>
        /// <param name="content"></param>
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        public static Boolean Send(String subject, String toEmail, String content, Boolean isBodyHtml = true)
        {
            return Send(subject, toEmail, content, null, isBodyHtml);
        }
        /// <summary>
        /// 发送单封邮件
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="toEmail"></param>
        /// <param name="content"></param>
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        public static Boolean Send(String subject, String toEmail, String content, List<String> files, Boolean isBodyHtml = true)
        {
            return BatchSend(subject, new List<String>() { toEmail }, content, files, isBodyHtml);
        }
        #endregion

        #region
        /// <summary>
        /// 批量发送邮件(非抄送)
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="toEmailList"></param>
        /// <param name="content"></param>
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        public static Boolean BatchSend(String subject, List<String> toEmailList, String content, Boolean isBodyHtml = true)
        {
            return BatchSend(subject, toEmailList, content, null, isBodyHtml);
        }
        /// <summary>
        /// 批量发送邮件(非抄送)
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="toEmailList"></param>
        /// <param name="content"></param>
        /// <param name="files">附件(文件地址)</param>
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        public static Boolean BatchSend(String subject, List<String> toEmailList, String content, List<String> files, Boolean isBodyHtml = true)
        {
            if (encoding == null)
                return false;
            MailMessage mailMessage = new MailMessage();
            foreach (String toEmail in toEmailList)
            {
                mailMessage.To.Add(new MailAddress(toEmail, null, encoding));
            }
            if (files != null)
            {
                foreach (String file in files)
                {
                    if (System.IO.File.Exists(file))
                        mailMessage.Attachments.Add(new Attachment(file));
                }
            }
            return Send(mailMessage, subject, content, isBodyHtml);
        }
        #endregion

        #region
        /// <summary>
        /// 批量发送邮件(抄送)
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="toEmailList"></param>
        /// <param name="content"></param>
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        public static Boolean CC(String subject, List<String> toEmailList, String content, Boolean isBodyHtml = true)
        {
            return CC(subject, toEmailList, content, isBodyHtml);
        }
        /// <summary>
        /// 批量发送邮件(抄送)
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="toEmailList"></param>
        /// <param name="content"></param>
        /// <param name="files">附件(文件地址)</param>
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        public static Boolean CC(String subject, List<String> toEmailList, String content, List<String> files, Boolean isBodyHtml = true)
        {
            if (encoding == null)
                return false;
            MailMessage mailMessage = new MailMessage();
            foreach (String toEmail in toEmailList)
            {
                mailMessage.CC.Add(new MailAddress(toEmail, null, encoding));
            }
            if (files != null)
            {
                foreach (String file in files)
                {
                    if (System.IO.File.Exists(file))
                        mailMessage.Attachments.Add(new Attachment(file));
                }
            }
            return Send(mailMessage, subject, content, isBodyHtml);
        }
        #endregion

        #region
        /// <summary>
        /// 同步发送
        /// </summary>
        /// <param name="mailMessage"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        private static Boolean Send(MailMessage mailMessage, String subject, String content, Boolean isBodyHtml)
        {
            if (string.IsNullOrEmpty(SmtpHost) || string.IsNullOrEmpty(SmtpAccount))
                return false;

            mailMessage.From = new MailAddress(FromEmail, null, encoding);
            mailMessage.Subject = subject;
            mailMessage.Body = content;
            mailMessage.IsBodyHtml = isBodyHtml;
            mailMessage.BodyEncoding = encoding;
            mailMessage.Priority = MailPriority.Normal;
            //mailMessage.BodyTransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable;
            // mailMessage.CC.Add()//抄送

            SmtpClient client = new SmtpClient(SmtpHost);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(SmtpAccount, SmtpPassword);
            if (Port > 0)
            {
                client.Port = Port;
                if (client.Port != 25)
                    client.EnableSsl = true;
            }
            Boolean sendResult = false;
            try
            {
                client.Send(mailMessage);
                //Task task = client.SendMailAsync(mailMessage);

                sendResult = true;
            }
            catch (Exception ex)
            {
                sendResult = false;
                throw ex;
            }
            finally
            {
                mailMessage.Dispose();
                client.Dispose();
            }
            return sendResult;
        }
        #endregion
    }
}