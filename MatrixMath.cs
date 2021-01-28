using System;
using System.Linq;


namespace TF
{
    public class MatrixMath
    {


        public static double[][] MatInverse(double[][] m)
        {
            // assumes determinant is not 0
            // that is, the matrix does have an inverse
            int n = m.Length;
            double[][] result = MatCreate(n, n); // make a copy
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    result[i][j] = m[i][j];

            double[][] lum; // combined lower & upper
            int[] perm;  // out parameter
            MatDecompose(m, out lum, out perm);  // ignore return

            double[] b = new double[n];
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                    if (i == perm[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;

                double[] x = Reduce(lum, b); // 
                for (int j = 0; j < n; ++j)
                    result[j][i] = x[j];
            }
            return result;
        }

        public static int MatDecompose(double[][] m, out double[][] lum, out int[] perm)
        {
            // Crout's LU decomposition for matrix determinant and inverse
            // stores combined lower & upper in lum[][]
            // stores row permuations into perm[]
            // returns +1 or -1 according to even or odd number of row permutations
            // lower gets dummy 1.0s on diagonal (0.0s above)
            // upper gets lum values on diagonal (0.0s below)

            int toggle = +1; // even (+1) or odd (-1) row permutatuions
            int n = m.Length;

            // make a copy of m[][] into result lu[][]
            lum = MatCreate(n, n);
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    lum[i][j] = m[i][j];

            // make perm[]
            perm = new int[n];
            for (int i = 0; i < n; ++i)
                perm[i] = i;

            for (int j = 0; j < n - 1; ++j) // process by column. note n-1 
            {
                double max = Math.Abs(lum[j][j]);
                int piv = j;

                for (int i = j + 1; i < n; ++i) // find pivot index
                {
                    double xij = Math.Abs(lum[i][j]);
                    if (xij > max)
                    {
                        max = xij;
                        piv = i;
                    }
                } // i

                if (piv != j)
                {
                    double[] tmp = lum[piv]; // swap rows j, piv
                    lum[piv] = lum[j];
                    lum[j] = tmp;

                    int t = perm[piv]; // swap perm elements
                    perm[piv] = perm[j];
                    perm[j] = t;

                    toggle = -toggle;
                }

                double xjj = lum[j][j];
                if (xjj != 0.0)
                {
                    for (int i = j + 1; i < n; ++i)
                    {
                        double xij = lum[i][j] / xjj;
                        lum[i][j] = xij;
                        for (int k = j + 1; k < n; ++k)
                            lum[i][k] -= xij * lum[j][k];
                    }
                }

            } // j

            return toggle;  // for determinant
        } // MatDecompose

        public static double[] Reduce(double[][] luMatrix, double[] b) // helper
        {
            int n = luMatrix.Length;
            double[] x = new double[n];
            //b.CopyTo(x, 0);
            for (int i = 0; i < n; ++i)
                x[i] = b[i];

            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1][n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum / luMatrix[i][i];
            }

            return x;
        } // Reduce

        public static double MatDeterminant(double[][] m)
        {
            double[][] lum;
            int[] perm;
            double result = MatDecompose(m, out lum, out perm);  // impl. cast
            for (int i = 0; i < lum.Length; ++i)
                result *= lum[i][i];
            return result;
        }

        public static double[][] MatCreate(int rows, int cols)
        {
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new double[cols];
            return result;
        }
        public static double[][] MatAlgebra(double[][] matA,
          double[][] matB, double sign)
        {
            int aRows = matA.Length;
            int aCols = matA[0].Length;
            int bRows = matB.Length;
            int bCols = matB[0].Length;
            if (aCols != bCols && aRows != bRows)
                throw new Exception("Non-conformable matrices");

            double[][] result = MatCreate(aRows, aCols);

            for (int i = 0; i < aRows; ++i) // each row of A
                for (int j = 0; j < aCols; ++j) // each col of B                    
                    result[i][j] = matA[i][j] + matB[i][j] * sign;

            return result;
        }

