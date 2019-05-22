﻿using Data;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Precipitation.Tests
{
    public class DaymetUnitTests
    {

        public string inputObject = "{\"Source\":\"daymet\",\"DateTimeSpan\":{\"StartDate\":\"2015-01-01T00:00:00\",\"EndDate\":\"2015-12-31T00:00:00\",\"DateTimeFormat\":\"yyyy-MM-dd HH\"},\"Geometry\":{\"Description\":\"EPA Athens Office\",\"Point\":{\"Latitude\":33.925673,\"Longitude\":-83.355723},\"GeometryMetadata\":{\"City\":\"Athens\",\"State\":\"Georgia\",\"Country\":\"United States\"},\"Timezone\":{\"Name\":\"EST\",\"Offset\":-5.0,\"DLS\":false}},\"DataValueFormat\":\"E3\",\"TemporalResolution\":\"default\",\"TimeLocalized\":true,\"Units\":\"default\",\"OutputFormat\":\"json\",\"BaseURL\":[\"https://daymet.ornl.gov/data/send/saveData?\"],\"InputTimeSeries\":{}}";
        public string rawOutput = "Latitude: 33.925673  Longitude: -83.355723\nX & Y on Lambert Conformal Conic: 1476706.87 -764020.19\nTile: 11029\nElevation: 210 meters\nAll years; all variables; Daymet Software Version 3.0; Daymet Data Version 3.0.\nHow to cite: Thornton; P.E.; M.M. Thornton; B.W. Mayer; Y. Wei; R. Devarakonda; R.S. Vose; and R.B. Cook. 2016. Daymet: Daily Surface Weather Data on a 1-km Grid for North America; Version 3. ORNL DAAC; Oak Ridge; Tennessee; USA. http://dx.doi.org/10.3334/ORNLDAAC/1328 \n\nyear,yday,prcp (mm/day)\n2015.0,1.0,0.0\n2015.0,2.0,3.0\n2015.0,3.0,16.0\n2015.0,4.0,17.0\n2015.0,5.0,14.0\n2015.0,6.0,0.0\n2015.0,7.0,0.0\n2015.0,8.0,0.0\n2015.0,9.0,0.0\n2015.0,10.0,0.0\n2015.0,11.0,0.0\n2015.0,12.0,4.0\n2015.0,13.0,1.0\n2015.0,14.0,2.0\n2015.0,15.0,0.0\n2015.0,16.0,3.0\n2015.0,17.0,0.0\n2015.0,18.0,0.0\n2015.0,19.0,0.0\n2015.0,20.0,0.0\n2015.0,21.0,0.0\n2015.0,22.0,0.0\n2015.0,23.0,9.0\n2015.0,24.0,14.0\n2015.0,25.0,0.0\n2015.0,26.0,0.0\n2015.0,27.0,0.0\n2015.0,28.0,0.0\n2015.0,29.0,0.0\n2015.0,30.0,0.0\n2015.0,31.0,0.0\n2015.0,32.0,0.0\n2015.0,33.0,16.0\n2015.0,34.0,0.0\n2015.0,35.0,0.0\n2015.0,36.0,0.0\n2015.0,37.0,0.0\n2015.0,38.0,0.0\n2015.0,39.0,0.0\n2015.0,40.0,5.0\n2015.0,41.0,14.0\n2015.0,42.0,0.0\n2015.0,43.0,0.0\n2015.0,44.0,0.0\n2015.0,45.0,0.0\n2015.0,46.0,0.0\n2015.0,47.0,0.0\n2015.0,48.0,25.0\n2015.0,49.0,0.0\n2015.0,50.0,0.0\n2015.0,51.0,0.0\n2015.0,52.0,0.0\n2015.0,53.0,15.0\n2015.0,54.0,10.0\n2015.0,55.0,2.0\n2015.0,56.0,4.0\n2015.0,57.0,15.0\n2015.0,58.0,0.0\n2015.0,59.0,0.0\n2015.0,60.0,4.0\n2015.0,61.0,6.0\n2015.0,62.0,1.0\n2015.0,63.0,5.0\n2015.0,64.0,1.0\n2015.0,65.0,3.0\n2015.0,66.0,0.0\n2015.0,67.0,0.0\n2015.0,68.0,0.0\n2015.0,69.0,0.0\n2015.0,70.0,0.0\n2015.0,71.0,1.0\n2015.0,72.0,3.0\n2015.0,73.0,14.0\n2015.0,74.0,0.0\n2015.0,75.0,0.0\n2015.0,76.0,0.0\n2015.0,77.0,0.0\n2015.0,78.0,11.0\n2015.0,79.0,13.0\n2015.0,80.0,0.0\n2015.0,81.0,4.0\n2015.0,82.0,17.0\n2015.0,83.0,0.0\n2015.0,84.0,0.0\n2015.0,85.0,1.0\n2015.0,86.0,0.0\n2015.0,87.0,0.0\n2015.0,88.0,0.0\n2015.0,89.0,2.0\n2015.0,90.0,3.0\n2015.0,91.0,0.0\n2015.0,92.0,0.0\n2015.0,93.0,4.0\n2015.0,94.0,6.0\n2015.0,95.0,0.0\n2015.0,96.0,0.0\n2015.0,97.0,11.0\n2015.0,98.0,0.0\n2015.0,99.0,0.0\n2015.0,100.0,0.0\n2015.0,101.0,12.0\n2015.0,102.0,0.0\n2015.0,103.0,9.0\n2015.0,104.0,41.0\n2015.0,105.0,7.0\n2015.0,106.0,35.0\n2015.0,107.0,4.0\n2015.0,108.0,20.0\n2015.0,109.0,12.0\n2015.0,110.0,27.0\n2015.0,111.0,1.0\n2015.0,112.0,0.0\n2015.0,113.0,0.0\n2015.0,114.0,0.0\n2015.0,115.0,8.0\n2015.0,116.0,11.0\n2015.0,117.0,0.0\n2015.0,118.0,0.0\n2015.0,119.0,9.0\n2015.0,120.0,10.0\n2015.0,121.0,0.0\n2015.0,122.0,0.0\n2015.0,123.0,0.0\n2015.0,124.0,0.0\n2015.0,125.0,0.0\n2015.0,126.0,0.0\n2015.0,127.0,0.0\n2015.0,128.0,0.0\n2015.0,129.0,0.0\n2015.0,130.0,0.0\n2015.0,131.0,0.0\n2015.0,132.0,0.0\n2015.0,133.0,0.0\n2015.0,134.0,0.0\n2015.0,135.0,0.0\n2015.0,136.0,0.0\n2015.0,137.0,0.0\n2015.0,138.0,0.0\n2015.0,139.0,4.0\n2015.0,140.0,0.0\n2015.0,141.0,0.0\n2015.0,142.0,0.0\n2015.0,143.0,0.0\n2015.0,144.0,0.0\n2015.0,145.0,0.0\n2015.0,146.0,5.0\n2015.0,147.0,19.0\n2015.0,148.0,0.0\n2015.0,149.0,4.0\n2015.0,150.0,0.0\n2015.0,151.0,0.0\n2015.0,152.0,9.0\n2015.0,153.0,5.0\n2015.0,154.0,21.0\n2015.0,155.0,9.0\n2015.0,156.0,0.0\n2015.0,157.0,0.0\n2015.0,158.0,0.0\n2015.0,159.0,0.0\n2015.0,160.0,9.0\n2015.0,161.0,7.0\n2015.0,162.0,0.0\n2015.0,163.0,1.0\n2015.0,164.0,0.0\n2015.0,165.0,0.0\n2015.0,166.0,0.0\n2015.0,167.0,0.0\n2015.0,168.0,0.0\n2015.0,169.0,0.0\n2015.0,170.0,3.0\n2015.0,171.0,0.0\n2015.0,172.0,6.0\n2015.0,173.0,0.0\n2015.0,174.0,0.0\n2015.0,175.0,0.0\n2015.0,176.0,0.0\n2015.0,177.0,0.0\n2015.0,178.0,0.0\n2015.0,179.0,9.0\n2015.0,180.0,0.0\n2015.0,181.0,0.0\n2015.0,182.0,17.0\n2015.0,183.0,13.0\n2015.0,184.0,32.0\n2015.0,185.0,30.0\n2015.0,186.0,4.0\n2015.0,187.0,0.0\n2015.0,188.0,0.0\n2015.0,189.0,0.0\n2015.0,190.0,0.0\n2015.0,191.0,0.0\n2015.0,192.0,0.0\n2015.0,193.0,0.0\n2015.0,194.0,0.0\n2015.0,195.0,0.0\n2015.0,196.0,29.0\n2015.0,197.0,0.0\n2015.0,198.0,0.0\n2015.0,199.0,0.0\n2015.0,200.0,0.0\n2015.0,201.0,0.0\n2015.0,202.0,3.0\n2015.0,203.0,5.0\n2015.0,204.0,0.0\n2015.0,205.0,0.0\n2015.0,206.0,0.0\n2015.0,207.0,0.0\n2015.0,208.0,0.0\n2015.0,209.0,0.0\n2015.0,210.0,0.0\n2015.0,211.0,0.0\n2015.0,212.0,0.0\n2015.0,213.0,0.0\n2015.0,214.0,0.0\n2015.0,215.0,0.0\n2015.0,216.0,0.0\n2015.0,217.0,0.0\n2015.0,218.0,0.0\n2015.0,219.0,17.0\n2015.0,220.0,0.0\n2015.0,221.0,0.0\n2015.0,222.0,0.0\n2015.0,223.0,52.0\n2015.0,224.0,0.0\n2015.0,225.0,0.0\n2015.0,226.0,0.0\n2015.0,227.0,0.0\n2015.0,228.0,0.0\n2015.0,229.0,0.0\n2015.0,230.0,43.0\n2015.0,231.0,13.0\n2015.0,232.0,23.0\n2015.0,233.0,2.0\n2015.0,234.0,0.0\n2015.0,235.0,8.0\n2015.0,236.0,12.0\n2015.0,237.0,0.0\n2015.0,238.0,0.0\n2015.0,239.0,0.0\n2015.0,240.0,0.0\n2015.0,241.0,0.0\n2015.0,242.0,0.0\n2015.0,243.0,19.0\n2015.0,244.0,0.0\n2015.0,245.0,0.0\n2015.0,246.0,0.0\n2015.0,247.0,0.0\n2015.0,248.0,0.0\n2015.0,249.0,4.0\n2015.0,250.0,0.0\n2015.0,251.0,0.0\n2015.0,252.0,11.0\n2015.0,253.0,13.0\n2015.0,254.0,13.0\n2015.0,255.0,0.0\n2015.0,256.0,0.0\n2015.0,257.0,0.0\n2015.0,258.0,0.0\n2015.0,259.0,0.0\n2015.0,260.0,0.0\n2015.0,261.0,0.0\n2015.0,262.0,0.0\n2015.0,263.0,0.0\n2015.0,264.0,0.0\n2015.0,265.0,5.0\n2015.0,266.0,0.0\n2015.0,267.0,0.0\n2015.0,268.0,32.0\n2015.0,269.0,5.0\n2015.0,270.0,2.0\n2015.0,271.0,1.0\n2015.0,272.0,7.0\n2015.0,273.0,5.0\n2015.0,274.0,2.0\n2015.0,275.0,10.0\n2015.0,276.0,17.0\n2015.0,277.0,46.0\n2015.0,278.0,1.0\n2015.0,279.0,4.0\n2015.0,280.0,0.0\n2015.0,281.0,0.0\n2015.0,282.0,0.0\n2015.0,283.0,24.0\n2015.0,284.0,15.0\n2015.0,285.0,0.0\n2015.0,286.0,5.0\n2015.0,287.0,0.0\n2015.0,288.0,0.0\n2015.0,289.0,0.0\n2015.0,290.0,0.0\n2015.0,291.0,0.0\n2015.0,292.0,0.0\n2015.0,293.0,0.0\n2015.0,294.0,0.0\n2015.0,295.0,0.0\n2015.0,296.0,0.0\n2015.0,297.0,0.0\n2015.0,298.0,0.0\n2015.0,299.0,5.0\n2015.0,300.0,12.0\n2015.0,301.0,9.0\n2015.0,302.0,3.0\n2015.0,303.0,0.0\n2015.0,304.0,0.0\n2015.0,305.0,21.0\n2015.0,306.0,39.0\n2015.0,307.0,26.0\n2015.0,308.0,1.0\n2015.0,309.0,2.0\n2015.0,310.0,13.0\n2015.0,311.0,16.0\n2015.0,312.0,21.0\n2015.0,313.0,35.0\n2015.0,314.0,4.0\n2015.0,315.0,0.0\n2015.0,316.0,0.0\n2015.0,317.0,0.0\n2015.0,318.0,0.0\n2015.0,319.0,0.0\n2015.0,320.0,0.0\n2015.0,321.0,0.0\n2015.0,322.0,11.0\n2015.0,323.0,38.0\n2015.0,324.0,0.0\n2015.0,325.0,0.0\n2015.0,326.0,0.0\n2015.0,327.0,0.0\n2015.0,328.0,0.0\n2015.0,329.0,0.0\n2015.0,330.0,0.0\n2015.0,331.0,0.0\n2015.0,332.0,0.0\n2015.0,333.0,0.0\n2015.0,334.0,0.0\n2015.0,335.0,0.0\n2015.0,336.0,1.0\n2015.0,337.0,1.0\n2015.0,338.0,0.0\n2015.0,339.0,0.0\n2015.0,340.0,0.0\n2015.0,341.0,0.0\n2015.0,342.0,0.0\n2015.0,343.0,0.0\n2015.0,344.0,0.0\n2015.0,345.0,0.0\n2015.0,346.0,0.0\n2015.0,347.0,0.0\n2015.0,348.0,2.0\n2015.0,349.0,3.0\n2015.0,350.0,0.0\n2015.0,351.0,48.0\n2015.0,352.0,13.0\n2015.0,353.0,0.0\n2015.0,354.0,0.0\n2015.0,355.0,0.0\n2015.0,356.0,48.0\n2015.0,357.0,12.0\n2015.0,358.0,43.0\n2015.0,359.0,26.0\n2015.0,360.0,5.0\n2015.0,361.0,0.0\n2015.0,362.0,8.0\n2015.0,363.0,29.0\n2015.0,364.0,19.0\n2015.0,365.0,57.0\n";
        public string outputObject = "{\"Dataset\":\"Precipitation\",\"DataSource\":\"daymet\",\"Metadata\":{\"daymet_Latitude\":\"33.925673\",\"daymet_Longitude\":\"-83.355723\",\"daymet_X & Y on Lambert Conformal Conic\":\"1476706.87 -764020.19\",\"daymet_Tile\":\"11029\",\"daymet_Elevation\":\"210 meters\",\"daymet_url_reference:\":\"How to cite: Thornton; P.E.; M.M. Thornton; B.W. Mayer; Y. Wei; R. Devarakonda; R.S. Vose; and R.B. Cook. 2016. Daymet: Daily Surface Weather Data on a 1-km Grid for North America; Version 3. ORNL DAAC; Oak Ridge; Tennessee; USA. http://dx.doi.org/10.3334/ORNLDAAC/1328\",\"daymet_unit\":\"mm\"},\"Data\":{\"2015-01-01 00\":[\"0.000E+000\"],\"2015-01-02 00\":[\"3.000E+000\"],\"2015-01-03 00\":[\"1.600E+001\"],\"2015-01-04 00\":[\"1.700E+001\"],\"2015-01-05 00\":[\"1.400E+001\"],\"2015-01-06 00\":[\"0.000E+000\"],\"2015-01-07 00\":[\"0.000E+000\"],\"2015-01-08 00\":[\"0.000E+000\"],\"2015-01-09 00\":[\"0.000E+000\"],\"2015-01-10 00\":[\"0.000E+000\"],\"2015-01-11 00\":[\"0.000E+000\"],\"2015-01-12 00\":[\"4.000E+000\"],\"2015-01-13 00\":[\"1.000E+000\"],\"2015-01-14 00\":[\"2.000E+000\"],\"2015-01-15 00\":[\"0.000E+000\"],\"2015-01-16 00\":[\"3.000E+000\"],\"2015-01-17 00\":[\"0.000E+000\"],\"2015-01-18 00\":[\"0.000E+000\"],\"2015-01-19 00\":[\"0.000E+000\"],\"2015-01-20 00\":[\"0.000E+000\"],\"2015-01-21 00\":[\"0.000E+000\"],\"2015-01-22 00\":[\"0.000E+000\"],\"2015-01-23 00\":[\"9.000E+000\"],\"2015-01-24 00\":[\"1.400E+001\"],\"2015-01-25 00\":[\"0.000E+000\"],\"2015-01-26 00\":[\"0.000E+000\"],\"2015-01-27 00\":[\"0.000E+000\"],\"2015-01-28 00\":[\"0.000E+000\"],\"2015-01-29 00\":[\"0.000E+000\"],\"2015-01-30 00\":[\"0.000E+000\"],\"2015-01-31 00\":[\"0.000E+000\"],\"2015-02-01 00\":[\"0.000E+000\"],\"2015-02-02 00\":[\"1.600E+001\"],\"2015-02-03 00\":[\"0.000E+000\"],\"2015-02-04 00\":[\"0.000E+000\"],\"2015-02-05 00\":[\"0.000E+000\"],\"2015-02-06 00\":[\"0.000E+000\"],\"2015-02-07 00\":[\"0.000E+000\"],\"2015-02-08 00\":[\"0.000E+000\"],\"2015-02-09 00\":[\"5.000E+000\"],\"2015-02-10 00\":[\"1.400E+001\"],\"2015-02-11 00\":[\"0.000E+000\"],\"2015-02-12 00\":[\"0.000E+000\"],\"2015-02-13 00\":[\"0.000E+000\"],\"2015-02-14 00\":[\"0.000E+000\"],\"2015-02-15 00\":[\"0.000E+000\"],\"2015-02-16 00\":[\"0.000E+000\"],\"2015-02-17 00\":[\"2.500E+001\"],\"2015-02-18 00\":[\"0.000E+000\"],\"2015-02-19 00\":[\"0.000E+000\"],\"2015-02-20 00\":[\"0.000E+000\"],\"2015-02-21 00\":[\"0.000E+000\"],\"2015-02-22 00\":[\"1.500E+001\"],\"2015-02-23 00\":[\"1.000E+001\"],\"2015-02-24 00\":[\"2.000E+000\"],\"2015-02-25 00\":[\"4.000E+000\"],\"2015-02-26 00\":[\"1.500E+001\"],\"2015-02-27 00\":[\"0.000E+000\"],\"2015-02-28 00\":[\"0.000E+000\"],\"2015-03-01 00\":[\"4.000E+000\"],\"2015-03-02 00\":[\"6.000E+000\"],\"2015-03-03 00\":[\"1.000E+000\"],\"2015-03-04 00\":[\"5.000E+000\"],\"2015-03-05 00\":[\"1.000E+000\"],\"2015-03-06 00\":[\"3.000E+000\"],\"2015-03-07 00\":[\"0.000E+000\"],\"2015-03-08 00\":[\"0.000E+000\"],\"2015-03-09 00\":[\"0.000E+000\"],\"2015-03-10 00\":[\"0.000E+000\"],\"2015-03-11 00\":[\"0.000E+000\"],\"2015-03-12 00\":[\"1.000E+000\"],\"2015-03-13 00\":[\"3.000E+000\"],\"2015-03-14 00\":[\"1.400E+001\"],\"2015-03-15 00\":[\"0.000E+000\"],\"2015-03-16 00\":[\"0.000E+000\"],\"2015-03-17 00\":[\"0.000E+000\"],\"2015-03-18 00\":[\"0.000E+000\"],\"2015-03-19 00\":[\"1.100E+001\"],\"2015-03-20 00\":[\"1.300E+001\"],\"2015-03-21 00\":[\"0.000E+000\"],\"2015-03-22 00\":[\"4.000E+000\"],\"2015-03-23 00\":[\"1.700E+001\"],\"2015-03-24 00\":[\"0.000E+000\"],\"2015-03-25 00\":[\"0.000E+000\"],\"2015-03-26 00\":[\"1.000E+000\"],\"2015-03-27 00\":[\"0.000E+000\"],\"2015-03-28 00\":[\"0.000E+000\"],\"2015-03-29 00\":[\"0.000E+000\"],\"2015-03-30 00\":[\"2.000E+000\"],\"2015-03-31 00\":[\"3.000E+000\"],\"2015-04-01 00\":[\"0.000E+000\"],\"2015-04-02 00\":[\"0.000E+000\"],\"2015-04-03 00\":[\"4.000E+000\"],\"2015-04-04 00\":[\"6.000E+000\"],\"2015-04-05 00\":[\"0.000E+000\"],\"2015-04-06 00\":[\"0.000E+000\"],\"2015-04-07 00\":[\"1.100E+001\"],\"2015-04-08 00\":[\"0.000E+000\"],\"2015-04-09 00\":[\"0.000E+000\"],\"2015-04-10 00\":[\"0.000E+000\"],\"2015-04-11 00\":[\"1.200E+001\"],\"2015-04-12 00\":[\"0.000E+000\"],\"2015-04-13 00\":[\"9.000E+000\"],\"2015-04-14 00\":[\"4.100E+001\"],\"2015-04-15 00\":[\"7.000E+000\"],\"2015-04-16 00\":[\"3.500E+001\"],\"2015-04-17 00\":[\"4.000E+000\"],\"2015-04-18 00\":[\"2.000E+001\"],\"2015-04-19 00\":[\"1.200E+001\"],\"2015-04-20 00\":[\"2.700E+001\"],\"2015-04-21 00\":[\"1.000E+000\"],\"2015-04-22 00\":[\"0.000E+000\"],\"2015-04-23 00\":[\"0.000E+000\"],\"2015-04-24 00\":[\"0.000E+000\"],\"2015-04-25 00\":[\"8.000E+000\"],\"2015-04-26 00\":[\"1.100E+001\"],\"2015-04-27 00\":[\"0.000E+000\"],\"2015-04-28 00\":[\"0.000E+000\"],\"2015-04-29 00\":[\"9.000E+000\"],\"2015-04-30 00\":[\"1.000E+001\"],\"2015-05-01 00\":[\"0.000E+000\"],\"2015-05-02 00\":[\"0.000E+000\"],\"2015-05-03 00\":[\"0.000E+000\"],\"2015-05-04 00\":[\"0.000E+000\"],\"2015-05-05 00\":[\"0.000E+000\"],\"2015-05-06 00\":[\"0.000E+000\"],\"2015-05-07 00\":[\"0.000E+000\"],\"2015-05-08 00\":[\"0.000E+000\"],\"2015-05-09 00\":[\"0.000E+000\"],\"2015-05-10 00\":[\"0.000E+000\"],\"2015-05-11 00\":[\"0.000E+000\"],\"2015-05-12 00\":[\"0.000E+000\"],\"2015-05-13 00\":[\"0.000E+000\"],\"2015-05-14 00\":[\"0.000E+000\"],\"2015-05-15 00\":[\"0.000E+000\"],\"2015-05-16 00\":[\"0.000E+000\"],\"2015-05-17 00\":[\"0.000E+000\"],\"2015-05-18 00\":[\"0.000E+000\"],\"2015-05-19 00\":[\"4.000E+000\"],\"2015-05-20 00\":[\"0.000E+000\"],\"2015-05-21 00\":[\"0.000E+000\"],\"2015-05-22 00\":[\"0.000E+000\"],\"2015-05-23 00\":[\"0.000E+000\"],\"2015-05-24 00\":[\"0.000E+000\"],\"2015-05-25 00\":[\"0.000E+000\"],\"2015-05-26 00\":[\"5.000E+000\"],\"2015-05-27 00\":[\"1.900E+001\"],\"2015-05-28 00\":[\"0.000E+000\"],\"2015-05-29 00\":[\"4.000E+000\"],\"2015-05-30 00\":[\"0.000E+000\"],\"2015-05-31 00\":[\"0.000E+000\"],\"2015-06-01 00\":[\"9.000E+000\"],\"2015-06-02 00\":[\"5.000E+000\"],\"2015-06-03 00\":[\"2.100E+001\"],\"2015-06-04 00\":[\"9.000E+000\"],\"2015-06-05 00\":[\"0.000E+000\"],\"2015-06-06 00\":[\"0.000E+000\"],\"2015-06-07 00\":[\"0.000E+000\"],\"2015-06-08 00\":[\"0.000E+000\"],\"2015-06-09 00\":[\"9.000E+000\"],\"2015-06-10 00\":[\"7.000E+000\"],\"2015-06-11 00\":[\"0.000E+000\"],\"2015-06-12 00\":[\"1.000E+000\"],\"2015-06-13 00\":[\"0.000E+000\"],\"2015-06-14 00\":[\"0.000E+000\"],\"2015-06-15 00\":[\"0.000E+000\"],\"2015-06-16 00\":[\"0.000E+000\"],\"2015-06-17 00\":[\"0.000E+000\"],\"2015-06-18 00\":[\"0.000E+000\"],\"2015-06-19 00\":[\"3.000E+000\"],\"2015-06-20 00\":[\"0.000E+000\"],\"2015-06-21 00\":[\"6.000E+000\"],\"2015-06-22 00\":[\"0.000E+000\"],\"2015-06-23 00\":[\"0.000E+000\"],\"2015-06-24 00\":[\"0.000E+000\"],\"2015-06-25 00\":[\"0.000E+000\"],\"2015-06-26 00\":[\"0.000E+000\"],\"2015-06-27 00\":[\"0.000E+000\"],\"2015-06-28 00\":[\"9.000E+000\"],\"2015-06-29 00\":[\"0.000E+000\"],\"2015-06-30 00\":[\"0.000E+000\"],\"2015-07-01 00\":[\"1.700E+001\"],\"2015-07-02 00\":[\"1.300E+001\"],\"2015-07-03 00\":[\"3.200E+001\"],\"2015-07-04 00\":[\"3.000E+001\"],\"2015-07-05 00\":[\"4.000E+000\"],\"2015-07-06 00\":[\"0.000E+000\"],\"2015-07-07 00\":[\"0.000E+000\"],\"2015-07-08 00\":[\"0.000E+000\"],\"2015-07-09 00\":[\"0.000E+000\"],\"2015-07-10 00\":[\"0.000E+000\"],\"2015-07-11 00\":[\"0.000E+000\"],\"2015-07-12 00\":[\"0.000E+000\"],\"2015-07-13 00\":[\"0.000E+000\"],\"2015-07-14 00\":[\"0.000E+000\"],\"2015-07-15 00\":[\"2.900E+001\"],\"2015-07-16 00\":[\"0.000E+000\"],\"2015-07-17 00\":[\"0.000E+000\"],\"2015-07-18 00\":[\"0.000E+000\"],\"2015-07-19 00\":[\"0.000E+000\"],\"2015-07-20 00\":[\"0.000E+000\"],\"2015-07-21 00\":[\"3.000E+000\"],\"2015-07-22 00\":[\"5.000E+000\"],\"2015-07-23 00\":[\"0.000E+000\"],\"2015-07-24 00\":[\"0.000E+000\"],\"2015-07-25 00\":[\"0.000E+000\"],\"2015-07-26 00\":[\"0.000E+000\"],\"2015-07-27 00\":[\"0.000E+000\"],\"2015-07-28 00\":[\"0.000E+000\"],\"2015-07-29 00\":[\"0.000E+000\"],\"2015-07-30 00\":[\"0.000E+000\"],\"2015-07-31 00\":[\"0.000E+000\"],\"2015-08-01 00\":[\"0.000E+000\"],\"2015-08-02 00\":[\"0.000E+000\"],\"2015-08-03 00\":[\"0.000E+000\"],\"2015-08-04 00\":[\"0.000E+000\"],\"2015-08-05 00\":[\"0.000E+000\"],\"2015-08-06 00\":[\"0.000E+000\"],\"2015-08-07 00\":[\"1.700E+001\"],\"2015-08-08 00\":[\"0.000E+000\"],\"2015-08-09 00\":[\"0.000E+000\"],\"2015-08-10 00\":[\"0.000E+000\"],\"2015-08-11 00\":[\"5.200E+001\"],\"2015-08-12 00\":[\"0.000E+000\"],\"2015-08-13 00\":[\"0.000E+000\"],\"2015-08-14 00\":[\"0.000E+000\"],\"2015-08-15 00\":[\"0.000E+000\"],\"2015-08-16 00\":[\"0.000E+000\"],\"2015-08-17 00\":[\"0.000E+000\"],\"2015-08-18 00\":[\"4.300E+001\"],\"2015-08-19 00\":[\"1.300E+001\"],\"2015-08-20 00\":[\"2.300E+001\"],\"2015-08-21 00\":[\"2.000E+000\"],\"2015-08-22 00\":[\"0.000E+000\"],\"2015-08-23 00\":[\"8.000E+000\"],\"2015-08-24 00\":[\"1.200E+001\"],\"2015-08-25 00\":[\"0.000E+000\"],\"2015-08-26 00\":[\"0.000E+000\"],\"2015-08-27 00\":[\"0.000E+000\"],\"2015-08-28 00\":[\"0.000E+000\"],\"2015-08-29 00\":[\"0.000E+000\"],\"2015-08-30 00\":[\"0.000E+000\"],\"2015-08-31 00\":[\"1.900E+001\"],\"2015-09-01 00\":[\"0.000E+000\"],\"2015-09-02 00\":[\"0.000E+000\"],\"2015-09-03 00\":[\"0.000E+000\"],\"2015-09-04 00\":[\"0.000E+000\"],\"2015-09-05 00\":[\"0.000E+000\"],\"2015-09-06 00\":[\"4.000E+000\"],\"2015-09-07 00\":[\"0.000E+000\"],\"2015-09-08 00\":[\"0.000E+000\"],\"2015-09-09 00\":[\"1.100E+001\"],\"2015-09-10 00\":[\"1.300E+001\"],\"2015-09-11 00\":[\"1.300E+001\"],\"2015-09-12 00\":[\"0.000E+000\"],\"2015-09-13 00\":[\"0.000E+000\"],\"2015-09-14 00\":[\"0.000E+000\"],\"2015-09-15 00\":[\"0.000E+000\"],\"2015-09-16 00\":[\"0.000E+000\"],\"2015-09-17 00\":[\"0.000E+000\"],\"2015-09-18 00\":[\"0.000E+000\"],\"2015-09-19 00\":[\"0.000E+000\"],\"2015-09-20 00\":[\"0.000E+000\"],\"2015-09-21 00\":[\"0.000E+000\"],\"2015-09-22 00\":[\"5.000E+000\"],\"2015-09-23 00\":[\"0.000E+000\"],\"2015-09-24 00\":[\"0.000E+000\"],\"2015-09-25 00\":[\"3.200E+001\"],\"2015-09-26 00\":[\"5.000E+000\"],\"2015-09-27 00\":[\"2.000E+000\"],\"2015-09-28 00\":[\"1.000E+000\"],\"2015-09-29 00\":[\"7.000E+000\"],\"2015-09-30 00\":[\"5.000E+000\"],\"2015-10-01 00\":[\"2.000E+000\"],\"2015-10-02 00\":[\"1.000E+001\"],\"2015-10-03 00\":[\"1.700E+001\"],\"2015-10-04 00\":[\"4.600E+001\"],\"2015-10-05 00\":[\"1.000E+000\"],\"2015-10-06 00\":[\"4.000E+000\"],\"2015-10-07 00\":[\"0.000E+000\"],\"2015-10-08 00\":[\"0.000E+000\"],\"2015-10-09 00\":[\"0.000E+000\"],\"2015-10-10 00\":[\"2.400E+001\"],\"2015-10-11 00\":[\"1.500E+001\"],\"2015-10-12 00\":[\"0.000E+000\"],\"2015-10-13 00\":[\"5.000E+000\"],\"2015-10-14 00\":[\"0.000E+000\"],\"2015-10-15 00\":[\"0.000E+000\"],\"2015-10-16 00\":[\"0.000E+000\"],\"2015-10-17 00\":[\"0.000E+000\"],\"2015-10-18 00\":[\"0.000E+000\"],\"2015-10-19 00\":[\"0.000E+000\"],\"2015-10-20 00\":[\"0.000E+000\"],\"2015-10-21 00\":[\"0.000E+000\"],\"2015-10-22 00\":[\"0.000E+000\"],\"2015-10-23 00\":[\"0.000E+000\"],\"2015-10-24 00\":[\"0.000E+000\"],\"2015-10-25 00\":[\"0.000E+000\"],\"2015-10-26 00\":[\"5.000E+000\"],\"2015-10-27 00\":[\"1.200E+001\"],\"2015-10-28 00\":[\"9.000E+000\"],\"2015-10-29 00\":[\"3.000E+000\"],\"2015-10-30 00\":[\"0.000E+000\"],\"2015-10-31 00\":[\"0.000E+000\"],\"2015-11-01 00\":[\"2.100E+001\"],\"2015-11-02 00\":[\"3.900E+001\"],\"2015-11-03 00\":[\"2.600E+001\"],\"2015-11-04 00\":[\"1.000E+000\"],\"2015-11-05 00\":[\"2.000E+000\"],\"2015-11-06 00\":[\"1.300E+001\"],\"2015-11-07 00\":[\"1.600E+001\"],\"2015-11-08 00\":[\"2.100E+001\"],\"2015-11-09 00\":[\"3.500E+001\"],\"2015-11-10 00\":[\"4.000E+000\"],\"2015-11-11 00\":[\"0.000E+000\"],\"2015-11-12 00\":[\"0.000E+000\"],\"2015-11-13 00\":[\"0.000E+000\"],\"2015-11-14 00\":[\"0.000E+000\"],\"2015-11-15 00\":[\"0.000E+000\"],\"2015-11-16 00\":[\"0.000E+000\"],\"2015-11-17 00\":[\"0.000E+000\"],\"2015-11-18 00\":[\"1.100E+001\"],\"2015-11-19 00\":[\"3.800E+001\"],\"2015-11-20 00\":[\"0.000E+000\"],\"2015-11-21 00\":[\"0.000E+000\"],\"2015-11-22 00\":[\"0.000E+000\"],\"2015-11-23 00\":[\"0.000E+000\"],\"2015-11-24 00\":[\"0.000E+000\"],\"2015-11-25 00\":[\"0.000E+000\"],\"2015-11-26 00\":[\"0.000E+000\"],\"2015-11-27 00\":[\"0.000E+000\"],\"2015-11-28 00\":[\"0.000E+000\"],\"2015-11-29 00\":[\"0.000E+000\"],\"2015-11-30 00\":[\"0.000E+000\"],\"2015-12-01 00\":[\"0.000E+000\"],\"2015-12-02 00\":[\"1.000E+000\"],\"2015-12-03 00\":[\"1.000E+000\"],\"2015-12-04 00\":[\"0.000E+000\"],\"2015-12-05 00\":[\"0.000E+000\"],\"2015-12-06 00\":[\"0.000E+000\"],\"2015-12-07 00\":[\"0.000E+000\"],\"2015-12-08 00\":[\"0.000E+000\"],\"2015-12-09 00\":[\"0.000E+000\"],\"2015-12-10 00\":[\"0.000E+000\"],\"2015-12-11 00\":[\"0.000E+000\"],\"2015-12-12 00\":[\"0.000E+000\"],\"2015-12-13 00\":[\"0.000E+000\"],\"2015-12-14 00\":[\"2.000E+000\"],\"2015-12-15 00\":[\"3.000E+000\"],\"2015-12-16 00\":[\"0.000E+000\"],\"2015-12-17 00\":[\"4.800E+001\"],\"2015-12-18 00\":[\"1.300E+001\"],\"2015-12-19 00\":[\"0.000E+000\"],\"2015-12-20 00\":[\"0.000E+000\"],\"2015-12-21 00\":[\"0.000E+000\"],\"2015-12-22 00\":[\"4.800E+001\"],\"2015-12-23 00\":[\"1.200E+001\"],\"2015-12-24 00\":[\"4.300E+001\"],\"2015-12-25 00\":[\"2.600E+001\"],\"2015-12-26 00\":[\"5.000E+000\"],\"2015-12-27 00\":[\"0.000E+000\"],\"2015-12-28 00\":[\"8.000E+000\"],\"2015-12-29 00\":[\"2.900E+001\"],\"2015-12-30 00\":[\"1.900E+001\"],\"2015-12-31 00\":[\"5.700E+001\"]}}";

        [Trait("Priority", "1")]
        [Fact]
        public void CheckStatus()
        {
            Precipitation precip = new Precipitation()
            {
                Input = Newtonsoft.Json.JsonConvert.DeserializeObject<TimeSeriesInput>(inputObject)
            };
            Dictionary<string, string> status = Daymet.CheckStatus(precip.Input);
            Assert.Equal("OK", status["status"]);
        }

        [Trait("Priority", "1")]
        [Theory]
        [InlineData("default", "2015-01-01 00", 0.0)]
        [InlineData("daily", "2015-01-02 00", 3.0)]
        [InlineData("weekly", "2015-01-01 00", 50.0)]
        [InlineData("monthly", "2015-01-01 00", 83.0)]
        [InlineData("yearly", "2015-01-01 00", 1731.0)]
        public void TemporalAggregation(string aggregation, string date, double expected)
        {
            Precipitation precip = new Precipitation
            {
                Input = Newtonsoft.Json.JsonConvert.DeserializeObject<TimeSeriesInput>(inputObject),
                Output = Newtonsoft.Json.JsonConvert.DeserializeObject<TimeSeriesOutput>(outputObject)
            };
            string errorMsg = "";
            Daymet daymet = new Daymet();
            precip.Input.TemporalResolution = aggregation;
            precip.Output = daymet.TemporalAggregation(out errorMsg, precip.Output, precip.Input);
            Assert.Equal(expected, Convert.ToDouble(precip.Output.Data[date][0]));

        }

    }
}
