using System.Collections.Generic;
using System.Threading;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;

namespace Utilities
{
    public class Logger
    {
        /// <summary>
        /// Generic exception logger that returns an object stating unkown error. Used in API controllers
        /// to handle and log exceptions from trycatch blocks, then return a generic error.
        /// </summary>
        public static IActionResult LogAPIException<T>(Exception ex, T input)
        {
            var exceptionLog = Log.ForContext("Type", "exception");
            exceptionLog.Fatal(ex.Message);
            exceptionLog.Fatal(ex.StackTrace);
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };
            exceptionLog.Fatal(System.Text.Json.JsonSerializer.Serialize(input, options));
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();
            return new ObjectResult(err.ReturnError("Unable to complete request due to invalid request or unknown error."));
        }

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
