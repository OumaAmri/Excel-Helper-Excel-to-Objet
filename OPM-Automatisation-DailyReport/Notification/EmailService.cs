using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace OPM_Automatisation_DailyReport.Notification
{
    public class EmailService
    {
        private readonly int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SMTP_PORT"]);
        private readonly string displayName = System.Configuration.ConfigurationManager.AppSettings["EmailDisplayName"];
        private readonly string sourceEmail = System.Configuration.ConfigurationManager.AppSettings["EMAIL"];
        private readonly string host = System.Configuration.ConfigurationManager.AppSettings["SMTP_HOST"];
        private readonly string userName = System.Configuration.ConfigurationManager.AppSettings["SMTP_USER"];
        private readonly string password = System.Configuration.ConfigurationManager.AppSettings["SMTP_PASSWORD"];
        private readonly bool enableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["ENABLE_SSL"]);
        private readonly string listEmailMiddelware = System.Configuration.ConfigurationManager.AppSettings["List_Email_Middelware"];
        private readonly string listEmailCC = System.Configuration.ConfigurationManager.AppSettings["List_Email_CC"];

        private static Logger Logger = LogManager.GetLogger("Rule");

        #region Send Email to the responsible of site (status "NOK" in CSV & Ping NOK)
        public void SendMsgNOKWithPingNOK(string To, string Subject, string Body)
        {
            List<string> CCs = new List<string>();
            CCs = listEmailCC.Split(',').ToList();
            try
            {
                var message = new MailMessage();
                message.To.Add(new MailAddress(To));   
                foreach (var CC in CCs)
                {
                    message.CC.Add(CC);
                }
                message.Subject = Subject;
                message.Body = Body;
                message.IsBodyHtml = true;
                message.From = new MailAddress(sourceEmail, displayName);
                SmtpClient smtpClient = new SmtpClient(host, port);
                smtpClient.EnableSsl = enableSsl;
                smtpClient.Credentials = new System.Net.NetworkCredential(userName, password);
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception when sending Email PingNOK : " + ex.Message+"\nInnerException : "+ex.InnerException.Message+"\n");
            }
        }

        #endregion

        #region Send Email to middelware team (status "NOK" in CSV but Ping OK)
        public void SendMsgNOKWithPingOK(string Subject, string Body)
        {
            List<string> Tos = new List<string>();
            Tos = listEmailMiddelware.Split(',').ToList();
            List<string> CCs = new List<string>();
            CCs = listEmailCC.Split(',').ToList();
            try
            {
                var message = new MailMessage();
                foreach (var to in Tos)
                {
                    message.To.Add(to);
                }
                foreach (var CC in CCs)
                {
                    message.CC.Add(CC);
                }
                message.Subject = Subject;
                message.Body = Body;
                message.IsBodyHtml = true;
                message.From = new MailAddress(sourceEmail, displayName);
                SmtpClient smtpClient = new SmtpClient(host, port);
                smtpClient.EnableSsl = enableSsl;
                smtpClient.Credentials = new System.Net.NetworkCredential(userName, password);
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception when sending Email PingOK : " + ex.Message + "\nInnerException : " + ex.InnerException.Message+"\n");
            }
        }
        #endregion
    }
}
