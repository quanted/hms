using System;
using System.Data;

namespace GCSOLAR
{
    public class Bener
    {
        protected int _iatmos;
        private double elevation;

        public Bener()
        {
            _iatmos = 1;
            elevation = 0.0;
        }

        public int iatmos
        {
            get
            {
                return _iatmos;
            }
            set
            {
                _iatmos = value;
            }
        }

        public double Elevation
        {
            get
            {
                return this.elevation;
            }
            set
            {
                this.elevation = value;
            }
        }

        /// <summary>
        /// The routine generates tabular values of intensities as found in Bener on pages 22-39. 
        /// the tables were used to verify the data that is produced by an equation that was fit 
        /// to bener's data. Sky intensities from the whole sky on a horizontal surface (h) and 
        /// the vertical component of direct solar intensity (sn) are produced as a function of 
        /// solar altitude, ozone amount, and wavelength. All intensities pertain to sea level 
        /// and cloudless skies.
        /// </summary>
        /// <param name="solat"></param>
        /// <param name="wavgth"></param>
        /// <param name="ozamt"></param>
        /// <param name="hh"></param>
        /// <param name="sn"></param>
        /// <param name="err"></param>
        public void BenerMethod(double solat, double wavgth, double ozamt, out double hh, out double sn, out string err)
        {
            err = "";

            double[] p = new double[36] {0.5777, 0.427, 0.977, 19.9, 3.285, 1.104, 0.7879, 12.8,
                                       1.523, 0.1978, 1.079, 1.8, 0.3, 7.4, 0.1, 0.288, 0.5399, 0.023, 0.7847,  
                                       0.671, 0.5866, 0.08, 1.433, 0.067, 0.437, 6.35, 0.96, -0.145, 0.952,     
                                       16.33, 3.09, 0.039, 2.35, 2.66, 22.51, 4.92};
            
            double[,] atmos = new double[2, 2] { {2.955, 12.8}, {0.081, 0.1} };

            double radinv = 0.017453293;

            double a;
            double ak3;
            double alam;
            double amu;
            double amu1;
            double amu2;
            double amu3;
            double amusq;
            double arg;
            double argo;
            double d;
            double d0;
            double denom;
            double earg1;
            double earg2;
            double earg3;
            double earg4;
            double earg5;
            double earg6;
            double earg7;
            double earg8;
            double earg9;
            double earg10;
            double earg11;
            double earg12;
            double exp12;
            double exp34;
            double f;
            double form;
            double g;
            double go;
            double h;
            double num;
            double n1;
            double n2;
            double n3;
            double pet2;
            double pet4;
            double phi;
            double rdfnom;
            double refl;
            double rnum;
            double s;
            double sc;
            double scal;
            double t1;
            double t2;
            double t3;
            double tau1;
            double tau2;
            double tau3;
            double tau4;
            double tau20;
            double tau40;
            double v;
            double w3;
            double x;
            double y;
            double za;
                        

            //                                                                       
            //       y is the elevation(in km) above sea level                       
            //                                                                       
            y = this.elevation;

            //-------                                                                
            //       iatmos = 1, terrestrial atmosphere, iatmos = 2, marine atmospher
            //-------                                                                
            //                                                                       
            if (_iatmos < 1 || _iatmos > 2) 
            {
                err = "Incorrect selection of atmosphere.";
                hh = 0.0;
                sn = 0.0;
                return;
            }
            p[7] = atmos[0, _iatmos-1]; 
            p[14] = atmos[1, _iatmos-1]; 
            p[15] = ozamt; 
            x = 90.0 - solat; 
            za = x * radinv; 
            amu = Math.Cos(za); 
            amusq = amu * amu; 
            alam = wavgth; 
            tau1 = 1.221 * Math.Pow((300.0 / alam), 4.27); 

            //                                                                       
            // +++++                                                                 
            //                                                                       
            earg1 = (alam - 300.0) / 7.294; 
            if (earg1 > 50.0) earg1 = 50.0;   // Bug fix: replaced earg1y by earg1.  Wilson Melendez
            if (earg1 < -50.0) earg1 = -50.0;

            //                                                                       
            // +++++                                                                 
            //                  
                                                     
            ak3 = 9.517 * 1.0445 / (0.0445 + Math.Exp(earg1));
         
            tau20 = p[14]; 
            tau40 = (0.034 / 0.204) * tau20; 
            w3 = p[15]; 
            tau3 = ak3 * w3;
 
            pet2 = 0.204;                                             
            pet4 = 0.034;
            tau2 = (tau20 / pet2) * (0.205 + (alam - 302.5) * 1.75e-4); 
            tau4 = (tau40 / pet4) * (0.034 + (302.5 - alam) * 5.0e-5); 

            //                                                                       
            // +++++                                                                 
            //                                                                       
            earg2 = y / p[25]; 
            if (earg2 > 50.0) earg2 = 50.0; 
            if (earg2 <  -50.0) earg2 = -50.0; 

            //                                                                       
            // +++++                                                                 
            //                                                                       
            n1 = (1 + p[24] ) / (p[24] + Math.Exp(earg2)); 
                                         
            //                                                                       
            // +++++                                                                 
            //                                                                       
            earg3 = y / p[28]; 
            if (earg3 > 50.0) earg3 = 50.0; 
            if (earg3 < -50.0) earg3 = -50.0; 
            earg4 = -p[29] / p[30]; 
            if (earg4 > 50.0) earg4 = 50.0; 
            if (earg4 < -50.0) earg4 = -50.0; 
            earg5 = (y - p[29] ) / p[30]; 
            if (earg5 > 50.0) earg5 = 50.0; 
            if (earg5 < -50.0) earg5 = -50.0; 

            //                                                                       
            // +++++                                                                 
            //                                                                       
            n2 = p[26] * (1 + p[27] ) / (p[27] + Math.Exp(earg3)) + (1 - p[26]) * (1 + Math.Exp(earg4) ) / (1 + Math.Exp(earg5));  
                 
            //                                                                       
            // +++++                                                                 
            //                                                                       
            earg6 = y / p[33]; 
            if (earg6 > 50.0) earg6 = 50.0; 
            if (earg6 < -50.0) earg6 = -50.0; 
            earg7 = -p[34] / p[35]; 
            if (earg7 > 50.0) earg7 = 50.0; 
            if (earg7 < -50.0) earg7 = -50.0; 
            earg8 = (y - p[34] ) / p[35]; 
            if (earg8 > 50.0) earg8 = 50.0; 
            if (earg8 < -50.0) earg8 = -50.0; 

            //                                                                       
            // +++++                                                                 
            //                                                                                    
            n3 = p[31] * (1 + p[32] ) / (p[32] + Math.Exp(earg6)) + (1 - p[31]) * (1 + Math.Exp(earg7)) / (1 + Math.Exp(earg8));                  
            f = 1.0 / ( 1.0 + p[4] * Math.Pow( (tau3 * n3 + tau4 * n2 + (p[22] + p[23] * y) * ak3), p[5] ) );                                             
            num = p[6] * tau1 * n1 + p[7] * Math.Pow((tau2 * n2), p[8]); 
            denom = 1.0 + p[9] * w3 * n3 * Math.Pow((tau3 * n3 + tau4 * n2), p[10]); 
            scal = num / denom; 
            v = p[3] / 1000.0; 
            phi = Math.Sqrt( (1.0 + v) / (amusq + v) ) - 1.0;
 
            //                                                                       
            // +++++                                                                 
            //                                                                       
            earg9 = -(p[0] * tau1 + p[1] * tau2) * phi; 
            if (earg9 > 50.0) earg9 = 50.0; 
            if (earg9 < -50.0) earg9 = -50.0;
 
            //                                                                       
            // +++++                                                                 
            //                                                                       
            exp12 = Math.Exp(earg9); 
                                    
            //                                                                       
            // +++++                                                                 
            //                                                                       
            earg10 = -p[2] * (tau3 + tau4) * phi; 
            if (earg10 > 50.0) earg10 = 50.0; 
            if (earg10 < -50.0) earg10 = -50.0;
 
            //                                                                       
            // +++++                                                                 
            //                                                                       
            exp34 = Math.Exp(earg10); 
                                       
            form = (f + (1.0 - f) * exp34) * exp12; 
            t1 = p[11] / 1000.0; 
            t2 = p[12] / 1000.0; 
            t3 = p[13] / 1000.0; 
            amu1 = Math.Sqrt( (amusq + t1) / (1.0 + t1) ); 
            amu2 = Math.Sqrt( (amusq + t2) / (1.0 + t2) ); 
            amu3 = Math.Sqrt( (amusq + t3) / (1.0 + t3) );
            arg = -(n1 * tau1) / amu1 - (n2 * tau2) / amu2 - (n3 * tau3) / amu3 - (n2 * tau4) / amu2;                                       
            argo = -(n1 * tau1) - (n2 * tau2) - (n3 * tau3) - (n2 * tau4); 

            //                                                                       
            // +++++                                                                 
            //                                                                       
            earg11 = (alam - 300.0) / 23.74; 
            if (earg11 > 50.0) earg11 = 50.0; 
            if (earg11 < -50.0) earg11 = -50.0; 
            earg12 = -0.6902 * Math.Exp(earg11); 
            if (earg12 > 50.0) earg12 = 50.0; 
            if (earg12 < -50.0) earg12 = -50.0; 

            //                                                                       
            // +++++                                                                 
            //                                                                                      
            h = 1.095 * (1.0 - Math.Exp(earg12)); 

            //                                                                       
            // +++++                                                                 
            //                                                                       
            if (arg > 50.0) arg = 50.0; 
            if (arg < -50.0) arg = -50.0;
 
            //                                                                       
            if (argo > 50.0) argo = 50.0; 
            if (argo < -50.0) argo = -50.0;
 
            //                                                                       
            // +++++                                                                 
            //                                                                       
            d = amu * h * Math.Exp(arg); 
            d0 = h * Math.Exp(argo); 
            s = form * scal * d0; 
            go = d + s; 
            rnum = p[16] * tau1 * n1 + p[17] * Math.Pow((tau2 * n2), p[18]); 

            //                                                                       
            //-------   changed tau4 to tau2 in the following line              
            //                                                                       
            rdfnom = 1.0 + p[19] * Math.Pow((tau3 * n3 + tau4 * n2), p[20]); 
            refl = rnum / rdfnom; 
            a = p[21]; 
            g = go / (1.0 - refl * a); 
            sc = s + (g - go); 

            //                                                                       
            //-------                                                                
            //       convert to watts/cm**2                                          
            //-------                                                                
            //                                                                       
            hh = sc / 1.0e4; 
            sn = d / 1.0e4;       

        }

