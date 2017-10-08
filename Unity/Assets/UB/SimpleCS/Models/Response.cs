using Newtonsoft.Json;

namespace UB.SimpleCS.Models
{
    /// <summary>
    /// Response of the API calls
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Response<T> where T : BaseDto
    {
        /// <summary>
        /// It is true if an error happens
        /// </summary>
        public bool HasError { get; set; }

        /// <summary>
        /// it contains the error description(mostly a const acronym string like "INV_USRNAME" which is "invalid user name" for multi language apps)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }

        /// <summary>
        /// The result. A single entity or a list of T with paging <see cref="PageDto{T}"/> 
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Entity { get; set; }
    }
}