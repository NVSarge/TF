﻿using System;
using System.Collections.Generic;
using System.Linq;
using NumSharp;


namespace TF
{
    public class KXMSolvers
    {
        public double[] Check(int nelx, int nely, double rmin, double[] x)
        {
            double[] dcn = new double[nelx * nely];
            for (int i = 0; i < nelx; i++)
            {
                for (int j = 0; j < nely; j++)
                {
                    double sum = 0.0;
                    for (int k = Math.Max(i - (int)Math.Floor(rmin), 0); k < Math.Min(i + (int)Math.Floor(rmin), nelx); k++)
                    {
                        for (int l = Math.Max(j - (int)Math.Floor(rmin), 0); l < Math.Min(j + (int)Math.Floor(rmin), nely); l++)
                        {
                            double fac = rmin - Math.Sqrt((i - k) * (i - k) + (j - l) * (j - l));
                            sum += Math.Max(0, fac);
                            dcn[j + i * nely] += Math.Max(0, fac) * x[l + k * nely];
                        }
                    }
                    dcn[j + i * nely] = dcn[j + i * nely] / (sum);
                }
            }
            return dcn;
        }
        public double[] Check(int nelx, int nely, double rmin, double[] x, double[] dc)
        {
            double[] dcn = new double[nelx * nely];
            for (int i = 0; i < nelx; i++)
            {
                for (int j = 0; j < nely; j++)
                {
                    double sum = 0.0;
                    for (int k = Math.Max(i - (int)Math.Ceiling(rmin), 0); k < Math.Min(i + (int)Math.Ceiling(rmin), nelx); k++)
                    {
                        for (int l = Math.Max(j - (int)Math.Ceiling(rmin), 0); l < Math.Min(j + (int)Math.Ceiling(rmin), nely); l++)
                        {
                            double fac = rmin - Math.Sqrt((i - k) * (i - k) + (j - l) * (j - l));
                            sum += Math.Max(0, fac);
                            dcn[j + i * nely] += Math.Max(0, fac) * x[l + k * nely] * dc[l + k * nely];
                        }
                    }
                    dcn[j + i * nely] = dcn[j + i * nely] / (x[j + i * nely] * sum);
                }
            }
            return dcn;
        }

        public double[] OC(int nelx, int nely, double[] x, double volfraq, double[] dc)//ballance x for volume constraint
        {
            double[] xnew = new double[nelx * nely];
            double l1 = 0;
            double l2 = 1e5;
            double move = 0.2;
            while (l2 - l1 > 1e-4)
            {
                double lmid = (l2 + l1) * 0.5;
                double summ = 0.0;
                for (int i = 0; i < dc.Length; i++)
                {
                    xnew[i] = Math.Max(0.001, Math.Max(x[i] - move, Math.Min(1.0, Math.Min(x[i] + move, x[i] * Math.Sqrt(-dc[i] / lmid)))));
                    summ += xnew[i];
                }
                if (summ - volfraq * (double)nelx * (double)nely > 0)
                {
                    l1 = lmid;
                }
                else
                {
                    l2 = lmid;
                }
            }

            return xnew;
        }

