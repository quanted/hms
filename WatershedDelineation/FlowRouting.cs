using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace WatershedDelineation
{
    public class FlowRouting
    {
        public ITimeSeriesOutput getData(ITimeSeriesInput input, out string errorMsg)
        {
            errorMsg = "";


            //TODO: Remove dummy data and integrate algorithms from Flask endpoint
            ITimeSeriesOutputFactory iFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput flowOutput = iFactory.Initialize();
            while(input.DateTimeSpan.StartDate <= input.DateTimeSpan.EndDate)
            {
                string date = input.DateTimeSpan.StartDate.ToShortDateString();
                List<string> data = new List<string>();
                data.Add("1.234");
                flowOutput.Data.Add(date, data);
                input.DateTimeSpan.StartDate = input.DateTimeSpan.StartDate.AddDays(1.0);
            }
            flowOutput.Dataset = "StreamHydrology";
            return flowOutput;

            //get arguments from input and pass into baseURL

            /*
            string baseURL = "http://localhost:7777/hms/hydrodynamic/constant_volume/?submodel=constant_volume&startDate=2010-01-01&endDate=2010-01-10&timestep=0.5&boundary_flow=0.5&segments=0";

            WebClient myWC = new WebClient();
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();
            string data = "";
            try
            {
                int retries = 5;                                        // Max number of request retries
                string status = "";                                     // response status code

                while (retries > 0 && !status.Contains("OK"))
                {
                    Thread.Sleep(100);
                    WebRequest wr = WebRequest.Create(baseURL);
                    wr.Method = "POST";
                    wr.Timeout = 900000;//15 min
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    data = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    retries -= 1;
                }
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: Unable to obtain data for the specified query." + ex.Message;
            }

            //format Data into ITimeSeriesOutput
            string[] values = data.Split("data");
            char[] totrim = { '}', '\"', '}' };
            string trim = values[2].Substring(3).TrimEnd(totrim);
            List<List<string>> results = JsonConvert.DeserializeObject<List<List<string>>>(trim);
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput flowRoutingOutput = oFactory.Initialize();
            foreach(List<string> flow in results)
            {
                flowRoutingOutput.Data.Add(flow[0], flow);//how to add flow[1:end}?
            }*/
        }
    }
}
