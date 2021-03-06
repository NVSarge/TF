﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static Tensorflow.Binding;
using NumSharp;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace TF
{
    public class GeneticsSearch
    {

        public static double[] convertWeaverToField(Genome<int> g, int WX, int WY, double basefill = 0.001)
        {

            return convertWeaverToField(g.Genom, WX, WY, basefill);
        }
        public static double[] convertWeaverToField(int[] g, int WX, int WY, double basefill = 0.001)
        {
            double[] retval = new double[WX * WY];

            int lastx = WX / 2;
            int lasty = WY / 2;
            int oldx = lastx;
            int oldy = lasty;
            for (int i = 0; i < WX * WY; i++)
            {
                retval[i] = basefill;
            }
            retval[lastx * WY + lasty] = 1.0;
            for (int i = 0; i < g.Length / 2; i++)
            {
                switch (g[i])
                {
                    case 0:
                        lasty--;
                        break;
                    case 1:
                        lastx++;
                        break;
                    case 2:
                        lasty++;
                        break;
                    case 3:
                        lastx--;
                        break;
                    case 4:
                        lasty--;
                        lastx++;
                        break;
                    case 5:
                        lastx++;
                        lasty++;
                        break;
                    case 6:
                        lasty++;
                        lastx--;
                        break;
                    case 7:
                        lastx--;
                        lasty--;
                        break;
                }
                if (lastx >= 0 && lastx < WX && lasty >= 0 && lasty < WY)
                {
                    if (g[i] > 3)
                    {
                        retval[oldx * WY + lasty] = Math.Max(0.5, retval[oldx * WY + lasty]);
                        retval[lastx * WY + oldy] = Math.Max(0.5, retval[lastx * WY + oldy]);
                    }
                    retval[lastx * WY + lasty] = 1.0;
                    oldx = lastx;
                    oldy = lasty;
                }
                else
                {
                    break;
                }
            }
            lastx = WX / 2;
            lasty = WY / 2;
            oldx = lastx;
            oldy = lasty;
            retval[lastx * WY + lasty] = 1.0;
            for (int i = g.Length / 2; i < g.Length; i++)
            {
                switch (g[i])
                {
                    case 0:
                        lasty--;
                        break;
                    case 1:
                        lastx++;
                        break;
                    case 2:
                        lasty++;
                        break;
                    case 3:
                        lastx--;
                        break;
                    case 4:
                        lasty--;
                        lastx++;
                        break;
                    case 5:
                        lastx++;
                        lasty++;
                        break;
                    case 6:
                        lasty++;
                        lastx--;
                        break;
                    case 7:
                        lastx--;
                        lasty--;
                        break;
                }
                if (lastx >= 0 && lastx < WX && lasty >= 0 && lasty < WY)
                {
                    if (g[i] > 3)
                    {
                        retval[oldx * WY + lasty] = Math.Max(0.5, retval[oldx * WY + lasty]);
                        retval[lastx * WY + oldy] = Math.Max(0.5, retval[lastx * WY + oldy]);
                    }
                    retval[lastx * WY + lasty] = 1.0;
                    oldx = lastx;
                    oldy = lasty;
                }
                else
                {
                    break;
                }
            }
            return retval;
        }


        public static double[] Harmonic_eval_simp(Genome<double> g, int WX, int WY, double volfraq, double f1, double f2, double f3)
        {
            double[] retval = new double[1];
            int penal = 3;
            int qenal = 1;
            KXMSolvers skx = new KXMSolvers();
            Dictionary<int, double> SelectedUs = new Dictionary<int, double>();
            List<double> AFC=new List<double>();
            for (double w = f1; w <= f2; w += f3)
            {
                var U = skx.Harmonic(WX, WY, g.Genom, penal, qenal, w);
                double multW = Math.Pow(2 * Math.PI * w, 2);
                double CornerDeform = Math.Sqrt(U[2 * (WY + 1)*(WX + 1) - 1]* U[2 * (WY + 1) * (WX + 1) - 1] + U[2 * (WY + 1) * (WX + 1) - 2]* U[2 * (WY + 1) * (WX + 1) - 2]);
                AFC.Add(CornerDeform);
                for (int elx = 0; elx <=WX; elx++)
                {
                    for (int ely = 0; ely <=WY; ely++)
                    {
                        if (ely == WY/2)
                        {
                            
                            double Mass = g.Genom[Math.Min(ely,WY-1) + Math.Min(elx,WX-1) * WY];
                            int ind = 2 * ((WY + 1) * elx + ely);
                            if (SelectedUs.ContainsKey(ind))
                            {
                                SelectedUs[ind] += multW * U[2 * ((WY + 1) * elx + ely)]* Mass;
                            }
                            else
                            {
                                SelectedUs.Add(ind, multW * U[2 * ((WY + 1) * elx + ely)] * Mass);
                            }

                            if (SelectedUs.ContainsKey(ind + 1))
                            {
                                SelectedUs[ind] += multW * U[2 * ((WY + 1) * elx + ely) + 1] * Mass;
                            }
                            else
                            {
                                SelectedUs.Add(ind + 1, multW * U[2 * ((WY + 1) * elx + ely) + 1] * Mass);
                            }



                        }
                    }
                }
            }
            foreach (var q in SelectedUs.Keys.ToList())
            {
                SelectedUs[q] = SelectedUs[q] / ((f2-f1)/f3);
            }/**/

            for (int inner = 0; inner <1; inner++)
            {
                var Ukx = skx.FE_inded(WX, WY, g.Genom, SelectedUs, penal);
                var ke = skx.KE;
                double c = 0.0;
                double[] dc = new double[WX * WY];
                for (int elx = 0; elx < WX; elx++)
                {
                    for (int ely = 0; ely < WY; ely++)
                    {
                        int n1 = (WY + 1) * elx + ely;
                        int n2 = (WY + 1) * (elx + 1) + ely;
                        var edof = np.array(2 * n1, 2 * n1 + 1, 2 * n2, 2 * n2 + 1, 2 * n2 + 2, 2 * n2 + 3, 2 * n1 + 2, 2 * n1 + 3);
                        double[] Ue = new double[8];
                        for (int i = 0; i < edof.size; i++)
                        {
                            Ue[i] = Ukx[edof[i]];
                        }
                        double xp = Math.Pow(g.Genom[ely + elx * WY], penal);
                        double[] KEUe = MatrixMath.MatVecProd(ke, Ue);
                        double UEKUE = 0;
                        for (int i = 0; i < KEUe.Length; i++)
                        {
                            UEKUE += Ue[i] * KEUe[i];
                        }
                        c += xp * UEKUE;
                        dc[ely + elx * WY] = -penal * Math.Pow(g.Genom[ely + elx * WY], penal - 1) * UEKUE;
                    }
                }

                dc = skx.Check(WX, WY, 1.5, g.Genom, dc);
                g.Genom = skx.OC(WX, WY, g.Genom, volfraq, dc);                
            }

            return AFC.ToArray();
        }

        public static double[] Test_eval_simp(Genome<double> g, int WX, int WY, double volfraq, double f1, double f2, double f3)
        {
            double[] retval = new double[1];
            int penal = 3;


            KXMSolvers skx = new KXMSolvers();
            var U = skx.FE(WX, WY, g.Genom, penal);
            var ke = skx.KE;
            double c = 0.0;
            double[] dc = new double[WX * WY];
            for (int elx = 0; elx < WX; elx++)
            {
                for (int ely = 0; ely < WY; ely++)
                {
                    int n1 = (WY + 1) * elx + ely;
                    int n2 = (WY + 1) * (elx + 1) + ely;
                    var edof = np.array(2 * n1, 2 * n1 + 1, 2 * n2, 2 * n2 + 1, 2 * n2 + 2, 2 * n2 + 3, 2 * n1 + 2, 2 * n1 + 3);
                    double[] Ue = new double[8];
                    for (int i = 0; i < edof.size; i++)
                    {
                        Ue[i] = U[edof[i]];
                    }
                    double xp = Math.Pow(g.Genom[ely + elx * WY], penal);
                    double[] KEUe = MatrixMath.MatVecProd(ke, Ue);
                    double UEKUE = 0;
                    for (int i = 0; i < KEUe.Length; i++)
                    {
                        UEKUE += Ue[i] * KEUe[i];
                    }
                    c += xp * UEKUE;
                    dc[ely + elx * WY] = -penal * Math.Pow(g.Genom[ely + elx * WY], penal - 1) * UEKUE;
                }
            }
            dc = skx.Check(WX, WY, 1.5, g.Genom, dc);
            g.Genom = skx.OC(WX, WY, g.Genom, volfraq, dc);
            double sumV = 0;
            foreach (var gg in g.Genom)
            {
                sumV += (double)gg;
            }
            sumV = sumV - WX * WY * volfraq;
            retval[0] = c;
            return retval;
        }
        public static double Test_eval(Genome<double> g, int WX, int WY, double volfraq)
        {
            double retval = 0.0f;
            int penal = 3;


            KXMSolvers skx = new KXMSolvers();
            var U = skx.FE(WX, WY, g.Genom, penal);

            var ke = skx.KE;
            double c = 0.0;

            for (int elx = 0; elx < WX; elx++)
            {
                for (int ely = 0; ely < WY; ely++)
                {
                    int n1 = (WY + 1) * elx + ely;
                    int n2 = (WY + 1) * (elx + 1) + ely;
                    var edof = np.array(2 * n1, 2 * n1 + 1, 2 * n2, 2 * n2 + 1, 2 * n2 + 2, 2 * n2 + 3, 2 * n1 + 2, 2 * n1 + 3);
                    double[] Ue = new double[8];
                    for (int i = 0; i < edof.size; i++)
                    {
                        Ue[i] = U[edof[i]];
                    }
                    double xp = Math.Pow(g.Genom[ely + elx * WY], penal);
                    double[] KEUe = MatrixMath.MatVecProd(ke, Ue);
                    double UEKUE = 0;
                    for (int i = 0; i < KEUe.Length; i++)
                    {
                        UEKUE += Ue[i] * KEUe[i];
                    }
                    c += xp * UEKUE;
                }
            }
            //  g.Genom = skx.Check(WX, WY, 1.5, g.Genom);
            double sumV = 0;
            foreach (var gg in g.Genom)
            {
                sumV += (double)gg;
            }
            sumV = sumV - WX * WY * volfraq;
            retval = c * c + sumV * sumV * 2.0;
            return retval;
        }
        /**/
        public static double Test_eval_weave(Genome<int> g, int WX, int WY, double volfraq)
        {
            double retval = 0;
            int penal = 3;

            var ge = convertWeaverToField(g, WX, WY);
            KXMSolvers skx = new KXMSolvers();
            var U = skx.FE(WX, WY, ge, penal);

            var ke = skx.KE;
            double c = 0.0;

            for (int elx = 0; elx < WX; elx++)
            {
                for (int ely = 0; ely < WY; ely++)
                {
                    int n1 = (WY + 1) * elx + ely;
                    int n2 = (WY + 1) * (elx + 1) + ely;
                    var edof = np.array(2 * n1, 2 * n1 + 1, 2 * n2, 2 * n2 + 1, 2 * n2 + 2, 2 * n2 + 3, 2 * n1 + 2, 2 * n1 + 3);
                    double[] Ue = new double[8];
                    for (int i = 0; i < edof.size; i++)
                    {
                        Ue[i] = U[edof[i]];
                    }
                    double xp = Math.Pow(ge[ely + elx * WY], penal);

                    double[] KEUe = MatrixMath.MatVecProd(ke, Ue);
                    double UEKUE = 0;
                    for (int i = 0; i < KEUe.Length; i++)
                    {
                        UEKUE += Ue[i] * KEUe[i];
                    }

                    c += xp * UEKUE;

                }
            }
            double sumV = ge.Sum();
            sumV = sumV - (double)(WX * WY) * volfraq;
            retval = c * c * 1.0 + sumV * sumV * 0.0;
            return retval;
        }


        public static double[] Test_eval_weave_Modal(Genome<int> g, int WX, int WY, double volfraq, double F1, double F2, double F3)
        {
            double[] retval = new double[1];
            int penal = 3;

            var ge = convertWeaverToField(g, WX, WY);
            KXMSolvers skx = new KXMSolvers();
            double[,] U;
            var lambdas = skx.Modal(WX, WY, ge, penal, penal, out U);
            List<double> womega = new List<double>();
            int iwReal = 0;
            for (int i = 0; i < lambdas.Length; i++)
            {
                double freq = Math.Sqrt(lambdas[i]) / (2 * Math.PI);
                womega.Add(freq);
                if (freq <= 1.0 || double.IsNaN(freq))
                {
                    iwReal++;
                }
            }
            if (iwReal >= womega.Count && womega.Count > 0)
            {
                iwReal = womega.Count - 1;
            }
            else if (womega.Count == 0)
            {
                womega.Add(10000);
                iwReal = 0;
            }
            if (womega.Count > iwReal + 3)
            {
                g.Addata = "";
                for (int i = iwReal; i < iwReal + 3; i++)
                {
                    g.Addata += "\t" + womega[i];
                }

                g.defs = U;
            }
            double sumV = ge.Sum();
            sumV = sumV - (double)(WX * WY) * volfraq;
            if (womega.Count < iwReal + 3)
            {
                retval[0] = sumV * sumV + Math.Pow(womega[iwReal] - 28.16, 2) * 1000.0;
            }
            else
            {
                double d1 = womega[iwReal] - F1;
                double d2 = womega[iwReal + 1] - F2;
                double d3 = womega[iwReal + 2] - F3;
                retval[0] = (d1 * d1 + d2 * d2 + d3 * d3) * 1000.0;
            }
            return retval;
        }

        public static void GoSimp(IProgress<Image> progress, int WX, int WY, double volfraq, int Pops, int PopSize)
        {

            Population<double> Search = new Population<double>(PopSize, WX * WY);
            Search.InitialFill(0.5);
            Search.Show(1);


            for (int i = 0; i < Pops; i++)
            {

                var G = Search.GetTop();
                var RT = Search.GetTopRating();
                string S = "pop: " + i + "[" + RT + "]";
                print(S);
                //refresh the picture box
                float MultySize = 20.0f;
                Image MyImage = new Bitmap((int)((WX + 3) * MultySize), (int)((WY + 3) * MultySize));
                Graphics g = Graphics.FromImage(MyImage);

                for (int ix = 0; ix < WX; ix++)
                {
                    for (int iy = 0; iy < WY; iy++)
                    {
                        double H = 1.0 - (float)G[ix * WY + iy];
                        H = Math.Max(0, Math.Min(1.0, H));
                        H = H * 255;
                        if (!double.IsNaN(H))
                        {
                            Color customColor = Color.FromArgb(255, (int)H, (int)H, (int)H);
                            SolidBrush shadowBrush = new SolidBrush(customColor);
                            g.FillRectangle(shadowBrush, ix * MultySize, iy * MultySize, MultySize, MultySize);
                        }

                    }
                }

                progress.Report(MyImage);
                g.Dispose();


                Search.Eval(Test_eval_simp, WX, WY, volfraq);
                //  Search.SortAndEliminate(1.0);
                //  Search.Mutate(2,0);

            }
            print("finished");
            //refresh the picture box
            Search.Show(1);

        }
        public static void GoHarmonic(IProgress<Image> progress, int WX, int WY, double volfraq, int Pops, int PopSize)
        {

            Population<double> Search = new Population<double>(PopSize, WX * WY);
            Search.InitialFill(0.5);


            var Gn = Search.GetTopGenom();

            for (int elx = 0; elx < WX; elx++)
            {
                for (int ely = 0; ely < WY; ely++)
                {
                    if (ely >= WY / 2)
                    {
                        Gn.Genom[ely + elx * WY] = 1.0;
                    }
                }
            }

            float MultySize = 10.0f;
            Image MyImage = new Bitmap(900, 900);
            Graphics g = Graphics.FromImage(MyImage);
            List<double> ZeroGraph = new List<double>();
            for (int i = 0; i < Pops; i++)
            {
                List<double> Graph = new List<double>();
                var G = Search.GetTop();
                var RT = Search.GetTopRating();
                string S = "pop: " + i + "[" + RT + "]";
                print(S);
                //refresh the picture box


                for (int ix = 0; ix < WX; ix++)
                {
                    for (int iy = 0; iy < WY; iy++)
                    {
                        double H = 1.0 - (float)G[ix * WY + iy];
                        H = Math.Max(0, Math.Min(1.0, H));
                        H = H * 255;
                        if (!double.IsNaN(H))
                        {
                            Color customColor = Color.FromArgb(255, (int)H, (int)H, (int)H);
                            SolidBrush shadowBrush = new SolidBrush(customColor);
                            g.FillRectangle(shadowBrush, ix * MultySize, iy * MultySize, MultySize, MultySize);
                        }

                    }
                }


                double w0 = 1;
                double w1 = 20;
                double dw = 0.2;
                Search.Eval(Harmonic_eval_simp, WX, WY, volfraq, w0, w1, dw);

                Graph = Search.GetTopRating().OfType<double>().ToList();
                if (i == 0)
                {
                    Graph.ForEach(x=> { ZeroGraph.Add(x); });
                }

                float dY = 600;
                float dx = 500.0f/(float)((w1 - w0) / dw);

                g.FillRectangle(new SolidBrush(Color.White), 0, dY - 150, 600, dY);
                g.DrawLine(new Pen(Color.Black, 2), 0, dY, 500, dY);
                for (int id = 0; id <50; id++)
                {
                    g.DrawLine(new Pen(Color.Gray, 1), id * 10, dY - 50, id * 10, dY + 50);
                    if (id % 10 == 0)
                    {
                        g.DrawLine(new Pen(Color.DarkGray, 2), id * 10, dY - 50, id * 10, dY + 50);
                    }
                }
                double data = ZeroGraph[0];
                double olddata = ZeroGraph[0];
                float MaxG = (float)(Graph.Max() + double.Epsilon);
                Debug.WriteLine(Graph.First());
                for (int gi = 0; gi < ZeroGraph.Count; gi++)
                {
                    if (!double.IsNaN(Graph[gi]))
                    {
                        data = ZeroGraph[gi] / MaxG;
                        g.DrawLine(new Pen(Color.DarkGray, 3), gi * dx, dY - 150.0f * (float)olddata, (gi + 1.0f) * dx, dY - 150.0f * (float)data);
                        olddata = data;
                    }
                }

                for (int gi = 0; gi < Graph.Count; gi++)
                {
                    if (!double.IsNaN(Graph[gi]))
                    {
                        data = Graph[gi] / MaxG;
                        g.DrawLine(new Pen(Color.Blue, 2), gi*dx, dY - 150.0f * (float)olddata,(gi + 1.0f)*dx, dY - 150.0f * (float)data);
                        olddata = data;
                    }
                }
                progress.Report(MyImage);
            }
            g.Dispose();
            print("finished");
            Search.Show(1);

        }
        /*
        public static void GoBreed(IProgress<Image> progress, int WX, int WY, double volfraq, int Pops, int PopSize)
        {
            Population Search = new Population(PopSize, WX * WY);
            Search.AddToGenePool(0.001);
            Search.AddToGenePool(0.999);

            Search.InitialFillGP();
            Search.Show(1);


            for (int i = 0; i < Pops; i++)
            {

                var G = Search.GetTop();
                var RT = Search.GetTopRating();
                string S = "pop: " + i + "[" + RT + "]";
                print(S);
                //refresh the picture box
                float MultySize = 20.0f;
                Image MyImage = new Bitmap((int)((WX * 4) * MultySize), (int)((WY * 4) * MultySize));
                Graphics g = Graphics.FromImage(MyImage);

                for (int ix = 0; ix < WX; ix++)
                {
                    for (int iy = 0; iy < WY; iy++)
                    {
                        double H = 1.0 - (float)G[ix * WY + iy];
                        H = Math.Max(0, Math.Min(1.0, H));
                        if (double.IsNaN(H))
                         {
                            H = 0;
                        }
                        
                        H = H * 255;
                        Color customColor = Color.FromArgb(255, (int)H, (int)H, (int)H);
                            SolidBrush shadowBrush = new SolidBrush(customColor);
                            g.FillRectangle(shadowBrush, ix * MultySize, iy * MultySize, MultySize, MultySize);
                        


                    }
                }
                var deltaY = WY * MultySize + 5;
                MultySize = 5.0f;
                for (int ig = 0; ig < 5; ig++)
                {
                    for (int jg = 0; jg < 3; jg++)
                    {
                        G = Search.GetTop(ig * 3 + jg);
                        for (int ix = 0; ix < WX; ix++)
                        {
                            for (int iy = 0; iy < WY; iy++)
                            {
                                double H = 1.0 - (float)G[ix * WY + iy];
                                H = Math.Max(0, Math.Min(1.0, H));
                                if (double.IsNaN(H))
                                {
                                    H = 0;
                                }
                                H = H * 255;
                                Color customColor = Color.FromArgb(255, (int)H, (int)H, (int)H);
                                SolidBrush shadowBrush = new SolidBrush(customColor);
                                g.FillRectangle(shadowBrush, ix * MultySize + ig * (WX + 1) * MultySize, iy * MultySize + deltaY + jg * (WY + 1) * MultySize, MultySize, MultySize);

                            }
                        }
                    }
                }

                progress.Report(MyImage);
                g.Dispose();
                Search.Eval(Test_eval, WX, WY, volfraq);
                Search.SortAndEliminate(0.5);
                Search.Mutate(4,4, true);

            }
            print("finished");
            //refresh the picture box
            Search.Show(1);

        }
    /**/
        /*  public static void GoWeave(IProgress<Image> progress, int WX, int WY, double volfraq, int Pops, int PopSize)
          {

              Population Search = new Population(PopSize, WX * WY / 2);

              Search.AddToGenePool(0);
              Search.AddToGenePool(1);
              Search.AddToGenePool(2);
              Search.AddToGenePool(3);
              Search.AddToGenePool(4);
              Search.AddToGenePool(5);
              Search.AddToGenePool(6);
              Search.AddToGenePool(7);
              Search.InitialFillGP();
              Search.Show(1);


              for (int i = 0; i < Pops; i++)
              {

                  var G = convertWeaverToField(Search.GetTop(), WX, WY);
                  var RT = Search.GetTopRating();
                  string S = "pop:  " + i + " / " + RT;
                  using (StreamWriter outputFile = new StreamWriter("output.txt", true))
                  {
                      outputFile.Write(S + "\n");
                  }
                  print(S);
                  //refresh the picture box
                  float MultySize = 20.0f;
                  Image MyImage = new Bitmap((int)((WX * 4) * MultySize), (int)((WY * 4) * MultySize));
                  Graphics g = Graphics.FromImage(MyImage);
                  SolidBrush Text = new SolidBrush(Color.Red);
                  for (int ix = 0; ix < WX; ix++)
                  {
                      for (int iy = 0; iy < WY; iy++)
                      {
                          double H = 1.0 - (float)G[ix * WY + iy];
                          H = Math.Max(0, Math.Min(1.0, H));
                          if (double.IsNaN(H))
                          {
                              H = 0;
                          }

                          H = H * 255;
                          Color customColor = Color.FromArgb(255, (int)H, (int)H, (int)H);
                          SolidBrush shadowBrush = new SolidBrush(customColor);
                          g.FillRectangle(shadowBrush, ix * MultySize, iy * MultySize, MultySize, MultySize);



                      }
                  }
                  Font drawFont = new Font("Arial", 8);
                  // g.DrawString("f:"+RT, drawFont, Text, 0.0f,20);

                  var deltaY = WY * MultySize + 5;
                  MultySize = 5.0f;
                  for (int ig = 0; ig < 5; ig++)
                  {
                      for (int jg = 0; jg < 4; jg++)
                      {
                          G = convertWeaverToField(Search.GetTop(ig * 3 + jg), WX, WY);
                          RT = Search.GetTopRating(ig * 3 + jg);
                          for (int ix = 0; ix < WX; ix++)
                          {
                              for (int iy = 0; iy < WY; iy++)
                              {
                                  double H = 1.0 - (float)G[ix * WY + iy];
                                  H = Math.Max(0, Math.Min(1.0, H));
                                  if (double.IsNaN(H))
                                  {
                                      H = 0;
                                  }
                                  H = H * 255;
                                  Color customColor = Color.FromArgb(255, (int)H, (int)H, (int)H);
                                  SolidBrush shadowBrush = new SolidBrush(customColor);
                                  g.FillRectangle(shadowBrush, ix * MultySize + ig * (WX + 1) * MultySize, iy * MultySize + deltaY + jg * (WY + 1) * MultySize, MultySize, MultySize);

                              }
                          }
                          //  g.DrawString("f:" + RT, drawFont, Text, ig * (WX + 1) * MultySize, deltaY + jg * (WY + 1) * MultySize);
                      }

                  }

                  progress.Report(MyImage);
                  g.Dispose();
                  Search.Eval(Test_eval_weave, WX, WY, volfraq);
                  Search.SortAndEliminate(0.2);
                  Search.Mutate(2, 10, true, WX, WY);

              }
              print("finished");
              //refresh the picture box
              Search.Show(1);
              var outModel = convertWeaverToField(Search.GetTop(), WX, WY);
              using (StreamWriter outputFile = new StreamWriter("final.txt"))
              {
                  outputFile.Write(outModel);
              }


          }/**/

        public static void GoWeave_Modal(IProgress<Image> progress, int WX, int WY, double volfraq, int Pops, int PopSize, IProgress<double> prog)
        {

            Population<int> Search = new Population<int>(PopSize, WX * WY);

            Search.AddToGenePool(0);
            Search.AddToGenePool(1);
            Search.AddToGenePool(2);
            Search.AddToGenePool(3);
            Search.AddToGenePool(4);
            Search.AddToGenePool(5);
            Search.AddToGenePool(6);
            Search.AddToGenePool(7);
            Search.InitialFillGP();
            Search.Show(1);

            using (StreamWriter outputFile = new StreamWriter("output.txt", false))
            {
                outputFile.Write("");
            }

            for (int i = 0; i < Pops; i++)
            {
                double pr = (double)i / (double)Pops;
                if (prog != null)
                    prog.Report(pr);

                var G = convertWeaverToField(Search.GetTop(), WX, WY);
                var RT = Search.GetTopRating();

                string S = "pop:\t" + i + "\t" + RT + "\t" + Search.GetTopData();
                using (StreamWriter outputFile = new StreamWriter("output.txt", true))
                {
                    outputFile.Write(S + "\n\r");
                }
                print(S);
                var defs = Search.GetTopDataU(0);
                //refresh the picture box
                float MultySize = 20.0f;
                Image MyImage = new Bitmap((int)((WX * 4) * MultySize), (int)((WY * 4) * MultySize));
                Graphics g = Graphics.FromImage(MyImage);
                SolidBrush Text = new SolidBrush(Color.Red);
                var probs = G;
                for (int ix = 0; ix < WX; ix++)
                {
                    for (int iy = 0; iy < WY; iy++)
                    {
                        double H = 1.0 - (float)G[ix * WY + iy];
                        H = Math.Max(0, Math.Min(1.0, H));
                        if (double.IsNaN(H))
                        {
                            H = 0;
                        }
                        probs[ix * WY + iy] = 1.0 - H;
                        H = H * 255;
                        Color customColor = Color.FromArgb(255, (int)H, (int)H, (int)H);
                        SolidBrush shadowBrush = new SolidBrush(customColor);
                        g.FillRectangle(shadowBrush, ix * MultySize, iy * MultySize, MultySize, MultySize);


                    }
                }
                if (defs != null)
                {


                    for (int Neig = 0; Neig < 3; Neig++)
                    {
                        double maxD = 0.0;
                        for (int id = 0; id < 2 * (WX + 1) * (WY + 1); id += 2)
                        {
                            maxD = Math.Max(maxD, Math.Sqrt(defs[Neig, id] * defs[Neig, id] + defs[Neig, id + 1] * defs[Neig, id + 1]));
                        }

                        for (int ix = 0; ix < WX; ix++)
                        {
                            for (int iy = 0; iy < WY; iy++)
                            {
                                int n1 = (WY + 1) * ix + iy;
                                int n2 = (WY + 1) * (ix + 1) + iy;
                                var edof = np.array(2 * n1, 2 * n2);
                                double TotalDisp = 0;
                                for (int ei = 0; ei < edof.size; ei++)
                                {
                                    double Hd = (float)defs[Neig, edof[ei]];
                                    double Wd = (float)defs[Neig, edof[ei] + 1];
                                    TotalDisp += Math.Sqrt(Hd * Hd + Wd * Wd);
                                }
                                var H = TotalDisp / maxD;
                                H = Math.Max(0, Math.Min(1.0, H));
                                if (double.IsNaN(H))
                                {
                                    H = 0;
                                }

                                H = H * 255;
                                Color customColor = Color.FromArgb(255, (int)H, 24, 255 - (int)H);
                                SolidBrush shadowBrush = new SolidBrush(customColor);
                                g.FillRectangle(shadowBrush, ix * MultySize + (WX * MultySize + 3) * (Neig + 1), iy * MultySize, MultySize, MultySize);


                            }
                        }
                    }
                }
                Font drawFont = new Font("Arial", 8);
                // g.DrawString("f:"+RT, drawFont, Text, 0.0f,20);

                var deltaY = WY * MultySize + 5;
                MultySize = 5.0f;
                for (int ig = 0; ig < 5; ig++)
                {
                    for (int jg = 0; jg < 4; jg++)
                    {
                        G = convertWeaverToField(Search.GetTop(ig * 3 + jg), WX, WY);
                        RT = Search.GetTopRating(ig * 3 + jg);
                        for (int ix = 0; ix < WX; ix++)
                        {
                            for (int iy = 0; iy < WY; iy++)
                            {
                                double H = 1.0 - (float)G[ix * WY + iy];
                                H = Math.Max(0, Math.Min(1.0, H));
                                if (double.IsNaN(H))
                                {
                                    H = 0;
                                }
                                H = H * 255;
                                Color customColor = Color.FromArgb(255, (int)H, (int)H, (int)H);
                                SolidBrush shadowBrush = new SolidBrush(customColor);
                                g.FillRectangle(shadowBrush, ix * MultySize + ig * (WX + 1) * MultySize, iy * MultySize + deltaY + jg * (WY + 1) * MultySize, MultySize, MultySize);

                            }
                        }
                        //  g.DrawString("f:" + RT, drawFont, Text, ig * (WX + 1) * MultySize, deltaY + jg * (WY + 1) * MultySize);
                    }

                }/**/

                progress.Report(MyImage);
                g.Dispose();
                Search.Eval(Test_eval_weave_Modal, WX, WY, volfraq, 50, 50.05, 75.382);
                Search.SortAndEliminate(0.2);
                Search.Mutate(2, 5, WX, WY);

            }
            print("finished");
            //refresh the picture box
            Search.Show(1);
            var outModel = convertWeaverToField(Search.GetTop(), WX, WY);
            using (StreamWriter outputFile = new StreamWriter("final.txt"))
            {
                outputFile.Write(outModel);
            }


        }

        /* private static void BeginInvoke(Action action)
         {
             throw new NotImplementedException();
         }/**/

        /* public static void GoBreed_Modal(IProgress<Image> progress, int WX, int WY, double volfraq, int Pops, int PopSize)
         {

             Population Search = new Population(PopSize, WX * WY);

             Search.AddToGenePool(0.001);
             Search.AddToGenePool(0.9);
             Search.AddToGenePool(1.0);
             Search.InitialFillGP();
             Search.Show(1);


             for (int i = 0; i < Pops; i++)
             {

                 var G = Search.GetTop();
                 var RT = Search.GetTopRating();
                 string S = "pop:  " + i + " / " + RT;
                 using (StreamWriter outputFile = new StreamWriter("output.txt", true))
                 {
                     outputFile.Write(S + "\n");
                 }
                 print(S);
                 //refresh the picture box
                 float MultySize = 20.0f;
                 Image MyImage = new Bitmap((int)((WX * 4) * MultySize), (int)((WY * 4) * MultySize));
                 Graphics g = Graphics.FromImage(MyImage);
                 SolidBrush Text = new SolidBrush(Color.Red);
                 for (int ix = 0; ix < WX; ix++)
                 {
                     for (int iy = 0; iy < WY; iy++)
                     {
                         double H = 1.0 - (float)G[ix * WY + iy];
                         H = Math.Max(0, Math.Min(1.0, H));
                         if (double.IsNaN(H))
                         {
                             H = 0;
                         }

                         H = H * 255;
                         Color customColor = Color.FromArgb(255, (int)H, (int)H, (int)H);
                         SolidBrush shadowBrush = new SolidBrush(customColor);
                         g.FillRectangle(shadowBrush, ix * MultySize, iy * MultySize, MultySize, MultySize);



                     }
                 }
                 Font drawFont = new Font("Arial", 8);
                 // g.DrawString("f:"+RT, drawFont, Text, 0.0f,20);

                 var deltaY = WY * MultySize + 5;
                 MultySize = 5.0f;
                 for (int ig = 0; ig < 5; ig++)
                 {
                     for (int jg = 0; jg < 4; jg++)
                     {
                         G = Search.GetTop(ig * 3 + jg);
                         RT = Search.GetTopRating(ig * 3 + jg);
                         for (int ix = 0; ix < WX; ix++)
                         {
                             for (int iy = 0; iy < WY; iy++)
                             {
                                 double H = 1.0 - (float)G[ix * WY + iy];
                                 H = Math.Max(0, Math.Min(1.0, H));
                                 if (double.IsNaN(H))
                                 {
                                     H = 0;
                                 }
                                 H = H * 255;
                                 Color customColor = Color.FromArgb(255, (int)H, (int)H, (int)H);
                                 SolidBrush shadowBrush = new SolidBrush(customColor);
                                 g.FillRectangle(shadowBrush, ix * MultySize + ig * (WX + 1) * MultySize, iy * MultySize + deltaY + jg * (WY + 1) * MultySize, MultySize, MultySize);

                             }
                         }
                         //  g.DrawString("f:" + RT, drawFont, Text, ig * (WX + 1) * MultySize, deltaY + jg * (WY + 1) * MultySize);
                     }

                 }

                 progress.Report(MyImage);
                 g.Dispose();
                 Search.Eval(Test_eval_Modal, WX, WY, volfraq);
                 Search.SortAndEliminate(0.2);
                 Search.Mutate(2, 10, true, WX, WY);

             }
             print("finished");
             //refresh the picture box
             Search.Show(1);
             var outModel = convertWeaverToField(Search.GetTop(), WX, WY);
             using (StreamWriter outputFile = new StreamWriter("final.txt"))
             {
                 outputFile.Write(outModel);
             }


         }/**/

    };



}
