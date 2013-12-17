using System.Linq;
using Magentrix.Entities;

namespace Magentrix
{
    public class Sample
    {

        public static void  Main()
        {
            string _EndPoint = @"https://yourcompany.magentrix.com";
            string _User = @"ADMIN USERNAME";
            string _Password = "ADMIN PASSWORD";

            //Create new instance of REST class
            REST api = new REST(_EndPoint);

            //Perform the login.
            var loginResult = api.Login(_User, _Password);

            if (loginResult.IsSuccess)
            {
                QueryResult<User> queryResult = null;

                //Querying Active user
                queryResult = api.Query<User>(string.Format("FROM User WHERE IsActive == true"));

                //Querying Active users whose name starts with "ch"
                queryResult = api.Query<User>(string.Format("FROM User WHERE IsActive == true AND Username.ToLower().StartsWith(\"ch\")"));

                //Querying (Active users whose name starts with "ch") OR Email contains "secureauth"
                queryResult = api.Query<User>(string.Format("FROM User WHERE (IsActive == true AND Username.ToLower().StartsWith(\"ch\")) OR Email.ToLower().Contains(\"secureauth\")"));
            }
        }
    }
}