        /// <summary>
        /// This method calls the Bener method and stores the output in a table.
        /// </summary>
        /// <returns></returns>
        public DataTable Compute()
        {
            

            double[] ozoneAmount = new double[6] {0.24, 0.28, 0.32, 0.36, 0.40, 0.44}; 
          
            double[] solarAltitude = new double[10] {5.0, 10.0, 20.0, 30.0, 40.0, 50.0, 60.0, 70.0, 80.0, 90.0};

            double[] waveLength = new double[15] {297.5, 300.0, 302.5, 305.0, 307.5, 310.0, 312.5, 315.0, 
                                                  317.5, 320.0, 325.0, 330.0, 340.0, 360.0, 380.0};

            double SN = 0.0;
            double H  = 0.0;
            string errorMsg = "";
            DataTable dt = new DataTable();
            DataRow dr1 = null;
            dt.Columns.Add("Ozone Amount");
            dt.Columns.Add("Solar Altitude");
            dt.Columns.Add("Wavelength");                       
            dt.Columns.Add("H");
            dt.Columns.Add("SN");

            // Three loops to generate calls to the BenerMethod (formerly known as the Blue subroutine)
            // k - points to the ozone amount
            // j - points to the solar altitude
            // i - points to the wavelengths           

            for (int k = 0; k < 6; k++)
            {
                for (int j = 0; j < 10; j++)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        BenerMethod(solarAltitude[j], waveLength[i], ozoneAmount[k], out H, out SN, out errorMsg);
                        dr1 = dt.NewRow();
                        dr1["Ozone Amount"] = ozoneAmount[k];
                        dr1["Solar Altitude"] = solarAltitude[j];
                        dr1["Wavelength"] = waveLength[i];                                               
                        dr1["H"] = H;
                        dr1["SN"] = SN;
                        dt.Rows.Add(dr1);
                    }
                }
            }

            return dt;

        }
    }
}
