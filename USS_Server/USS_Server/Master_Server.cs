using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USS_Server
{
    public class Master_Server
    {
        public static Master_Server inst;
        public List<Func<bool>> Tasks = new List<Func<bool>>();
        public List<String> logs = new List<string>();
        public static List<ClientProperties> Clients = new List<ClientProperties>();
        static void Main(string[] args)
        {
            inst = new Master_Server();
            inst.Start();
        }

        public void Start()
        {
            ServerProperties.New();
            while (true)
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
                        Console.WriteLine(logs[i]);
                        logs.RemoveAt(i);
                    }
                }
            }
        }
        public void Task(Func<bool> func) { Tasks.Add(func); }
        public void Log(string message) { logs.Add(message); }
        public void Log(string format, params object[] args)
        {
            string result = string.Format(format, args);
            logs.Add(result);
        }
    }
}
