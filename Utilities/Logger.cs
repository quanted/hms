using System.Collections.Generic;
using System.Threading;

namespace Utilities
{
    public class Logger
    {
        public static void WriteToFile(string taskID, string log)
        {
            string filePath = "App_Data\\workflow_" + taskID + ".txt";
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, true))
                {
                    file.WriteLine(log);
                }
            }
            catch(System.IO.IOException ex){
                Thread.Sleep(1000);
                WriteToFile(taskID, log);
            }
        }

        public static void WriteToFile(string taskID, List<string> log)
        {
            string filePath = "App_Data\\workflow_" + taskID + ".txt";
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, true))
                {
                    foreach (string l in log)
                    {
                        file.WriteLine(l);
                    }
                    file.Close();
                }
            }
            catch (System.IO.IOException ex)
            {
                Thread.Sleep(1000);
                WriteToFile(taskID, log);
            }
        }
    }
}
