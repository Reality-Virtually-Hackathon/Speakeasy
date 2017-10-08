using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Timers;
using System.Linq;

public class ExecuteOnMainThread : MonoBehaviour {

    private List<ThreadAction> _actions = new List<ThreadAction>();

    public static object Lock = new object();

    protected void Execute(Action action, string groupName = "", float delay=0f, bool stopOnTimeScale = false)
    {
        //Debug.Log("Added item to thread with delay: " + delay);
        lock (Lock)
        {
            var neww = new ThreadAction
            {
                Action = action,
                IsReady = false,
                StopOnTimeScale = stopOnTimeScale,
                GroupName = groupName
            };

            if (delay <= 0f)
            {
                neww.IsReady = true;
            } else
            {
                neww.Start(delay);   
            }

            _actions.Add(neww);
        }
    }

    protected void CancelExecute(string groupName)
    {
        lock (Lock)
        {
            var copy = _actions.ToList();
            for (int i = 0; i < copy.Count; i++)
            {
                if (copy [i].GroupName == groupName)
                {
                    _actions.Remove(copy [i]);
                    //Debug.Log("removed action: " + copy [i].GroupName);
                }
            }
        }
    }

    protected virtual void Update()
    {
        lock (Lock)
        {
            var copy = _actions.ToList();
            if (Time.timeScale > 0f)
            {
                for (int i = 0; i < copy.Count; i++)
                {
                    copy [i].Continue();
                }
            } 
            else
            {
                for (int i = 0; i < copy.Count; i++)
                {
                    if (copy[i].StopOnTimeScale)
                        copy [i].Pause();
                }
            }

            for (int i = 0; i < copy.Count; i++)
            {
                if (copy [i].IsReady)
                {
                    copy [i].Action.Invoke();
                    _actions.Remove(copy [i]);
                }
            }
        }
    }
}

internal class ThreadAction{
    public Action Action
    {
        get;
        set;
    }
    public bool IsReady
    {
        get;
        set;
    }
    public bool StopOnTimeScale
    {
        get;
        set;
    }
    public string GroupName
    {
        get;
        set;
    }
    private Timer _timer;

    public void Start(float delay)
    {
        _timer = new Timer(delay);
        _timer.Elapsed += (o,e) =>
        {
            _timer.Stop();
            lock (ExecuteOnMainThread.Lock)
            {
                IsReady = true;
            }
        };
        _timer.Start();
    }
    public void Pause()
    {
        if (_timer != null && _timer.Enabled)
        {
            _timer.Enabled = false;
        }
    }
    public void Continue()
    {
        if (_timer != null && !_timer.Enabled)
        {
            _timer.Enabled = true;
        }
    }
}