        public static double[] MatAlgebra(double[] matA,
          double[] matB, double sign)
        {
            int aRows = matA.Length;

            int bRows = matB.Length;

            if (aRows != bRows)
                throw new Exception("Non-conformable matrices");

            double[] result = new double[aRows];

            for (int i = 0; i < aRows; ++i) // each row of A                
                result[i] = matA[i] + matB[i] * sign;

            return result;
        }
        public static double[][] MatProduct(double[][] matA,
          double[][] matB)
        {
            int aRows = matA.Length;
            int aCols = matA[0].Length;
            int bRows = matB.Length;
            int bCols = matB[0].Length;
            if (aCols != bRows)
                throw new Exception("Non-conformable matrices");

            double[][] result = MatCreate(aRows, bCols);

            for (int i = 0; i < aRows; ++i) // each row of A
                for (int j = 0; j < bCols; ++j) // each col of B
                    for (int k = 0; k < aCols; ++k) // could use bRows
                        result[i][j] += matA[i][k] * matB[k][j];

            return result;
        }

        public static double[] MatVecProd(double[][] m, double[] v)
        {
            int nRows = m.Length;
            int nCols = m[0].Length;
            int n = v.Length;
            if (nCols != n)
                throw new Exception("non-comform in MatVecProd");

            double[] result = new double[n];

            for (int i = 0; i < nRows; ++i)
            {
                for (int j = 0; j < nCols; ++j)
                {
                    result[i] += m[i][j] * v[j];
                }
            }
            return result;
        }

