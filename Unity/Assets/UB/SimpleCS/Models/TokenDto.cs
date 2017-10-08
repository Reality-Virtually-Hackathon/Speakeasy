using Newtonsoft.Json;

namespace UB.SimpleCS.Models
{
    /// <summary>
    /// A custom class for simple security, you can write and change this if you need another approaches
    /// </summary>
    public class TokenDto : BaseDto
    {
        public static string KeyPropertyName = "Key";
        private string _key;
        /// <summary>
        /// A unique and unpredictable security key (server creates this key for your login information -mostly username and password- and sends to you, so for other API calls you can use this key to identify your self)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Key
        {
            get { return _key; }
            set
            {
                _key = value;
                OnPropertyChanged(KeyPropertyName);
            }
        }
        
    }
}