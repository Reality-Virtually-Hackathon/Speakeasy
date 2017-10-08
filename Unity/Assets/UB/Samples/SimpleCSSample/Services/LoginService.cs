using System;
using UB.SimpleCS.Models;
using UB.SimpleCS.Services;
using UB.Samples.SimpleCSSample.Models;
using UB.SimpleCS;
using UnityEngine;

namespace UB.Samples.SimpleCSSample.Services
{
    /// <summary>
    /// Sample class to call my API for login controller, your services must be implemented this way
    /// </summary>
    public class LoginService : BaseService
    {
        public static LoginService Instance
        {
            get { return new LoginService(); }
        }

        /// <summary>
        /// I want to send my login information (<see cref="LoginDto"/> class) and expecting a token information from
        /// server (if username and password is correct). 
        /// </summary>
        /// <param name="loginDto">My login information holder class</param>
        /// <param name="success">Action for success</param>
        /// <param name="error">Action for error</param>
        /// <param name="validate">Do i want to validate data before send?</param>
        public void Login(MonoBehaviour monoBehavior, LoginDto loginDto, Action<TokenDto, 
            PageDto<TokenDto>> success, Action<Exception> error, bool validate)
        {
            Call(monoBehavior, RequestType.Post, "login/authenticate", loginDto, success, error, validate);
        }
    }
}