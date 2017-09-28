using System;
using System.Data;
using System.Globalization;

namespace GCSOLAR
{
    public class Depend
    {
        /// <summary>
        /// For user selected dates and positions on the northern hemisphere, this routine computes 
        /// photolysis rates and half-lives for xenobiotics in waterbodies as a function of time-of-day.
        /// Author, date: D. Cline and R. Zepp, Feb. 1976
        /// Algorithm: Given molar extinction coefficients of the pollutant at wave lengths > 297.5 nm, 
        /// the attentuation coefficients and refractive index of the reaction medium, the quantum yield 
        /// for the reaction of the pollutant, the average ozone thickness in cm, the latitude and longitude, 
        /// the solar declination, the solar right ascension, and the sidereal time for the date of interest, 
        /// the routine computes direct photolysis rates. 
        /// </summary>
        /// <param name="dtday"></param>
        public void CalculatePhotolysisRatesHalfLivesTDay(out DataTable dtOutput, out DataTable dtKL, Common common)
        {

            /* For user selected dates and positions on the northern hemisphere, this routine computes 
             * photolysis rates and half-lives for xenobiotics in waterbodies as a function of time-of-day.
             * Author, date: D. Cline and R. Zepp, Feb. 1976
             * Algorithm: Given molar extinction coefficients of the pollutant at wave lengths > 297.5 nm, 
             * the attentuation coefficients and refractive index of the reaction medium, the quantum yield 
             * for the reaction of the pollutant, the average ozone thickness in cm, the latitude and longitude, 
             * the solar declination, the solar right ascension, and the sidereal time for the date of interest, 
             * the routine computes direct photolysis rates. 
             */

            //
            // Xplot and yplot are used to store the time-of-day
            // vs. photolysis rate coordinates.                  
            //
            double[] xplot = new double[25];
            double[] yplot = new double[25];

            // DtLocal is a table to store information locally.
            // DtOutput is a table used to store the simulation output, which includes 
            // photolysis rates.  DtKL is used to store the Ka-Lambdas.
            DataTable dtLocal = new DataTable();
            dtOutput = new DataTable();            
            dtKL = new DataTable();           
            
            //
            // Table is used to store Z-lambda or w-lambda values.                   
            //
            double[,,] table = new double[46, 4, 10];
            double[] s = new double[46];
            double[] x1 = new double[41];
            double[] s1 = new double[41];
            double[] y = new double[46];
            double[] z = new double[46];
            
            //
            // Used to store the full season  name.
            // 
            string[] seatst = new string[4] {"SPRING", "SUMMER", "FALL", "WINTER"};
            string[] seats2 = new string[4] {"spring", "summer", "fall", "winter"};
     
            //
            // Wave1 contains the valid wavelengths for this routine.
            // Sa contains the valid solar altitudes.
            // Ut is used to store the universal times.
            // X2 is used as temporary storage for the invisible sn data.
            //
            double[] wave1 = new double[15] { 297.5, 300.0, 302.5, 305.0, 307.5, 310.0, 312.5, 315.0, 317.5, 320.0, 325.0, 
                                              330.0, 340.0, 360.0, 380.0 };
            double[] sa = new double[10] {5.0, 10.0, 20.0, 30.0, 40.0, 50.0, 60.0, 70.0, 80.0, 90.0};
            double[] ut;
            double[] x2 = new double[15];
            

            /*
             * Sunmid is used to store the solar noon time.
             * Sunsav is used to store the sunrise and sunset times.
             * Sun is used to store the sunrise and sunset times.
            */
            double[] sunmid = new double[2]; 
            double[] sunsav = new double[2];
            double[] sun = new double[2];

            double[,] defdec = common.getDefdec();
            double[,] defrgt = common.getDefrgt();
            double[,] defsid = common.getDefsid();
            double[,] defoza = common.getDefoza();
            double[] wave = common.getWave();
            double[] wgt = common.getWgt();
            double[] xx = common.xx;

            double depth;
            double zz;
            double xlat;
            double samid;
            double tempvar;
            double samidtemp;
            double saisa;
            double sum = 0.0;
            double half;
            double half1;
            double rate;
            double rate1;
            double ozz = 0.0;
            double ksa;
            double ut1;
            double ut2;
            double tottim;
            double totint;
            double sunave;
            double satemp;
            double yptemp;
            double q = common.q;
            double xlon = common.xlon;
            double aveozo = common.aveozo;

            double deltaz = 0.0;
            if (common.useDeltaz) deltaz = common.deltaz;
            
            double dfinal = common.dfinal;
            double dinc = common.dinc;

            int ilatsw = common.ilatsw;
            int numSeasons = common.iseasw;
            int ioz = common.ioz;
            int iwlam = common.iwlam;
            int izlam = common.izlam;
            int minwav = common.minwav;
            int maxwav = common.maxwav;

            int numLats = ilatsw;
            int isap1;
           
            bool noSeasonMatch = true;
            bool equalSA = false;
            bool higherSA = false;
            

            string errorMsg = "";
            
            // Add columns to the tables.
            if (common.ityp == 0)
            {
                dtLocal.Columns.Add("Latitude");
                dtLocal.Columns.Add("Season");
                dtOutput.Columns.Add("Latitude");
                dtOutput.Columns.Add("Season");
                dtKL.Columns.Add("Latitude");
                dtKL.Columns.Add("Season");
            }
            else
            {
                dtLocal.Columns.Add("Latitude");
                dtOutput.Columns.Add("Latitude");
                dtKL.Columns.Add("Latitude");
            }

            dtLocal.Columns.Add("Summary Items");
            dtLocal.Columns.Add("Solar Altitude");
            dtLocal.Columns.Add("Time of Day (hours)", typeof(double));
            dtLocal.Columns.Add("Rate (sec**-1)");
            dtLocal.Columns.Add("Half-Life (hours)");

            dtOutput.Columns.Add("Summary Items");
            dtOutput.Columns.Add("Solar Altitude");
            dtOutput.Columns.Add("Time of Day (hours)", typeof(double));
            dtOutput.Columns.Add("Rate (sec**-1)");
            dtOutput.Columns.Add("Half-Life (hours)");       

            dtKL.Columns.Add("Solar Altitude");
            dtKL.Columns.Add("Depth (cm)");
            dtKL.Columns.Add("Wavelength (nm)");
            dtKL.Columns.Add("Ka-Lambda (sec^(-1) nm^(-1))");

            if (ilatsw == 0) numLats = 10;
            if (numSeasons == 0) numSeasons = 4;

            /*
             * Main loop for latitude selection.
             */
            for (int ilat = 1; ilat <= numLats; ilat++)
            {
                int jj1 = 0;
                for (int j = 1; j <= 10; j++)
                {
                    if (common.ilattm[ilat - 1] <= ((j - 1) * 10.0))
                    {
                        jj1 = j;
                        break;
                    }
                }

                int jj2 = 0;
                if (jj1 != 1) jj2 = jj1 - 1;  // This will be needed for interpolation.
                
                // If non-typical conditions were selected, set season switch to 1.               
                if (common.ityp == 1) numSeasons = 1;

                /*
                 * Main season selection loop.
                 * */
                for (int iseason = 1; iseason <= numSeasons; iseason++)
                {
                    // Test for season selection.
                    int j = iseason;
                    if (common.ityp != 1 && numSeasons != 4)
                    {
                        for (j = 1; j <= 4; j++)
                        {
                            if (common.sease[iseason-1].Equals(seatst[j-1], StringComparison.OrdinalIgnoreCase)) 
                            {
                                noSeasonMatch = false;
                                break;
                            }
                        }
                        if (noSeasonMatch) continue;  // If the condition is true, no season match was found so go try again.
                    }
                    

                    // Begin the depth loop.
                    depth = common.dinit;

                    do
                    {
                        if (depth <= 0.0) depth = 0.001;

                        // Compute maximum solar altitude.
                        zz = 0.0;
                        xlat = common.ilattm[ilat - 1];

                        // If non-typical values were selected, use the specified value.
                        if (common.ityp == 1) xlat = common.typlat;
                         
                        //
                        // If non-typical values were selected, bypass the default ephemeride selection.
                        //
                        if (common.ityp != 1) 
                        {
                            // Get default mid-season values.
                            for (int i = 1; i <= 3; i++)
                            {
                                xx[i-1] = defdec [i-1, j-1];
                                xx[i + 2] = defrgt [i-1, j-1];
                                xx[i + 5] = defsid [i-1, j-1];
                            }
                        }

                        // Compute the solar zenith angle.
                        tempvar = 0.0;
                        common.timeofDay(out samid, tempvar, xx, out ut, xlat, xlon, zz);

                        samid = 90.0 - samid;

                        sunmid[0] = ut[0];
                        sunmid[1] = ut[1];

                        //
                        // Compute sunrise and sunset times.
                        //
                        zz = 1.0;
                        tempvar = 90.0 + 0.833; 
                        common.timeofDay (out samidtemp, tempvar, xx, out ut, xlat, xlon, zz);
                        sun[0] = ut[0];
                        sun[1] = ut[1];
                        sunsav[0] = ut[0];
                        sunsav[1] = ut[1];

                        //
                        // Determine number of passes thru loop
                        // e. g., determine if the solar altitude is
                        // between 5 and 90 degrees.
                        //
                        int isa1 = 11;
                        if (samid > 90.0) 
                        {
                            errorMsg = "Maximum solar altitude is out of range: " + samid;
                            return;
                        }
                        for (int isa = 1; isa <= 10; isa++)
                        {
                            isa1 = isa1 - 1;
                            if (samid == sa[isa1-1]) 
                            {
                                equalSA = true;
                                break;
                            }
                            if (samid > sa[isa1-1]) 
                            {
                                higherSA = true;
                                break;
                            }
                        }

                        if (equalSA == false && higherSA == false)
                        {
                            errorMsg = "An invalid solar altitude was encountered.";
                            return;
                        }

                        if (higherSA)
                        {
                            isa1 = isa1 + 1;
                        }

                        // Store ouptut data in table.
                        zz = 0.0;

                        // Declare temporary data rows.
                        DataRow drLocal1 = dtLocal.NewRow();
                        DataRow drLocal2 = dtLocal.NewRow();
                        drLocal1["Latitude"] = xlat;

                        if (common.ityp == 0)
                        {                           
                            drLocal1["Season"] = seatst[j - 1];
                        }

                        drLocal1["Solar Altitude"] = zz.ToString("F2", CultureInfo.InvariantCulture);
                        drLocal2["Solar Altitude"] = zz.ToString("F2", CultureInfo.InvariantCulture);
                        drLocal1["Time of Day (hours)"] = ut[0].ToString("F2", CultureInfo.InvariantCulture);
                        drLocal2["Time of Day (hours)"] = ut[1].ToString("F2", CultureInfo.InvariantCulture);
                        drLocal1["Rate (sec**-1)"] = zz.ToString("#.#####E+00", CultureInfo.InvariantCulture);
                        drLocal2["Rate (sec**-1)"] = zz.ToString("#.#####E+00", CultureInfo.InvariantCulture);
                        drLocal1["Half-Life (hours)"] = zz.ToString("#.#####E+00", CultureInfo.InvariantCulture);
                        drLocal2["Half-Life (hours)"] = zz.ToString("#.#####E+00", CultureInfo.InvariantCulture);
                        dtLocal.Rows.Add(drLocal1);
                        dtLocal.Rows.Add(drLocal2);
      
                        //
                        // Sunsum is used to store the total photolysis rate for a day.
                        //
                        double sunsum = 0.0;

                        //
                        // Initialize the plotting arrays.
                        //
                        xplot[0] = sun[0];
                        int maxplt = isa1 * 2 + 1;
                        xplot[maxplt - 1] = sun[1];
                        yplot[0] = 0.0;
                        yplot[maxplt - 1] = 0.0;

                        //
                        // Compute the intensity data.
                        //

                        //
                        // Select either the default or user specified ozone amounts.
                        //
                        if (ioz == 0)
                        {
                            double slope = 0.0;
                            double ozz2, ozz1, lat2, lat1;
                            lat1 = (jj1 - 1) * 10;
                            ozz1 = defoza[jj1 - 1, j - 1];
                            if (jj2 > 0)
                            {
                                ozz2 = defoza[jj2 - 1, j - 1];
                                lat2 = (jj2 - 1) * 10.0;
                                slope = (ozz2 - ozz1) / (lat2 - lat1);
                            }
                            ozz = ozz1 + slope * (xlat - lat1);
                        }
                        if (ioz == 1) ozz = aveozo;

                        //
                        // Begin the solar altitude loop.
                        //
                        for (int isa = 1; isa <= isa1; isa++)
                        {
                            //---------------------------------------------------------------------
                            // Compute the invisible data.
                            //---------------------------------------------------------------------
                            // Get the current solar altitude.
                            //---------------------------------------------------------------------
                            saisa = sa[isa - 1];
                            ksa = saisa;

                            //---------------------------------------------------------------------
                            // Call the SolarIntensities routine to compute the solar intensities.
                            //---------------------------------------------------------------------
                            SolarIntensities(common, sa[isa - 1], depth, deltaz, ozz, out sum, s);

                            //
                            // Test for zero rate of absorption.
                            //
                            if (Math.Abs(sum) <= 1.0e-35)
                            {
                                rate = 0.0;
                                half = 0.0;
                            }
                            else
                            {
                                if (deltaz != 0.0)
                                {
                                    sum = sum / (0.602e21 * deltaz);
                                }
                                else
                                {
                                    sum = sum / (0.602e21 * depth);
                                }
                                if (Math.Abs(sum) <= 1.0e-35) // Test for zero rate of absorption.
                                {
                                    rate = 0.0;
                                    half = 0.0;
                                }
                                else
                                {
                                    rate = q * sum;
                                    half = 0.693 / rate / 3600.0;
                                }
                            }

                            //---------------------------------------------------------------------
                            // Now include the solar altitude influence.
                            //---------------------------------------------------------------------
                            zz = 1.0;
                            tempvar = 90.0 - saisa;
                            common.timeofDay(out samidtemp, tempvar, xx, out ut, xlat, xlon, zz);

                            //---------------------------------------------------------------------
                            // Max solar altitude exceeded?
                            //---------------------------------------------------------------------
                            // if (isa != isa1) maxSolar = true;

                            //
                            // Test to see if max sa falls on a sa boundary.
                            //
                            //if (samid == sa[isa1-1] ) maxBoundary = true;

                            if ((isa == isa1) && (samid != sa[isa1 - 1]))
                            {
                                //
                                // No it dosen't, so interpolate.
                                //
                                satemp = sa[isa - 2];
                                yptemp = yplot[isa - 1];
                                rate = (samid - satemp) / (sa[isa - 1] - satemp) * (rate - yptemp) + yptemp;
                                half = 0.693 / rate / 3600.0;
                                ut[0] = sunmid[0];
                                ut[1] = sunmid[1];
                                saisa = samid;
                            }

                            isap1 = isa + 1;

                            //
                            // Compute the incremental times for morning and afternoon.
                            //
                            ut1 = ut[0] - sunsav[0];
                            ut2 = ut[1] - sunsav[1];

                            //
                            // Save the morning and afteernoon times for plotting.
                            //
                            xplot[isap1 - 1] = ut[0];
                            int itemp = maxplt - isa;
                            xplot[itemp - 1] = ut[1];

                            //
                            // Save the rates for plotting.
                            //
                            yplot[isap1 - 1] = rate;
                            yplot[itemp - 1] = rate;

                            //
                            // Compute the area under the time/rate curve for this time segment.
                            //
                            rate1 = ut1 * rate;
                            if (rate1 > 0.0) half1 = 0.693 / rate1 / 3600.0;

                            //
                            // Sum the area.
                            //
                            sunsum = sunsum + rate1;

                            //
                            // Save the times for the next interval.
                            //
                            sunsav[0] = ut[0];
                            sunsav[1] = ut[1];

                            // Store ouptut data in table
                            DataRow drLocal3 = dtLocal.NewRow();
                            DataRow drLocal4 = dtLocal.NewRow();

                            drLocal3["Solar Altitude"] = saisa.ToString("F2", CultureInfo.InvariantCulture);
                            drLocal4["Solar Altitude"] = saisa.ToString("F2", CultureInfo.InvariantCulture);
                            drLocal3["Time of Day (hours)"] = ut[0].ToString("F2", CultureInfo.InvariantCulture);
                            drLocal4["Time of Day (hours)"] = ut[1].ToString("F2", CultureInfo.InvariantCulture);
                            drLocal3["Rate (sec**-1)"] = rate.ToString("#.#####E+00", CultureInfo.InvariantCulture);
                            drLocal4["Rate (sec**-1)"] = rate.ToString("#.#####E+00", CultureInfo.InvariantCulture);
                            drLocal3["Half-Life (hours)"] = half.ToString("#.#####E+00", CultureInfo.InvariantCulture);
                            drLocal4["Half-Life (hours)"] = half.ToString("#.#####E+00", CultureInfo.InvariantCulture);

                            if (isa != isa1)
                            {
                                dtLocal.Rows.Add(drLocal3);
                                dtLocal.Rows.Add(drLocal4);
                            }
                            else
                            {
                                dtLocal.Rows.Add(drLocal3);
                            }


                            //-----------------------------------------------------------
                            // Compute the Ka-Lambda values and store them in a table.
                            //-----------------------------------------------------------                           
                            for (int k = minwav; k <= maxwav; k++)
                            {
                                s1[k - 1] = s[k - 1] / (0.602e21 * depth) / wgt[k - 1];
                                x1[k - 1] = wave[k - 1];
                                // Store values in a table.
                                DataRow drk1 = dtKL.NewRow();
                                if (k == minwav)
                                {
                                    drk1["Latitude"] = xlat;
                                }
                                if ((common.ityp == 0) && (k == minwav))
                                {
                                    drk1["Season"] = seatst[j - 1];
                                }
                                if (k == minwav) drk1["Depth (cm)"] = depth.ToString("E3", CultureInfo.InstalledUICulture);
                                if (k == minwav) drk1["Solar Altitude"] = saisa.ToString("F2", CultureInfo.InvariantCulture);
                                drk1["Wavelength (nm)"] = x1[k - 1].ToString("F2", CultureInfo.InstalledUICulture);
                                drk1["Ka-Lambda (sec^(-1) nm^(-1))"] = s1[k - 1].ToString("E3", CultureInfo.InstalledUICulture);
                                dtKL.Rows.Add(drk1);
                            }

                        }

                        //-----------------------------------------------------------
                        // Compute summaries
                        //-----------------------------------------------------------
                        tottim = sunmid[0] - sun[0];
                        totint = common.dintpt(maxplt, xplot, yplot, out errorMsg) * 3600.0;
                        rate = 0.693 / totint;
                        sunave = totint / (7200.0 * tottim);
                            
                        // Sort the data in ascending order using the "Time of day (hours)" column.
                        DataRow[] dra = dtLocal.Select("","[Time of Day (hours)] ASC");

                        // Store sorted data in temporary table.
                        DataTable dtTemp = dtLocal.Clone();
                        // System.Data.DataSetExtensions is not supported in .NET Core 2
                        foreach (DataRow row in dra)
                        {
                            dtTemp.ImportRow(row);
                        }
                        //dtTemp = dra.CopyToDataTable();

                        // Store the rows of the temporary table in output table. 
                        foreach (DataRow rows in dtTemp.Rows)
                        {
                            DataRow drTemp = dtOutput.NewRow();
                            drTemp["Latitude"] = rows["Latitude"].ToString();
                            if (common.ityp == 0) drTemp["Season"] = rows["Season"].ToString();
                            drTemp["Summary Items"] = rows["Summary Items"].ToString();
                            drTemp["Solar Altitude"] = rows["Solar Altitude"].ToString();
                            drTemp["Time of Day (hours)"] = rows["Time of Day (hours)"];
                            drTemp["Rate (sec**-1)"] = rows["Rate (sec**-1)"].ToString();
                            drTemp["Half-Life (hours)"] = rows["Half-Life (hours)"].ToString();
                            dtOutput.Rows.Add(drTemp);
                        }

                        // Declare new rows that will be used to store summary information.
                        DataRow drLocal5 = dtOutput.NewRow();
                        DataRow drLocal6 = dtOutput.NewRow();
                        DataRow drLocal7 = dtOutput.NewRow();
                        DataRow drLocal8 = dtOutput.NewRow();
                        DataRow drLocal9 = dtOutput.NewRow();

                        // Store summary information under the "Summary Items" column.
                        drLocal5["Summary Items"] = "Depth (cm) = " + depth.ToString("#.###E+00", CultureInfo.InstalledUICulture);
                        if (common.useDeltaz)
                        {
                            drLocal6["Summary Items"] = "Depth Point (cm) = " + deltaz.ToString("#.###E+00", CultureInfo.InstalledUICulture);
                        }
                        else
                        {
                            drLocal6["Summary Items"] = "Depth Point (cm) = None";
                        }

                        drLocal7["Summary Items"] = "Average Rate (sec**-1) = " + sunave.ToString("#.##E+00", CultureInfo.InvariantCulture);
                        drLocal8["Summary Items"] = "Integrated Rate (day**-1) = " + totint.ToString("#.##E+00", CultureInfo.InvariantCulture);
                        drLocal9["Summary Items"] = "Integrated Half-Life (days) = " + rate.ToString("#.##E+00", CultureInfo.InvariantCulture);
                        
                        dtOutput.Rows.Add(drLocal5);
                        dtOutput.Rows.Add(drLocal6);
                        dtOutput.Rows.Add(drLocal7);
                        dtOutput.Rows.Add(drLocal8);
                        dtOutput.Rows.Add(drLocal9);

                        dtLocal.Clear();  // Clear local table before next cycle loop.

                        //-----------------------------------------------------------
                        // Increment depth.
                        //-----------------------------------------------------------
                        if (depth == 0.001) depth = 0.0;
            
                        if (dinc != 0.0)
                        {
                            depth = depth + dinc;
                        }

                    } while(depth <= dfinal);

                }  //  End of season loop

                //
                // If non-typical values were selected, only one latitude is required.
                //
                if (common.ityp == 1) break;

            }  // End of Latitude Loop

            //
            // If w-lambda or z-lambda tables were requested, go
            // print them.
            //
             if (iwlam == 1 || izlam == 1) 
            {
                // CALL ZLAM (TABLE)
            }

        } // Closing bracket of the function


