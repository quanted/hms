using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using GeoAPI.Geometries;
using ProjNet.CoordinateSystems;
using GeoAPI.CoordinateSystems;
using ProjNet.Converters.WellKnownText;
using ProjNet.CoordinateSystems.Transformations;
using GeoAPI.CoordinateSystems.Transformations;
using NetTopologySuite.Geometries;
using NetTopologySuite.Precision;
using NetTopologySuite.Features;

namespace HMSGDAL
{
    public class HMSGDAL
    {
  
        private List<TimeZones> geotimezones = new List<TimeZones>();
        private List<NetTopologySuite.Features.Feature> featureCollection = new List<NetTopologySuite.Features.Feature>();
        private static string geometryName;
        public List<Tuple<double,double>> coordinatesInShapefile;
        public List<double> areaPrecentInGeometry;
        public List<double> areaGeometryIntersection;
        public string geoJSON { get; set; }
        private class TimeZones
        {
            public string timezone { get; set; }
            public GeoAPI.Geometries.IGeometry[] geometry { get; set; }
        }

        public HMSGDAL() { }

        public HMSGDAL(out string errorMsg, string shapeFile)
        {
            errorMsg = "";
            SetShapeFile(out errorMsg, shapeFile);
            if (errorMsg.Contains("ERROR")) { return; }
        }

