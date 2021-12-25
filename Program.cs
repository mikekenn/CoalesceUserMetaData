using System;
using System.Collections.Generic;
using System.Diagnostics;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace CoalesceUserMetadata
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Main");

        #region Constants

        public static string LogMessage;

        #endregion

        static void Main(string[] args)
        {

            Stopwatch sw = new Stopwatch();

            ActiveDirectory AD = new ActiveDirectory();

            List<Person> List = new List<Person>();

            sw.Start();

            AD.LoadActiveOU(List);

            string oracleRunDateTime = PersonInfoInterface.UpdatePersonTable(List);

            sw.Stop();

            if (!oracleRunDateTime.Contains("Merge failed") && PersonInfoInterface.GetErrorLogCount(oracleRunDateTime) == 0)
            {
                LogMessage += sw.Elapsed + ",Successfully with no errors.";
                log.Info(LogMessage);
                //PersonInfoInterface.GetChangeLogInformation(DateTime.Now.ToString("M/d/yyyy h:m:ss.ff tt"));
                //Email.SuccessEmail("Successful", oracleRunDateTime, List.Count, sw.Elapsed.ToString("g"));
            }
            else if (!oracleRunDateTime.Contains("Merge failed") && PersonInfoInterface.GetErrorLogCount(oracleRunDateTime) > 0)
            {
                LogMessage += sw.Elapsed + ",Successfully with errors.";
                log.Info(LogMessage);
                PersonInfoInterface.GetErrorLogInformation(oracleRunDateTime);
                Email.SuccessEmail("Successful with errors", oracleRunDateTime, List.Count, sw.Elapsed.ToString("g"));
            }
            else
            {
                LogMessage += sw.Elapsed + ",Failed";
                log.Info(LogMessage);
                PersonInfoInterface.GetErrorLogInformation(oracleRunDateTime.Substring(0, 19));
                Email.FailEmail("Failed", oracleRunDateTime.Substring(0, 19));
            }
        }
    }
}
