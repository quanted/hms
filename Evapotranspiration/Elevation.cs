using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Evapotranspiration
{
    public class Elevation
    {
        protected string _baseURL = @"http://nationalmap.gov/epqs/pqs.php?x=Longitude&y=Latitude&units=Meters&output=xml";
        protected double _latitude = 0;
        protected double _longitude = 0;
        public Elevation(double latitude, double longitude)
        {
            _latitude = latitude;
            _longitude = longitude;
        }
        public double Latitude
        {
            get { return _latitude; }
        }
        public double Longitude
        {
            get { return _longitude; }
        }
        public double getElevation(out string errorMessage)
        {
            errorMessage = "";
            string strElevation = "";
            string url = _baseURL;
            url = url.Replace("Latitude", _latitude.ToString());
            url = url.Replace("Longitude", _longitude.ToString());
            WebClient client = new WebClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //if the above security protocol does not work then try one of the following - RSP
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
            try
            {
                byte[] bytes = client.DownloadData(url);

                if (bytes != null)
                {
                    string str = Encoding.UTF8.GetString(bytes);
                    if (!str.Contains("<USGS_Elevation_Point_Query_Service>"))
                    {
                        string str1 = @"Failed to retrieve Elevation from NED. " + str;
                        str1 = str1.Replace("?", "");
                        throw new System.Exception(str1);
                    }
                    int intStart = str.IndexOf("<Elevation>");
                    int intEnd = str.IndexOf("</Elevation>");
                    str = str.Substring(intStart);
                    str = str.Remove(0, 11);
                    intStart = str.IndexOf("</Elevation>");
                    strElevation = str.Remove(intStart);
                }
                return Convert.ToDouble(strElevation);
            }
            catch (WebException ex)
            {
                errorMessage = "Web Exception from NED (elevation database): " + ex.Message;
                double badElev = -99999.0;
                return badElev;
            }
            catch (System.Exception ex)
            {
                errorMessage = "System Exception from NED (elevation database): " + ex.Message;
                double badElev = -99999.0;
                return badElev;
            }
        }
    }
}