        /// <summary>
        /// This method computes solar intensities.
        /// </summary>
        /// <param name="sa"></param>
        /// <param name="depth"></param>
        /// <param name="deltaz"></param>
        /// <param name="ozone"></param>
        /// <param name="sum"></param>
        /// <param name="s"></param>
        public void SolarIntensities(Common common, double sa, double depth, double deltaz, double ozone, out double sum, double[] s)
        {
            double one1 = 1.0;
            double arad = 0.174533E-01;
            double onerfs = 0.93;
            double one2;
            double abwat1;
            double abxld;
            double abxls;
            double abd10;
            double abs10;
            double stheta;
            double th;
            double xls;
            double snfac;
            double hfac;
            double xld;
            double xtemp;
            double sinzmx;
            double sin2mx;
            double sinzpx;
            double sin2px;
            double tanzmx;
            double tan2mx;
            double tanzpx;
            double tan2px;
            double rfd;
            double murm1;
            double murp1;
            double onerfd;
            double xldave;
            double xlsave;
            double yold = 0.0;
            double zold = 0.0;
            double ztemp;
            double ytemp;
            double diviso;
            double musubr = common.musubr;
            int minwav = common.minwav;
            int maxwav = common.maxwav;
            double[] abwat = common.getAbwat();
            double[] eppest = null;
            double[] weight = common.getWeight();
            double[] wave = common.getWave();
            double[] h = new double[46];
            double[] sn = new double[46];
            double[] x1 = new double[46];
            double[] x2 = new double[46];
            double[] wave1 =  new double[22] {280.0, 282.5, 285.0, 287.5, 290.0, 292.5, 295.0, 297.5, 300.0, 302.5, 305.0,
                                              307.5, 310.0, 312.5, 315.0, 317.5, 320.0, 325.0, 330.0, 340.0, 360.0, 380.0};

            // Get absorption coefficients for specific type of contaminant.
            if (common.contaminantType == "Chemical")
            {
                eppest = common.getEppest();  // Units are in L/(mole cm)
            }
            if (common.contaminantType == "Biological")
            {
                eppest = common.getAbsBiological();  // Units are in hr^(-1) Watts^(-1) cm^2 nm
            }

            //
            // Loop thru the invisible wave lengths to compute the invisible intensities.
            //
            string errorMsg = "";
            GCSOLAR.Bener bn = new GCSOLAR.Bener();
            bn.Elevation = common.elevation;
            for (int i = 1; i <= 22; i++)
            {
                bn.BenerMethod(sa, wave1[i-1], ozone, out x1[i-1], out x2[i-1], out errorMsg);
                if (errorMsg != "")
                {
                    sum = 0.0;
                    return;
                }
            }
      
            //
            // Convert the intensity units.
            //
            if (common.contaminantType == "Chemical")
            {
                common.convert(x1, h, minwav, maxwav);
                common.convert(x2, sn, minwav, maxwav);
            }
            if (common.contaminantType == "Biological")
            {
                common.computeSolarIntensityOverInterval(x1, h, minwav, maxwav);
                common.computeSolarIntensityOverInterval(x1, sn, minwav, maxwav);
            }
            

            //
            // Compute the visible data if necessary
            //
            if (maxwav >= 25)
            {
                for (int i = 25; i <= maxwav; i++)
                {
                    common.tslam(ozone, sa, wave[i-1], out h[i-1], out sn[i-1], out errorMsg);
                    if (errorMsg != "")
                    {
                        sum = 0.0;
                        return;
                    }
                }
            }
     
            //
            // Compute intensities as a function of wavelength.
            //
            sum = 0.0;
            xls = depth * 2.0 * musubr * (musubr - Math.Sqrt(musubr * musubr - 1));
            th = (90.0 - sa) * arad;
            stheta = Math.Sin(th);
            snfac = 1.0;
            hfac = 1.0;
            xld = depth * musubr / (Math.Sqrt(musubr * musubr - stheta * stheta));

            if (th != 0.0)
            {
                //
                // If the solar zenith angle is > 0, compute it here
                //
                xtemp = Math.Asin (stheta / musubr);
                sinzmx = Math.Sin(th - xtemp);
                sin2mx = sinzmx * sinzmx;
                sinzpx = Math.Sin(th + xtemp);
                sin2px = sinzpx * sinzpx;
                tanzmx = Math.Tan(th - xtemp);
                tan2mx = tanzmx * tanzmx;
                tanzpx = Math.Tan(th + xtemp);
                tan2px = tanzpx * tanzpx;
                rfd = (sin2mx / sin2px + tan2mx / tan2px) / 2.0;

            }
            else
            {
                 murm1 = musubr - 1.0;
                 murp1 = musubr + 1.0;
                 rfd = (murm1 * murm1) / (murp1 * murp1);
            }

            onerfd = 1.0 - rfd;

            // * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
            // Compute the average rate of absorption per unit volume.
            // K is varied to process the incremental intensity for each wavelength.
            // * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
            xldave = xld;
            xlsave = xls;
            for (int k = minwav; k <= maxwav; k++)
            {
                abwat1 = -abwat[k-1];                         
                abxld = abwat1 * xldave;
                abxls = abwat1 * xlsave;

                if (deltaz != 0.0) 
                {
                    yold = h[k-1];
                    zold = sn[k-1];
                    h[k-1] = h[k-1] * Math.Pow(10.0, abxls);
                    sn[k-1] = sn[k-1] * Math.Pow(10.0, abxld);
                    xld = deltaz * musubr / (Math.Sqrt(musubr * musubr - stheta * stheta) );
                    xls = deltaz * 2.0 * musubr * (musubr - Math.Sqrt(musubr * musubr - 1) );
                    abxld = abwat1 * xld;
                    abxls = abwat1 * xls;
                }
                 
                one2 = Math.Pow(10.0, abxld);
                abd10 = one1 - one2;
                one2 = Math.Pow(10.0, abxls);
                abs10 = one1 - one2;
                ztemp = abd10 * sn[k-1];
                ztemp = ztemp * snfac * onerfd;
                ytemp = abs10 * h[k-1];
                ytemp = ytemp * hfac * onerfs;
                diviso = weight[k-1] * eppest[k-1] / ( -abwat1);
                s[k-1] = (ztemp + ytemp) * diviso;
                sum = sum + s[k-1];

                if (deltaz != 0.00)
                {
                    h[k-1] = yold;
                    sn[k-1] = zold;
                }               
            }

        } // Closing bracket of SolarIntensities

    }  // Closing bracket of the class
}