        public double[] Modal(int nelx,
                              int nely,
                              double[] x,
                              double p,
                              double q,
                              out double[,] V)
        {
            var ke = K_Plain_strain(1e10, 0.3);
            var me = M_Plain_strain(1000, 1);
            int N = 2 * (nelx + 1) * (nely + 1);
            double[][] K = new double[N][];
            double[][] M = new double[N][];
            for (int i = 0; i < N; i++)
            {
                K[i] = new double[N];
                M[i] = new double[N];
            }
            for (int elx = 0; elx < nelx; elx++)
            {
                for (int ely = 0; ely < nely; ely++)
                {
                    int n1 = (nely + 1) * elx + ely;
                    int n2 = (nely + 1) * (elx + 1) + ely;
                    var edof = np.array(2 * n1, 2 * n1 + 1, 2 * n2, 2 * n2 + 1, 2 * n2 + 2, 2 * n2 + 3, 2 * n1 + 2, 2 * n1 + 3);
                    for (int i = 0; i < edof.size; i++)
                    {

                        for (int j = 0; j < edof.size; j++)
                        {
                            K[edof[i]][edof[j]] = K[edof[i]][edof[j]] + Math.Pow(x[ely + elx * (nely)], p) * ke[i][j];
                            M[edof[i]][edof[j]] = M[edof[i]][edof[j]] + Math.Pow(x[ely + elx * (nely)], q) * me[i][j];
                        }
                    }
                }
            }

            int[] inds = new int[4 * (nely + 1)];
            //left and right border fix
            for (int i = 0; i < 2 * (nely + 1); i++)
            {
                inds[i] = i;
                inds[i + 2 * (nely + 1)] = N - 2 * (nely + 1) + i - 1;
            }


            double[,] Kmm = MatrixMath.To2D<double>(K);
            double[,] Mmm = MatrixMath.To2D<double>(M);


            Kmm = MatrixMath.CrossOut(Kmm, inds);
            Mmm = MatrixMath.CrossOut(Mmm, inds);


            int reducedN = Kmm.GetLength(0);
            double[] wr = new double[reducedN];
            //solve
            alglib.smatrixgevd(Kmm, reducedN, false, Mmm, false, 1, 1, out wr, out V);
            for (int i = 0; i < V.GetLength(0); i++)
            {
                for (int j = 0; j < V.GetLength(1); j++)
                {
                    V[i, j] = Math.Cos(wr[i]) * V[i, j];
                }
            }
            V = MatrixMath.FillOnWeird(V, inds);


            return wr;
        }

        public double[] Harmonic(int nelx, int nely, double[] x, double p, double q, double wmega)
        {
            var ke = K_Plain_strain(1e10, 0.3);
            var me = M_Plain_strain(1000, 1);
            int N = 2 * (nelx + 1) * (nely + 1);
            double[][] K = new double[N][];
            double[][] M = new double[N][];
            for (int i = 0; i < N; i++)
            {
                K[i] = new double[N];
                M[i] = new double[N];
            }
            for (int elx = 0; elx < nelx; elx++)
            {
                for (int ely = 0; ely < nely; ely++)
                {
                    int n1 = (nely + 1) * elx + ely;
                    int n2 = (nely + 1) * (elx + 1) + ely;
                    var edof = np.array(2 * n1, 2 * n1 + 1, 2 * n2, 2 * n2 + 1, 2 * n2 + 2, 2 * n2 + 3, 2 * n1 + 2, 2 * n1 + 3);
                    for (int i = 0; i < edof.size; i++)
                    {

                        for (int j = 0; j < edof.size; j++)
                        {
                            K[edof[i]][edof[j]] = K[edof[i]][edof[j]] + Math.Pow(x[ely + elx * (nely)], p) * ke[i][j];
                            M[edof[i]][edof[j]] = M[edof[i]][edof[j]] + Math.Pow(x[ely + elx * (nely)], q) * me[i][j];
                        }
                    }
                }
            }

            int[] inds = new int[2 * (nely + 1)];
            //left border fix
            for (int i = 0; i < 2 * (nely + 1); i++)
            {
                inds[i] = i;

            }


            double[,] Kmm = MatrixMath.To2D<double>(K);
            double[,] Mmm = MatrixMath.To2D<double>(M);


            Kmm = MatrixMath.CrossOut(Kmm, inds);
            Mmm = MatrixMath.CrossOut(Mmm, inds);

            List<double> X = new List<double>();
            int reducedN = Kmm.GetLength(0);
            double[,] DSW = new double[reducedN, reducedN];

            for (int ik = 0; ik < Kmm.GetLength(0); ik++)
            {
                for (int jk = 0; jk < Kmm.GetLength(1); jk++)
                {
                    double l = wmega * 2 * Math.PI;
                    l = l * l;
                    DSW[ik, jk] = Kmm[ik, jk] - l * Mmm[ik, jk];
                }
            }
            double[] F = new double[reducedN];
            double[] U = new double[reducedN];
            F[reducedN-1] = -1e6;
            alglib.rmatrixsolve(DSW, reducedN, F, out _, out _, out U);
            int c = 0;
            for (int i = 0; i < N; i++)
            {
                if (c < inds.Length && inds[c] == i)
                {
                    X.Add(0);
                    c++;
                }
                else
                {
                    X.Add(U[i - c]);
                }
            }
            return X.ToArray();
        }


