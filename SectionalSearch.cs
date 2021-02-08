using NumSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF
{
    public class SectionalSearch
    {


        public SectionalSearch(int WX, int WY)
        {

        }

        private static double[,] convertTree2Mesh(QuadTree<double> QT, int WX, int WY)
        {
            double[,] retval = new double[WX, WY];
            var L = QT.getLeafs();
            foreach (var q in L)
            {
                int x = (int)q.BB.X;
                int y = (int)q.BB.Y;
                int w = (int)q.BB.Width;
                int h = (int)q.BB.Height;
                for (int ix = 0; ix < w; ix++)
                {
                    for (int iy = 0; iy < h; iy++)
                    {
                        retval[ix + x, iy + y] = q.NodeValue;
                    }
                }
            }

            return retval;
        }

        public static double[,] GoSearch(IProgress<Image> progress, int WX, int WY, double volfraq, IProgress<double> prog)
        {
            Random r = new Random();
            double[,] retval = new double[WX, WY];
            QuadTree<double> SearchTree = new QuadTree<double>();
            SearchTree.Root.BB = new System.Windows.Rect(0, 0, WX, WY);
            SearchTree.Root.NodeValue = 1.0;
            for (int h = 0; h < 10000; h++)
            {
                var SearchRootList = SearchTree.getLeafs();
                double step = 0.3*r.NextDouble()-0.1;                
                List<double> Gradient = new List<double>();                
                retval = convertTree2Mesh(SearchTree, WX, WY);
                int penal = 3;
                double Fzero = CalcTreeKX(WX, WY, volfraq, retval, penal);

                foreach (var q in SearchRootList)
                {
                    var oldVal = q.NodeValue;
                    q.NodeValue = Math.Min(1.0, Math.Max(0.001, q.NodeValue + step));
                    retval = convertTree2Mesh(SearchTree, WX, WY);
                   
                    double F = CalcTreeKX(WX, WY, volfraq, retval, penal);
                    Gradient.Add((F- Fzero)/step);
                    q.NodeValue = oldVal;
                    ///
                    float MultySize = 20.0f;
                    Image MyImage = new Bitmap((int)((WX * 4) * MultySize), (int)((WY * 4) * MultySize));
                    Graphics g = Graphics.FromImage(MyImage);
                    SolidBrush Text = new SolidBrush(Color.Red);
                    for (int ix = 0; ix < WX; ix++)
                    {
                        for (int iy = 0; iy < WY; iy++)
                        {
                            double H = 1.0 - (float)retval[ix, iy];
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
                 
                    progress.Report(MyImage);
                    g.Dispose();
                }
                SearchRootList = SearchRootList.OrderBy(d =>(Gradient[SearchRootList.IndexOf(d)])).ToList();                
                KXMSolvers kx = new KXMSolvers();
                var xnew = kx.OC(Gradient.Count, 1, SearchTree.GetLeafsValues().ToArray(), 0.5, Gradient.ToArray());
                double gMx=Gradient.Max();
                double gMn=Gradient.Min();
                foreach (var q in SearchRootList)
                {
                    double delta = Gradient[SearchRootList.IndexOf(q)];                  
                    if (!q.IsLast)
                    {
                         
                        q.NodeValue = Math.Min(1.0, Math.Max(0.001, xnew[SearchRootList.IndexOf(q)]));
                        if (q.NodeValue >= 0.9)
                        {                            
                            q.SubDivide(q.NodeValue/4.0);
                        }
                        //break;
                    }
                }

               
            }
            retval = convertTree2Mesh(SearchTree, WX, WY);
            return retval;
        }

        private static double CalcTreeKX(int WX, int WY, double volfraq, double[,] retval,  int penal)
        {
            KXMSolvers kx = new KXMSolvers();
            var Rd1 = MatrixMath.To1D(retval);
            var U = kx.FE(WX, WY, Rd1, penal);
            var ke = kx.KE;
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
                    double xp = Math.Pow(Rd1[ely + elx * WY], penal);

                    double[] KEUe = MatrixMath.MatVecProd(ke, Ue);
                    double UEKUE = 0;
                    for (int i = 0; i < KEUe.Length; i++)
                    {
                        UEKUE += Ue[i] * KEUe[i];
                    }

                    c += xp * UEKUE;

                }
            }
            double sumV = Rd1.Sum();
            sumV = sumV - (double)(WX * WY) * volfraq;
            //
            double F = c;
            return F;
        }
    }
}
