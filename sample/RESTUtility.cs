namespace Magentrix
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;

    public class RESTUtility
    {
        //Perform an HTTP Get.
        protected static string HttpGet(string uri, string authorization)
        {
            string response = null;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            if (!string.IsNullOrEmpty(authorization))
            {
                httpWebRequest.Headers.Add("Authorization", authorization);
            }
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
        protected static string HttpPost(string uri, string parameters, string authorization)
        {
            string response = "";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            if (!string.IsNullOrEmpty(authorization))
            {
                httpWebRequest.Headers.Add("Authorization", authorization);
            }
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

    public class LoginResult
    {
        public LoginResult()
        {
            IsSuccess = false;
            Errors = new List<Error>();
        }

        public bool IsSuccess { get; set; }

        public string SessionId { get; set; }

        public DateTime? DateIssued { get; set; }

        public List<Error> Errors { get; set; }

        public void AddError(string message)
        {
            Errors.Add(new Error { Message=message });
         }
    }

    public class QueryResult<T>
    {
        public List<T> Records { get; set; }

        public long Count { get; set; }

        public Exception Exception { get; set; }
    }

    public class SaveResult
    {
        public List<Error> Errors { get; set; }

        public string Id { get; set; }

        public bool HasError { get; set; }

        public Exception Exception { get; set; }
    }

    public class DeleteResult
    {
        public List<Error> Errors { get; set; }

        public string Id { get; set; }

        public bool HasError { get; set; }

        public Exception Exception { get; set; }
    }

    public class Error
    {
        public string Code { get; set; }

        public string PropertyName { get; set; }
        
        public string Message { get; set; }

        public int Index { get; set; }
    }

    public class REST : RESTUtility
    {
        string _uri = "";
        string _username = "";
        string _password = "";
        string _AuthToken = "";
        DateTime? _sessionIssued = null;
        string _version = "";

        public string SessionId
        {
            get
            {
                return _AuthToken;
            }
            set
            {
                _AuthToken = value;
            }
        }

        public DateTime? SessionIssueDate
        {
            get
            {
                return _sessionIssued;
            }
            set
            {
                _sessionIssued = value;
            }
        }

        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }

        private string VersionPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_version))
                    return "";
                else
                    return _version + "/";
            }
        }

        /// <summary>
        /// Creates instance of Magentrix REST Wrapper API
        /// </summary>
        /// <param name="uri">URL to the Magentrix instance.</param>
        /// <param name="username">Username REST API should use authenticate</param>
        /// <param name="password">Password REST API should use authenticate</param>
        public REST(string uri)
        {
            if (string.IsNullOrEmpty("uri")) return;
            _uri = uri.EndsWith("/") ? uri : uri + "/";

        }

        /// <summary>
        /// Creates instance of Magentrix REST Wrapper API
        /// </summary>
        /// <param name="uri">URL to the Magentrix instance.</param>
        /// <param name="username">Username REST API should use authenticate</param>
        /// <param name="password">Password REST API should use authenticate</param>
        public REST(string uri, string version)
        {
            if (string.IsNullOrEmpty("uri")) return;
            _uri = uri.EndsWith("/") ? uri : uri + "/";
            _version = version;
        }

        /// <summary>
        /// This method should be called immediately after instantiation of the class to authenticate with Magentrix.
        /// </summary>
        /// <param name="logActivity">If true, this login will be recoded in Login History for the the authenticated user.  If false, login will not be recorded in Login History of the user.</param>
        /// <returns>Returns the Login Result object.  If login is successful, LoginResult.Success will be set to true, otherwise, it will be false.</returns>
        public LoginResult Login(string username, string password)
        {
            try
            {
                _username = username;
                _password = password;
                string url = string.Format("{0}rest/{1}login?un={2}&pw={3}", _uri, VersionPath, Uri.EscapeDataString(_username), Uri.EscapeDataString(_password));
                LoginResult result = Deserialize<LoginResult>(HttpGet(url, null));
                if (result.IsSuccess || !string.IsNullOrEmpty(result.SessionId))
                {
                    _AuthToken = result.SessionId;
                    _sessionIssued = result.DateIssued;
                }
                return result;
            }
            catch (Exception ex)
            {
                var result = new LoginResult();
                result.AddError(ex.Message);
                return result;
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
                string url = string.Format("{0}REST/{1}Query?q={2}", _uri, VersionPath, Uri.EscapeDataString(query));
                return Deserialize<QueryResult<T>>(HttpGet(url, _AuthToken));
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
                string url = string.Format("{0}REST/{1}Delete?id={2}&permanent={3}", _uri, VersionPath, id, permanent);
                return Deserialize<DeleteResult>(HttpGet(url, _AuthToken));
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
                string url = string.Format("{0}REST/{1}Create", _uri, VersionPath);
                string param = string.Format("e={0}&data={1}", typeof(T).Name, Serialize<T>(model));
                return Deserialize<SaveResult>(HttpPost(url, param, _AuthToken));
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
                string url = string.Format("{0}REST/Edit{1}", _uri, VersionPath);
                string param = string.Format("e={0}&data={1}", typeof(T).Name, Serialize<T>(model));
                return Deserialize<SaveResult>(HttpPost(url, param, _AuthToken));
            }
            catch (Exception ex)
            {
                return new SaveResult { Exception = ex };
            }
        }
        
        private string Post(string action, string parameters)
        {
            string post = string.Format("{0}{1}", _uri, action);
            return HttpPost(post, parameters, _AuthToken);
        }

        private string Get(string action)
        {
            string get = string.Format("{0}{1}", _uri, action);
            return HttpGet(get, _AuthToken);
        }

        protected static T Deserialize<T>(string response)
        {
            if (string.IsNullOrEmpty(response)) return Activator.CreateInstance<T>();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(response);
        }

        protected static string Serialize<T>(T model) where T : class
        {
            if (model == null) return null;
            return Newtonsoft.Json.JsonConvert.SerializeObject(model);
        }
    }
}
