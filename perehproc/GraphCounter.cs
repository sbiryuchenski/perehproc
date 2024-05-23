using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace perehproc
{
    class GraphCounter
    {
        public GraphCounter() 
        {
        }
        #region vars

        int index_temp = 0;
 
        float   h11,
                h12,
                h13,
                h14,
                h21,
                h22,
                h23,
                h31,
                h32,
                b1,
                b2;

        float   h,
                K11,
                K12,
                K21,
                K22,
                K31,
                K32,
                K41,
                K42,
                x1_i_1,
                x1_i,
                x2_i_1,
                x2_i,
                x3_i_1,
                x3_i,
                x4_i_1,
                x4_i,
                a1,
                a2,
                a3,
                a4;

        float[] KP = new float[] { 0.0f, 120.0f, 839.0f, 6.0f, 12.0f };
        float tMKs = 0, Ktotal, T_Max = 3;

        #endregion


        private void CountKoefficients()
        {
            float Idv = 0.00058f,
               l = 0.07f,
               r = 0.01f,
               lamd = 0.09f,
               Rya = 33.0f,
               mr = 0.52f,
               Ke = (float)(1.0 / 1.56f),
               Km = 0.35f;


            h11 = (float)(mr * (l * l + lamd * lamd) + 2.0 * Idv);
            h12 = (float)(mr * l * r - 2.0 * Idv);
            h13 = (float)(mr * l * r - Idv);
            h14 = r * r * mr + Idv;
            h21 = (float)((2.0 * Km * Ke) / (Rya));
            h22 = (float)(mr * 9.8 * l);
            h23 = Km * Ke / (Rya);
            h31 = (float)(2.0 * Km / (Rya));
            h32 = Km / (Rya);

            a1 = (h12 * h23 + h14 * h21) / (h11 * h14 - h12 * h13);
            a2 = (h14 * h22) / (h11 * h14 - h12 * h13);
            a3 = (h11 * h23 + h13 * h21) / (h11 * h14 - h12 * h13);
            a4 = (h13 * h22) / (h11 * h14 - h12 * h13);

            b1 = (h12 * h32 + h14 * h31) / (h11 * h14 - h12 * h13);
            b2 = (h11 * h32 + h13 * h31) / (h11 * h14 - h12 * h13);
        }

        void calcShema(float u)
        {
            x1_i = x1_i_1;
            x2_i = x2_i_1;
            x3_i = x3_i_1;
            x4_i = x4_i_1;

            K11 = -a1 * x1_i + a2 * x2_i + a1 * x3_i - b1 * u;
            K21 = x1_i;
            K31 = a3 * x1_i - a4 * x2_i - a3 * x3_i + b2 * u;
            K41 = x3_i;

            K12 = -a1 * (x1_i + h * K11) + a2 * (x2_i + h * K21) + a1 * (x3_i + h * K31) - b1 * u;
            K22 = x1_i + h * K11;
            K32 = a3 * (x1_i + h * K11) - a4 * (x2_i + h * K21) - a3 * (x3_i + h * K31) + b2 * u;
            K42 = x3_i + h * K31;

            x1_i_1 = (float)(x1_i + 0.5 * h * (K11 + K12));
            x2_i_1 = (float)(x2_i + 0.5 * h * (K21 + K22));
            x3_i_1 = (float)(x3_i + 0.5 * h * (K31 + K32));
            x4_i_1 = (float)(x4_i + 0.5 * h * (K41 + K42));
        }

        public List<Point> mainmethod(float x1, float x2, float x3, float x4)
        {
            CountKoefficients();
            h = 0.001f;
            index_temp = 1;
            Ktotal = -(-KP[1] * x1_i_1 - KP[2] * x2_i_1 - KP[3] * x3_i_1 - KP[4] * x4_i_1);

            procCal_K();
            x1_i_1 = x1;
            x2_i_1 = x2;
            x3_i_1 = x3;
            x4_i_1 = x4;
            return graphProcess();
        }
        float theta()
        {
            float t = 0, T = 30.0f;
            float tetTemp = 0;

            x1_i_1 = 0;
            x2_i_1 = 0.1f;
            x3_i_1 = 0.0f;
            x4_i_1 = 0;

            for (; t <= T; t += h)
            {
                Ktotal = KP[1] * x1_i_1 + KP[2] * x2_i_1 + KP[3] * x3_i_1 + KP[4] * x4_i_1;
                calcShema(Ktotal);
                tetTemp += t * Math.Abs(x2_i_1);
            }

            return tetTemp;

        }

        void procCal_K()
        {
            float thetaMin, tTheta;
            int Qst = 25;
            float Kbuf, Ko = 0;
            int i = 0, j = 0;
            float step = 2;

            thetaMin = theta();
            for (i = 1; i < 5; i++)
            {

                Ko = KP[i];
                Kbuf = Ko;
                for (j = 1; j < Qst; j++)
                {
                    KP[i] = KP[i] * step;
                    tTheta = theta();
                    if (tTheta < thetaMin) Ko = KP[i];
                    else j = Qst;
                }
                KP[i] = Kbuf;
                for (j = 1; j < Qst; j++)
                {
                    KP[i] = KP[i] / step;
                    tTheta = theta();
                    if (tTheta < thetaMin) Ko = KP[i];
                    else j = Qst;
                }

                KP[i] = Ko;
                //println(KP[1] + "  " + KP[2] + "  " + KP[3] + "  " + KP[4]);
            }
        }

        private List<Point> graphProcess()
        {
            List<Point> list = new List<Point>();       
            tMKs = 0;
            int i = 0;
            for (float t = 0; t < 1; tMKs++)
            {
                if ((tMKs % 2) == 0) Ktotal = KP[1] * x1_i_1 + KP[2] * x2_i_1 + KP[3] * x3_i_1 + KP[4] * x4_i_1;
                calcShema(Ktotal);
                if ((tMKs % 4) == 0)
                {
                    list.Add(new Point(x2_i_1, t));
                    i++;
                }
                t += h;
            }
            return list;
        }

    }
}
