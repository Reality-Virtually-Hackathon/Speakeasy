using System.Collections.Generic;
using Newtonsoft.Json;

namespace UB.SimpleCS.Models
{
    /// <summary>
    /// Request entity for api calls
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Request<T> where T : BaseDto
    {
        /// <summary>
        /// Needed for every call (if you are writing secure API's). Holds the stringToken when a user logs into the system and receives a security token. Every call must be done with this key.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TokenDto Token { get; set; }
        /// <summary>
        /// Custom entity if a parameter or a entity needed to pass
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Entity { get; set; }
    }
}