using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;

public class Master_Client : MonoBehaviour
{
    public static Master_Client inst;
    public List<Func<bool>> Tasks = new List<Func<bool>>();
    public List<String> logs = new List<string>();
    public ClientProperties cp;

    void Awake()
    {
        if (!inst)
        {
            inst = this;
            DontDestroyOnLoad(this.gameObject);
            inst.gameObject.name = "Program (Master)";
        }
        else
        {
            if (inst != this)
                Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        if (!cp.IsSet)
        {
            Log("Starting Up!!!");
            cp = new ClientProperties(true);
        }
    }
    void Update()
    {
        if (Tasks.Count > 0)
        {
            for (int i = 0; i < Tasks.Count - 1; i++)
            {
                Tasks[i]();
                Log("Task was executed");
                Tasks.RemoveAt(i);
            }
        }

        if (logs.Count > 0)
        {
            for (int i = 0; i < logs.Count; i++)
            {
                Debug.Log(logs[i]);
                logs.RemoveAt(i);
            }
        }
    }

    public void Task(Func<bool> func) { inst.Tasks.Add(func); }
    public void Log(string message) { logs.Add(message); }
    public void Log(string format, params object[] args)
    {
        string result = string.Format(format, args);
        logs.Add(result);
    }
}