        public double[] FE_inded(int nelx, int nely, double[] x,Dictionary<int,double> f_map,double p)
        {
            var ke = KE;
            int N = 2 * (nelx + 1) * (nely + 1);
            double[][] K_m = new double[N][];
            for (int i = 0; i < N; i++)
            {
                K_m[i] = new double[N];
            }
            for (int elx = 0; elx < nelx; elx++)
            {
                for (int ely = 0; ely < nely; ely++)
                {
                    int n1 = (nely + 1) * elx + ely;
                    int n2 = (nely + 1) * (elx + 1) + ely;
                    var edof = np.array(2 * n1, 2 * n1 + 1, 2 * n2, 2 * n2 + 1, 2 * n2 + 2, 2 * n2 + 3, 2 * n1 + 2, 2 * n1 + 3);
                    for (int i = 0; i < edof.size; i++)
                    {

                        for (int j = 0; j < edof.size; j++)
                        {
                            K_m[edof[i]][edof[j]] = K_m[edof[i]][edof[j]] + Math.Pow(x[ely + elx * (nely)], p) * ke[i][j]; //?diagonal                            
                        }
                    }
                }
            }


            int[] inds = new int[2 * (nely + 1)];
            //left and right border fix
            for (int i = 0; i < 2 * (nely + 1); i++)
            {
                inds[i] = i;
            }
            Random r = new Random(123);

            double[,] Kmm = MatrixMath.To2D<double>(K_m);
            Kmm = MatrixMath.CrossOut(Kmm, inds);
            int reducedN = Kmm.GetLength(0);
            List<double> F_full = Enumerable.Repeat(0.0,N).ToList();
            double[] X = new double[N];
            double[] F = new double[reducedN];
            //index crossout
            int c = 0;
            foreach (var q in f_map)
            {
                F_full[q.Key] =q.Value;
            }

            for (int i = 0; i < N; i++)
            {
                if (c < inds.Length && inds[c] == i)
                {                    
                    c++;
                }
                else
                {
                    F[i-c] = F_full[i];
                }
                
            }
           // F[reducedN -1] = -1e6;

            alglib.rmatrixsolvefast(Kmm, reducedN, ref F, out _);
            //index backfill
            c = 0;
            for (int i = 0; i < N; i++)
            {
                if (c < inds.Length && inds[c] == i)
                {
                    X[i] = 0;
                    c++;
                }
                else
                {
                    X[i] = F[i - c];
                }
            }
            return X;
        }

