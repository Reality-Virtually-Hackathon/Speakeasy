using System;
using UB.SimpleCS.Models;
using Newtonsoft.Json;

namespace UB.Samples.SimpleCSSample.Models
{
    /// <summary>
    /// A sample class to hold my user login info and pass to API call
    /// </summary>
    public class LoginDto : BaseDto
    {
        public static string UsernamePropertyName = "Username";
        private string _username;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged(UsernamePropertyName);
            }
        }

        public static string PasswordPropertyName = "Password";
        private string _password;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(PasswordPropertyName);
            }
        }

        public static string IsAutoLoginPropertyName = "IsAutoLogin";
        private bool _isAutoLogin;
        public bool IsAutoLogin
        {
            get { return _isAutoLogin; }
            set
            {
                _isAutoLogin = value;
                OnPropertyChanged(IsAutoLoginPropertyName);
            }
        }
        
        /// <summary>
        /// I want to validate the user info before sending to the server, so i overrided this method and written some custom logic
        /// </summary>
        public override void Validate()
        {
            if (String.IsNullOrEmpty(Username))
                throw new Exception("Please type your user name!");
            if (String.IsNullOrEmpty(Password))
                throw new Exception("Please type your password!");
        }
    }
}