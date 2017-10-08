using System;
using UB.SimpleCS.Models;
using Newtonsoft.Json;

namespace UB.Samples.SimpleCSSample.Models
{
    /// <summary>
    /// A custom class just to hold some score info :)
    /// </summary>
    public class ScoreDto : BaseDto
    {
        public static string UserPropertyName = "User";
        private string _user;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged(UserPropertyName);
            }
        }
        
        public static string PointPropertyName = "Point";
        private Int32 _point;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Int32 Point
        {
            get { return _point; }
            set
            {
                _point = value;
                OnPropertyChanged(PointPropertyName);
            }
        }
    }
}