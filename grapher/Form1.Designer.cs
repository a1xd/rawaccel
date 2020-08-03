using System.Linq;

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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.AccelerationChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.accelTypeDrop = new System.Windows.Forms.ComboBox();
            this.sensitivityBoxX = new System.Windows.Forms.TextBox();
            this.sensitivityLabel = new System.Windows.Forms.Label();
            this.rotationBox = new System.Windows.Forms.TextBox();
            this.rotationLabel = new System.Windows.Forms.Label();
            this.accelerationBox = new System.Windows.Forms.TextBox();
            this.constantOneLabel = new System.Windows.Forms.Label();
            this.capBoxX = new System.Windows.Forms.TextBox();
            this.capLabel = new System.Windows.Forms.Label();
            this.weightBoxFirst = new System.Windows.Forms.TextBox();
            this.weightLabel = new System.Windows.Forms.Label();
            this.weightBoxSecond = new System.Windows.Forms.TextBox();
            this.limitBox = new System.Windows.Forms.TextBox();
            this.constantTwoLabel = new System.Windows.Forms.Label();
            this.midpointBox = new System.Windows.Forms.TextBox();
            this.constantThreeLabel = new System.Windows.Forms.Label();
            this.offsetBox = new System.Windows.Forms.TextBox();
            this.offsetLabel = new System.Windows.Forms.Label();
            this.writeButton = new System.Windows.Forms.Button();
            this.sensitivityBoxY = new System.Windows.Forms.TextBox();
            this.capBoxY = new System.Windows.Forms.TextBox();
            this.sensXYLock = new System.Windows.Forms.CheckBox();
            this.capXYLock = new System.Windows.Forms.CheckBox();
            this.weightXYLock = new System.Windows.Forms.CheckBox();
            this.LockXYLabel = new System.Windows.Forms.Label();
            this.VelocityChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.GainChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.AccelerationChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VelocityChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GainChart)).BeginInit();
            this.SuspendLayout();
            // 
            // AccelerationChart
            // 
            chartArea1.AxisX.Title = "Speed (counts/ms)";
            chartArea1.AxisY.Title = "Sensitivity (magnitude ratio)";
            chartArea1.Name = "ChartArea1";
            this.AccelerationChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.AccelerationChart.Legends.Add(legend1);
            this.AccelerationChart.Location = new System.Drawing.Point(242, 1);
            this.AccelerationChart.Name = "AccelerationChart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "Accelerated Sensitivity";
            this.AccelerationChart.Series.Add(series1);
            this.AccelerationChart.Size = new System.Drawing.Size(721, 312);
            this.AccelerationChart.TabIndex = 0;
            this.AccelerationChart.Text = "chart1";
            // 
            // accelTypeDrop
            // 
            this.accelTypeDrop.FormattingEnabled = true;
            this.accelTypeDrop.Location = new System.Drawing.Point(14, 89);
            this.accelTypeDrop.Name = "accelTypeDrop";
            this.accelTypeDrop.Size = new System.Drawing.Size(151, 21);
            this.accelTypeDrop.TabIndex = 2;
            this.accelTypeDrop.Text = "Acceleration Type";
            // 
            // sensitivityBoxX
            // 
            this.sensitivityBoxX.Location = new System.Drawing.Point(95, 37);
            this.sensitivityBoxX.Name = "sensitivityBoxX";
            this.sensitivityBoxX.Size = new System.Drawing.Size(32, 20);
            this.sensitivityBoxX.TabIndex = 3;
            // 
            // sensitivityLabel
            // 
            this.sensitivityLabel.AutoSize = true;
            this.sensitivityLabel.Location = new System.Drawing.Point(14, 40);
            this.sensitivityLabel.Name = "sensitivityLabel";
            this.sensitivityLabel.Size = new System.Drawing.Size(54, 13);
            this.sensitivityLabel.TabIndex = 4;
            this.sensitivityLabel.Text = "Sensitivity";
            // 
            // rotationBox
            // 
            this.rotationBox.Location = new System.Drawing.Point(95, 63);
            this.rotationBox.Name = "rotationBox";
            this.rotationBox.Size = new System.Drawing.Size(70, 20);
            this.rotationBox.TabIndex = 5;
            // 
            // rotationLabel
            // 
            this.rotationLabel.AutoSize = true;
            this.rotationLabel.Location = new System.Drawing.Point(24, 66);
            this.rotationLabel.Name = "rotationLabel";
            this.rotationLabel.Size = new System.Drawing.Size(47, 13);
            this.rotationLabel.TabIndex = 6;
            this.rotationLabel.Text = "Rotation";
            // 
            // accelerationBox
            // 
            this.accelerationBox.Location = new System.Drawing.Point(96, 116);
            this.accelerationBox.Name = "accelerationBox";
            this.accelerationBox.Size = new System.Drawing.Size(70, 20);
            this.accelerationBox.TabIndex = 7;
            // 
            // constantOneLabel
            // 
            this.constantOneLabel.AutoSize = true;
            this.constantOneLabel.Location = new System.Drawing.Point(14, 119);
            this.constantOneLabel.Name = "constantOneLabel";
            this.constantOneLabel.Size = new System.Drawing.Size(66, 13);
            this.constantOneLabel.TabIndex = 9;
            this.constantOneLabel.Text = "Acceleration";
            this.constantOneLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // capBoxX
            // 
            this.capBoxX.Location = new System.Drawing.Point(95, 142);
            this.capBoxX.Name = "capBoxX";
            this.capBoxX.Size = new System.Drawing.Size(32, 20);
            this.capBoxX.TabIndex = 10;
            // 
            // capLabel
            // 
            this.capLabel.AutoSize = true;
            this.capLabel.Location = new System.Drawing.Point(24, 146);
            this.capLabel.Name = "capLabel";
            this.capLabel.Size = new System.Drawing.Size(26, 13);
            this.capLabel.TabIndex = 11;
            this.capLabel.Text = "Cap";
            this.capLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // weightBoxFirst
            // 
            this.weightBoxFirst.Location = new System.Drawing.Point(95, 168);
            this.weightBoxFirst.Name = "weightBoxFirst";
            this.weightBoxFirst.Size = new System.Drawing.Size(32, 20);
            this.weightBoxFirst.TabIndex = 12;
            // 
            // weightLabel
            // 
            this.weightLabel.AutoSize = true;
            this.weightLabel.Location = new System.Drawing.Point(24, 171);
            this.weightLabel.Name = "weightLabel";
            this.weightLabel.Size = new System.Drawing.Size(41, 13);
            this.weightLabel.TabIndex = 13;
            this.weightLabel.Text = "Weight";
            this.weightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // weightBoxSecond
            // 
            this.weightBoxSecond.Location = new System.Drawing.Point(134, 168);
            this.weightBoxSecond.Name = "weightBoxSecond";
            this.weightBoxSecond.Size = new System.Drawing.Size(32, 20);
            this.weightBoxSecond.TabIndex = 14;
            // 
            // limitBox
            // 
            this.limitBox.Location = new System.Drawing.Point(95, 220);
            this.limitBox.Name = "limitBox";
            this.limitBox.Size = new System.Drawing.Size(70, 20);
            this.limitBox.TabIndex = 15;
            // 
            // constantTwoLabel
            // 
            this.constantTwoLabel.AutoSize = true;
            this.constantTwoLabel.Location = new System.Drawing.Point(14, 223);
            this.constantTwoLabel.Name = "constantTwoLabel";
            this.constantTwoLabel.Size = new System.Drawing.Size(78, 13);
            this.constantTwoLabel.TabIndex = 16;
            this.constantTwoLabel.Text = "Limit/Exponent";
            this.constantTwoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // midpointBox
            // 
            this.midpointBox.Location = new System.Drawing.Point(95, 246);
            this.midpointBox.Name = "midpointBox";
            this.midpointBox.Size = new System.Drawing.Size(70, 20);
            this.midpointBox.TabIndex = 17;
            // 
            // constantThreeLabel
            // 
            this.constantThreeLabel.AutoSize = true;
            this.constantThreeLabel.Location = new System.Drawing.Point(21, 249);
            this.constantThreeLabel.Name = "constantThreeLabel";
            this.constantThreeLabel.Size = new System.Drawing.Size(47, 13);
            this.constantThreeLabel.TabIndex = 18;
            this.constantThreeLabel.Text = "Midpoint";
            this.constantThreeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // offsetBox
            // 
            this.offsetBox.Location = new System.Drawing.Point(95, 194);
            this.offsetBox.Name = "offsetBox";
            this.offsetBox.Size = new System.Drawing.Size(70, 20);
            this.offsetBox.TabIndex = 19;
            // 
            // offsetLabel
            // 
            this.offsetLabel.AutoSize = true;
            this.offsetLabel.Location = new System.Drawing.Point(24, 197);
            this.offsetLabel.Name = "offsetLabel";
            this.offsetLabel.Size = new System.Drawing.Size(35, 13);
            this.offsetLabel.TabIndex = 20;
            this.offsetLabel.Text = "Offset";
            this.offsetLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // writeButton
            // 
            this.writeButton.Location = new System.Drawing.Point(47, 272);
            this.writeButton.Name = "writeButton";
            this.writeButton.Size = new System.Drawing.Size(102, 23);
            this.writeButton.TabIndex = 21;
            this.writeButton.Text = "Write To Driver";
            this.writeButton.UseVisualStyleBackColor = true;
            this.writeButton.Click += new System.EventHandler(this.writeButton_Click);
            // 
            // sensitivityBoxY
            // 
            this.sensitivityBoxY.Location = new System.Drawing.Point(133, 37);
            this.sensitivityBoxY.Name = "sensitivityBoxY";
            this.sensitivityBoxY.Size = new System.Drawing.Size(32, 20);
            this.sensitivityBoxY.TabIndex = 22;
            // 
            // capBoxY
            // 
            this.capBoxY.Location = new System.Drawing.Point(135, 142);
            this.capBoxY.Name = "capBoxY";
            this.capBoxY.Size = new System.Drawing.Size(31, 20);
            this.capBoxY.TabIndex = 23;
            // 
            // sensXYLock
            // 
            this.sensXYLock.AutoSize = true;
            this.sensXYLock.Checked = true;
            this.sensXYLock.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sensXYLock.Location = new System.Drawing.Point(188, 40);
            this.sensXYLock.Name = "sensXYLock";
            this.sensXYLock.Size = new System.Drawing.Size(15, 14);
            this.sensXYLock.TabIndex = 24;
            this.sensXYLock.UseVisualStyleBackColor = true;
            // 
            // capXYLock
            // 
            this.capXYLock.AutoSize = true;
            this.capXYLock.Checked = true;
            this.capXYLock.CheckState = System.Windows.Forms.CheckState.Checked;
            this.capXYLock.Location = new System.Drawing.Point(188, 145);
            this.capXYLock.Name = "capXYLock";
            this.capXYLock.Size = new System.Drawing.Size(15, 14);
            this.capXYLock.TabIndex = 25;
            this.capXYLock.UseVisualStyleBackColor = true;
            // 
            // weightXYLock
            // 
            this.weightXYLock.AutoSize = true;
            this.weightXYLock.Checked = true;
            this.weightXYLock.CheckState = System.Windows.Forms.CheckState.Checked;
            this.weightXYLock.Location = new System.Drawing.Point(188, 171);
            this.weightXYLock.Name = "weightXYLock";
            this.weightXYLock.Size = new System.Drawing.Size(15, 14);
            this.weightXYLock.TabIndex = 26;
            this.weightXYLock.UseVisualStyleBackColor = true;
            // 
            // LockXYLabel
            // 
            this.LockXYLabel.AutoSize = true;
            this.LockXYLabel.Location = new System.Drawing.Point(165, 21);
            this.LockXYLabel.Name = "LockXYLabel";
            this.LockXYLabel.Size = new System.Drawing.Size(60, 13);
            this.LockXYLabel.TabIndex = 27;
            this.LockXYLabel.Text = "Lock X && Y";
            // 
            // VelocityChart
            // 
            chartArea2.AxisX.Title = "Speed (count/ms)";
            chartArea2.AxisY.Title = "Output Speed (counts/ms)";
            chartArea2.Name = "ChartArea1";
            this.VelocityChart.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.VelocityChart.Legends.Add(legend2);
            this.VelocityChart.Location = new System.Drawing.Point(242, 319);
            this.VelocityChart.Name = "VelocityChart";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Legend = "Legend1";
            series2.Name = "Mouse Velocity";
            this.VelocityChart.Series.Add(series2);
            this.VelocityChart.Size = new System.Drawing.Size(721, 300);
            this.VelocityChart.TabIndex = 28;
            this.VelocityChart.Text = "chart1";
            // 
            // GainChart
            // 
            chartArea3.AxisX.Title = "Speed (counts/ms)";
            chartArea3.AxisY.Title = "Slope of Velocity Chart";
            chartArea3.Name = "ChartArea1";
            this.GainChart.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.GainChart.Legends.Add(legend3);
            this.GainChart.Location = new System.Drawing.Point(242, 625);
            this.GainChart.Name = "GainChart";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.Name = "Velocity Gain";
            this.GainChart.Series.Add(series3);
            this.GainChart.Size = new System.Drawing.Size(721, 300);
            this.GainChart.TabIndex = 29;
            this.GainChart.Text = "chart1";
            // 
            // RawAcceleration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(963, 925);
            this.Controls.Add(this.GainChart);
            this.Controls.Add(this.VelocityChart);
            this.Controls.Add(this.LockXYLabel);
            this.Controls.Add(this.weightXYLock);
            this.Controls.Add(this.capXYLock);
            this.Controls.Add(this.sensXYLock);
            this.Controls.Add(this.capBoxY);
            this.Controls.Add(this.sensitivityBoxY);
            this.Controls.Add(this.writeButton);
            this.Controls.Add(this.offsetLabel);
            this.Controls.Add(this.offsetBox);
            this.Controls.Add(this.constantThreeLabel);
            this.Controls.Add(this.midpointBox);
            this.Controls.Add(this.constantTwoLabel);
            this.Controls.Add(this.limitBox);
            this.Controls.Add(this.weightBoxSecond);
            this.Controls.Add(this.weightLabel);
            this.Controls.Add(this.weightBoxFirst);
            this.Controls.Add(this.capLabel);
            this.Controls.Add(this.capBoxX);
            this.Controls.Add(this.constantOneLabel);
            this.Controls.Add(this.accelerationBox);
            this.Controls.Add(this.rotationLabel);
            this.Controls.Add(this.rotationBox);
            this.Controls.Add(this.sensitivityLabel);
            this.Controls.Add(this.sensitivityBoxX);
            this.Controls.Add(this.accelTypeDrop);
            this.Controls.Add(this.AccelerationChart);
            this.Name = "RawAcceleration";
            this.Text = "Raw Acceleration Graph";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.AccelerationChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VelocityChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GainChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart AccelerationChart;
        private System.Windows.Forms.ComboBox accelTypeDrop;
        private System.Windows.Forms.TextBox sensitivityBoxX;
        private System.Windows.Forms.Label sensitivityLabel;
        private System.Windows.Forms.TextBox rotationBox;
        private System.Windows.Forms.Label rotationLabel;
        private System.Windows.Forms.TextBox accelerationBox;
        private System.Windows.Forms.Label constantOneLabel;
        private System.Windows.Forms.TextBox capBoxX;
        private System.Windows.Forms.Label capLabel;
        private System.Windows.Forms.TextBox weightBoxFirst;
        private System.Windows.Forms.Label weightLabel;
        private System.Windows.Forms.TextBox weightBoxSecond;
        private System.Windows.Forms.TextBox limitBox;
        private System.Windows.Forms.Label constantTwoLabel;
        private System.Windows.Forms.TextBox midpointBox;
        private System.Windows.Forms.Label constantThreeLabel;
        private System.Windows.Forms.TextBox offsetBox;
        private System.Windows.Forms.Label offsetLabel;
        private System.Windows.Forms.Button writeButton;
        private System.Windows.Forms.TextBox sensitivityBoxY;
        private System.Windows.Forms.TextBox capBoxY;
        private System.Windows.Forms.CheckBox sensXYLock;
        private System.Windows.Forms.CheckBox capXYLock;
        private System.Windows.Forms.CheckBox weightXYLock;
        private System.Windows.Forms.Label LockXYLabel;
        private System.Windows.Forms.DataVisualization.Charting.Chart VelocityChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart GainChart;
    }
}

