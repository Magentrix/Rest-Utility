using System.Linq;
using Magentrix.Entities;
using Sample;

namespace SampleRest
{
    public class Sample
    {
        private static string _EndPoint = @"https://aizan.magentrix.com";
        private static string _User = @"YOUR USERNAME";
        private static string _Password = "YOUR PASSWORD";

        public static void  Main()
        {
            //Create new instance of REST class
            REST api = new REST(_EndPoint, _User, _Password);

            //Perform the login.
            if (api.Login(true).Success)
            {
                QueryResult<Account> queryResult = api.Query<Account>(string.Format("FROM Account WHERE Active == true"));

                SaveResult createResult = api.Create<Account>(new Account { Active = false, Name = "Test", Password = "Secret", Web = "https://www.google.com" });
                queryResult = api.Query<Account>(string.Format("FROM Account WHERE Active == false"));

                if (queryResult.Count > 0)
                {
                    foreach (Account account in queryResult.Records)
                    {
                        account.Password = "new secret";
                        SaveResult saveResult = api.Update<Account>(account);
                    }
                }

                DeleteResult deleteResult = api.Delete(createResult.Id, false);
            }
        }

        //public static bool HasSoftwareAssurance(REST api, decimal sentinelNumber)
        //{
        //    //Query salesforce account object from Ease
        //    QueryResult<Account> queryResult = api.Query<Account>(string.Format("FROM Force.Force__Account WHERE Sentinel_Number__c == {0}", sentinelNumber));
        //    if (queryResult.Count > 0)
        //    {
        //        //We found some records that match the criterial.  Test the Customer_Status__c field.
        //        return queryResult.Records.First().Customer_Status__c == "Active - Software Assurance";

        //    }
        //    else if (queryResult.Exception != null)
        //    {
        //        // There was an exception.  Need to handle it.
        //    }
        //    //No records were found.  Return false
        //    return false;
        //}
    }
}
