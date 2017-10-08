using System;
using Newtonsoft.Json;
using UB.SimpleCS.Models;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Text;


#if !UNITY_WEBGL
using System.Threading;
#endif

namespace UB.SimpleCS.Services
{
    /// <summary>
    /// Base service for all API calls
    /// </summary>
    public abstract class BaseService
    {
        /// <summary>
        /// Makes a web request and calls API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="requestType">Request type of the api call <see cref="RequestType"/></param>
        /// <param name="path">Path of the api call</param>
        /// <param name="entity">Paramater if a single entity will be send to server<see cref="BaseDto"/></param>
        /// <param name="success">Action method that will be called on success</param>
        /// <param name="error">Action method that will called on error</param>
        /// <param name="validate"></param>
        protected void Call<T, K>(
            MonoBehaviour monoBehavior,
            RequestType requestType,
            string path,
            T entity,
            Action<K, PageDto<K>> success,
            Action<Exception> error,
            bool validate = false) where T : BaseDto where K : BaseDto
        {
            #if !UNITY_WEBGL
            Thread call = new Thread(()=>WebCallForThread(requestType,path,entity,success,error,validate));
            call.Start();
            #endif
            #if UNITY_WEBGL
            monoBehavior.StartCoroutine(WebCallForCoroutine(requestType,path,entity,success,error,validate));
            #endif
        }

        void WebCallForThread<T, K>(
            RequestType requestType,
            string path,
            T entity,
            Action<K, PageDto<K>> success,
            Action<Exception> error,
            bool validate = false) where T : BaseDto where K : BaseDto
        {
            try
            {
                if (validate && entity != null)
                {
                    entity.Validate();
                }

                var response = RestApi.Instance.Send(requestType, CSSettings.ApiPath + path, entity);

                var singleK = JsonConvert.DeserializeObject<Response<K>>(response);
                var pageK = JsonConvert.DeserializeObject<Response<PageDto<K>>>(response);

                if (success != null)
                {
                    success.Invoke(singleK.Entity, pageK.Entity);
                }
            }
            catch (Exception ex) //a webcall exception occured or your service returned an error
            {
                if (error != null)
                {
                    error.Invoke(ex);
                }
            }
        }

        IEnumerator WebCallForCoroutine<T, K>(
            RequestType requestType,
            string path,
            T entity,
            Action<K, PageDto<K>> successAction,
            Action<Exception> errorAction,
            bool validate = false) where T : BaseDto where K : BaseDto
        {
            if (validate && entity != null)
            {
                entity.Validate();
            }

            var request = new UnityWebRequest(CSSettings.ApiPath + path);
            if (requestType == RequestType.Get)
            {
                request.method = "GET";
            }
            else
            {
                request.method = "POST";
            }

            if (entity != null)
            {
                var dataToSend = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new Request<T>
                {
                    Token = CSSettings.MyTokenDto, //automatically send token info for every call
                    Entity = entity
                }));
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(dataToSend);
                request.SetRequestHeader("Content-Type", "application/json");
            }
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            //Debug.Log("send starts...");

            yield return request.Send();
            while (!request.isDone)
                yield return new WaitForSeconds(0.01f);

//            Debug.Log("send finished...");
//            Debug.Log("responseCode: " + request.responseCode);
//            Debug.Log("error: " + request.error);

            try
            {
                string response = request.downloadHandler.text;
                //Debug.Log("Response: "+response);
                //just trying to convert response to my helper empty dto, so i can understand if any error happened:)
                var error = JsonConvert.DeserializeObject<Response<EmptyDto>>(response);
                if (error != null && error.HasError)
                {
                    throw new Exception(error.Error); //throw error message
                }

                var singleK = JsonConvert.DeserializeObject<Response<K>>(response);
                var pageK = JsonConvert.DeserializeObject<Response<PageDto<K>>>(response);

                if (successAction != null)
                {
                    successAction.Invoke(singleK.Entity, pageK.Entity);
                }
            }
            catch (Exception ex) //a webcall exception occured or your service returned an error
            {
                if (errorAction != null)
                {
                    errorAction.Invoke(ex);
                }
            }

            yield break;
        }
    }
}
