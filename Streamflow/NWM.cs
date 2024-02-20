using Data;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Streamflow
{
    public class NWM
    {
        public ITimeSeriesOutput<List<double>> GetData(out string errorMsg, ITimeSeriesOutput<List<double>> output, ITimeSeriesInput input, int retries = 0)
        {
            errorMsg = "";

            string comids = input.Geometry.ComID.ToString();
            if (input.Geometry.GeometryMetadata.ContainsKey("comids"))
            {
                comids = comids + "," + input.Geometry.GeometryMetadata["comids"];
            }

            string waterbody = "false";
            if (input.Geometry.GeometryMetadata.ContainsKey("waterbody"))
            {
                waterbody = input.Geometry.GeometryMetadata["waterbody"];
            }

            string dataRequest = "/hms/nwm/data/?dataset=streamflow&comid=" + comids;
            dataRequest += "&startDate=" + input.DateTimeSpan.StartDate.ToString("yyyy-MM-dd");
            dataRequest += "&endDate=" + input.DateTimeSpan.EndDate.ToString("yyyy-MM-dd");
            dataRequest += "&waterbody=" + waterbody;

            FlaskData<TimeSeriesOutput<List<double>>> results = Utilities.WebAPI.RequestData<FlaskData<TimeSeriesOutput<List<double>>>>(dataRequest, 1000).Result;
            if ((results == null) || (results.data == null)) {
                errorMsg = "ERROR: Unable to complete NWM Flask Data Request for " + comids;  // JSC 9/7/2022
                return null; 
            }

            // Remote request test
            //string flaskURL = "https://ceamdev.ceeopdev.net/hms/rest/api/v2/hms/nwm/data/?";
            //dataRequest = "dataset=streamflow&comid=" + comids;
            //dataRequest += "&startDate=" + input.DateTimeSpan.StartDate.ToString("yyyy-MM-dd");
            //dataRequest += "&endDate=" + input.DateTimeSpan.EndDate.ToString("yyyy-MM-dd");
            //string dataURL = "https://ceamdev.ceeopdev.net/hms/rest/api/v2/hms/data";
            //FlaskData<TimeSeriesOutput<List<double>>> results = Utilities.WebAPI.RequestData<FlaskData<TimeSeriesOutput<List<double>>>>(dataRequest, 1000, flaskURL, dataURL).Result;

            try
            {
                output = results.data;
                output.Metadata.Add("comids", comids);
                if (input.TemporalResolution.ToLower() == "daily")
                {
                    output.Data = output.ToDaily(input.DateTimeSpan.DateTimeFormat, input, true, false);
                    output.Metadata.Add("temporal_timestep", "daily");
                }
                else if (input.TemporalResolution.ToLower() == "monthly")
                {
                    output.Data = output.ToMonthly(input.DateTimeSpan.DateTimeFormat, input, true, false);
                    output.Metadata.Add("temporal_timestep", "monthly");
                }
                else
                {
                    output.Metadata.Add("temporal_timestep", "hourly");
                }
                return output;
            }
            catch
            {
                errorMsg = "ERROR: Unable to read results from NWM Flask Data Request for " + comids;  // JSC 2/13/2024
                return null;
            }

            }
        }
}
