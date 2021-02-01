using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;
using Tensorflow;
using NumSharp;
using System.Diagnostics;


namespace TF
{
    public partial class Form1 : Form
    {



        public Form1()
        {
            InitializeComponent();

        }

        public void DrawCallBack(double[] G)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var training_steps = 1000;
            var learning_rate = 0.01f;
            var display_step = 100;

            // Sample data
            var X = np.array(3.3f, 4.4f, 5.5f, 6.71f, 6.93f, 4.168f, 9.779f, 6.182f, 7.59f, 2.167f,
                         7.042f, 10.791f, 5.313f, 7.997f, 5.654f, 9.27f, 3.1f);
            var Y = np.array(1.7f, 2.76f, 2.09f, 3.19f, 1.694f, 1.573f, 3.366f, 2.596f, 2.53f, 1.221f,
                         2.827f, 3.465f, 1.65f, 2.904f, 2.42f, 2.94f, 1.3f);
            var n_samples = X.shape[0];

            // We can set a fixed init value in order to demo
            var W = tf.Variable(-0.06f, name: "weight");
            var b = tf.Variable(-0.73f, name: "bias");

            var optimizer = keras.optimizers.SGD(learning_rate);

            // Run training for the given number of steps.
            foreach (var step in range(1, training_steps + 1))
            {
                // Run the optimization to update W and b values.
                // Wrap computation inside a GradientTape for automatic differentiation.
                var g = tf.GradientTape();
                // Linear regression (Wx + b).
                var pred = W * X + b;
                // Mean square error.
                var loss = tf.reduce_sum(tf.pow(pred - Y, 2)) / (2 * n_samples);
                // should stop recording
                // Compute gradients.
                var gradients = g.gradient(loss, (W, b));

                // Update W and b following gradients.
                optimizer.apply_gradients(zip(gradients, (W, b)));

                if (step % display_step == 0)
                {
                    pred = W * X + b;
                    loss = tf.reduce_sum(tf.pow(pred - Y, 2)) / (2 * n_samples);
                    print($"step: {step}, loss: {loss.numpy()}, W: {W.numpy()}, b: {b.numpy()}");
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var ProgessDraw = new Progress<Image>(s => Plotter.Image = s);
            totalProgress.Value = 0;
            var progressB = new Progress<double>(percent =>
            {
                totalProgress.Value = (int)(percent*100.0);
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                var tm = ts.TotalSeconds;
                tm=100.0*tm / (1.0 - percent);
                ts=TimeSpan.FromSeconds(tm);
                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}:{3:00}",
                    ts.Days,ts.Hours, ts.Minutes, ts.Seconds);
                ETA.Text = elapsedTime;
                stopWatch.Restart();
                
            });
            int WX = int.Parse(sizeX.Text);
            int WY = int.Parse(sizeY.Text);
            double volfraq = 0.5;
            int pps = int.Parse(Pops.Text);
            int flk = int.Parse(Flock.Text);

            //await Task.Factory.StartNew(() => GeneticsSearch.GoBreed_Modal(ProgessDraw, WX, WY, volfraq, pps, flk), TaskCreationOptions.LongRunning);
            await Task.Factory.StartNew(() => GeneticsSearch.GoWeave_Modal(ProgessDraw, WX, WY, volfraq, pps, flk, progressB), TaskCreationOptions.LongRunning);
            //await Task.Factory.StartNew(() => GeneticsSearch.GoWeave(ProgessDraw, WX, WY, volfraq, pps,flk), TaskCreationOptions.LongRunning);
            //await Task.Factory.StartNew(() => GeneticsSearch.GoBreed(ProgessDraw, WX, WY, volfraq, 10000, 300), TaskCreationOptions.LongRunning);
            // await Task.Factory.StartNew(() => GeneticsSearch.GoSimp(ProgessDraw, WX, WY, volfraq, 300,1), TaskCreationOptions.LongRunning);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Console.WriteLine("\nBegin matrix inverse demo\n");
            double[][] m = new double[4][];
            m[0] = new double[] { 3, 7, 2, 5 };
            m[1] = new double[] { 4, 0, 1, 1 };
            m[2] = new double[] { 1, 6, 3, 0 };
            m[3] = new double[] { 2, 8, 4, 3 };

            Console.WriteLine("Original matrix m is ");
            MatrixMath.MatShow(m, 4, 8);

            double d = MatrixMath.MatDeterminant(m);
            if (Math.Abs(d) < 1.0e-5)
                Console.WriteLine("\nMatrix has no inverse");
            else
                Console.WriteLine("\nDet(m) = " + d.ToString("F4"));

            double[][] inv = MatrixMath.MatInverse(m);
            Console.WriteLine("\nInverse matrix inv is ");
            MatrixMath.MatShow(inv, 4, 8);

            double[][] prod = MatrixMath.MatProduct(m, inv);
            Console.WriteLine("\nThe product of m * inv is ");
            MatrixMath.MatShow(prod, 1, 6);

            double[][] lum;
            int[] perm;
            int toggle = MatrixMath.MatDecompose(m, out lum, out perm);
            Console.WriteLine("\nThe combined lower-upper decomposition of m is");
            MatrixMath.MatShow(lum, 4, 8);

            double[][] lower = MatrixMath.ExtractLower(lum);
            double[][] upper = MatrixMath.ExtractUpper(lum);

            Console.WriteLine("\nThe lower part of LUM is");
            MatrixMath.MatShow(lower, 4, 8);

            Console.WriteLine("\nThe upper part of LUM is");
            MatrixMath.MatShow(upper, 4, 8);

            Console.WriteLine("\nThe perm[] array is");
            MatrixMath.VecShow(perm, 4);

            double[][] lowUp = MatrixMath.MatProduct(lower, upper);
            Console.WriteLine("\nThe product of lower * upper is ");
            MatrixMath.MatShow(lowUp, 4, 8);

            Console.WriteLine("\nVector b = ");
            double[] b = new double[] { 12, 7, 7, 13 };
            MatrixMath.VecShow(b, 1, 8);

            Console.WriteLine("\nSolving m*x = b");
            double[] x = MatrixMath.MatVecProd(inv, b);  // (1, 0, 2, 1)



            Console.WriteLine("\nSolution x = ");
            MatrixMath.VecShow(x, 1, 8);

            double[,] mm = MatrixMath.To2D<double>(m);
            MatrixMath.SolveLU tc = new MatrixMath.SolveLU(4, mm, b);
            double[] U = new double[4];

            if (tc.LUdecomp())
            {
                U = tc.SolveCrout();

            }/**/



            Console.WriteLine("\nEnd demo");
            Console.ReadLine();
        }
    }



}
