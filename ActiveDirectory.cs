using Oracle.DataAccess.Client;
using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Security.Principal;

namespace CoalesceUserMetadata
{
    public class ActiveDirectory
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("ActiveDirectory");

        #region Constants

        static readonly NameValueCollection section = ConfigurationManager.GetSection("ActiveDirectory") as NameValueCollection;
        private static readonly string[] AD_OUs = section["AD_OUs"].Split(';').ToArray<string>();
        private static readonly string[] ADPropertiesToLoad = section["ADPropertiesToLoad"].Split(',').ToArray<string>();
        private static readonly string AD_OUs_Path1 = section["AD_OUs_Path1"];
        private static readonly string AD_OUs_Path2 = section["AD_OUs_Path2"];
        private static readonly string ADSearchFilter = section["ADSearchFilter"];
        private static readonly int ADSearchPageSize = Int32.Parse(section["ADSearchPageSize"]);
        private static readonly IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

        #endregion

        public void LoadActiveOU(List<Person> people)
        {

            Program.LogMessage += host.AddressList[0].ToString() + "," + WindowsIdentity.GetCurrent().Name + "," + DateTime.Now;

            try
            {
                for (int i = 0; i < AD_OUs.Length; i++)
                {
                    string path = AD_OUs_Path1 + AD_OUs[i] + AD_OUs_Path2;

                    string tmp = AD_OUs[i].Substring(3);

                    string AD_OU = tmp.Substring(0, tmp.IndexOf(","));

                    using (DirectoryEntry de = new DirectoryEntry(path))
                    {
                        using (DirectorySearcher srch = new DirectorySearcher(de))
                        {
                            srch.PageSize = ADSearchPageSize;
                            srch.Filter = ADSearchFilter;
                            srch.SearchScope = SearchScope.OneLevel;
                            for (int indx = 0; indx < ADPropertiesToLoad.Length; indx++)
                            {
                                srch.PropertiesToLoad.Add(ADPropertiesToLoad[indx]);
                            }
                            var results = srch.FindAll();

                            Program.LogMessage += "," + results.Count;
                            Console.WriteLine(AD_OU + ": " + results.Count);

                            foreach (SearchResult entry in results)
                                try
                                {
                                    Person person = new Person
                                    {
                                        cn = entry.GetPropertyValue("userPrincipalName"),
                                        adGivenName = entry.GetPropertyValue("givenname"),
                                        adSurname = entry.GetPropertyValue("sn"),
                                        adVoiceTelephoneNumber = entry.GetPropertyValue("telephoneNumber"),
                                        adEmailAddress = entry.GetPropertyValue("userPrincipalName"),
                                        Description = entry.GetPropertyValue("title"),
                                        Department = entry.GetPropertyValue("department"),
                                        adPhysicalLocation = entry.GetPropertyValue("physicalDeliveryOfficeName"),
                                        adFaxnumber = entry.GetPropertyValue("facsimileTelephoneNumber"),
                                        adOrganizationalunit = AD_OU,
                                        adDisplayname = entry.GetPropertyValue("displayName"),
                                        adWhenCreated = entry.GetPropertyValue("whenCreated")
                                    };
                                    person.cn = person.cn.Substring(0, person.cn.LastIndexOf("@"));
                                    people.Add(person);
                                }
                                catch (Exception e)
                                {
                                    log.Error(e);
                                }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }
            Program.LogMessage += "," + people.Count() + ",";

            Console.WriteLine("Total AD Objects: " + people.Count());
        }
    }
}

public static class ExtensionMethods
{
    public static string GetPropertyValue(this SearchResult searchResult, string propertyName)
    {
        string tmp = string.Empty;

        if (searchResult.Properties[propertyName].Count > 0)
        {
            tmp = searchResult.Properties[propertyName][0].ToString();
        }
        return tmp;
    }

    public static string OraclePropertyValue(this OracleDataReader searchResult, int n)
    {
        string tmp = string.Empty;

        if (searchResult.IsDBNull(n))
        {
            return tmp;
        }
        return searchResult.GetValue(n).ToString();
    }
}
