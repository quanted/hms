/*
 * 
Test method retrieves temperature, precipitation, humidity, zonal wind, meridional wind, 
shortwave radiation, and longwave radiation.

5 total data points are provided, the central point that is given by the latitude/longitude variables, 
and the four surrounding data points.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSCompleteDataSet
{
    public class CompleteDataSet
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public double gmtOffset { get; set; }    //Timezone offset from GMT   
        public string tzName { get; set; }       //Timezone name
        public string dataSource { get; set; }
        public bool localTime { get; set; }
        public List<HMSTimeSeries.HMSTimeSeries> ts { get; set; }
        public List<HMSTimeSeries.HMSTimeSeries> precipTS { get; set; }
        public List<HMSTimeSeries.HMSTimeSeries> humidityTS { get; set; }
        public List<HMSTimeSeries.HMSTimeSeries> tempTS { get; set; }
        public List<HMSTimeSeries.HMSTimeSeries> zonalWindTS { get; set; }
        public List<HMSTimeSeries.HMSTimeSeries> meridWindTS { get; set; }
        public List<HMSTimeSeries.HMSTimeSeries> shortwaveRadTS { get; set; }
        public List<HMSTimeSeries.HMSTimeSeries> longwaveRadTS { get; set; }
        public double cellWidth { get; set; }
        public string shapefilePath { get; set; }
        public HMSGDAL.HMSGDAL gdal { get; set; }

        public CompleteDataSet() { }

        /// <summary>
        /// Constructor using a shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <param name="local"></param>
        /// <param name="sfPath"></param>
        public CompleteDataSet(out string errorMsg, string startDate, string endDate, string source, bool local, string sfPath)
        {
            errorMsg = "";
            if (String.IsNullOrWhiteSpace(startDate) || String.IsNullOrWhiteSpace(endDate) || String.IsNullOrWhiteSpace(source) || String.IsNullOrWhiteSpace(sfPath)) { errorMsg = "ERROR: Required arguments are missing."; return; }
            this.gmtOffset = 0.0;
            this.dataSource = source;
            this.localTime = local;
            this.tzName = "GMT";
            SetDates(out errorMsg, startDate, endDate);
            if (errorMsg.Contains("ERROR")) { return; }
            this.ts = new List<HMSTimeSeries.HMSTimeSeries>();
            this.precipTS = new List<HMSTimeSeries.HMSTimeSeries>();
            this.humidityTS = new List<HMSTimeSeries.HMSTimeSeries>();
            this.tempTS = new List<HMSTimeSeries.HMSTimeSeries>();
            this.zonalWindTS = new List<HMSTimeSeries.HMSTimeSeries>();
            this.meridWindTS = new List<HMSTimeSeries.HMSTimeSeries>();
            this.shortwaveRadTS = new List<HMSTimeSeries.HMSTimeSeries>();
            this.longwaveRadTS = new List<HMSTimeSeries.HMSTimeSeries>();
            this.shapefilePath = sfPath.Substring(0, sfPath.LastIndexOf('.'));
            if (this.dataSource == "NLDAS") { this.cellWidth = 0.12500; }
            else if (this.dataSource == "GLDAS") { this.cellWidth = 0.2500; }
            else { errorMsg = "ERROR: Invalid source."; return; }
            this.gdal = new HMSGDAL.HMSGDAL();
        }

        /// <summary>
        /// Constructor using latitude and longitude.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <param name="sfPath"></param>
        public CompleteDataSet(out string errorMsg, string latitude, string longitude, string startDate, string endDate, string source, bool local, string sfPath)
        {
            errorMsg = "";
            if (String.IsNullOrWhiteSpace(latitude) || String.IsNullOrWhiteSpace(longitude) || String.IsNullOrWhiteSpace(startDate) || String.IsNullOrWhiteSpace(endDate) || String.IsNullOrWhiteSpace(source)) { errorMsg = "ERROR: Required arguments are missing."; return; }
            this.gmtOffset = 0.0;
            this.dataSource = source;
            this.localTime = local;
            this.tzName = "GMT";
            this.latitude = ConvertStringToDouble(out errorMsg, latitude);
            this.longitude = ConvertStringToDouble(out errorMsg, longitude);
            if (errorMsg.Contains("ERROR")) { return; }
            SetDates(out errorMsg, startDate, endDate);
            if (errorMsg.Contains("ERROR")) { return; }
            this.ts = new List<HMSTimeSeries.HMSTimeSeries>();
            this.precipTS = new List<HMSTimeSeries.HMSTimeSeries>();
            this.humidityTS = new List<HMSTimeSeries.HMSTimeSeries>();
            this.tempTS = new List<HMSTimeSeries.HMSTimeSeries>();
            this.zonalWindTS = new List<HMSTimeSeries.HMSTimeSeries>();
            this.meridWindTS = new List<HMSTimeSeries.HMSTimeSeries>();
            this.shortwaveRadTS = new List<HMSTimeSeries.HMSTimeSeries>();
            this.longwaveRadTS = new List<HMSTimeSeries.HMSTimeSeries>();
            this.shapefilePath = null;
            if (this.dataSource == "NLDAS") { this.cellWidth = 0.12500; }
            else if (this.dataSource == "GLDAS") { this.cellWidth = 0.2500; }
            else { errorMsg = "ERROR: Invalid source."; return; }
            this.gdal = new HMSGDAL.HMSGDAL();
        }

        /// <summary>
        /// Method first checks if the string is numeric then attemps to convert to a double.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        private double ConvertStringToDouble(out string errorMsg, string str)
        {
            errorMsg = "";
            double result = 0.0;
            if (Double.TryParse(str, out result))
            {
                try
                {
                    return result = Convert.ToDouble(str);
                }
                catch
                {
                    errorMsg = "ERROR: Unable to convert string value to double.";
                    return result;
                }
            }
            else
            {
                errorMsg = "ERROR: Coordinates contain invalid characters.";
                return result;
            }
        }

        /// <summary>
        /// Sets startDate and endDate, checks that dates are valid (start date before end date, end date no greater than today, start dates are valid for data sources)
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void SetDates(out string errorMsg, string start, string end)
        {
            errorMsg = "";
            try
            {
                this.startDate = Convert.ToDateTime(start);
                this.endDate = Convert.ToDateTime(end);
                //Add Hours to start/end dates
                DateTime newStartDate = new DateTime(this.startDate.Year, this.startDate.Month, this.startDate.Day, 00, 00, 00);
                DateTime newEndDate = new DateTime(this.endDate.Year, this.endDate.Month, this.endDate.Day, 23, 00, 00);
                this.startDate = newStartDate;
                this.endDate = newEndDate;
            }
            catch
            {
                errorMsg = "ERROR: Invalid data format. Please provide a date as mm-dd-yyyy or mm/dd/yyyy.";
                return;
            }
            if (DateTime.Compare(this.endDate, DateTime.Today) > 0)   //If endDate is past today's date, endDate is set to 2 days prior to today.
            {
                this.endDate = DateTime.Today.AddDays(-2.0);
            }
            if (DateTime.Compare(this.startDate, this.endDate) > 0)
            {
                errorMsg = "ERROR: Invalid dates entered. Please enter an end date set after the start date.";
                return;
            }
            if (this.dataSource.Contains("NLDAS"))   //NLDAS data collection start date
            {
                DateTime minDate = new DateTime(1979, 01, 02);
                if (DateTime.Compare(this.startDate, minDate) < 0)
                {
                    this.startDate = minDate;   //start date is set to NLDAS start date
                }
            }
            else if (this.dataSource.Contains("GLDAS"))   //GLDAS data collection start date
            {
                DateTime minDate = new DateTime(2000, 02, 25);
                if (DateTime.Compare(this.startDate, minDate) < 0)
                {
                    this.startDate = minDate;   //start date is set to GLDAS start date
                }
            }
        }

        /// <summary>
        /// Copies TotalFlow variable values to BaseFlow variables.
        /// </summary>
        /// <param name="newSurfaceFlow"></param>
        //private void InitializeTimeSeries()
        //{
        //    newSurfaceFlow.latitude = this.latitude;
        //    newSurfaceFlow.longitude = this.longitude;
        //    newSurfaceFlow.startDate = this.startDate;
        //    newSurfaceFlow.endDate = this.endDate;
        //    newSurfaceFlow.gmtOffset = this.gmtOffset;
        //    newSurfaceFlow.tzName = this.tzName;
        //    newSurfaceFlow.dataSource = this.dataSource;
        //    newSurfaceFlow.localTime = this.localTime;
        //    newSurfaceFlow.ts = new List<HMSTimeSeries.HMSTimeSeries>();
        //    newSurfaceFlow.cellWidth = this.cellWidth;
        //    newSurfaceFlow.shapefilePath = this.shapefilePath;
        //    newSurfaceFlow.gdal = this.gdal;
        //}

        /// <summary>
        /// Gets complete data set.
        /// </summary>
        /// <param name="errorMsg"></param>
        public List<HMSTimeSeries.HMSTimeSeries> GetDataSets(out string errorMsg)
        {
            errorMsg = "";
            HMSLDAS.HMSLDAS gldas = new HMSLDAS.HMSLDAS();
            HMSGDAL.HMSGDAL gdal = new HMSGDAL.HMSGDAL();
            double offset = gmtOffset;

            HMSTimeSeries.HMSTimeSeries newTS = new HMSTimeSeries.HMSTimeSeries();
            ts.Add(newTS);

            bool sourceNLDAS = true;
            if (this.dataSource.Contains("GLDAS")) { sourceNLDAS = false; }
            double[] center = gldas.DetermineReturnCoordinates(out errorMsg, new double[] { latitude, longitude } , sourceNLDAS);
            this.latitude = center[0];
            this.longitude = center[1];

            if (this.localTime == true && offset == 0.0)
            {
                this.gmtOffset = gdal.GetGMTOffset(out errorMsg, this.latitude, this.longitude, ts[0]);    //Gets the GMT offset
                if (errorMsg.Contains("ERROR")) { return null; }
                this.tzName = ts[0].tzName;              //Gets the Timezone name
                if (errorMsg.Contains("ERROR")) { return null; }
                this.startDate = gdal.AdjustDateByOffset(out errorMsg, this.gmtOffset, this.startDate, true);
                this.endDate = gdal.AdjustDateByOffset(out errorMsg, this.gmtOffset, this.endDate, false);
            }

            HMSTimeSeries.HMSTimeSeries newPrecipTS;
            HMSTimeSeries.HMSTimeSeries newHumidTS;
            HMSTimeSeries.HMSTimeSeries newTempTS;
            HMSTimeSeries.HMSTimeSeries newLongWTS;
            HMSTimeSeries.HMSTimeSeries newShortWTS;
            HMSTimeSeries.HMSTimeSeries newZonalWTS;
            HMSTimeSeries.HMSTimeSeries newMeridWTS;
            string dataPrecip = "";
            string dataHumid = "";
            string dataTemp = "";
            string dataLongW = "";
            string dataShortW = "";
            string dataZonalW = "";
            string dataMeridW = "";
            int[] mod = new int[] { 0, 0, 0, 1, -1, 0, 0, -1, 1, 0 };

            for (int i = 0; i < mod.Length; i += 2)
            {

                newTS = new HMSTimeSeries.HMSTimeSeries();
                newPrecipTS = new HMSTimeSeries.HMSTimeSeries();
                newHumidTS = new HMSTimeSeries.HMSTimeSeries();
                newTempTS = new HMSTimeSeries.HMSTimeSeries();
                newLongWTS = new HMSTimeSeries.HMSTimeSeries();
                newShortWTS = new HMSTimeSeries.HMSTimeSeries();
                newZonalWTS = new HMSTimeSeries.HMSTimeSeries();
                newMeridWTS = new HMSTimeSeries.HMSTimeSeries();

               
                precipTS.Add(newPrecipTS);
                humidityTS.Add(newHumidTS);
                tempTS.Add(newTempTS);
                zonalWindTS.Add(newZonalWTS);
                meridWindTS.Add(newMeridWTS);
                shortwaveRadTS.Add(newShortWTS);
                longwaveRadTS.Add(newLongWTS);
                
                if (dataSource.Contains("NLDAS"))
                {
                    dataPrecip = gldas.LDAS(out errorMsg, (center[0] + (mod[i] * this.cellWidth)), (center[1] + (mod[i + 1] * this.cellWidth)), startDate, endDate, "NLDAS_Precip", newPrecipTS, shapefilePath);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    precipTS[precipTS.Count-1].SetTimeSeriesVariables(out errorMsg, newPrecipTS, dataPrecip, dataSource);
                    if (errorMsg.Contains("ERROR")) { return null; }

                    dataHumid = gldas.LDAS(out errorMsg, (center[0] + (mod[i] * this.cellWidth)), (center[1] + (mod[i + 1] * this.cellWidth)), startDate, endDate, "NLDAS_Humidity", newHumidTS, shapefilePath);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    humidityTS[humidityTS.Count-1].SetTimeSeriesVariables(out errorMsg, newHumidTS, dataHumid, dataSource);
                    if (errorMsg.Contains("ERROR")) { return null; }

                    dataTemp = gldas.LDAS(out errorMsg, (center[0] + (mod[i] * this.cellWidth)), (center[1] + (mod[i + 1] * this.cellWidth)), startDate, endDate, "NLDAS_Temp", newTempTS, shapefilePath);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    tempTS[tempTS.Count-1].SetTimeSeriesVariables(out errorMsg, newTempTS, dataTemp, dataSource);
                    if (errorMsg.Contains("ERROR")) { return null; }

                    dataLongW = gldas.LDAS(out errorMsg, (center[0] + (mod[i] * this.cellWidth)), (center[1] + (mod[i + 1] * this.cellWidth)), startDate, endDate, "NLDAS_Longwave_Rad", newLongWTS, shapefilePath);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    longwaveRadTS[longwaveRadTS.Count-1].SetTimeSeriesVariables(out errorMsg, newLongWTS, dataLongW, dataSource);
                    if (errorMsg.Contains("ERROR")) { return null; }

                    dataShortW = gldas.LDAS(out errorMsg, (center[0] + (mod[i] * this.cellWidth)), (center[1] + (mod[i + 1] * this.cellWidth)), startDate, endDate, "NLDAS_Shortwave_Rad", newShortWTS, shapefilePath);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    shortwaveRadTS[shortwaveRadTS.Count-1].SetTimeSeriesVariables(out errorMsg, newShortWTS, dataShortW, dataSource);
                    if (errorMsg.Contains("ERROR")) { return null; }

                    dataZonalW= gldas.LDAS(out errorMsg, (center[0] + (mod[i] * this.cellWidth)), (center[1] + (mod[i + 1] * this.cellWidth)), startDate, endDate, "NLDAS_Zonal_Wind", newZonalWTS, shapefilePath);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    zonalWindTS[zonalWindTS.Count-1].SetTimeSeriesVariables(out errorMsg, newZonalWTS, dataZonalW, dataSource);
                    if (errorMsg.Contains("ERROR")) { return null; }

                    dataMeridW = gldas.LDAS(out errorMsg, (center[0] + (mod[i] * this.cellWidth)), (center[1] + (mod[i + 1] * this.cellWidth)), startDate, endDate, "NLDAS_Merid_Wind", newMeridWTS, shapefilePath);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    meridWindTS[meridWindTS.Count-1].SetTimeSeriesVariables(out errorMsg, newMeridWTS, dataMeridW, dataSource);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else
                {
                    errorMsg = "ERROR: Invalid data source selected.";
                    return null;
                }
                SetTotalFlowTimeSeries(out errorMsg, newTS);
            }
            
            if (errorMsg.Contains("ERROR")) { return null; }

            return ts;
        }

        /// <summary>
        /// Combines both land surface flow and base flow data to calculate total flow.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="newTS"></param>
        private void SetTotalFlowTimeSeries(out string errorMsg, HMSTimeSeries.HMSTimeSeries newTS)
        {
            errorMsg = "";
            try
            {
                newTS = new HMSTimeSeries.HMSTimeSeries();
                string data = "";

                string[] precipData = precipTS[precipTS.Count-1].timeSeries.Split('\n').Select(s => s.Trim()).ToArray();
                string[] humidityData = humidityTS[humidityTS.Count-1].timeSeries.Split('\n').Select(s => s.Trim()).ToArray();
                string[] tempData = tempTS[tempTS.Count-1].timeSeries.Split('\n').Select(s => s.Trim()).ToArray();
                string[] zonalWData = zonalWindTS[zonalWindTS.Count-1].timeSeries.Split('\n').Select(s => s.Trim()).ToArray();
                string[] meridWData = meridWindTS[meridWindTS.Count-1].timeSeries.Split('\n').Select(s => s.Trim()).ToArray();
                string[] shortWData = shortwaveRadTS[shortwaveRadTS.Count-1].timeSeries.Split('\n').Select(s => s.Trim()).ToArray();
                string[] longWData = longwaveRadTS[longwaveRadTS.Count-1].timeSeries.Split('\n').Select(s => s.Trim()).ToArray();
                
                for (int i = 0; i < precipData.Length - 1; i++)
                {
                    string[] precipDataArrayLine = precipData[i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string[] humidDataArrayLine = humidityData[i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string[] tempDataArrayLine = tempData[i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string[] zonalWDataArrayLine = zonalWData[i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string[] meridWDataArrayLine = meridWData[i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string[] shortWDataArrayLine = shortWData[i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string[] longWDataArrayLine = longWData[i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    
                    data += precipDataArrayLine[0] + " " + precipDataArrayLine[1] + ", " + precipDataArrayLine[2] + ", " + humidDataArrayLine[2] + ", " + tempDataArrayLine[2] + ", " + zonalWDataArrayLine[2] + ", " + meridWDataArrayLine[2] + ", " + shortWDataArrayLine[2] + ", " + longWDataArrayLine[2] + "\n";
                }
                if (precipTS.Count > 1) { ts.Add(newTS); }
                ts[ts.Count - 1].SetTimeSeriesVariables(out errorMsg, newTS, String.Concat(precipTS[precipTS.Count - 1].metaData, "\n\n     Date&Time     Data\n", data, "MEAN"), "NLDAS");
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: " + ex;
                return;
            }
        }

    }
}
