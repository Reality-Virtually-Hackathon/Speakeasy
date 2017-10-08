using System;
using UB.SimpleCS.Models;
using UB.SimpleCS.Services;
using UB.Samples.SimpleCSSample.Models;
using UB.SimpleCS;
using UnityEngine;

namespace UB.Samples.SimpleCSSample.Services
{
    /// <summary>
    /// Sample class to call my API for game controller, your services must be implemented this way
    /// </summary>
    public class GameService : BaseService
    {
        public static GameService Instance
        {
            get { return new GameService(); }
        }

        /// <summary>
        /// This method does need a parameter on server side, so I'm calling this as Empty
        /// </summary>
        /// <param name="success">Action for success</param>
        /// <param name="error">Action for error</param>
        /// <param name="validate">Do i want to validate data before send?</param>
        public void TopScores(MonoBehaviour monoBehavior, Action<ScoreDto, PageDto<ScoreDto>> success, 
            Action<Exception> error, bool validate)
        {
            Call<EmptyDto,ScoreDto>(monoBehavior, RequestType.Get, "game/top-scores", null, success, error, validate);
        }

        /// <summary>
        /// Get scores of mine, this will make auth check!
        /// </summary>
        /// <param name="page">Which page-number and what page-size?</param>
       /// <param name="success">Action for success</param>
        /// <param name="error">Action for error</param>
        /// <param name="validate">Do i want to validate data before send?</param>
        public void Scores(MonoBehaviour monoBehavior, PageDto<EmptyDto> page, Action<ScoreDto, 
            PageDto<ScoreDto>> success, Action<Exception> error, bool validate)
        {
            Call(monoBehavior, RequestType.Post, "game/scores", page, success, error, validate);
        }
    }
}