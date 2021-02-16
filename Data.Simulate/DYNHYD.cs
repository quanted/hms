using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Simulate
{
    public class DYNHYD
    {
        bool validInput = true;
        bool qinit = true;
        int noseg = 0;          // number of segments;
        int[] IQOPT;
        double sumexp;

        bool[] kinFlow;
        bool[] weirFlow;
        bool[] dynFlow;

        int[] itype;

        int nChannels;
        int ch;

        double[] dVolume;
        double[] mVol;
        double[] bVol;

        double[] rDepth;

        double[] depthG;
        double[] veloCG;

        double[] volQ0;
        double[] widthQ0;
        double[] depthQ0;

        double[] depthWeir;
        double[] weirHeight;

        double[] qseg;
        double[] segWidth;
        double[] segLength;
        double[] segSlope;
        double[] segRough;
        int[] topSeg;

        double[] degWidth;

        double[] idynSeg;

        double[] bElev;
        double[] segBotElev;
        double[] surfElev;
        double[] surfElT0;
        bool[] boundElevSeg;
        int[] iElevFunc;
        double[] surfElevBoundT0;
        double[] surfElevBound;

        double[] dx;
        double[] vexp;
        double[] dxp;
        double[] bexp;

        double[] vmult;
        double[] dmult;
        double[] bmult;

        double[] qM;
        double[] alpha;
        double[] beta;

        int[] ninQ;
        bool[] juncNetwork;

        int[][] noQS;
        int[][] iqFlow;
        int[][] jqFlow;
        int[][] njunc;

        double[] chArea;
        double[] chLength;
        double[] chWidth;
        double[] velocT0;
        double[] velocT1;
        double[] qChan;

        double[][] qsum;
        double[][] qint;


        public DYNHYD()
        {


        }

        public void SetInputs()
        {
            // initialize inputs;
        }

        public void Initialize()
        {
            // QCALC_KW_Init.f95
            this.qinit = false;
            double sedTFactor = 1.0;
            int inTYP = 0;
            int nDynSeg = 0;
            double datumDiff = 0.0;
            double q0 = 0.0;
            double qAve;
            double q05;
            double axs;
            double area;
            double qMinDepth;
            double aveWidth;
            double nDQ;
            int nf;
            int nl;
            int ninQX;
            int noQ;
            bool addChannel;
            int ni;
            int nh;
            int nseg;
            bool dynFlowSeg = false;
            double v0;
            double y0;
            double b0;


            for (int iseg = 1; iseg < noseg; iseg++)
            {
                kinFlow[iseg] = false;
                addChannel = false;
                if (itype[iseg] < 3)
                {
                    switch (IQOPT[iseg])
                    {
                        case (1):
                            rDepth[iseg] = depthG[iseg] - depthQ0[iseg];
                            if(rDepth[iseg] <= 0.0)
                            {
                                rDepth[iseg] = depthQ0[iseg];
                            }
                            bexp[iseg] = 1.0 - vexp[iseg] - dxp[iseg];
                            qAve = rDepth[iseg] * segWidth[iseg] * veloCG[iseg];
                            q05 = 0.05 * qAve;
                            if (qAve >= 0.0)                                        // l:56
                            {
                                bmult[iseg] = segWidth[iseg] * Math.Pow(qAve, -1.0 * bexp[iseg]);
                                widthQ0[iseg] = bmult[iseg] * Math.Pow(q05, bexp[iseg]);
                                if (qseg[iseg] >= q05)
                                {
                                    segWidth[iseg] = bmult[iseg] * Math.Pow(qseg[iseg], bexp[iseg]);
                                }
                                else
                                {
                                    segWidth[iseg] = widthQ0[iseg];
                                }
                                axs = segWidth[iseg] * rDepth[iseg];
                                veloCG[iseg] = qseg[iseg] / axs;
                                if (qseg[iseg] >= q05)
                                {
                                    vmult[iseg] = veloCG[iseg] * Math.Pow(qseg[iseg], -1.0 * vexp[iseg]);
                                }
                                else
                                {
                                    vmult[iseg] = veloCG[iseg] * Math.Pow(q05, -1.0 * vexp[iseg]);
                                }
                            }
                            else if (qseg[iseg] > 0.0)                             // l:71
                            {
                                segWidth[iseg] = bVol[iseg] / (segLength[iseg] * depthG[iseg]);
                                bmult[iseg] = segWidth[iseg] * Math.Pow(iseg, -1.0 * bexp[iseg]);
                                if(q05 <= 0.0)
                                {
                                    q05 = 0.05 * qseg[iseg];
                                }
                                widthQ0[iseg] = bmult[iseg] * Math.Pow(q05, bexp[iseg]);
                                axs = segWidth[iseg] * rDepth[iseg];
                                veloCG[iseg] = qseg[iseg] / axs;
                                if (qseg[iseg] >= qseg[iseg])
                                {
                                    vmult[iseg] = veloCG[iseg] * Math.Pow(qseg[iseg], -1.0 * vexp[iseg]);
                                }
                                else
                                {
                                    vmult[iseg] = veloCG[iseg] * Math.Pow(q05, -1.0 * vexp[iseg]);
                                }
                            }
                            else 
                            {
                                segWidth[iseg] = bVol[iseg] / (segLength[iseg] * depthG[iseg]);
                                widthQ0[iseg] = segWidth[iseg];
                                depthQ0[iseg] = depthG[iseg];
                                rDepth[iseg] = 0.0;
                                bmult[iseg] = segWidth[iseg];
                                bexp[iseg] = 0.0;
                                vexp[iseg] = 1.0 - bexp[iseg] - dxp[iseg];
                                vmult[iseg] = 1.0 / (dmult[iseg] * bmult[iseg]);
                                dmult[iseg] = 0.0;
                                veloCG[iseg] = 0.0;
                            }
                            break;
                        case (2):                                                   // l:97
                            weirFlow[iseg] = true;
                            depthWeir[iseg] = weirHeight[iseg];
                            vmult[iseg] = 1.0 / (segWidth[iseg] * depthG[iseg]);
                            vexp[iseg] = 1.0;
                            dxp[iseg] = 0.0;
                            bmult[iseg] = segWidth[iseg];
                            bexp[iseg] = 0.0;
                            if (depthWeir[iseg] < 0.05)
                            {
                                depthWeir[iseg] = 0.05;
                            }
                            widthQ0[iseg] = segWidth[iseg];
                            veloCG[iseg] = vmult[iseg] * Math.Pow(qseg[iseg], vexp[iseg]);
                            if (depthG[iseg] > 0.05 * depthWeir[iseg])
                            {
                                depthG[iseg] = 1.05 * depthWeir[iseg];
                            }
                            if (depthG[iseg] > depthWeir[iseg])
                            {
                                area = depthG[iseg] * segWidth[iseg];
                                veloCG[iseg] = qseg[iseg] / area;                         
                            }
                            else
                            {
                                qseg[iseg] = 0.0;
                                veloCG[iseg] = 0.0;
                            }
                            rDepth[iseg] = depthG[iseg] - depthQ0[iseg];
                            break;
                        case (4):                                                   // l:136
                            kinFlow[iseg] = true;
                            if (segSlope[iseg] < 0.0000001)
                            {
                                segSlope[iseg] = 0.0000001;
                            }
                            veloCG[iseg] = vmult[iseg];
                            qM[iseg] = segWidth[iseg] * depthG[iseg] * veloCG[iseg];
                            if (dxp[iseg] > 0.01)
                            {
                                if (vexp[iseg] <= 0.0)
                                {
                                    vexp[iseg] = (2.0 / 3.0) * dxp[iseg];
                                }
                                bexp[iseg] = 1.0 - dxp[iseg] - vexp[iseg];
                                if (dx[iseg] > 0.6)
                                {
                                    validInput = false;
                                    return;
                                }
                                else if (bexp[iseg] < 0.0)
                                {
                                    if ( bexp[iseg] > -1.0e-6)
                                    {
                                        bexp[iseg] = 0.0;
                                    }
                                    else
                                    {
                                        sumexp = dxp[iseg] + vexp[iseg];
                                        validInput = false;
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                dxp[iseg] = 0.6;
                                vexp[iseg] = (2.0 / 3.0) * dxp[iseg];
                                bexp[iseg] = 0.0;
                            }
                            bmult[iseg] = segWidth[iseg] * Math.Pow(qM[iseg], -1.0 * bexp[iseg]);
                            beta[iseg] = 0.6 + 0.4 * bexp[iseg];
                            alpha[iseg] = Math.Pow(segRough[iseg] * Math.Pow(bmult[iseg], (2.0 / 3.0)) / Math.Pow(segSlope[iseg], 0.5), (3.0/5.0));
                            vmult[iseg] = 1.0 / alpha[iseg];
                            dmult[iseg] = alpha[iseg] / bmult[iseg];
                            
                            if (depthQ0[iseg] == 0.0)
                            {
                                depthQ0[iseg] = 0.001;
                            }
                            qMinDepth = Math.Pow(depthQ0[iseg] / dmult[iseg], 1.0 / dxp[iseg]);
                            widthQ0[iseg] = bmult[iseg] * Math.Pow(qMinDepth, bexp[iseg]);
                            if (widthQ0[iseg] <= 0.0)
                            {
                                widthQ0[iseg] = 0.05 * segWidth[iseg];
                            }
                            bVol[iseg] = segLength[iseg] * segWidth[iseg] * depthG[iseg];
                            aveWidth = segWidth[iseg];
                            rDepth[iseg] = dmult[iseg] * Math.Pow(qseg[iseg], dxp[iseg]);
                            depthG[iseg] = rDepth[iseg] + depthQ0[iseg];
                            degWidth[iseg] = bmult[iseg] * Math.Pow(qseg[iseg], bexp[iseg]);
                            break;
                        case (6):                                               // l:201
                            dynFlow[iseg] = true;
                            dynFlowSeg = true;
                            nDynSeg = nDynSeg + 1;
                            idynSeg[iseg] = nDynSeg;
                            bElev[iseg] = segBotElev[iseg];
                            depthWeir[iseg] = 0;
                            if ( depthQ0[iseg] <= 0.0)
                            {
                                depthQ0[iseg] = 0.001;
                            }
                            widthQ0[iseg] = segWidth[iseg];
                            rDepth[iseg] = depthG[iseg] - depthQ0[iseg];
                            bmult[iseg] = segWidth[iseg];
                            vmult[iseg] = 1.0 / (segWidth[iseg] * depthG[iseg]);
                            vexp[iseg] = 1.0;
                            dxp[iseg] = 0.0;
                            bexp[iseg] = 0.0;
                            nDQ = 0;
                            nf = 1;
                            ninQX = ninQ[nf];
                            for(ni = 1; ni < ninQX; ni++)
                            {
                                if (!juncNetwork[ni])
                                {
                                    noQ = noQS[nf][ni];
                                    int i;
                                    int j;
                                    for(int nq = 1; nq < noQ; nq++)
                                    {
                                        i = iqFlow[ni][nq];
                                        j = jqFlow[ni][nq];
                                        if (j == iseg)
                                        {
                                            addChannel = true;
                                            for(int ichn = 1; ichn < nChannels; ichn++)
                                            {
                                                nl = njunc[ichn][1];
                                                nh = njunc[ichn][2];
                                                if (i == nl && j == nh)
                                                {
                                                    addChannel = false;
                                                    break;
                                                }
                                                else if ( j == nl && i == nh){
                                                    addChannel = false;
                                                    break;
                                                }
                                            }
                                            if (addChannel)
                                            {
                                                nChannels = nChannels + 1;
                                                if (nChannels > ch)
                                                {
                                                    validInput = false;
                                                    return;
                                                }
                                                njunc[nChannels][1] = j;
                                                njunc[nChannels][2] = i;
                                                chArea[nChannels] = 0.0;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                    volQ0[iseg] = segLength[iseg] * widthQ0[iseg] * depthQ0[iseg];
                    bVol[iseg] = depthG[iseg] * segWidth[iseg] * segLength[iseg];
                    mVol[iseg] = bVol[iseg];
                    depthG[iseg] = rDepth[iseg] + depthQ0[iseg];
                    qsum[iseg][iseg] = qsum[iseg][0];
                    surfElev[iseg] = bElev[iseg] + depthG[iseg];
                    surfElT0[iseg] = surfElev[iseg];
                    if (boundElevSeg[iseg])
                    {
                        ni = iElevFunc[iseg];
                        surfElevBoundT0[iseg] = qint[1][ni];
                        surfElevBound[iseg] = qint[1][ni];
                        datumDiff = surfElevBoundT0[iseg] - surfElev[iseg];
                    }
                }
                else
                {
                    nseg = topSeg[iseg];
                    if (segSlope[iseg] == 0.0)
                    {
                        segSlope[iseg] = segSlope[nseg]; ;
                    }
                    if (segWidth[iseg] == 0.0)
                    {
                        segWidth[iseg] = segWidth[nseg];
                    }
                    if (segRough[iseg] == 0.0)
                    {
                        segRough[iseg] = 0.1;
                    }
                }
                dVolume[iseg] = mVol[iseg] - volQ0[iseg];
            }
            for (int iseg = 1; iseg < noseg; iseg++)
            {
                if(weirFlow[iseg] || dynFlow[iseg])
                {
                    depthG[iseg] = surfElev[iseg] - bElev[iseg];
                    surfElev[iseg] = surfElev[iseg] + datumDiff;
                    surfElT0[iseg] = surfElev[iseg];
                }
            }
            if (dynFlowSeg)
            {
                double R;
                double B;
                double vt0;
                double X;
                double Rnh;
                double Rnl;
                for(int ichn = 1; ichn < nChannels; ichn++)
                {
                    nl = njunc[ichn][1];
                    nh = njunc[ichn][2];
                    if (nh == 0.0)
                    {
                        R = depthG[nl];
                        B = segWidth[nl];
                        vt0 = veloCG[nl];
                        X = segLength[nl];
                    }
                    else
                    {
                        Rnh = depthG[nh];
                        Rnl = depthG[nl];
                        R = (Rnh + Rnl) / 2.0;
                        B = (segWidth[nh] + segWidth[nl]) / 2.0;
                        vt0 = (veloCG[nh] + veloCG[nl]) / 2.0;
                        X = (segLength[nh] + segLength[nl]) / 2.0;
                    }
                    if(chArea[ichn] <= 1.0)
                    {
                        chArea[ichn] = R * B;
                        chLength[ichn] = X;
                        chWidth[ichn] = B;
                    }
                    else
                    {
                        chWidth[ichn] = chArea[ichn] / R;
                    }
                    if (chLength[ichn] == 0.0)
                    {
                        chLength[ichn] = X;
                    }
                    velocT0[ichn] = vt0;
                    velocT1[ichn] = vt0;
                    qChan[ichn] = vt0 * chArea[ichn];
                }
            }
            nf = 1;
            ni = 1;
            noQ = noQS[nf][ni];
            q0 = 0.0;
            v0 = 0.0;
            for (int iseg = 1; iseg < noseg; iseg++)
            {
                if(itype[iseg] < 2)
                {
                    volQ0[iseg] = segLength[iseg] * widthQ0[iseg] * depthQ0[iseg];
                    y0 = depthQ0[iseg] + dmult[iseg] * Math.Pow(q0, dxp[iseg]);
                    b0 = widthQ0[iseg];
                }
            }

        }




        
        private double[] QChannel(double y_nh, double y_nl, double yt_nh, double yt_nl, double r_nh, double r_nl, double B, double clen, double cn, double v, double vt0, double dthyd, double dqdt, double q)
        {
            // QChannel.f95

            double r = (r_nh + r_nl) / 2.0;                             // l:33
            double area = B * r;                                        // l:34
            double dthq = (tsave - tstart) / 2.0;                       // l:36
            double g = 9.807;                                           // l:42 (m/s^2)

            // Friction coefficient
            double ak = g * Math.Pow(cn, 2);                            // l:46
            double kt = ak / Math.Pow(4, 3);                            // l:47

            double dyt = (y_nh - yt_nh + y_nl - yt_nl) / 2.0;           // l:62 (m)
            double dyx = (y_nh - y_nl);                                 // l:63 (m)
            double dr = (r_nh - r_nl);                                  // l:65 (m)

            // Derivatives
            double dydt = dyt / dthq;                                   // l:74 (m/sec)
            double dadt = dydt * B;                                     // l:75 (m^2/sec)
            double dydx = dyx / clen;                                   // l:76 (m/m)
            double drdx = dr / clen;                                    // l:77 (m/m)
            double dvdx = -1.0 * (dydt + vt0 + drdx) / r;               // l:78 (1/sec)
            double dvdt1 = v * dvdx;                                    // l:80 (m/sec^2)
            double dvdt2 = -1.0 * kt * v * Math.Abs(v);                 // l:81
            double dvdt3 = -1.0 * g * dydx;                             // l:82 (m/sec^2)
            double dvdt = dvdt1 + dvdt2 + dvdt3;                        // l:85 (m/sec^2)
            dqdt = (dvdt * area) + (v * dadt);                          // l:86 (m^3/sec^2)
            v = v + dvdt * dthyd;                                       // l:88 (m/sec)
            q = v * area;                                               // l:90 (m^3/sec)

            double[] derivatives = new double[]                     
            {
               dydt, dadt, dydx, drdx, dvdx, dvdt1, dvdt2, dvdt3, dvdt, dqdt, v, q
            };
            return derivatives;
        }
    
    
        private void CalculateQ()
        {
            // QCALC_KW.f95

            //double[] qIN0;
            //double[] qINdt;
            //double[] qINPore;
            //double[] qINAtm;
            //double[] surfElTl;
            //double[] cWaveTime;
            //double[] qSpecSeg;

            //double[] dSumQ;
            //double[] dQSeg;
            //double[] dQDeriv;
            //double[] d0Vol;
            //double[] qChanT0;
            //double[] dQSegT0;
            //double[] dQIN;
            //double[] xArea;
            //double[] dQSpecSeg;

            double da;
            double dv;
            double dhtStep;
            double Q;

            if (this.qinit)
            {
                this.Initialize();
                if (this.endPeriod)
                {
                    return;
                }
                this.tsave = this.time;
                this.maxNRiter = 20;
                this.qNRerr = 0.00001;
                this.startFactor = 0.2;
                this.startFactor2 = 0.05;
                this.iErrorCount = 0;
            }

            for (int iseg = 1; iseg < this.noseg; iseg++)
            {
                if(this.itype[iseg] <= 2)
                {
                    qIN0[iseg] = this.qsum[iseg][iseg];
                    this.qsum[iseg][0] = 0.0;
                    this.qINdt[iseg] = 0.0;
                    this.qINPore[iseg] = 0.0;
                    this.qINAtm[iseg] = 0.0;
                    this.qSpecSeg[iseg] = 0.0;
                    this.dqSpecSeg[iseg] = 0.0;
                    this.surfElTl[iseg] = this.surfElev[iseg];
                    this.qSegT0[iseg] = this.qseg[iseg];
                    this.dqSeg[iseg] = this.qseg[iseg];
                    this.dqSegT0[iseg] = this.qseg[iseg];
                    this.depthT0[iseg] = this.depthG[iseg];
                    double r = this.depthG[iseg] - this.depthQ0[iseg];
                    if(this.qseg[iseg] <= 0.0)
                    {
                        this.segWidth[iseg] = this.widthQ0[iseg];
                    }
                    else
                    {
                        this.segWidth[iseg] = this.bmult[iseg] * Math.Pow(this.qseg[iseg], this.bexp[iseg]);
                    }
                    this.xArea[iseg] = this.segWidth[iseg] * r;
                    this.d0Volume[iseg] = this.dVolume[iseg];
                    if (this.boundElevSeg[iseg])
                    {
                        int ni = this.iElevFunc[iseg];
                        this.surfElevBoundT0[iseg] = this.surfElevBound[iseg];
                        this.surfElevBound[iseg] = this.qint[1][ni];
                    }
                }
            }

            for (int ichn = 1; ichn < this.nChannels; ichn++)
            {
                this.qChanT0[ichn] = this.qChan[ichn];
                this.velocT0[ichn] = this.velocT1[ichn];
            }
            for (int j = 1; j < this.noseg; j++)
            {
                for (int i = 0; i < this.noseg; i++)
                {
                    this.qsum[i][j] = 0.0;
                }
            }

            int nf = 2;                             // l:148
            int ninqx = this.ninQ[nf];
            if (ninqx >= 0)
            {
                for (int ni = 1; ni < ninqx; ni++)
                {
                    int noQ = this.noQS[nf][ni];
                    for (int nq = 1; nq < noQ; nq++)
                    {
                        Q = this.bqPore[ni][nq] * this.qint[nf][ni];
                        int I = this.iqPore[ni][nq];
                        int J = this.jqPore[ni][nq];
                        if (this.itype[I] <= 2)
                        {
                            this.qINPore[I] = this.qINPore[I] + Q;
                        }
                    }
                }
            }

            nf = 6;                                 // l:165
            ninqx = this.ninQ[nf];
            if (ninqx >= 0)
            {
                for (int ni = 1; ni < ninqx; ni++)
                {
                    int noq = this.noQS[nf][ni];
                    for (int nq = 1; nq < ninqx; nq++)
                    {
                        Q = this.bqPore[ni][nq] * this.qint[nf][ni];
                        int I = this.iqPore[ni][nq];
                        int J = this.jqPore[ni][nq];
                        if (this.itype[I] <= 1)
                        {
                            this.qINAtm[I] = this.qINAtm[I] + Q;
                        }
                    }
                }
            }

            nf = 1;                                 // l:182
            ninqx = this.ninQ[nf];
            if (ninqx <= 0)
            {
                return;
            }
            double maxFlowRatio = 1.0;         
            for (int ni = 1; ni < ninqx; ni++)
            {
                int noq = this.noQS[nf][ni];
                if (this.internalFlowBoundary[ni])
                {
                    for (int nq = 1; nq < noq; nq++)
                    {
                        Q = this.bqFlow[ni][nq] * this.qint[nf][ni];
                        int I = this.iqFlow[ni][nq];
                        int J = this.jqFlow[ni][nq];

                        this.qSpecSeg[J] = Q / 86400.0;
                        this.dqSpecSeg[J] = this.qSpecSeg[J];
                    }
                }
                else
                {
                    for(int nq = 1; nq < noq; nq++)
                    {
                        Q = this.bqFlow[ni][nq] * this.qint[nf][ni];
                        int I = this.iqFlow[ni][nq];
                        int J = this.jqFlow[ni][nq];
                        if (J == 0)
                        {
                            this.qINdt[I] = this.qINdt[I] + Q;
                            if (this.qIN0[I] > 0.0)
                            {
                                if (this.boundElevSeg[I])
                                {
                                    this.flowRatio = 1.0;
                                }
                                else
                                {
                                    this.flowRatio = this.qINdt[I] / this.qIN0[I];
                                }
                            }
                            else
                            {
                                this.flowRatio = 2.0;
                            }
                            if (this.flowRatio > maxFlowRatio)
                            {
                                maxFlowRatio = this.flowRatio;
                            }
                        }
                    }
                }
            }
            for (int iseg = 1; iseg < this.noseg; iseg++)
            {
                double qINhdt = (this.qIN0[iseg] + this.qINdt[iseg]) / 2.0;
                this.qsum[iseg][0] = qINhdt + this.qINPore[iseg] + this.qINAtm[iseg];
                this.dqIN[iseg] = this.qsum[iseg][0] / 86400.0;
            }

            nf = 1;                                 // l:233
            int ni = this.nMain;
            int noq = this.noQS[nf][ni];


            // l:252

        }
    
    }

    
}
