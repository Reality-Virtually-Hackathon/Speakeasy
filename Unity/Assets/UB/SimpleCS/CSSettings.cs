using UB.SimpleCS.Models;

namespace UB.SimpleCS
{
    /// <summary>
    /// This class simply holds your settings, you have to change it according to your needs!
    /// </summary>
    public static class CSSettings {
        /// <summary>
        /// Your server api path //TODO: change with your ip and port
        /// </summary>
        public static string ApiPath = @"http://192.168.1.100/MyGameApi/";
        //public static string ApiPath = @"https://ip:port/";   //if secure !!! do not forget to use https

        /// <summary>
        /// If you want to make your calls secure(of course with ssl) you can use your own tokens, this token will be send to server via every call
        /// </summary>
        public static TokenDto MyTokenDto;
    }
}