        public static double[][] ExtractLower(double[][] lum)
        {
            // lower part of an LU Crout's decomposition
            // (dummy 1.0s on diagonal, 0.0s above)
            int n = lum.Length;
            double[][] result = MatCreate(n, n);
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (i == j)
                        result[i][j] = 1.0;
                    else if (i > j)
                        result[i][j] = lum[i][j];
                }
            }
            return result;
        }

        public static double[][] ExtractUpper(double[][] lum)
        {
            // upper part of an LU (lu values on diagional and above, 0.0s below)
            int n = lum.Length;
            double[][] result = MatCreate(n, n);
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (i <= j)
                        result[i][j] = lum[i][j];
                }
            }
            return result;
        }

        public static double[][] MatReconstruct(double[][] lower, double[][] upper, int[] perm)
        {
            double[][] tmp = MatProduct(lower, upper);  // scrambled rows
            double[][] result = MatCreate(lower.Length, lower[0].Length);
            // suppose perm = [1, 3, 0, 2]
            // row 0 of tmp goes to row 1 of result,
            // row 1 of tmp goes to row 3 of result
            // row 2 of tmp goes to row 0 of result
            // etc.
            for (int i = 0; i < lower.Length; ++i)
            {
                int r = perm[i];
                for (int j = 0; j < lower[0].Length; ++j)
                    result[r][j] = tmp[i][j];

            }
            return result;
        }

        public static void MatShow(double[][] m, int dec, int wid)
        {
            for (int i = 0; i < m.Length; ++i)
            {
                for (int j = 0; j < m[0].Length; ++j)
                {
                    double v = m[i][j];
                    if (Math.Abs(v) < 1.0e-5) v = 0.0;  // avoid "-0.00"
                    Console.Write(v.ToString("F" + dec).PadLeft(wid));
                }
                Console.WriteLine("");
            }
        }

        public static void VecShow(int[] vec, int wid)
        {
            for (int i = 0; i < vec.Length; ++i)
                Console.Write(vec[i].ToString().PadLeft(wid));
            Console.WriteLine("");
        }

        public static void VecShow(double[] vec, int dec, int wid)
        {
            for (int i = 0; i < vec.Length; ++i)
            {
                double x = vec[i];
                if (Math.Abs(x) < 1.0e-5) x = 0.0;  // avoid "-0.00"
                Console.Write(x.ToString("F" + dec).PadLeft(wid));
            }
            Console.WriteLine("");
        }

        public static double[][] MatTranspose(double[][] m)
        {
            int nr = m.Length;
            int nc = m[0].Length;
            double[][] result = MatCreate(nc, nr);  // note
            for (int i = 0; i < nr; ++i)
                for (int j = 0; j < nc; ++j)
                    result[j][i] = m[i][j];
            return result;
        }

        public class SolveLU
        {
            private int maxOrder = 4;
            private double[,] m;
            private double[,] l;
            private double[,] u;
            private double[] y;

            public void SetOrder(int O)
            {
                maxOrder = O;
            }
            public SolveLU(int size, double[,] matrix, double[] soultion)
            {
                maxOrder = size;
                m = matrix;
                y = soultion;
                u = new double[maxOrder, maxOrder]; // upper diagonal Matrix
                l = new double[maxOrder, maxOrder]; // lower diagonal Matrix
            }

            private void SwitchRows(int n)
            {
                double tempD;
                int i, j;
                for (i = n; i <= maxOrder - 2; i++)
                {
                    for (j = 0; j <= maxOrder - 1; j++)
                    {
                        tempD = m[i, j];
                        m[i, j] = m[i + 1, j];
                        m[i + 1, j] = tempD;
                    }
                    tempD = y[i];
                    y[i] = y[i + 1];
                    y[i + 1] = tempD;
                }
            }

            public bool Cholesky_decomp()
            {
                int i, j, k;
                bool result = true;

                for (i = 0; i < maxOrder; i++)
                {
                    for (j = 0; j <= i; j++)
                    {
                        if (i > j)
                        {
                            l[i, j] = m[i, j];
                            for (k = 0; k < j; k++)
                                l[i, j] = l[i, j] - l[i, k] * l[j, k];

                            if (Math.Abs(l[j, j]) > 1E-200)
                                l[i, j] = l[i, j] / l[j, j];
                            else
                                result = false;
                        }
                        if (i == j)
                        {
                            l[i, i] = m[i, i];
                            for (k = 0; k < j; k++)
                            {
                                l[i, i] = l[i, i] - (l[i, k] * l[i, k]);
                            }
                            if (l[i, i] >= 0)
                                l[i, i] = Math.Sqrt(l[i, i]);
                            else
                                result = false;
                        }
                    }
                }
                return result;
            }

            public bool LUdecomp()
            {
                int i, j, k;
                bool result = true;
                for (i = 0; i < maxOrder; i++)
                    l[i, i] = 1.0;

                for (i = 0; i < maxOrder; i++)
                {
                    if (Math.Abs(m[i, i]) < 1E-10)
                        SwitchRows(i);
                }

                for (j = 0; j < maxOrder; j++)
                {
                    for (i = 0; i < maxOrder; i++)
                    {
                        if (i >= j)
                        {
                            u[j, i] = m[j, i];
                            for (k = 0; k < j; k++)
                                u[j, i] = u[j, i] - u[k, i] * l[j, k];
                        }
                        if (i > j)
                        {
                            if (Math.Abs(u[j, j]) > 1E-20)
                            {
                                l[i, j] = m[i, j];
                                for (k = 0; k < j; k++)
                                    l[i, j] = l[i, j] - u[k, j] * l[i, k];
                                l[i, j] = l[i, j] / u[j, j];
                            }
                            else
                                result = false;
                        }
                    }
                }
                return result;
            }

            public bool SolveCholesky(out double[] x)
            {
                bool result = true;
                int i, j;
                x = new double[maxOrder];
                double[] t = new double[maxOrder];
                for (j = 0; j < maxOrder; j++)
                {
                    t[j] = y[j];
                    for (i = 0; i < j; i++)
                    {
                        t[j] = t[j] - t[i] * l[j, i];
                    }
                    if (Math.Abs(l[j, j]) > 1E-200)
                        t[j] = t[j] / l[j, j];
                    else
                        result = false;
                }
                x[maxOrder - 1] = t[maxOrder - 1] / l[maxOrder - 1, maxOrder - 1];

                for (j = maxOrder - 2; j >= 0; j--)
                {
                    x[j] = t[j];
                    for (i = maxOrder - 1; i > j; i--)
                    {
                        x[j] = x[j] - x[i] * l[i, j];
                    }
                    if (Math.Abs(l[j, j]) > 1E-200)
                        x[j] = x[j] / l[j, j];
                    else
                        result = false;
                }
                return result;
            }

            public double[] SolveCrout()
            {
                int i, j;
                double[] x = new double[maxOrder];
                for (j = 1; j < maxOrder; j++)
                {
                    for (i = 0; i < j; i++)
                    {
                        y[j] = y[j] - y[i] * l[j, i];
                    }
                }
                x[maxOrder - 1] = y[maxOrder - 1] / u[maxOrder - 1, maxOrder - 1];
                for (j = maxOrder - 2; j >= 0; j--)
                {
                    x[j] = y[j];
                    for (i = maxOrder - 1; i > j; i--)
                    {
                        x[j] = x[j] - x[i] * u[j, i];
                    }
                    x[j] = x[j] / u[j, j];
                }
                return x;
            }
            public double[] SolveUsingLU(double[][] M, double[] y, int n)
            {
                double[,] matrix = To2D<double>(M);
                // decomposition of matrix
                double[,] lu = new double[n, n];
                double sum = 0;
                for (int i = 0; i < n; i++)
                {
                    for (int j = i; j < n; j++)
                    {
                        sum = 0;
                        for (int k = 0; k < i; k++)
                            sum += lu[i, k] * lu[k, j];
                        lu[i, j] = matrix[i, j] - sum;
                    }
                    for (int j = i + 1; j < n; j++)
                    {
                        sum = 0;
                        for (int k = 0; k < i; k++)
                            sum += lu[j, k] * lu[k, i];
                        lu[j, i] = (1 / lu[i, i]) * (matrix[j, i] - sum);
                    }
                }

                // lu = L+U-I
                // find solution of Ly = b
                double[] t = new double[n];
                for (int i = 0; i < n; i++)
                {
                    sum = 0;
                    for (int k = 0; k < i; k++)
                        sum += lu[i, k] * t[k];
                    t[i] = y[i] - sum;
                }
                // find solution of Ux = y
                double[] x = new double[n];
                for (int i = n - 1; i >= 0; i--)
                {
                    sum = 0;
                    for (int k = i + 1; k < n; k++)
                        sum += lu[i, k] * x[k];
                    x[i] = (1 / lu[i, i]) * (t[i] - sum);
                }
                return x;
            }
            public static T[,] SubArray<T>(T[,] values, int row_min, int row_max, int col_min, int col_max)
            {
                // Allocate the result array.
                int num_rows = row_max - row_min + 1;
                int num_cols = col_max - col_min + 1;
                T[,] result = new T[num_rows, num_cols];

                // Get the number of columns in the values array.
                int total_cols = values.GetUpperBound(1) + 1;
                int from_index = row_min * total_cols + col_min;
                int to_index = 0;
                for (int row = 0; row <= num_rows - 1; row++)
                {
                    Array.Copy(values, from_index, result, to_index, num_cols);
                    from_index += total_cols;
                    to_index += num_cols;
                }

                return result;
            }
            public static T[,] To2D<T>(T[][] source, int startRow = 0, int startCol = 0, int endRow = 0, int endCol = 0)
            {
                try
                {
                    var FirstDim = source.Length - endRow - startRow;
                    var SecondDim =
                        source.GroupBy(row => row.Length).Single()
                            .Key - endCol - startCol; // throws InvalidOperationException if source is not rectangular

                    var result = new T[FirstDim, SecondDim];
                    for (var i = startRow; i < FirstDim; ++i)
                        for (var j = startCol; j < SecondDim; ++j)
                            result[i - startRow, j - startCol] = source[i][j];

                    return result;
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("The given jagged array is not rectangular.");
                }
            }
        }

    };



}
