namespace Sample
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;

    public class RESTUtility
    {
        //Perform an HTTP Get.
        public static string HttpGet(string uri, ref CookieContainer cookie)
        {
            string response = null;
            if (cookie == null) cookie = new CookieContainer();
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.CookieContainer = cookie;
            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "text/xml; encoding='utf-8'";
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
                streamReader.Close();
            }
            return response;
        }

        //Perform an HTTP Post.
        public static string HttpPost(string uri, string parameters, CookieContainer cookie)
        {
            // parameters: name1=value1&name2=value2	
            string response = "";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.CookieContainer = cookie;
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(parameters);
            httpWebRequest.ContentLength = bytes.Length;
            using (Stream responseStream = httpWebRequest.GetRequestStream()) responseStream.Write(bytes, 0, bytes.Length);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
                streamReader.Close();
            }
            return response;
        }
    }

    [DataContract]
    public class LoginResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Result { get; set; }

        public Exception Exception { get; set; }
    }

    [DataContract]
    public class QueryResult<T>
    {
        [DataMember]
        public List<T> Records { get; set; }

        [DataMember]
        public long Count { get; set; }

        public Exception Exception { get; set; }
    }

    [DataContract]
    public class SaveResult
    {
        [DataMember]
        public List<Error> Errors { get; set; }
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public bool HasError { get; set; }

        public Exception Exception { get; set; }
    }

    [DataContract]
    public class DeleteResult
    {
        [DataMember]
        public List<Error> Errors { get; set; }
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public bool HasError { get; set; }

        public Exception Exception { get; set; }
    }

    [DataContract]
    public class Error
    {
        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string PropertyName { get; set; }
        
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public int Index { get; set; }
    }

    public class REST : RESTUtility
    {
        CookieContainer _cookie = null;
        bool _isLoggedin = false;
        string _uri = "";
        string _username = "";
        string _password = "";

        /// <summary>
        /// Creates instance of Magentrix REST Wrapper API
        /// </summary>
        /// <param name="uri">URL to the Magentrix site.</param>
        /// <param name="username">Username REST API should use authenticate</param>
        /// <param name="password">Password REST API should use authenticate</param>
        public REST(string uri, string username, string password)
        {
            if (string.IsNullOrEmpty("uri")) return;
            _uri = uri.EndsWith("/") ? uri : uri + "/";
            _username = username;
            _password = password;
        }

        /// <summary>
        /// This method should be called immediately after instantiation of the class to authenticate with Magentrix.
        /// </summary>
        /// <param name="logActivity">If true, this login will be recoded in Login History for the the authenticated user.  If false, login will not be recorded in Login History of the user.</param>
        /// <returns>Returns the Login Result object.  If login is successful, LoginResult.Success will be set to true, otherwise, it will be false.</returns>
        public LoginResult Login(bool logActivity = true)
        {
            try
            {
                string url = string.Format("{0}User/restlogin?un={1}&pw={2}{3}", _uri, Uri.EscapeDataString(_username), Uri.EscapeDataString(_password), logActivity ? "" : "&logmode=off");
                LoginResult result = Deserialize<LoginResult>(HttpGet(url, ref _cookie));
                if (result.Success) _isLoggedin = true;
                return result;
            }
            catch (Exception ex)
            {
                return new LoginResult { Exception = ex };
            }
        }
        
        /// <summary>
        /// Allows the authenticated user query any object that it has access to.  Any object that is going to be used needs to have a corresponding class in the API.
        /// Refer to Account.cs to see how classes are defined.  For every object that is being queried, there must be a corresponding class defined in your application.
        /// </summary>
        /// <typeparam name="T">This Entity class that is going to be queried.</typeparam>
        /// <param name="query">This would be query that is sent to the magentrix including any conditions, ordering defined.</param>
        /// <returns></returns>
        public QueryResult<T> Query<T>(string query)
        {
            try
            {
                string url = string.Format("{0}REST/Query?q={1}", _uri, Uri.EscapeDataString(query));
                return Deserialize<QueryResult<T>>(HttpGet(url, ref _cookie));
            }
            catch (Exception ex)
            {
                return new QueryResult<T> { Exception = ex };
            }
        }

        /// <summary>
        /// Deletes a single record from Magentrix.
        /// </summary>
        /// <param name="id">This is the ID of the record being deleted.</param>
        /// <param name="permanent">If set to true, a permanent delete will occure, otherwise the record is marked as deleted and it is sent to the recycle bin.</param>
        /// <returns></returns>
        public DeleteResult Delete(string id, bool permanent)
        {
            try
            {
                string url = string.Format("{0}REST/Delete?id={1}&permanent={2}", _uri, id, permanent);
                return Deserialize<DeleteResult>(HttpGet(url, ref _cookie));
            }
            catch (Exception ex)
            {
                return new DeleteResult { Exception = ex };
            }
        }
        /// <summary>
        /// Creates a new record For a given Entity
        /// </summary>
        /// <typeparam name="T">Entity Class record is going to be created</typeparam>
        /// <param name="model">Instance of the model with populated values.</param>
        /// <returns>Status of the create.  HasError will be false on success, true otherwise.  If record creation fails, Errors property will provide further detail as to why record creation failed.</returns>
        public SaveResult Create<T>(T model) where T : class
        {
            try
            {
                string url = string.Format("{0}REST/Create", _uri);
                string param = string.Format("e={0}&data={1}", typeof(T).Name, Serialize<T>(model));
                return Deserialize<SaveResult>(HttpPost(url, param, _cookie));
            }
            catch (Exception ex)
            {
                return new SaveResult { Exception = ex };
            }
        }

        /// <summary>
        /// Updates an existing record in the system.
        /// </summary>
        /// <typeparam name="T">Entity Class record is going to be updated</typeparam>
        /// <param name="model">Instance of the model with populated values.  Id property must point to an existing record in the system.</param>
        /// <returns></returns>
        public SaveResult Update<T>(T model) where T : class
        {
            try
            {
                string url = string.Format("{0}REST/Edit", _uri);
                string param = string.Format("e={0}&data={1}", typeof(T).Name, Serialize<T>(model));
                return Deserialize<SaveResult>(HttpPost(url, param, _cookie));
            }
            catch (Exception ex)
            {
                return new SaveResult { Exception = ex };
            }
        }
        public string Post(string action, string parameters)
        {
            string post = string.Format("{0}{1}", _uri, action);
            return HttpPost(post, parameters, _cookie);
        }
        public string Get(string action)
        {
            string get = string.Format("{0}{1}", _uri, action);
            return HttpGet(get, ref _cookie);
        }
        public static T Deserialize<T>(string response)
        {
            if (string.IsNullOrEmpty(response)) return Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(response)))
            {
                DataContractJsonSerializer sz = new DataContractJsonSerializer(typeof(T));
                T result = (T)sz.ReadObject(ms);
                return result;
            }
        }
        public static string Serialize<T>(T model) where T : class
        {
            if (model == null) return null;
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer sz = new DataContractJsonSerializer(typeof(T));
                sz.WriteObject(ms, model);
                ms.Position = 0;
                using (StreamReader sr = new StreamReader(ms)) return sr.ReadToEnd();
            }
        }
    }
}
