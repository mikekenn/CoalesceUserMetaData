using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net.Mail;
using System.Text;

namespace CoalesceUserMetadata
{
    class Email
    {

        #region Constants

        static readonly NameValueCollection section = ConfigurationManager.GetSection("Email") as NameValueCollection;
        private static readonly string SuccessRecipient = section["SuccessRecipient"];
        private static readonly string DebugRecipient = section["DebugRecipient"];
        private static readonly string ErrorRecipient = section["ErrorRecipient"];
        private static readonly string smtpserver = section["smtpserver"];
        private static readonly int smtpport = Int32.Parse(section["smtpport"]);
        private static readonly string smtpusername = section["smtpusername"];
        private static readonly string smtppassword = section["smtppassword"];
        private static readonly string sendFrom = section["sendFrom"];
        private static readonly string sendFromName = section["sendFromName"];

        #endregion

        public static void SuccessEmail(string subStatus, string dt, int totAdAccntsProcess, string timeSpan)
        {
            SmtpClient client;
            MailMessage mail;
            SetUpEmail(out client, out mail);

            int numOfOracleErrs = PersonInfoInterface.GetErrorLogCount(dt);
            int totNewUsrCreated = PersonInfoInterface.GetNewRecordCount(dt);
            int totNumUpdatedVals = PersonInfoInterface.GetUpdatedRecordCount(dt);
            int totNumDeletedRecords = PersonInfoInterface.GetDeletedRecordCount(dt);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Date/Time Ran: {dt}.");
            sb.AppendLine($"Runtime = {timeSpan}\n");
            sb.AppendLine($"Total number of AD accounts processed: {totAdAccntsProcess}");
            sb.AppendLine($"Total number of new records created: {totNewUsrCreated}");
            sb.AppendLine($"Total number of updated values: {totNumUpdatedVals}");
            sb.AppendLine($"Total number of deleted records: {totNumDeletedRecords}");
            sb.AppendLine($"Total number of oracle errors: {numOfOracleErrs}\n");
            sb.AppendLine($"Logs: C:\\Your\\Local\\Logs\\Location\\");

            if (subStatus == "Successful")
            {
                mail.To.Add(SuccessRecipient);
                mail.Subject = $"CoalesceUserMetaData - {subStatus}.";
            }
            else if (subStatus == "Successful with errors")
            {
                mail.To.Add(DebugRecipient);
                mail.To.Add(ErrorRecipient); //Comment out during debugging.
                mail.Subject = $"CoalesceUserMetaData - {subStatus}.";
            }

            mail.Body = sb.ToString();

            client.Send(mail);
        }

        public static void FailEmail(string subStatus, string dt)
        {
            SmtpClient client;
            MailMessage mail;
            SetUpEmail(out client, out mail);

            int numOfOracleErrs = PersonInfoInterface.GetErrorLogCount(dt);
            StringBuilder sb = new StringBuilder();

            mail.To.Add(DebugRecipient);
            mail.Subject = $"CoalesceUserMetaData - {subStatus}.";

            sb.AppendLine($"Date/Time Ran: {dt}.");
            sb.AppendLine($"Total number of oracle errors: {numOfOracleErrs}\n");
            sb.AppendLine($"Logs: C:\\Your\\Local\\Logs\\Location\\");

            mail.Body = sb.ToString();

            client.Send(mail);
        }

        private static void SetUpEmail(out SmtpClient client, out MailMessage mail)
        {
            client = new SmtpClient
            {
                Host = smtpserver,
                Port = smtpport,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(smtpusername, smtppassword)
            };
            mail = new MailMessage
            {
                IsBodyHtml = false,
                From = new MailAddress(sendFrom, sendFromName)
            };
        }
    }
}
