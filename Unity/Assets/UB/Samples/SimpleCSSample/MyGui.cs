using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UB.Samples.SimpleCSSample.Models;
using UB.Samples.SimpleCSSample.Services;
using UB.SimpleCS;
using UB.SimpleCS.Models;
using UnityEngine;
using UnityEngine.UI;

public class MyGui : ExecuteOnMainThread
{
    public InputField Username;
    public InputField Password;
    public Text Result;

    private int scorePage = 1; //to simulate paging on server side
    
    void Start()
    {
        //!!!!!!!!trust certificates if using ssl-untrusted (test) certificates
        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

    }

    protected override void Update()
    {
        base.Update(); //important!!! because base class uses to this execute other functions
        //write your own code below
        //...
    }

    public void Login()
    {
        LoginService.Instance.Login(this,
            new LoginDto { Username = Username.text, Password = Password.text },
            (token, page) =>
            {   
                Execute(()=>{
                    Result.text = "Success, key is: " + token.Key;
                    //save token to our global setting property, so we will use it for calls which needs authentication
                    CSSettings.MyTokenDto = new TokenDto { Key = token.Key };
                });
            },
            (error) =>
            {
                Execute(()=>{
                    Result.text = "Error happened: " + error.Message;
                });
            },
            true); //i want user input validation, so i'm passing true here
    }

    public void MyScores()
    {
        GameService.Instance.Scores(this,
            new PageDto<EmptyDto> { CurrentPage = scorePage, PageSize = 10 }, //page size is static=5 on the server side, so 10 means nothing for this demo
            (score, page) =>
            {
                Execute(()=>{
                    if (page != null && page.Collection != null)
                    {
                        Result.text = "";
                        foreach (var data in page.Collection)
                        {
                            Result.text += data.User + " " + data.Point + Environment.NewLine;
                        }
                    }
                    if (scorePage == 1) //just simulate paging:)
                    {
                        scorePage = 2;
                    } else
                    {
                        scorePage = 1;
                    }
                });
            },
            (error) =>
            {
                Execute(()=>{
                    Result.text = "Error happened: " + error.Message;
                });
            },
            false);
    }

    public void TopScores()
    {
        GameService.Instance.TopScores(this,
            (score, page) =>
            {
                Execute(()=>{
                    if (page != null && page.Collection != null)
                    {
                        Result.text = "";
                        foreach (var data in page.Collection)
                        {
                            Result.text += data.User + " " + data.Point + Environment.NewLine;
                        }
                    }
                });
            },
            (error) =>
            {
                Execute(()=>{
                    Result.text = "Error happened: " + error.Message;
                });
            },
            false);
    }
}