        /// <summary>
        /// Loads timezones using shapefile of timezones. (tz_world)
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="shapeFile"></param>
        private void SetShapeFile(out string errorMsg, string shapeFileName)
        {
            errorMsg = "";
            try
            {
                List<NetTopologySuite.Features.Feature> featureCollection = new List<NetTopologySuite.Features.Feature>();

                //built feature collection from shapefile
                
                NetTopologySuite.IO.ShapefileDataReader dataReader = new NetTopologySuite.IO.ShapefileDataReader(shapeFileName, new NetTopologySuite.Geometries.GeometryFactory());

                while (dataReader.Read())
                {
                    //read current feature
                    NetTopologySuite.Features.Feature feature = new NetTopologySuite.Features.Feature();
                    feature.Geometry = dataReader.Geometry;
                    //get feature keys
                    int length = dataReader.DbaseHeader.NumFields;
                    string[] keys = new string[length];
                    for (int i = 0; i < length; i++)
                        keys[i] = dataReader.DbaseHeader.Fields[i].Name;
                    //get features attributes
                    feature.Attributes = new NetTopologySuite.Features.AttributesTable();
                    for (int i = 0; i < length; i++)
                    {
                        object val = dataReader.GetValue(i);
                        feature.Attributes.AddAttribute(keys[i], val);
                    }
                    //add feature to collection
                    featureCollection.Add(feature);
                }
               
                IGeometryFactory gf = GeoAPI.GeometryServiceProvider.Instance.CreateGeometryFactory();

                //built timezones array
                foreach (NetTopologySuite.Features.Feature feature in featureCollection)
                {
                    try
                    {
                        //get timezone
                        NetTopologySuite.Features.AttributesTable table = (NetTopologySuite.Features.AttributesTable)feature.Attributes;
                        string timezone = table["TZID"].ToString();

                        //set getometry
                        IGeometry geometry = feature.Geometry;

                        bool addnew = true;
                        foreach (TimeZones geotimezone in geotimezones)
                        {
                            //add geometry to existing timezone
                            if (geotimezone.timezone.CompareTo(timezone) == 0)
                            {
                                addnew = false;
                                geotimezone.geometry = geotimezone.geometry.Concat<IGeometry>(new[] { geometry }).ToArray();
                            }
                        }
                        //add a new timezone and first geometry
                        if (addnew)
                        {
                            TimeZones addgeotimezone = new TimeZones();
                            addgeotimezone.timezone = timezone;
                            addgeotimezone.geometry = new[] { geometry };
                            geotimezones.Add(addgeotimezone);
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMsg = "ERROR: " + ex;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: " + ex;
                return;
            }
        }

        /// <summary>
        /// Returns the Iana timezone name for a specific latitude/longitude coordinate.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        private string GetTZName(out string errorMsg, double latitude, double longitude)
        {
            errorMsg = "";
            string result = "";

            if (Math.Abs(latitude) > 180) { errorMsg = "ERROR: Invalid latitude value."; return null; }
            if (Math.Abs(longitude) > 180) { errorMsg = "ERROR: Invalid longitude value."; return null; }

            GeoAPI.Geometries.Coordinate point = new GeoAPI.Geometries.Coordinate(longitude, latitude);
            foreach (TimeZones geoTZ in geotimezones)
            {
                foreach (GeoAPI.Geometries.IGeometry geometry in geoTZ.geometry)
                {
                    if (NetTopologySuite.Algorithm.Locate.SimplePointInAreaLocator.Locate(point, geometry).Equals(GeoAPI.Geometries.Location.Interior))
                    {
                        return result = geoTZ.timezone;
                    }
                }
            }
            double distance = 0.01;
            if (result == "")
            {
                foreach (TimeZones geoTZ in geotimezones)
                {
                    foreach (GeoAPI.Geometries.IGeometry geometry in geoTZ.geometry)
                    {
                        NetTopologySuite.Algorithm.Distance.PointPairDistance ptdist = new NetTopologySuite.Algorithm.Distance.PointPairDistance();
                        NetTopologySuite.Algorithm.Distance.DistanceToPoint.ComputeDistance(geometry, point, ptdist);
                        if (ptdist.Distance < distance)
                        {
                            return result = geoTZ.timezone;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the gmt offset as a double.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public double GetGMTOffset( out string errorMsg, double latitude, double longitude, HMSTimeSeries.HMSTimeSeries ts)
        {
            errorMsg = "";
            //May need to remove \ from \bin\... (added beacuse of issue with unit tests)
            string tzShapeFile = String.Concat(AppDomain.CurrentDomain.BaseDirectory, @"\bin\TZShapeFile\tz_world"); // file containing the shapefile.
            string zonesFile = String.Concat(AppDomain.CurrentDomain.BaseDirectory, @"\bin\tzinfo.csv"); // file containing list of timezones with offset values.
            double offset = 0.0;
            string gmtOffset = "";
            string tzName = "";
            try
            {
                HMSGDAL tz = new HMSGDAL(out errorMsg, tzShapeFile);
                tzName = tz.GetTZName(out errorMsg, latitude, longitude);
                ts.tzName = tzName;
                ts.newMetaData += "tzName=" + tzName + "\n";
                if (errorMsg.Contains("ERROR")) { return 0; }
                StreamReader reader = new StreamReader(File.OpenRead(zonesFile));
                while(!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(',');
                    if (line[1].Equals(tzName, StringComparison.OrdinalIgnoreCase))
                    {
                        gmtOffset = line[2];
                    }
                }
            }
            catch
            {
                errorMsg = "ERROR: Failed to load or find timezone.";
                return 0;
            }
            ts.newMetaData += "gmtoffset=" + gmtOffset + "\n";
            string[] time = gmtOffset.Split(':');
            int minutes = 0;
            try
            {
                minutes = Convert.ToInt32(time[0]) * 3600 + Convert.ToInt32(time[1]) * 60;
            }
            catch
            {
                errorMsg = "ERROR: Failed to convert GMT offset value to hours.";
                return 0;
            }
            offset = Convert.ToDouble(minutes) / 3600.0;
            ts.gmtOffset = offset;
            return offset;
        }
                
        /// <summary>
        /// Returns a DateTime date that has been adjusted by offset depending on if it is the start or end date. Used to get data from LDAS that corresponds to the correct local time.
        /// LDAS returns data from GMT time.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="offset"></param>
        /// <param name="date"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public DateTime AdjustDateByOffset(out string errorMsg, double offset, DateTime date, bool start)
        {
            errorMsg = "";
            int startHour = 0;
            int endHour = 23;
            if (start == true)
            {
                if (offset < 0.0)
                {
                    startHour = Convert.ToInt16(System.Math.Abs(offset));
                }
                else if (offset > 0.0)
                {
                    date.AddDays(-1.0);
                    startHour = 24 - Convert.ToInt16(offset);
                }
                DateTime newStartDate = new DateTime(date.Year, date.Month, date.Day, startHour, 00, 00);
                return newStartDate;
            }
            else
            {
                if (offset < 0.0)
                {
                    date.AddDays(1.0);
                    endHour = -1 * Convert.ToInt16(offset) - 1;
                }
                else if (offset > 0.0)
                {
                    endHour = 23 - Convert.ToInt16(offset);
                }
                DateTime newEndDate = new DateTime(date.Year, date.Month, date.Day, endHour, 00, 00);
                return newEndDate;
            }
        }

        /// <summary>
        /// Sets the Date/Hour to local time. Reverts Date/Hour from GMT back to local, used after data collection.
        /// </summary>
        /// <param name="dateHour"></param>
        /// <returns></returns>
        public string SetDateToLocal(out string errorMsg, string dateHour, double offset)
        {
            errorMsg = "";
            if (Math.Abs(offset) > 12) { errorMsg = "ERROR: Invalid offset."; return null; }
            try
            {
                string[] date = dateHour.Split(' ');
                string hourStr = date[1].Substring(0, 2);
                string dateHourStr = date[0] + " " + hourStr;
                DateTime newDate = new DateTime();
                DateTime.TryParseExact(dateHourStr, new string[] { "yyyy-MM-dd HH" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out newDate);
                newDate = newDate.AddHours(offset);
                string newDateString = newDate.ToString("yyyy-MM-dd HH") + "Z";
                if (newDateString.Contains("0001")) { errorMsg = "ERROR: Invalid date provided, unable to convert to local date."; return null; }
                return newDateString;
            }
            catch
            {
                errorMsg = "ERROR: Failed to convert date back to local time.";
                return null;
            }
        }

        /// <summary>
        /// Returns a double[][] containing the centroid of the feature provided.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="featureFile"></param>
        /// <returns></returns>
        public double[] ReturnCentroid(out string errorMsg, string feature)
        {
            errorMsg = "";
            double[] coordinates = null;
            string type = Path.GetExtension(feature);
            if (type.Contains(".zip"))
            {
                geometryName = feature.Substring(0, feature.LastIndexOf('.'));
                featureCollection = FeatureFromFile(out errorMsg, feature);
                coordinates = FindCentroid(out errorMsg, featureCollection);
                coordinates = ConvertProjection(out errorMsg, coordinates);
            }
            else if (type.Contains(".json"))
            {
                geometryName = feature;
                featureCollection = FeatureFromGeoJson(out errorMsg);
                coordinates = FindCentroid(out errorMsg, featureCollection);
            }

            return coordinates;
        }

        /// <summary>
        /// Receices a shapefile and returns a list of features.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="featureFile"></param>
        /// <returns></returns>
        private List<NetTopologySuite.Features.Feature> FeatureFromFile(out string errorMsg, string featureFile)
        {
            errorMsg = "";
            List<NetTopologySuite.Features.Feature> featureCollection = new List<NetTopologySuite.Features.Feature>();
            try
            {
                NetTopologySuite.IO.ShapefileDataReader dataReader = new NetTopologySuite.IO.ShapefileDataReader(featureFile, new NetTopologySuite.Geometries.GeometryFactory());
                while (dataReader.Read())
                {
                    NetTopologySuite.Features.Feature feature = new NetTopologySuite.Features.Feature();
                    feature.Geometry = dataReader.Geometry;
                    // get feature keys
                    int length = dataReader.DbaseHeader.NumFields;
                    string[] keys = new string[length];
                    for (int i = 0; i < length; i++)
                        keys[i] = dataReader.DbaseHeader.Fields[i].Name;
                    //get features attributes
                    feature.Attributes = new NetTopologySuite.Features.AttributesTable();
                    for (int i = 0; i < length; i++)
                    {
                        object val = dataReader.GetValue(i);
                        feature.Attributes.AddAttribute(keys[i], val);
                    }
                    //add feature to collection
                    featureCollection.Add(feature);
                }
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: Unable to load feature file. " + ex.Message;
                return null;
            }
            return featureCollection;
        }

        /// <summary>
        /// Finds the centroid of the shapefile(s).
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="featureCollection"></param>
        /// <returns></returns>
        private double[] FindCentroid(out string errorMsg, List<NetTopologySuite.Features.Feature> featureCollection)
        {
            errorMsg = "";
            double[] coordinates = new double[2];
            foreach (NetTopologySuite.Features.Feature feature in featureCollection)
            {
                GeoAPI.Geometries.IGeometry geom = feature.Geometry;
                NetTopologySuite.Algorithm.Centroid centroid = new NetTopologySuite.Algorithm.Centroid(geom);
                coordinates[0] = centroid.GetCentroid().CoordinateValue.X;
                coordinates[1] = centroid.GetCentroid().CoordinateValue.Y;
            }
            return coordinates;
        }

        /// <summary>
        /// Checks if the given latitude/longitude point is contained within the shapefile. OBSOLETE
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="shapeFilename"></param>
        /// <returns></returns>
        //public void PointsInShapefile(out string errorMsg, double[] coordinate, double cellWidth)
        //{
        //    errorMsg = "";
        //    try
        //    {
        //        List<Tuple<double, double>> testList = new List<Tuple<double, double>>();
        //        coordinatesInShapefile = new List<Tuple<double, double>>();

        //        int totalPoints = 0;

        //        GeoAPI.Geometries.IGeometry geom = shapeFileFeature[0].Geometry;

        //        string line = System.IO.File.ReadAllText(shapeFileName + ".prj");

        //        IProjectedCoordinateSystem pcs = CoordinateSystemWktReader.Parse(line) as IProjectedCoordinateSystem;
        //        IGeographicCoordinateSystem gcs = GeographicCoordinateSystem.WGS84 as IGeographicCoordinateSystem;
        //        CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();
        //        ICoordinateTransformation transformTo = ctfac.CreateFromCoordinateSystems(pcs, gcs);
        //        IMathTransform inverseTransformTo = transformTo.MathTransform.Inverse();

        //        double[] transformedCoordinate = inverseTransformTo.Transform(new double[] { coordinate[1], coordinate[0] });       // Longitude,Latitude. coordinate is Latitude,Longitude
        //        double latitude = coordinate[0];
        //        double longitude = coordinate[1];
        //        double transformedStartLatitude = transformedCoordinate[1];
        //        double transformedStartLongitude = transformedCoordinate[0];

        //        GeoAPI.Geometries.Coordinate point = new GeoAPI.Geometries.Coordinate(transformedStartLongitude, transformedStartLatitude);      // Longitude,Latitude

        //        int i = 0;
        //        int j = 0;
        //        double tempLatitude = latitude;
        //        double tempLongitude = longitude;

        //        //checks +x,+y quadrant
        //        while (NetTopologySuite.Algorithm.Locate.SimplePointInAreaLocator.Locate(point, geom).Equals(GeoAPI.Geometries.Location.Interior))
        //        {
        //            j = 0;
        //            while (NetTopologySuite.Algorithm.Locate.SimplePointInAreaLocator.Locate(point, geom).Equals(GeoAPI.Geometries.Location.Interior))
        //            {
        //                coordinatesInShapefile.Add(Tuple.Create(tempLatitude, tempLongitude));
        //                j += 1;
        //                totalPoints += 1;
        //                tempLatitude = tempLatitude + (j * cellWidth);
        //                transformedCoordinate = inverseTransformTo.Transform(new double[] { tempLongitude, tempLatitude });
        //                point = new GeoAPI.Geometries.Coordinate(transformedCoordinate[0], transformedCoordinate[1]);
        //                testList.Add(Tuple.Create(tempLatitude, tempLongitude));    
        //            }
        //            i += 1;
        //            tempLongitude = tempLongitude + (i * cellWidth);
        //            transformedCoordinate = inverseTransformTo.Transform(new double[] { tempLongitude, tempLatitude });
        //            point = new GeoAPI.Geometries.Coordinate(transformedCoordinate[0], transformedCoordinate[1]);
        //            testList.Add(Tuple.Create(tempLatitude, tempLongitude));
        //        }

        //        i = 0;
        //        tempLatitude = latitude - (1 * cellWidth);
        //        tempLongitude = longitude;
        //        transformedCoordinate = inverseTransformTo.Transform(new double[] { tempLongitude, tempLatitude });
        //        point = new GeoAPI.Geometries.Coordinate(transformedCoordinate[0], transformedCoordinate[1]);

        //        //checks +x,-y quadrant
        //        while (NetTopologySuite.Algorithm.Locate.SimplePointInAreaLocator.Locate(point, geom).Equals(GeoAPI.Geometries.Location.Interior))
        //        {
        //            j = 0;
        //            while (NetTopologySuite.Algorithm.Locate.SimplePointInAreaLocator.Locate(point, geom).Equals(GeoAPI.Geometries.Location.Interior))
        //            {
        //                coordinatesInShapefile.Add(Tuple.Create(tempLatitude, tempLongitude)); totalPoints += 1;
        //                j += 1;
        //                tempLatitude = tempLatitude - (j * cellWidth);
        //                transformedCoordinate = inverseTransformTo.Transform(new double[] { tempLongitude, tempLatitude });
        //                point = new GeoAPI.Geometries.Coordinate(transformedCoordinate[0], transformedCoordinate[1]);
        //                testList.Add(Tuple.Create(tempLatitude, tempLongitude));
        //            }
        //            i += 1;
        //            tempLongitude = tempLongitude + (i * cellWidth);
        //            transformedCoordinate = inverseTransformTo.Transform(new double[] { tempLongitude, tempLatitude });
        //            point = new GeoAPI.Geometries.Coordinate(transformedCoordinate[0], transformedCoordinate[1]);
        //            testList.Add(Tuple.Create(tempLatitude, tempLongitude));
        //        }

        //        i = 0;
        //        tempLatitude = latitude;
        //        tempLongitude = longitude - (1 * cellWidth);
        //        transformedCoordinate = inverseTransformTo.Transform(new double[] { tempLongitude, tempLatitude });
        //        point = new GeoAPI.Geometries.Coordinate(transformedCoordinate[0], transformedCoordinate[1]);

        //        //checks -x,+y quadrant
        //        while (NetTopologySuite.Algorithm.Locate.SimplePointInAreaLocator.Locate(point, geom).Equals(GeoAPI.Geometries.Location.Interior))
        //        {
        //            j = 0;
        //            while (NetTopologySuite.Algorithm.Locate.SimplePointInAreaLocator.Locate(point, geom).Equals(GeoAPI.Geometries.Location.Interior))
        //            {
        //                coordinatesInShapefile.Add(Tuple.Create(tempLatitude, tempLongitude)); totalPoints += 1;
        //                j += 1;
        //                tempLatitude = tempLatitude + (j * cellWidth);
        //                transformedCoordinate = inverseTransformTo.Transform(new double[] { tempLongitude, tempLatitude });
        //                point = new GeoAPI.Geometries.Coordinate(transformedCoordinate[0], transformedCoordinate[1]);
        //                testList.Add(Tuple.Create(tempLatitude, tempLongitude));
        //            }
        //            i += 1;
        //            tempLongitude = tempLongitude - (i * cellWidth);
        //            transformedCoordinate = inverseTransformTo.Transform(new double[] { tempLongitude, tempLatitude });
        //            point = new GeoAPI.Geometries.Coordinate(transformedCoordinate[0], transformedCoordinate[1]);
        //            testList.Add(Tuple.Create(tempLatitude, tempLongitude));
        //        }

        //        i = 0;
        //        tempLatitude = latitude - (1 * cellWidth);
        //        tempLongitude = longitude - (1 * cellWidth);
        //        transformedCoordinate = inverseTransformTo.Transform(new double[] { tempLongitude, tempLatitude });
        //        point = new GeoAPI.Geometries.Coordinate(transformedCoordinate[0], transformedCoordinate[1]);

        //        //checks -x,-y quadrant
        //        while (NetTopologySuite.Algorithm.Locate.SimplePointInAreaLocator.Locate(point, geom).Equals(GeoAPI.Geometries.Location.Interior))
        //        {
        //            j = 0;
        //            while (NetTopologySuite.Algorithm.Locate.SimplePointInAreaLocator.Locate(point, geom).Equals(GeoAPI.Geometries.Location.Interior))
        //            {
        //                coordinatesInShapefile.Add(Tuple.Create(tempLatitude, tempLongitude)); totalPoints += 1;
        //                j += 1;
        //                tempLatitude = tempLatitude - (j * cellWidth);
        //                transformedCoordinate = inverseTransformTo.Transform(new double[] { tempLongitude, tempLatitude });
        //                point = new GeoAPI.Geometries.Coordinate(transformedCoordinate[0], transformedCoordinate[1]);
        //                testList.Add(Tuple.Create(tempLatitude, tempLongitude));
        //            }
        //            i += 1;
        //            tempLongitude = tempLongitude - (i * cellWidth);
        //            transformedCoordinate = inverseTransformTo.Transform(new double[] { tempLongitude, tempLatitude });
        //            point = new GeoAPI.Geometries.Coordinate(transformedCoordinate[0], transformedCoordinate[1]);
        //            testList.Add(Tuple.Create(tempLatitude, tempLongitude));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        errorMsg = "ERROR: " + ex;
        //    }
        //}

        /// <summary>
        /// Converts the coordinates of a projected coordinate system using the information in the .prj file.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="coord"></param>
        /// <returns></returns>
        private double[] ConvertProjection(out string errorMsg, double[] coord)
        {
            errorMsg = "";
            double[] convertedCoord = new double[3];
            double[] result = new double[3];
            string line = "";
            try
            {
                line = System.IO.File.ReadAllText(geometryName + ".prj");
            }
            catch
            {
                errorMsg = "ERROR: Unable to load projection for shapefile.";
                return null;
            }

            IProjectedCoordinateSystem pcs = CoordinateSystemWktReader.Parse(line) as IProjectedCoordinateSystem;
            IGeographicCoordinateSystem gcs = GeographicCoordinateSystem.WGS84 as IGeographicCoordinateSystem;
            CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();
            ICoordinateTransformation transform = ctfac.CreateFromCoordinateSystems(gcs, pcs);
            IMathTransform inverseTransform = transform.MathTransform.Inverse();
            convertedCoord = inverseTransform.Transform(coord);                 
            //sample cat.shp returns 40,-109  (located on the Colorado/Utah border)
            //convertedCoord = transform.MathTransform.Transform(coord);        //sample cat.shp returns 23,-96  (located in the Gulf of Mexico)

            result[0] = convertedCoord[1];      //coord provided as X,Y Longitude,Latitude. Results return as Latitude,Longitude.
            result[1] = convertedCoord[0];
            return result;
        }
                
        /// <summary>
        /// Checks if the given latitude/longitude point is contained within the shapefile. OBSOLETE.
        /// Does not return all possible data points that contain areas that intersect, due to 
        /// how the cells are checked.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="shapeFilename"></param>
        /// <returns></returns>
        //public void CellAreaInShapefile(out string errorMsg, double[] coordinate, double cellWidth)
        //{
        //    errorMsg = "";
        //    try
        //    {
        //        coordinatesInShapefile = new List<Tuple<double, double>>();
        //        areaPrecentInGeometry = new List<double>();
        //        areaGeometryIntersection = new List<double>();

        //        string line = System.IO.File.ReadAllText(shapeFileName + ".prj");
        //        GeoAPI.Geometries.IGeometry geom = shapeFileFeature[0].Geometry;    //Geometry from shapefile

        //        IProjectedCoordinateSystem pcs = CoordinateSystemWktReader.Parse(line) as IProjectedCoordinateSystem;
        //        IGeographicCoordinateSystem gcs = GeographicCoordinateSystem.WGS84 as IGeographicCoordinateSystem;
        //        CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();
        //        ICoordinateTransformation transformTo = ctfac.CreateFromCoordinateSystems(pcs, gcs);
        //        IMathTransform inverseTransformTo = transformTo.MathTransform.Inverse();
        //        double[] transformedCoordinate = inverseTransformTo.Transform(new double[] { coordinate[1], coordinate[0] });       // Longitude,Latitude. coordinate is Latitude,Longitude
        //        GeoAPI.Geometries.Coordinate point = new GeoAPI.Geometries.Coordinate(transformedCoordinate[0], transformedCoordinate[1]);      // Longitude,Latitude

        //        double latitude = coordinate[0];
        //        double longitude = coordinate[1];

        //        Coordinate[] coordinatesList = SetCellBounds(out errorMsg, latitude, longitude, cellWidth, inverseTransformTo);

        //        Polygon poly = CreatePolygon(out errorMsg, coordinatesList);
        //        var gpr = new GeometryPrecisionReducer(new PrecisionModel(10000000000));
        //        var polyR = gpr.Reduce(poly);   //Reduced precision of polygon required due to precision ERROR when intersecting with geom.

        //        bool valid = polyR.IsValid;
        //        int numPoints = polyR.NumPoints;

        //        GeoAPI.Geometries.IGeometry intersectGeo = geom.Intersection(polyR);     //Creates a geometry that contains the area that both geom and poly have in common. **
        //        IGeometry difference = polyR.Difference(geom);
        //        double intersectGeoArea = intersectGeo.Area;
        //        double areaOfPoly = polyR.Area;
        //        double precentCoverage = intersectGeoArea / areaOfPoly;
        //        double differencePolyArea = difference.Area;
        //        double precentCoverage2 = differencePolyArea / areaOfPoly;
        //        double tempLatitude = latitude;
        //        double tempLongitude = longitude;
        //        int i = 0;
        //        int j = 0;

        //        int[] modifier = new int[] { 1, 1, 1, -1, -1, 1, -1, -1 };

        //        // Latitude, Longitude values are for the center of each LDAS cell.
        //        if (geom.IsValid)
        //        {
        //            for (int k = 0; k < modifier.Length - 2;)
        //            {
        //                while (geom.Intersects(polyR))
        //                {
        //                    while (geom.Intersects(polyR)) 
        //                    {
        //                        if (!coordinatesInShapefile.Contains(Tuple.Create(tempLatitude, tempLongitude)))
        //                        {
        //                            coordinatesInShapefile.Add(Tuple.Create(tempLatitude, tempLongitude));
        //                            areaPrecentInGeometry.Add(precentCoverage);
        //                            areaGeometryIntersection.Add(intersectGeo.Area * 0.000001);
        //                        }
        //                        j += 1;
        //                        tempLatitude = tempLatitude + (modifier[k+1]) * (j * cellWidth);
        //                        coordinatesList = SetCellBounds(out errorMsg, tempLatitude, tempLongitude, cellWidth, inverseTransformTo);
        //                        poly = CreatePolygon(out errorMsg, coordinatesList);
        //                        polyR = gpr.Reduce(poly);                           //Reduced precision of polygon required due to precision ERROR when intersecting with geom.
        //                        intersectGeo = geom.Intersection(polyR);            //Create Intersection geometry
        //                        intersectGeoArea = intersectGeo.Area;               //Intersection geometry area
        //                        areaOfPoly = polyR.Area;                            //LDAS polygon area
        //                        precentCoverage = intersectGeoArea / areaOfPoly;    //Percent of shapefile geometry that intersects with LDAS polygon 
        //                    }
        //                    j = 0;
        //                    i += 1;
        //                    tempLatitude = latitude;
        //                    tempLongitude = tempLongitude + (modifier[k]) * (i * cellWidth);
        //                    coordinatesList = SetCellBounds(out errorMsg, tempLatitude, tempLongitude, cellWidth, inverseTransformTo);
        //                    poly = CreatePolygon(out errorMsg, coordinatesList);
        //                    polyR = gpr.Reduce(poly);                             //Reduced precision of polygon required due to precision ERROR when intersecting with geom.
        //                    intersectGeo = geom.Intersection(polyR);            //Create Intersection geometry
        //                    intersectGeoArea = intersectGeo.Area;               //Intersection geometry area
        //                    areaOfPoly = polyR.Area;                            //LDAS polygon area
        //                    precentCoverage = intersectGeoArea / areaOfPoly;    //Percent of shapefile geometry that intersects with LDAS polygon 
        //                }
        //                i = 0;
        //                k += 2;
        //                tempLatitude = latitude + (modifier[k+1]) * (i * cellWidth);
        //                tempLongitude = longitude + (modifier[k]) * (i * cellWidth);
        //                coordinatesList = SetCellBounds(out errorMsg, tempLatitude, tempLongitude, cellWidth, inverseTransformTo);
        //                poly = CreatePolygon(out errorMsg, coordinatesList);
        //                polyR = gpr.Reduce(poly);                           //Reduced precision of polygon required due to precision ERROR when intersecting with geom.
        //                intersectGeo = geom.Intersection(polyR);            //Create Intersection geometry
        //                intersectGeoArea = intersectGeo.Area;               //Intersection geometry area
        //                areaOfPoly = polyR.Area;                            //LDAS polygon area
        //                precentCoverage = intersectGeoArea / areaOfPoly;    //Percent of shapefile geometry that intersects with LDAS polygon 
        //            }
        //        }
        //        else
        //        {
        //            errorMsg = "ERROR: Shapefile geometry is not valid.";
        //            return;
        //        }
        //    }
        //    catch
        //    {
        //        errorMsg = "ERROR: Failed to find LDAS cells cooresponding to shapefile area.";
        //        return;
        //    }            
        //}

        /// <summary>
        /// Creates an array of coordinates that are projected by the IMathTransform object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="cellWidth"></param>
        /// <param name="inverseTransformTo"></param>
        /// <returns></returns>
        private Coordinate[] SetCellBounds(out string errorMsg, double latitude, double longitude, double cellWidth, IMathTransform inverseTransformTo)
        {
            errorMsg = "";
            try
            {
                double[] topLeftCorner = inverseTransformTo.Transform(new double[] { longitude - cellWidth / 2, latitude + cellWidth / 2 });
                double[] topRightCorner = inverseTransformTo.Transform(new double[] { longitude + cellWidth / 2, latitude + cellWidth / 2 });
                double[] bottomLeftCorner = inverseTransformTo.Transform(new double[] { longitude - cellWidth / 2, latitude - cellWidth / 2 });
                double[] bottomRightCorner = inverseTransformTo.Transform(new double[] { longitude + cellWidth / 2, latitude - cellWidth / 2 });

                Coordinate[] coordinatesList = new Coordinate[5] {  new Coordinate(topLeftCorner[0], topLeftCorner[1]),
                                                                    new Coordinate(topRightCorner[0], topRightCorner[1]),
                                                                    new Coordinate(bottomLeftCorner[0], bottomLeftCorner[1]),
                                                                    new Coordinate(bottomRightCorner[0], bottomRightCorner[1]),
                                                                    new Coordinate(topLeftCorner[0], topLeftCorner[1]) };
                return coordinatesList;
            }
            catch
            {
                errorMsg = "ERROR: Failed to create new polygon for LDAS data cell.";
                return null;
            }
        }

        /// <summary>
        /// Creates array of Coordinates that are not projected.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="cellWidth"></param>
        /// <returns></returns>
        private Coordinate[] SetCellBounds(out string errorMsg, double latitude, double longitude, double cellWidth)
        {
            errorMsg = "";
            try
            {
                double[] topLeftCorner = new double[] { longitude - cellWidth / 2, latitude + cellWidth / 2 };
                double[] topRightCorner = new double[] { longitude + cellWidth / 2, latitude + cellWidth / 2 };
                double[] bottomLeftCorner = new double[] { longitude - cellWidth / 2, latitude - cellWidth / 2 };
                double[] bottomRightCorner = new double[] { longitude + cellWidth / 2, latitude - cellWidth / 2 };

                Coordinate[] coordinatesList = new Coordinate[5] {  new Coordinate(topLeftCorner[0], topLeftCorner[1]),
                                                                    new Coordinate(topRightCorner[0], topRightCorner[1]),
                                                                    new Coordinate(bottomLeftCorner[0], bottomLeftCorner[1]),
                                                                    new Coordinate(bottomRightCorner[0], bottomRightCorner[1]),
                                                                    new Coordinate(topLeftCorner[0], topLeftCorner[1]) };
                return coordinatesList;
            }
            catch
            {
                errorMsg = "ERROR: Failed to create new polygon for LDAS data cell.";
                return null;
            }
        }

        /// <summary>
        /// Creates a polygon from an array of coordinates.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="coordinatesList"></param>
        /// <returns></returns>
        private Polygon CreatePolygon(out string errorMsg, Coordinate[] coordinatesList)
        {
            errorMsg = "";
            try
            {
                IGeometryFactory geoFactory = new NetTopologySuite.Geometries.GeometryFactory();
                NetTopologySuite.Geometries.LinearRing linear = (NetTopologySuite.Geometries.LinearRing)new GeometryFactory().CreateLinearRing(coordinatesList);
                Polygon poly = new Polygon(linear, null, geoFactory);
                return poly;
            }
            catch
            {
                errorMsg = "ERROR: Failed to create new polygon from coordinate.";
                return null;
            }
        }
                
        /// <summary>
        /// Checks if the given latitude/longitude point is contained within the shapefile. 
        /// Determines bounds of shapefile and generates a virtual grid that gets iterated over 
        /// checking for intersection with shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="shapeFilename"></param>
        /// <returns></returns>
        private void GeometryGridSearch(out string errorMsg, double[] coordinate, double cellWidth)
        {
            errorMsg = "";
            try
            {
                coordinatesInShapefile = new List<Tuple<double, double>>();
                areaPrecentInGeometry = new List<double>();
                areaGeometryIntersection = new List<double>();

                GeoAPI.Geometries.IGeometry geom = featureCollection[0].Geometry;    //Geometry from feature
                
                string line = System.IO.File.ReadAllText(geometryName + ".prj");

                IProjectedCoordinateSystem pcs = CoordinateSystemWktReader.Parse(line) as IProjectedCoordinateSystem;
                IGeographicCoordinateSystem gcs = GeographicCoordinateSystem.WGS84 as IGeographicCoordinateSystem;
                CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();
                ICoordinateTransformation transformTo = ctfac.CreateFromCoordinateSystems(pcs, gcs);
                IMathTransform inverseTransformTo = transformTo.MathTransform.Inverse();
                double[] transformedCoordinate = inverseTransformTo.Transform(new double[] { coordinate[1], coordinate[0] });       // Longitude,Latitude. coordinate is Latitude,Longitude
                //GeoAPI.Geometries.Coordinate point = new GeoAPI.Geometries.Coordinate(transformedCoordinate[0], transformedCoordinate[1]);      // Longitude,Latitude

                double latitude = coordinate[0];
                double longitude = coordinate[1];
                
                double[] transformedCoordinate2 = inverseTransformTo.Transform(new double[] { coordinate[1] + cellWidth, coordinate[0] });      //Test coordinate used to determine projected cell width
                double cW = Math.Abs(transformedCoordinate[0]) - Math.Abs(transformedCoordinate2[0]);                                           //Cell width determined from two projected points.

                //Calculate bounds of grid for cells.
                int aX = (Int32)Math.Ceiling((Math.Abs(transformedCoordinate[0] - geom.EnvelopeInternal.MinX) / cW)) + 1;                           //Cell x value of upperleft corner of grid from centroid
                int aY = (Int32)Math.Ceiling((Math.Abs(geom.EnvelopeInternal.MaxY - transformedCoordinate[1]) / cW)) + 1;                           //Cell y value of upperleft corner of grid from centroid          
                int iMax = (Int32)Math.Ceiling((geom.EnvelopeInternal.MaxX - geom.EnvelopeInternal.MinX) / cW) + 2;                                 //Total data grid cells along x
                int jMax = (Int32)Math.Ceiling((geom.EnvelopeInternal.MaxY - geom.EnvelopeInternal.MinY) / cW) + 2;                                 //Total data grid cells along y
                double tempLatitude = latitude + (aY * cellWidth);                                                                              //Latitude of upperleft corner start cell
                double tempLongitude = longitude - (aX * cellWidth);                                                                            //Longitude of upperleft corner start cell

                Coordinate[] coordinatesList = SetCellBounds(out errorMsg, tempLatitude, tempLongitude, cellWidth, inverseTransformTo);

                Polygon poly = CreatePolygon(out errorMsg, coordinatesList);                //Creates polygon
                var gpr = new GeometryPrecisionReducer(new PrecisionModel(10000000000));    //Precision reduction value set
                var polyR = gpr.Reduce(poly);                                               //Reduced precision of polygon required due to precision ERROR when intersecting with geom.

                GeoAPI.Geometries.IGeometry intersectGeo = geom.Intersection(polyR);        //Creates a geometry that contains the area that both geom and poly have in common. **
                double intersectGeoArea = intersectGeo.Area;                                //Area of grid that intersects shapefile                 
                double areaOfPoly = polyR.Area;                                             //Area of grid data cell
                double precentCoverage = intersectGeo.Area / polyR.Area;                    //Percent of grid data cell that intersects shapefile

                if (geom.IsValid)
                {
                    for (int i = 0; i <= iMax; i++)
                    {
                        for (int j = 0; j <= jMax; j++)
                        {
                            if (geom.Intersects(polyR) && !coordinatesInShapefile.Contains(Tuple.Create(tempLatitude, tempLongitude)))          //Checks if any intersection of cell in shapefile geometry, and if cell has already been added to list.
                            {
                                coordinatesInShapefile.Add(Tuple.Create(tempLatitude, tempLongitude));                      //Coordiantes added to list
                                areaPrecentInGeometry.Add(precentCoverage);                                                 //Precent of area in cell that intersects shapefile added to list
                                areaGeometryIntersection.Add(intersectGeo.Area);                                            //Area in cell that intersects shapefile added to list.
                            }
                            tempLatitude = latitude + ((aY - j) * cellWidth);
                            coordinatesList = SetCellBounds(out errorMsg, tempLatitude, tempLongitude, cellWidth, inverseTransformTo);
                            poly = CreatePolygon(out errorMsg, coordinatesList);
                            polyR = gpr.Reduce(poly);                           //Reduced precision of polygon required due to precision ERROR when intersecting with geom.
                            intersectGeo = geom.Intersection(polyR);            //Create Intersection geometry
                            intersectGeoArea = intersectGeo.Area;               //Intersection geometry area
                            areaOfPoly = polyR.Area;                            //LDAS polygon area
                            precentCoverage = intersectGeoArea / areaOfPoly;    //Percent of shapefile geometry that intersects with LDAS polygon 
                        }
                        tempLatitude = latitude + (aY * cellWidth);             //Reset latitude value to left most cell
                        tempLongitude = longitude - ((aX - i) * cellWidth);     //Updated longitude value 
                    }
                }
            }
            catch
            {
                errorMsg = "ERROR: Failed to find LDAS cells cooresponding to shapefile area.";
                return;
            }
        }

        /// <summary>
        /// Checks if the given latitude/longitude point is contained within the geoJson feature. 
        /// Determines bounds of geometry and generates a virtual grid that gets iterated over 
        /// checking for intersection with geometry.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="shapeFilename"></param>
        /// <returns></returns>
        private void FeatureGridSearch(out string errorMsg, double[] coordinate, double cellWidth)
        {
            errorMsg = "";
            try
            {
                coordinatesInShapefile = new List<Tuple<double, double>>();
                areaPrecentInGeometry = new List<double>();
                areaGeometryIntersection = new List<double>();

                GeoAPI.Geometries.IGeometry geom = featureCollection[0].Geometry;    //Geometry from shapefile
                
                double latitude = coordinate[1];
                double longitude = coordinate[0];
                
                double cW = cellWidth;
                //Calculate bounds of grid for cells.
                int aX = (Int32)Math.Ceiling((Math.Abs(longitude - geom.EnvelopeInternal.MinX) / cW)) + 2;                           //Cell x value of upperleft corner of grid from centroid
                int aY = (Int32)Math.Ceiling((Math.Abs(geom.EnvelopeInternal.MaxY - latitude) / cW)) + 2;                           //Cell y value of upperleft corner of grid from centroid          
                int iMax = (Int32)Math.Ceiling((geom.EnvelopeInternal.MaxX - geom.EnvelopeInternal.MinX) / cW) + 5;                                 //Total data grid cells along x
                int jMax = (Int32)Math.Ceiling((geom.EnvelopeInternal.MaxY - geom.EnvelopeInternal.MinY) / cW) + 5;                                 //Total data grid cells along y
                double tempLatitude = latitude + (aY * cellWidth);                                                                              //Latitude of upperleft corner start cell
                double tempLongitude = longitude - (aX * cellWidth);                                                                            //Longitude of upperleft corner start cell

                Coordinate[] coordinatesList = SetCellBounds(out errorMsg, tempLatitude, tempLongitude, cellWidth);

                Polygon poly = CreatePolygon(out errorMsg, coordinatesList);                //Creates polygon
                var gpr = new GeometryPrecisionReducer(new PrecisionModel(10000000000));    //Precision reduction value set
                var polyR = gpr.Reduce(poly);                                               //Reduced precision of polygon required due to precision ERROR when intersecting with geom.

                GeoAPI.Geometries.IGeometry intersectGeo = geom.Intersection(polyR);        //Creates a geometry that contains the area that both geom and poly have in common. **
                double intersectGeoArea = intersectGeo.Area;                                //Area of grid that intersects shapefile                 
                double areaOfPoly = polyR.Area;                                             //Area of grid data cell
                double precentCoverage = intersectGeo.Area / polyR.Area;                    //Percent of grid data cell that intersects 

                if (geom.IsValid)
                {
                    for (int i = 0; i <= iMax; i++)
                    {
                        for (int j = 0; j <= jMax; j++)
                        {
                            if (geom.Intersects(polyR) && !coordinatesInShapefile.Contains(Tuple.Create(tempLatitude, tempLongitude)))          //Checks if any intersection of cell in shapefile geometry, and if cell has already been added to list.
                            {
                                coordinatesInShapefile.Add(Tuple.Create(tempLatitude, tempLongitude));                      //Coordiantes added to list
                                areaPrecentInGeometry.Add(precentCoverage);                                                 //Precent of area in cell that intersects shapefile added to list
                                areaGeometryIntersection.Add(intersectGeo.Area);                                            //Area in cell that intersects shapefile added to list.
                            }
                            tempLatitude = latitude + ((aY - j) * cellWidth);
                            coordinatesList = SetCellBounds(out errorMsg, tempLatitude, tempLongitude, cellWidth);
                            poly = CreatePolygon(out errorMsg, coordinatesList);
                            polyR = gpr.Reduce(poly);                           //Reduced precision of polygon required due to precision ERROR when intersecting with geom.
                            intersectGeo = geom.Intersection(polyR);            //Create Intersection geometry
                            intersectGeoArea = intersectGeo.Area;               //Intersection geometry area
                            areaOfPoly = polyR.Area;                            //LDAS polygon area
                            precentCoverage = intersectGeoArea / areaOfPoly;    //Percent of shapefile geometry that intersects with LDAS polygon 
                        }
                        tempLatitude = latitude + (aY * cellWidth);             //Reset latitude value to left most cell
                        tempLongitude = longitude - ((aX - i) * cellWidth);     //Updated longitude value 
                    }
                }
            }
            catch
            {
                errorMsg = "ERROR: Failed to find LDAS cells cooresponding to shapefile area.";
                return;
            }
        }

        /// <summary>
        /// Finds all data points in a feature geometry, either from a shapefile or json.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="coordinate"></param>
        /// <param name="cellWidth"></param>
        public void CellAreaInShapefileByGrid(out string errorMsg, double[] coordinate, double cellWidth)
        {
            errorMsg = "";
            if (geoJSON != null)
            {
                FeatureGridSearch(out errorMsg, coordinate, cellWidth);
            }
            else
            {
                GeometryGridSearch(out errorMsg, coordinate, cellWidth);
            }
        }

        //----------------------- GEOJSON FUNCTIONS --------------------------//

        /// <summary>
        /// Returns a double[][] containing the centroid of the feature provided.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="geoJSON"></param>
        /// <returns></returns>
        public double[] ReturnCentroidFromGeoJSON(out string errorMsg)
        {
            errorMsg = "";
            double[] coordinates = null;
            featureCollection = FeatureFromGeoJson(out errorMsg);
            coordinates = FindCentroid(out errorMsg, featureCollection);
            return coordinates;
        }

        /// <summary>
        /// Receices a geojson and returns a list of features, as featureCollection.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="featureFile"></param>
        /// <returns></returns>
        private List<NetTopologySuite.Features.Feature> FeatureFromGeoJson(out string errorMsg)
        {
            errorMsg = "";
            List<NetTopologySuite.Features.Feature> featureCollection = new List<NetTopologySuite.Features.Feature>();
            try
            {
                NetTopologySuite.IO.GeoJsonReader geoReader = new NetTopologySuite.IO.GeoJsonReader();
                featureCollection.Add(geoReader.Read<Feature>(this.geoJSON));
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: Unable to load geoJSON. " + ex.Message;
                return null;
            }
            return featureCollection;
        }
    }
}