        public double[] FE(int nelx, int nely, double[] x, double p)
        {
            var ke = KE;
            int N = 2 * (nelx + 1) * (nely + 1);
            double[][] K_m = new double[N][];
            for (int i = 0; i < N; i++)
            {
                K_m[i] = new double[N];
            }
            for (int elx = 0; elx < nelx; elx++)
            {
                for (int ely = 0; ely < nely; ely++)
                {
                    int n1 = (nely + 1) * elx + ely;
                    int n2 = (nely + 1) * (elx + 1) + ely;
                    var edof = np.array(2 * n1, 2 * n1 + 1, 2 * n2, 2 * n2 + 1, 2 * n2 + 2, 2 * n2 + 3, 2 * n1 + 2, 2 * n1 + 3);
                    for (int i = 0; i < edof.size; i++)
                    {

                        for (int j = 0; j < edof.size; j++)
                        {
                            K_m[edof[i]][edof[j]] = K_m[edof[i]][edof[j]] + Math.Pow(x[ely + elx * (nely)], p) * ke[i][j]; //?diagonal                            
                        }
                    }
                }
            }


            int[] inds = new int[2 * (nely + 1)];
            //left and right border fix
            for (int i = 0; i < 2 * (nely + 1); i++)
            {
                inds[i] = i;
            }
            Random r = new Random(123);

            double[,] Kmm = MatrixMath.To2D<double>(K_m);
            Kmm = MatrixMath.CrossOut(Kmm, inds);
            int reducedN = Kmm.GetLength(0);
            double[] F = new double[reducedN];
            double[] X = new double[N];
            for (int i = 0; i < nely; i++)
            {
                F[(2 * nely + 1) * nelx + i * 2 - 1] = -1;
            }
            F[reducedN - nely - 1] = r.NextDouble();
            alglib.rmatrixsolvefast(Kmm, reducedN, ref F, out _);
            int c = 0;
            for (int i = 0; i < N; i++)
            {
                if (c < inds.Length && inds[c] == i)
                {
                    X[i] = 0;
                    c++;
                }
                else
                {
                    X[i] = F[i - c];
                }
            }
            return X;
        }
        public double[][] KE
        {
            get
            {
                double E = 1;
                double nu = 0.3;
                var k = new double[] { 1.0 / 2.0 - nu / 6.0, 1.0 / 8.0 + nu / 8.0, -1.0 / 4.0 - nu / 12.0, -1.0 / 8.0 + 3.0 * nu / 8.0, -1.0 / 4.0 + nu / 12.0, -1.0 / 8.0 - nu / 8.0, nu / 6.0, 1.0 / 8.0 - 3.0 * nu / 8.0 };
                var KE = new double[8][];
                KE[0] = new double[] { k[0], k[1], k[2], k[3], k[4], k[5], k[6], k[7] };
                KE[1] = new double[] { k[1], k[0], k[7], k[6], k[5], k[4], k[3], k[2] };
                KE[2] = new double[] { k[2], k[7], k[0], k[5], k[6], k[3], k[4], k[1] };
                KE[3] = new double[] { k[3], k[6], k[5], k[0], k[7], k[2], k[1], k[4] };
                KE[4] = new double[] { k[4], k[5], k[6], k[7], k[0], k[1], k[2], k[3] };
                KE[5] = new double[] { k[5], k[4], k[3], k[2], k[1], k[0], k[7], k[6] };
                KE[6] = new double[] { k[6], k[3], k[4], k[1], k[2], k[7], k[0], k[5] };
                KE[7] = new double[] { k[7], k[2], k[1], k[4], k[3], k[6], k[5], k[0] };/**/
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        KE[i][j] = E / (1.0 - nu * nu) * KE[i][j];
                    }
                }
                return (KE);
            }
        }

        public double[][] K_Plain_strain(double E, double nu)
        {

            var k = new double[] { 0.5 - 0.666666 * nu, 0.125, 0.166666 * nu - 0.25, 0.5 * nu - 0.125, nu / 3.0 - 0.25, -0.125, 0.166666 * nu, 0.125 - nu / 2.0 };
            var KE = new double[8][];
            KE[0] = new double[] { k[0], k[1], k[2], k[3], k[4], k[5], k[6], k[7] };
            KE[1] = new double[] { k[1], k[0], k[7], k[6], k[5], k[4], k[3], k[2] };
            KE[2] = new double[] { k[2], k[7], k[0], k[5], k[6], k[3], k[4], k[1] };
            KE[3] = new double[] { k[3], k[6], k[5], k[0], k[7], k[2], k[1], k[4] };
            KE[4] = new double[] { k[4], k[5], k[6], k[7], k[0], k[1], k[2], k[3] };
            KE[5] = new double[] { k[5], k[4], k[3], k[2], k[1], k[0], k[7], k[6] };
            KE[6] = new double[] { k[6], k[3], k[4], k[1], k[2], k[7], k[0], k[5] };
            KE[7] = new double[] { k[7], k[2], k[1], k[4], k[3], k[6], k[5], k[0] };/**/
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    KE[i][j] = E / ((1 + nu) * (1 - 2 * nu)) * KE[i][j];
                }
            }
            return (KE);
        }
        public double[][] M_Plain_strain(double ro, double s)
        {

            var m = new double[] { 1.0, 0, 0.5, 0, 0.25, 0, 0.5, 0 };
            var ME = new double[8][];
            ME[0] = new double[] { m[0], m[1], m[2], m[3], m[4], m[5], m[6], m[7] };
            ME[1] = new double[] { m[1], m[0], m[7], m[6], m[5], m[4], m[3], m[2] };
            ME[2] = new double[] { m[2], m[7], m[0], m[5], m[6], m[3], m[4], m[1] };
            ME[3] = new double[] { m[3], m[6], m[5], m[0], m[7], m[2], m[1], m[4] };
            ME[4] = new double[] { m[4], m[5], m[6], m[7], m[0], m[1], m[2], m[3] };
            ME[5] = new double[] { m[5], m[4], m[3], m[2], m[1], m[0], m[7], m[6] };
            ME[6] = new double[] { m[6], m[3], m[4], m[1], m[2], m[7], m[0], m[5] };
            ME[7] = new double[] { m[7], m[2], m[1], m[4], m[3], m[6], m[5], m[0] };
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    ME[i][j] = (ro * s / 9.0) * ME[i][j];
                }
            }
            return (ME);
        }
    };



}
