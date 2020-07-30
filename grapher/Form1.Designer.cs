namespace grapher
{
    partial class RawAcceleration
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea12 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend12 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series12 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.AccelerationChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.accelTypeDrop = new System.Windows.Forms.ComboBox();
            this.sensitivityBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rotationBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.accelerationBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.capBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.weightBoxFirst = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.weightBoxSecond = new System.Windows.Forms.TextBox();
            this.limitBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.midpointBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.offsetBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.writeButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.AccelerationChart)).BeginInit();
            this.SuspendLayout();
            // 
            // AccelerationChart
            // 
            chartArea12.AxisX.Title = "Speed (counts/ms)";
            chartArea12.AxisY.Title = "Sensitivity (magnitude ratio)";
            chartArea12.Name = "ChartArea1";
            this.AccelerationChart.ChartAreas.Add(chartArea12);
            legend12.Name = "Legend1";
            this.AccelerationChart.Legends.Add(legend12);
            this.AccelerationChart.Location = new System.Drawing.Point(162, 0);
            this.AccelerationChart.Name = "AccelerationChart";
            series12.ChartArea = "ChartArea1";
            series12.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series12.Legend = "Legend1";
            series12.Name = "Accelerated Sensitivity";
            this.AccelerationChart.Series.Add(series12);
            this.AccelerationChart.Size = new System.Drawing.Size(801, 312);
            this.AccelerationChart.TabIndex = 0;
            this.AccelerationChart.Text = "chart1";
            // 
            // accelTypeDrop
            // 
            this.accelTypeDrop.FormattingEnabled = true;
            this.accelTypeDrop.Items.AddRange(new object[] {
            "Off",
            "Linear",
            "Classic",
            "Natural",
            "Logarithmic",
            "Sigmoid",
            "Power"});
            this.accelTypeDrop.Location = new System.Drawing.Point(15, 86);
            this.accelTypeDrop.Name = "accelTypeDrop";
            this.accelTypeDrop.Size = new System.Drawing.Size(132, 21);
            this.accelTypeDrop.TabIndex = 2;
            this.accelTypeDrop.Text = "Acceleration Type";
            // 
            // sensitivityBox
            // 
            this.sensitivityBox.Location = new System.Drawing.Point(96, 15);
            this.sensitivityBox.Name = "sensitivityBox";
            this.sensitivityBox.Size = new System.Drawing.Size(51, 20);
            this.sensitivityBox.TabIndex = 3;
            this.sensitivityBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.sensitivityBox_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Sensitivity";
            // 
            // rotationBox
            // 
            this.rotationBox.Location = new System.Drawing.Point(96, 45);
            this.rotationBox.Name = "rotationBox";
            this.rotationBox.Size = new System.Drawing.Size(51, 20);
            this.rotationBox.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Rotation";
            // 
            // accelerationBox
            // 
            this.accelerationBox.Location = new System.Drawing.Point(96, 113);
            this.accelerationBox.Name = "accelerationBox";
            this.accelerationBox.Size = new System.Drawing.Size(51, 20);
            this.accelerationBox.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Acceleration";
            // 
            // capBox
            // 
            this.capBox.Location = new System.Drawing.Point(96, 140);
            this.capBox.Name = "capBox";
            this.capBox.Size = new System.Drawing.Size(51, 20);
            this.capBox.TabIndex = 10;
            this.capBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.capBox_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Cap";
            // 
            // weightBoxFirst
            // 
            this.weightBoxFirst.Location = new System.Drawing.Point(96, 167);
            this.weightBoxFirst.Name = "weightBoxFirst";
            this.weightBoxFirst.Size = new System.Drawing.Size(24, 20);
            this.weightBoxFirst.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 173);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Weight";
            // 
            // weightBoxSecond
            // 
            this.weightBoxSecond.Location = new System.Drawing.Point(126, 167);
            this.weightBoxSecond.Name = "weightBoxSecond";
            this.weightBoxSecond.Size = new System.Drawing.Size(21, 20);
            this.weightBoxSecond.TabIndex = 14;
            // 
            // limitBox
            // 
            this.limitBox.Location = new System.Drawing.Point(96, 219);
            this.limitBox.Name = "limitBox";
            this.limitBox.Size = new System.Drawing.Size(51, 20);
            this.limitBox.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 222);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Limit/Exponent";
            // 
            // midpointBox
            // 
            this.midpointBox.Location = new System.Drawing.Point(96, 245);
            this.midpointBox.Name = "midpointBox";
            this.midpointBox.Size = new System.Drawing.Size(51, 20);
            this.midpointBox.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(22, 248);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Midpoint";
            // 
            // offsetBox
            // 
            this.offsetBox.Location = new System.Drawing.Point(96, 193);
            this.offsetBox.Name = "offsetBox";
            this.offsetBox.Size = new System.Drawing.Size(51, 20);
            this.offsetBox.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(25, 200);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Offset";
            // 
            // writeButton
            // 
            this.writeButton.Location = new System.Drawing.Point(28, 271);
            this.writeButton.Name = "writeButton";
            this.writeButton.Size = new System.Drawing.Size(102, 23);
            this.writeButton.TabIndex = 21;
            this.writeButton.Text = "Write To Driver";
            this.writeButton.UseVisualStyleBackColor = true;
            this.writeButton.Click += new System.EventHandler(this.writeButton_Click);
            // 
            // RawAcceleration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(963, 310);
            this.Controls.Add(this.writeButton);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.offsetBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.midpointBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.limitBox);
            this.Controls.Add(this.weightBoxSecond);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.weightBoxFirst);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.capBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.accelerationBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rotationBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sensitivityBox);
            this.Controls.Add(this.accelTypeDrop);
            this.Controls.Add(this.AccelerationChart);
            this.Name = "RawAcceleration";
            this.Text = "Raw Acceleration Graph";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.AccelerationChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart AccelerationChart;
        private System.Windows.Forms.ComboBox accelTypeDrop;
        private System.Windows.Forms.TextBox sensitivityBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox rotationBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox accelerationBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox capBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox weightBoxFirst;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox weightBoxSecond;
        private System.Windows.Forms.TextBox limitBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox midpointBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox offsetBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button writeButton;
    }
}

