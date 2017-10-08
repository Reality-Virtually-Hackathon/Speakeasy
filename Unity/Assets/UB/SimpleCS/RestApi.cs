using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using UB.SimpleCS.Models;
using UnityEngine.Networking;
using UnityEngine;

namespace UB.SimpleCS
{
    /// <summary>
    /// Base class to make web calls
    /// </summary>
    public class RestApi
    {
        public static RestApi Instance
        {
            get { return new RestApi(); }
        }
        
        public string Send<T>(RequestType apiRequestType, string url, T entity) where T : BaseDto
        {
            var request = new HttpWebRequest(new Uri(url));
            request.Timeout = 20000; //milliseconds
            if (apiRequestType == RequestType.Get)
            {
                request.Method = "GET";
            }
            else
            {
                request.Method = "POST";
            }

            if (entity != null)
            {
                var dataToSend = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new Request<T>
                {
                    Token = CSSettings.MyTokenDto, //automatically send token info for every call
                    Entity = entity
                }));
                request.ContentType = "application/json";
                request.ContentLength = dataToSend.Length;
                request.GetRequestStream().Write(dataToSend, 0, dataToSend.Length);
            }

            var response = request.GetResponse();

            //HttpResponseMessage response = client.PostAsync(url, content).Result;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                string responseString = reader.ReadToEnd();
                //just trying to convert response to my helper empty dto, so i can understand if any error happened:)
                var error = JsonConvert.DeserializeObject<Response<EmptyDto>>(responseString);
                if (error != null && error.HasError)
                {
                    throw new Exception(error.Error); //throw error message
                }
                return responseString;
            }
        }
    }
}
