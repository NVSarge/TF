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
            
            // var x=await Task.Factory.StartNew(() => SectionalSearch.GoSearch(ProgessDraw, WX, WY, volfraq, progressB), TaskCreationOptions.LongRunning);
            //await Task.Factory.StartNew(() => GeneticsSearch.GoBreed_Modal(ProgessDraw, WX, WY, volfraq, pps, flk), TaskCreationOptions.LongRunning);
            //await Task.Factory.StartNew(() => GeneticsSearch.GoWeave_Modal(ProgessDraw, WX, WY, volfraq, pps, flk, progressB), TaskCreationOptions.LongRunning);
            //await Task.Factory.StartNew(() => GeneticsSearch.GoWeave(ProgessDraw, WX, WY, volfraq, pps,flk), TaskCreationOptions.LongRunning);
            //await Task.Factory.StartNew(() => GeneticsSearch.GoBreed(ProgessDraw, WX, WY, volfraq, 10000, 300), TaskCreationOptions.LongRunning);
             //await Task.Factory.StartNew(() => GeneticsSearch.GoSimp(ProgessDraw, WX, WY, volfraq, 300,1), TaskCreationOptions.LongRunning);
            await Task.Factory.StartNew(() => GeneticsSearch.GoHarmonic(ProgessDraw, WX, WY, volfraq, 300, 1), TaskCreationOptions.LongRunning);

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            QuadTree<int> TestTree = new QuadTree<int>();
            TestTree.Root.NodeValue = 10;
            TestTree.Root.BB = new System.Windows.Rect(0,0, 250, 250);
            TestTree.Root.SubDivide();
            int i = 0;
            foreach (var q in TestTree.Root.Siblings)
            {
                q.NodeValue = i++;
            }
            TestTree.Root.Siblings[1].SubDivide();
            TestTree.Root.Siblings[2].SubDivide();
            foreach (var q in TestTree.Root.Siblings[2].Siblings)
            {
                q.NodeValue = i++;
            }
            foreach (var q in TestTree.Root.Siblings[1].Siblings)
            {
                q.NodeValue = i++;
            }  
            var Z = TestTree.getLeafs();
            var DrawArea = new Bitmap(Plotter.Size.Width, Plotter.Size.Height);
            Graphics g = Graphics.FromImage(DrawArea);
            SolidBrush shadowBrush = new SolidBrush(Color.DarkGreen);
            Pen p = new Pen(Color.Black);
            foreach (var q in Z)
            {
                print(q.NodeValue);
                q.NodeValue *= 2;
                g.FillRectangle(shadowBrush,new RectangleF((float)q.BB.X, (float)q.BB.Y, (float)q.BB.Width, (float)q.BB.Height));
                g.DrawRectangle(p, new Rectangle((int)q.BB.X, (int)q.BB.Y, (int)q.BB.Width, (int)q.BB.Height));
            }            
            g.Dispose();
            Plotter.Image = DrawArea;
        }
    }



}
