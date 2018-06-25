using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Globalization;

namespace GCSOLAR
{
    public class MidDay
    {
        /// <summary>
        /// For user selected seasons and latitudes, this routine computes photolysis rates 
        /// and half-lives for xenobiotics in waterbodies. the computed values are for
        /// mid-season, mid-day conditions.
        /// </summary>
        /// <param name="dtmday"></param>
        public void CalculatePhotolysisRatesHalfLives(out DataTable dtmday, Common common)
        {
            /* FOR USER SELECTED SEASONS AND LATITUDES, THIS ROUTINE COMPUTES PHOTOLYSIS RATES 
             * AND HALF-LIVES FOR XENOBIOTICS IN WATERBODIES. THE COMPUTED VALUES ARE FOR 
             * MID-SEASON, MID-DAY CONDITIONS.
            */

            dtmday = new DataTable();
            int j1, jj1;
            int istrt;
            int istop;
            int kscal;
            double[, ,] table = new double[46, 4, 10];
            double one1 = 1.0;
            double one2, abd10, abs10;
            double onerfs = 0.93;
            double sum, sum1;
            double depth;
            double xls;
            double th;
            double stheta;
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
            double xld;
            double xldave;
            double xlsave;
            double quatum;
            double abwat1;
            double abxld;
            double abxls;
            double yold = 0.0;
            double zold = 0.0;
            double ztemp;
            double ytemp;
            double www;
            double wwy;
            double ddd;
            double tzlam = 0.0;
            double twlam = 0.0;
            double diviso;
            double half;
            double rate;
            int ilow = common.ilow;
            int iup = common.iup;
            double q = common.q;
            double iscal = common.iscal;
            double deltaz = common.deltaz;
            double musubr = common.musubr;
            int minwav = common.minwav;
            int maxwav = common.maxwav;
            double izlam = common.izlam;
            double iwlam = common.iwlam;
            double dinc = common.dinc;
            double dfinal = common.dfinal;
            double arad = 0.174533e-01; 

            double[] s = new double[46];
            double[] y = new double[46];
            double[] z = new double[46];
            double[] s1 = new double[48];
            double[] x1 = new double[48];
            double[] xn1 = new double[22];
            double[] xn2 = new double[22];
            double[,] theta = new double[10,4] { {80.0, 70.0, 80.0, 70.0}, {90.0, 80.0, 70.0, 60.0},{80.0, 90.0, 60.0, 50.0}, 
                                                 {70.0, 80.0, 50.0, 40.0}, {60.0, 70.0, 40.0, 30.0}, {50.0, 60.0, 30.0, 20.0}, 
                                                 {40.0, 50.0, 20.0, 10.0}, {30.0, 40.0, 10.0, 00.0}, {20.0, 30.0, 00.0, -1.0}, 
                                                 {10.0, 20.0, -1.0, -1.0} };

            string[] season = new string[4] {"SPRING", "SUMMER", "FALL", "WINTER"};
    
            bool match1Found = false;
            bool match2Found = false;
            bool print_heading = false;
            bool print_output = true;
            bool print_output1 = false;
            bool print_output2 = false;
            bool do_scalar = false;

            int[] lat1 = common.getLat();
            string[] sea1 = common.getSeasons();
            double[,] xH = common.getMiddayH();
            double[,] xSN = common.getMiddaySN();
            
            double[,,] H = common.getH();
            double[,,] SN = common.getSN();

            double[] wave = common.getWave();
            double[] abwat = common.getAbwat();
            double[] weight = common.getWeight();
            double[] wgt = common.getWgt();
            double[] eppest = common.getEppest();

            DataRow dr1 = dtmday.NewRow();
            dtmday.Columns.Add("LATITUDE");
            dtmday.Columns.Add("SEASON");
            dtmday.Columns.Add("DEPTH (km)");
            dtmday.Columns.Add("LD");
            dtmday.Columns.Add("LS");
            dtmday.Columns.Add("KA/SEC");
            dtmday.Columns.Add("RATE (sec**-1)");
            dtmday.Columns.Add("HALF LIFE (days)");
            dtmday.Columns.Add("WAVELENGTH (nm)");
            dtmday.Columns.Add("LAMBDA (nm)");

            //
            //  Assume no 'scalar' computations are required.
            //
            kscal = -1;

            if ((iscal != 1) || (deltaz != 0.001)) do_scalar = false;

            if (do_scalar)
            {
                //
                //       Determine if the 'scalar' command is for a single
                //       wavelength or a range of wavelengths
                //       assume a range of values; kscal = 1
                //
                kscal = 1;
                if (wave[ilow] == wave[iup] ) kscal = 0;

                if (kscal == 1) print_output1 = true;

                if (print_output1)
                {
                   //  WRITE (IUNIT, 8) WAVE (ILOW), WAVE (IUP)
                   // 8 FORMAT  ('1THE PHOTON SCALAR IRRADIANCE IN PHOTONS CM-2',        &
                   // &  ' SEC-1',/,' IS PRINTED UNDER THE KA/SEC COLUMN.',/,            &
                   // &  ' THE SCALAR IRRADIANCE IN WATTS CM-2 APPEARS UNDER THE',       &
                   // &  ' RATE COLUMN.',/,'0 WAVE LENGTH RANGE IS: ',F5.1,' TO ',F5.1)
                }

                if (kscal == 0) print_output2 = true;

                if (print_output2)
                {
                    // WRITE (IUNIT, 9) WAVE (ILOW)
                    // 9 FORMAT  ('1THE PHOTON SCALAR IRRADIANCE IN PHOTONS CM-2',        &
                    // &  ' SEC-1 NM-1',/,' IS PRINTED UNDER THE KA/SEC COLUMN.',/,       &
                    // &  ' THE SCALAR IRRADIANCE IN WATTS CM-2 NM-1 APPEARS UNDER THE',  &
                    // &  ' RATE COLUMN.',/,'0 WAVE LENGTH IS: ',F5.1)
                    //  WRITE (IUNIT, 10)
                    // 10 FORMAT  (' IRRADIANCE IN WATTS CM-2 ON A HORIZONTAL COLLECTOR', &
                    //  &  /,' IS PRINTED UNDER THE RATE COLUMN.')
                }
                
            }
            
  

            //
            //  Main loop for computing mid-day, mid-season photolysis rates.
            //
            int index = 0;  
            int j;

            while (index < lat1.Length)
            {                
                for (int i = 0; i < 15; i++)
                {
                    j = i + 7;
                    xn1[j] = xH[index,i];
                    xn2[j] = xSN[index,i];
                }

                // See if latitude selection is required.
                if (common.ilatsw > 0)
                {
                    // Latitude selection is required.
                    for (int i = 0; i < common.ilattm.Length; i++)
                    {
                        if (lat1[index] == common.ilattm[i]) 
                        {
                           // noLatMatch = false;
                            match1Found = true;
                            break;
                        }
                    }

                    if (match1Found) 
                    {
                        match1Found = false;
                    }
                    else  // If the condition is true, no latitude match was found so go try again.
                    {
                        index = index + 1;
                        continue;
                    }

                }

                // See if season selection is required.
                if (common.iseasw > 0)
                {
                    // Season selection is required.
                    for (int i = 0; i < common.sease.Length; i++)
                    {
                        if ( sea1[index].Equals(common.sease[i], StringComparison.OrdinalIgnoreCase) ) 
                        {
                            match2Found = true;
                            break;
                        }
                    }

                    if (match2Found)
                    {
                        match2Found = false;
                    }
                    else     // No season match was found so go try again.
                    {
                        index = index + 1;
                        continue;
                    }

                }

                common.convert(xn1, y, minwav, maxwav);
                common.convert(xn2, z, minwav, maxwav);

                // Determine the season subscript.
                int seasonNum = 1;
                for (int i = 0; i < 4; i++)
                {
                    if (sea1[index].Equals(season[i], StringComparison.OrdinalIgnoreCase))
                    {
                        seasonNum = i + 1;
                        break;
                    }
                }

                //
                // Compute jj1, the latitude subscript
                //
                jj1 = lat1[index] / 10 + 1;

                //
                // Move the visible data.
                // 
                for (int jj = 25; jj <= 46; jj++)
                {
                    y[jj - 1] = H[jj - 25, jj1 - 1, seasonNum - 1];
                    z[jj - 1] = SN[jj - 25, jj1 - 1, seasonNum - 1];
                }

                //
                //  All raw intensity data has been input and moved
                //  for this season and latitude.

                //
                //  Begin the depth loop.  Using a do-while loop.
                //
                depth = common.dinit;

                do
                {

                    if (depth <= 0.0) depth = 0.001;

                    //
                    // Compute rate, half-life and totals.
                    // 
                    sum = 0.0;
                    sum1 = 0.0;

                    //
                    // Compute j1, the latitude subscript
                    //
                    j1 = lat1[index] / 10 + 1;

                    //
                    // Compute the average pathlength for sky
                    // radiation under water.
                    // 
                    xls = depth * 2.0 * musubr * (musubr - Math.Sqrt(musubr * musubr - 1));

                    //
                    // Compute the solar zenith angle.
                    // 
                    th = (90.0 - theta[j1 - 1, seasonNum - 1]) * arad;
                    stheta = Math.Sin(th);

                    //
                    //  Compute the under water path length, xld.
                    //
                    xld = depth * musubr / (Math.Sqrt(musubr * musubr - stheta * stheta));

                    //
                    //      COMPUTE RFD
                    //

                    //
                    //       IF THE SOLAR ZENITH ANGLE IS > ZERO, COMPUTE IT HERE
                    //
                    if (th != 0.0)
                    {
                        xtemp = Math.Asin(stheta / musubr);
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
                    else  // Case of th = 0.0
                    {
                        murm1 = musubr - 1.0;
                        murp1 = musubr + 1.0;
                        rfd = (murm1 * murm1) / (murp1 * murp1);
                    }

                    onerfd = 1.0 - rfd;

                    //
                    // Compute the average rate of absorption per unit volume.
                    //
                    // k is varied to process the incremental intensity for
                    // each wave length.

                    xldave = xld;
                    xlsave = xls;
                    quatum = 1.0;
                    istrt = minwav;
                    istop = maxwav;

                    if ((iscal != 0) && (deltaz != 0.0))
                    {
                        istrt = ilow;
                        istop = iup;
                    }

                    for (int k = istrt; k <= istop; k++)
                    {
                        if (iscal == 1 && deltaz != 0.0)
                        {
                            quatum = 1.0 / (wave[k - 1] * 5.035e15);
                        }

                        abwat1 = -abwat[k - 1];
                        abxld = abwat1 * xldave;
                        abxls = abwat1 * xlsave;

                        if (deltaz != 0.0)
                        {
                            yold = y[k - 1];
                            zold = z[k - 1];
                            y[k - 1] = y[k - 1] * Math.Pow(10.0, abxls);
                            z[k - 1] = z[k - 1] * Math.Pow(10.0, abxld);
                            xld = deltaz * musubr / (Math.Sqrt(musubr * musubr - stheta * stheta));
                            xls = deltaz * 2.0 * musubr * (musubr - Math.Sqrt(musubr * musubr - 1));
                            abxld = abwat1 * xld;
                            abxls = abwat1 * xls;
                        }

                        //
                        //       The following 4 statements require the use of double
                        //       precision arithmetic to preserve accuracy during
                        //       the subtraction process.
                        //
                        one2 = Math.Pow(10.0, abxld);
                        abd10 = one1 - one2;
                        one2 = Math.Pow(10.0, abxls);
                        abs10 = one1 - one2;
                        ztemp = abd10 * z[k - 1];
                        ztemp = ztemp * onerfd;
                        ytemp = abs10 * y[k - 1];
                        ytemp = ytemp * onerfs;
                        www = 1.0;
                        wwy = 1.0;
                        if (kscal >= 0) wwy = weight[k - 1];
                        if (kscal == 1) www = wgt[k - 1];
                        diviso = wwy * www * weight[k - 1] * eppest[k - 1] / (-abwat1);
                        s[k - 1] = (ztemp + ytemp) * diviso;
                        sum = sum + s[k - 1];
                        if ((iscal == 1) && (deltaz == 0.001)) sum1 = sum1 + s[k - 1] * quatum;
                        ddd = depth;
                        if ((iscal == 1) && (deltaz == 0.001)) ddd = 0.001;
                        tzlam = (z[k - 1] * onerfd * xld + y[k - 1] * onerfs * xls) / ddd;
                        if (izlam == 1) table[k - 1, seasonNum - 1, j1 - 1] = tzlam;
                        twlam = z[k - 1] * onerfd + y[k - 1] * onerfs;
                        if (iwlam == 1) table[k - 1, seasonNum - 1, j1 - 1] = twlam;
                        if (deltaz != 0.0)
                        {
                            y[k - 1] = yold;
                            z[k - 1] = zold;
                        }

                    }

                    //
                    // Test for zero rate of absorption.
                    //
                    if (Math.Abs(sum) > 1.0e-30)
                    {
                        //
                        //  Compute the photolysis rate and half-life.
                        //
                        if (deltaz != 0.0)
                        {
                            sum = sum / (0.602e21 * deltaz);
                        }
                        else
                        {
                            sum = sum / (0.602e21 * depth);
                        }

                        if (Math.Abs(sum) <= 1.0e-35)
                        {
                            //  Absorption was zero.
                            rate = 0.0;
                            half = 0.0;
                        }
                        rate = q * sum;
                        if ((iscal != 1) || (deltaz != 0.001))
                        {
                            half = 0.693 / rate / 3600.0;
                        }
                        else
                        {
                            sum1 = sum1 / (0.602e21 * deltaz);
                            rate = sum1;
                            half = rate * twlam / tzlam;  // Wilson Melendez: The use of twlam and tzlam here looks suspect. It could be a bug.
                        }
                    }
                    else  //  Absorption was zero.
                    {
                        rate = 0.0;
                        half = 0.0;
                    }

                    if (print_heading)
                    {
                        // WRITE (IUNIT, 1091) PNAME, RIVER
                        // 1091 FORMAT  ('1XENOBIOTIC NAME: ',5A4,/,' WATER IDENTIFICATION: ',5A4)
                        if (common.iatmos == 1)
                        {
                            //string str1 = "Type of atmosphere: terrestrial.";
                        }
                        else if (common.iatmos == 2)
                        {
                            //string str2 = "Type of atmosphere: marine.";
                        }

                    }

                    if (print_output)
                    {
                        dr1["LATITUDE"] = lat1[index].ToString("F2", CultureInfo.InvariantCulture);
                        dr1["SEASON"] = sea1[index];
                        dr1["DEPTH (km)"] = depth.ToString("F2", CultureInfo.InvariantCulture);
                        dr1["LD"] = xld.ToString("E2", CultureInfo.InvariantCulture);
                        dr1["LS"] = xls.ToString("E2", CultureInfo.InvariantCulture);
                        dr1["KA/SEC"] = sum.ToString("E2", CultureInfo.InvariantCulture);
                        dr1["RATE (sec**-1)"] = rate.ToString("E2", CultureInfo.InvariantCulture);
                        dr1["HALF LIFE (days)"] = half.ToString("E2", CultureInfo.InvariantCulture);
                        dtmday.Rows.Add(dr1);
                    }

                    //
                    //  Compute the KA-Lambda values
                    //
                    for (int k = minwav; k <= maxwav; k++)
                    {
                        s1[k - 1] = s[k - 1] / (0.602E21 * depth) / wgt[k - 1];
                        x1[k - 1] = wave[k - 1];
                        DataRow dr2 = dtmday.NewRow();
                        dr2["WAVELENGTH (nm)"] = x1[k - 1].ToString("E6", CultureInfo.InvariantCulture);
                        dr2["LAMBDA (nm)"] = s1[k - 1].ToString("E6", CultureInfo.InvariantCulture);
                        dtmday.Rows.Add(dr2);
                    }

                    //
                    // Increment depth.
                    //
                    if (depth == 0.001) depth = 0.0;

                    if (dinc != 0.0)
                    {
                        depth = depth + dinc;
                    }

                } while (depth <= dfinal);

                index = index + 1;
               
            }
      
            //
            // If w-lambda or z-lambda tables were requested, go
            // print them.
            //
             if (iwlam == 1 || izlam == 1) 
            {
                // CALL ZLAM (TABLE)
            }
            
        }
    }
}
