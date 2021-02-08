namespace TF
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button2 = new System.Windows.Forms.Button();
            this.Plotter = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.sizeY = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.sizeX = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Flock = new System.Windows.Forms.TextBox();
            this.Pops = new System.Windows.Forms.TextBox();
            this.totalProgress = new System.Windows.Forms.ProgressBar();
            this.ETA = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Plotter)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(16, 196);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(102, 52);
            this.button2.TabIndex = 1;
            this.button2.Text = "GO_GA";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Plotter
            // 
            this.Plotter.Location = new System.Drawing.Point(33, 37);
            this.Plotter.Name = "Plotter";
            this.Plotter.Size = new System.Drawing.Size(982, 888);
            this.Plotter.TabIndex = 4;
            this.Plotter.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.sizeY);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.sizeX);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.Flock);
            this.panel1.Controls.Add(this.Pops);
            this.panel1.Location = new System.Drawing.Point(1021, 37);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(179, 448);
            this.panel1.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(122, 139);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "SizeY";
            // 
            // sizeY
            // 
            this.sizeY.Location = new System.Drawing.Point(16, 136);
            this.sizeY.Name = "sizeY";
            this.sizeY.Size = new System.Drawing.Size(100, 20);
            this.sizeY.TabIndex = 6;
            this.sizeY.Text = "10";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(122, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "SizeX";
            // 
            // sizeX
            // 
            this.sizeX.Location = new System.Drawing.Point(16, 94);
            this.sizeX.Name = "sizeX";
            this.sizeX.Size = new System.Drawing.Size(100, 20);
            this.sizeX.TabIndex = 4;
            this.sizeX.Text = "10";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(122, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Flock";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(122, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Pops";
            // 
            // Flock
            // 
            this.Flock.Location = new System.Drawing.Point(16, 53);
            this.Flock.Name = "Flock";
            this.Flock.Size = new System.Drawing.Size(100, 20);
            this.Flock.TabIndex = 1;
            this.Flock.Text = "200";
            // 
            // Pops
            // 
            this.Pops.Location = new System.Drawing.Point(16, 18);
            this.Pops.Name = "Pops";
            this.Pops.Size = new System.Drawing.Size(100, 20);
            this.Pops.TabIndex = 0;
            this.Pops.Text = "10000";
            // 
            // totalProgress
            // 
            this.totalProgress.Location = new System.Drawing.Point(33, 12);
            this.totalProgress.Name = "totalProgress";
            this.totalProgress.Size = new System.Drawing.Size(982, 23);
            this.totalProgress.Step = 1;
            this.totalProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.totalProgress.TabIndex = 6;
            // 
            // ETA
            // 
            this.ETA.AutoSize = true;
            this.ETA.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ETA.Location = new System.Drawing.Point(1021, 9);
            this.ETA.Name = "ETA";
            this.ETA.Size = new System.Drawing.Size(150, 25);
            this.ETA.TabIndex = 8;
            this.ETA.Text = "0000:00:00.00";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1048, 623);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1212, 937);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ETA);
            this.Controls.Add(this.totalProgress);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Plotter);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.Plotter)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PictureBox Plotter;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox Flock;
        private System.Windows.Forms.TextBox Pops;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox sizeX;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox sizeY;
        private System.Windows.Forms.ProgressBar totalProgress;
        private System.Windows.Forms.Label ETA;
        private System.Windows.Forms.Button button1;
    }
}

