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
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series10 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series11 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series12 = new System.Windows.Forms.DataVisualization.Charting.Series();
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.graphsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showVelocityGainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scaleByDPIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dPIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DPITextBox = new System.Windows.Forms.ToolStripTextBox();
            this.pollRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PollRateTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.ScaleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.capStyleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gainCapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.legacyCapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AutoWriteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AccelerationChartY = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.VelocityChartY = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.GainChartY = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.MouseLabel = new System.Windows.Forms.Label();
            this.ActiveValueTitle = new System.Windows.Forms.Label();
            this.SensitivityActiveXLabel = new System.Windows.Forms.Label();
            this.SensitivityActiveYLabel = new System.Windows.Forms.Label();
            this.RotationActiveLabel = new System.Windows.Forms.Label();
            this.AccelTypeActiveLabel = new System.Windows.Forms.Label();
            this.AccelerationActiveLabel = new System.Windows.Forms.Label();
            this.CapActiveXLabel = new System.Windows.Forms.Label();
            this.WeightActiveXLabel = new System.Windows.Forms.Label();
            this.WeightActiveYLabel = new System.Windows.Forms.Label();
            this.CapActiveYLabel = new System.Windows.Forms.Label();
            this.OffsetActiveLabel = new System.Windows.Forms.Label();
            this.LimitExpActiveLabel = new System.Windows.Forms.Label();
            this.MidpointActiveLabel = new System.Windows.Forms.Label();
            this.offsetStyleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gainOffsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.legacyOffsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.AccelerationChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VelocityChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GainChart)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AccelerationChartY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VelocityChartY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GainChartY)).BeginInit();
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
            this.AccelerationChart.Location = new System.Drawing.Point(333, 0);
            this.AccelerationChart.Name = "AccelerationChart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "Accelerated Sensitivity";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            series2.Legend = "Legend1";
            series2.Name = "LastAccelVal";
            this.AccelerationChart.Series.Add(series1);
            this.AccelerationChart.Series.Add(series2);
            this.AccelerationChart.Size = new System.Drawing.Size(723, 328);
            this.AccelerationChart.TabIndex = 0;
            this.AccelerationChart.Text = "chart1";
            // 
            // accelTypeDrop
            // 
            this.accelTypeDrop.FormattingEnabled = true;
            this.accelTypeDrop.Location = new System.Drawing.Point(24, 98);
            this.accelTypeDrop.Name = "accelTypeDrop";
            this.accelTypeDrop.Size = new System.Drawing.Size(151, 21);
            this.accelTypeDrop.TabIndex = 2;
            this.accelTypeDrop.Text = "Acceleration Type";
            // 
            // sensitivityBoxX
            // 
            this.sensitivityBoxX.Location = new System.Drawing.Point(105, 46);
            this.sensitivityBoxX.Name = "sensitivityBoxX";
            this.sensitivityBoxX.Size = new System.Drawing.Size(32, 20);
            this.sensitivityBoxX.TabIndex = 3;
            // 
            // sensitivityLabel
            // 
            this.sensitivityLabel.AutoSize = true;
            this.sensitivityLabel.Location = new System.Drawing.Point(24, 49);
            this.sensitivityLabel.Name = "sensitivityLabel";
            this.sensitivityLabel.Size = new System.Drawing.Size(54, 13);
            this.sensitivityLabel.TabIndex = 4;
            this.sensitivityLabel.Text = "Sensitivity";
            // 
            // rotationBox
            // 
            this.rotationBox.Location = new System.Drawing.Point(105, 72);
            this.rotationBox.Name = "rotationBox";
            this.rotationBox.Size = new System.Drawing.Size(70, 20);
            this.rotationBox.TabIndex = 5;
            // 
            // rotationLabel
            // 
            this.rotationLabel.AutoSize = true;
            this.rotationLabel.Location = new System.Drawing.Point(34, 75);
            this.rotationLabel.Name = "rotationLabel";
            this.rotationLabel.Size = new System.Drawing.Size(47, 13);
            this.rotationLabel.TabIndex = 6;
            this.rotationLabel.Text = "Rotation";
            // 
            // accelerationBox
            // 
            this.accelerationBox.Location = new System.Drawing.Point(105, 125);
            this.accelerationBox.Name = "accelerationBox";
            this.accelerationBox.Size = new System.Drawing.Size(69, 20);
            this.accelerationBox.TabIndex = 7;
            // 
            // constantOneLabel
            // 
            this.constantOneLabel.AutoSize = true;
            this.constantOneLabel.Location = new System.Drawing.Point(24, 128);
            this.constantOneLabel.Name = "constantOneLabel";
            this.constantOneLabel.Size = new System.Drawing.Size(66, 13);
            this.constantOneLabel.TabIndex = 9;
            this.constantOneLabel.Text = "Acceleration";
            this.constantOneLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // capBoxX
            // 
            this.capBoxX.Location = new System.Drawing.Point(105, 151);
            this.capBoxX.Name = "capBoxX";
            this.capBoxX.Size = new System.Drawing.Size(32, 20);
            this.capBoxX.TabIndex = 10;
            // 
            // capLabel
            // 
            this.capLabel.AutoSize = true;
            this.capLabel.Location = new System.Drawing.Point(34, 155);
            this.capLabel.Name = "capLabel";
            this.capLabel.Size = new System.Drawing.Size(26, 13);
            this.capLabel.TabIndex = 11;
            this.capLabel.Text = "Cap";
            this.capLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // weightBoxFirst
            // 
            this.weightBoxFirst.Location = new System.Drawing.Point(105, 177);
            this.weightBoxFirst.Name = "weightBoxFirst";
            this.weightBoxFirst.Size = new System.Drawing.Size(32, 20);
            this.weightBoxFirst.TabIndex = 12;
            // 
            // weightLabel
            // 
            this.weightLabel.AutoSize = true;
            this.weightLabel.Location = new System.Drawing.Point(34, 180);
            this.weightLabel.Name = "weightLabel";
            this.weightLabel.Size = new System.Drawing.Size(41, 13);
            this.weightLabel.TabIndex = 13;
            this.weightLabel.Text = "Weight";
            this.weightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // weightBoxSecond
            // 
            this.weightBoxSecond.Location = new System.Drawing.Point(144, 177);
            this.weightBoxSecond.Name = "weightBoxSecond";
            this.weightBoxSecond.Size = new System.Drawing.Size(31, 20);
            this.weightBoxSecond.TabIndex = 14;
            // 
            // limitBox
            // 
            this.limitBox.Location = new System.Drawing.Point(105, 229);
            this.limitBox.Name = "limitBox";
            this.limitBox.Size = new System.Drawing.Size(70, 20);
            this.limitBox.TabIndex = 15;
            // 
            // constantTwoLabel
            // 
            this.constantTwoLabel.AutoSize = true;
            this.constantTwoLabel.Location = new System.Drawing.Point(24, 232);
            this.constantTwoLabel.Name = "constantTwoLabel";
            this.constantTwoLabel.Size = new System.Drawing.Size(78, 13);
            this.constantTwoLabel.TabIndex = 16;
            this.constantTwoLabel.Text = "Limit/Exponent";
            this.constantTwoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // midpointBox
            // 
            this.midpointBox.Location = new System.Drawing.Point(105, 255);
            this.midpointBox.Name = "midpointBox";
            this.midpointBox.Size = new System.Drawing.Size(70, 20);
            this.midpointBox.TabIndex = 17;
            // 
            // constantThreeLabel
            // 
            this.constantThreeLabel.AutoSize = true;
            this.constantThreeLabel.Location = new System.Drawing.Point(31, 258);
            this.constantThreeLabel.Name = "constantThreeLabel";
            this.constantThreeLabel.Size = new System.Drawing.Size(47, 13);
            this.constantThreeLabel.TabIndex = 18;
            this.constantThreeLabel.Text = "Midpoint";
            this.constantThreeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // offsetBox
            // 
            this.offsetBox.Location = new System.Drawing.Point(105, 203);
            this.offsetBox.Name = "offsetBox";
            this.offsetBox.Size = new System.Drawing.Size(70, 20);
            this.offsetBox.TabIndex = 19;
            // 
            // offsetLabel
            // 
            this.offsetLabel.AutoSize = true;
            this.offsetLabel.Location = new System.Drawing.Point(34, 206);
            this.offsetLabel.Name = "offsetLabel";
            this.offsetLabel.Size = new System.Drawing.Size(35, 13);
            this.offsetLabel.TabIndex = 20;
            this.offsetLabel.Text = "Offset";
            this.offsetLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // writeButton
            // 
            this.writeButton.Location = new System.Drawing.Point(57, 281);
            this.writeButton.Name = "writeButton";
            this.writeButton.Size = new System.Drawing.Size(102, 23);
            this.writeButton.TabIndex = 21;
            this.writeButton.Text = "Write To Driver";
            this.writeButton.UseVisualStyleBackColor = true;
            this.writeButton.Click += new System.EventHandler(this.writeButton_Click);
            // 
            // sensitivityBoxY
            // 
            this.sensitivityBoxY.Location = new System.Drawing.Point(143, 46);
            this.sensitivityBoxY.Name = "sensitivityBoxY";
            this.sensitivityBoxY.Size = new System.Drawing.Size(32, 20);
            this.sensitivityBoxY.TabIndex = 22;
            // 
            // capBoxY
            // 
            this.capBoxY.Location = new System.Drawing.Point(144, 151);
            this.capBoxY.Name = "capBoxY";
            this.capBoxY.Size = new System.Drawing.Size(31, 20);
            this.capBoxY.TabIndex = 23;
            // 
            // sensXYLock
            // 
            this.sensXYLock.AutoSize = true;
            this.sensXYLock.Checked = true;
            this.sensXYLock.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sensXYLock.Location = new System.Drawing.Point(198, 49);
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
            this.capXYLock.Location = new System.Drawing.Point(198, 154);
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
            this.weightXYLock.Location = new System.Drawing.Point(198, 180);
            this.weightXYLock.Name = "weightXYLock";
            this.weightXYLock.Size = new System.Drawing.Size(15, 14);
            this.weightXYLock.TabIndex = 26;
            this.weightXYLock.UseVisualStyleBackColor = true;
            // 
            // LockXYLabel
            // 
            this.LockXYLabel.AutoSize = true;
            this.LockXYLabel.Location = new System.Drawing.Point(174, 30);
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
            this.VelocityChart.Location = new System.Drawing.Point(333, 334);
            this.VelocityChart.Name = "VelocityChart";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.Name = "Mouse Velocity";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series4.Color = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            series4.Legend = "Legend1";
            series4.Name = "LastVelocityVal";
            this.VelocityChart.Series.Add(series3);
            this.VelocityChart.Series.Add(series4);
            this.VelocityChart.Size = new System.Drawing.Size(723, 307);
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
            this.GainChart.Location = new System.Drawing.Point(333, 647);
            this.GainChart.Name = "GainChart";
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series5.Legend = "Legend1";
            series5.Name = "Velocity Gain";
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series6.Color = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            series6.Legend = "Legend1";
            series6.Name = "LastGainVal";
            this.GainChart.Series.Add(series5);
            this.GainChart.Series.Add(series6);
            this.GainChart.Size = new System.Drawing.Size(723, 309);
            this.GainChart.TabIndex = 29;
            this.GainChart.Text = "chart1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.graphsToolStripMenuItem,
            this.advancedToolStripMenuItem,
            this.startupToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1786, 24);
            this.menuStrip1.TabIndex = 30;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // graphsToolStripMenuItem
            // 
            this.graphsToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.graphsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showVelocityGainToolStripMenuItem,
            this.scaleByDPIToolStripMenuItem});
            this.graphsToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.graphsToolStripMenuItem.Name = "graphsToolStripMenuItem";
            this.graphsToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.graphsToolStripMenuItem.Text = "Charts";
            // 
            // showVelocityGainToolStripMenuItem
            // 
            this.showVelocityGainToolStripMenuItem.Name = "showVelocityGainToolStripMenuItem";
            this.showVelocityGainToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.showVelocityGainToolStripMenuItem.Text = "Show Velocity && Gain";
            // 
            // scaleByDPIToolStripMenuItem
            // 
            this.scaleByDPIToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dPIToolStripMenuItem,
            this.pollRateToolStripMenuItem,
            this.ScaleMenuItem});
            this.scaleByDPIToolStripMenuItem.Name = "scaleByDPIToolStripMenuItem";
            this.scaleByDPIToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.scaleByDPIToolStripMenuItem.Text = "Scale by Mouse Settngs";
            // 
            // dPIToolStripMenuItem
            // 
            this.dPIToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DPITextBox});
            this.dPIToolStripMenuItem.Name = "dPIToolStripMenuItem";
            this.dPIToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.dPIToolStripMenuItem.Text = "DPI";
            // 
            // DPITextBox
            // 
            this.DPITextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.DPITextBox.Name = "DPITextBox";
            this.DPITextBox.Size = new System.Drawing.Size(100, 23);
            // 
            // pollRateToolStripMenuItem
            // 
            this.pollRateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PollRateTextBox});
            this.pollRateToolStripMenuItem.Name = "pollRateToolStripMenuItem";
            this.pollRateToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.pollRateToolStripMenuItem.Text = "Poll Rate";
            // 
            // PollRateTextBox
            // 
            this.PollRateTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.PollRateTextBox.Name = "PollRateTextBox";
            this.PollRateTextBox.Size = new System.Drawing.Size(100, 23);
            // 
            // ScaleMenuItem
            // 
            this.ScaleMenuItem.Name = "ScaleMenuItem";
            this.ScaleMenuItem.Size = new System.Drawing.Size(169, 22);
            this.ScaleMenuItem.Text = "Re-scale by above";
            // 
            // advancedToolStripMenuItem
            // 
            this.advancedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.capStyleToolStripMenuItem,
            this.offsetStyleToolStripMenuItem});
            this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
            this.advancedToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
            this.advancedToolStripMenuItem.Text = "Advanced";
            // 
            // capStyleToolStripMenuItem
            // 
            this.capStyleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gainCapToolStripMenuItem,
            this.legacyCapToolStripMenuItem});
            this.capStyleToolStripMenuItem.Name = "capStyleToolStripMenuItem";
            this.capStyleToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.capStyleToolStripMenuItem.Text = "Cap Style";
            // 
            // gainCapToolStripMenuItem
            // 
            this.gainCapToolStripMenuItem.Checked = true;
            this.gainCapToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.gainCapToolStripMenuItem.Name = "gainCapToolStripMenuItem";
            this.gainCapToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.gainCapToolStripMenuItem.Text = "Gain (Default)";
            // 
            // legacyCapToolStripMenuItem
            // 
            this.legacyCapToolStripMenuItem.Name = "legacyCapToolStripMenuItem";
            this.legacyCapToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.legacyCapToolStripMenuItem.Text = "Legacy";
            // 
            // startupToolStripMenuItem
            // 
            this.startupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AutoWriteMenuItem});
            this.startupToolStripMenuItem.Name = "startupToolStripMenuItem";
            this.startupToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.startupToolStripMenuItem.Text = "Startup";
            // 
            // AutoWriteMenuItem
            // 
            this.AutoWriteMenuItem.Checked = true;
            this.AutoWriteMenuItem.CheckOnClick = true;
            this.AutoWriteMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AutoWriteMenuItem.Name = "AutoWriteMenuItem";
            this.AutoWriteMenuItem.Size = new System.Drawing.Size(229, 22);
            this.AutoWriteMenuItem.Text = "Apply Settings File on Startup";
            // 
            // AccelerationChartY
            // 
            chartArea4.AxisX.Title = "Speed (counts/ms)";
            chartArea4.AxisY.Title = "Sensitivity (magnitude ratio)";
            chartArea4.Name = "ChartArea1";
            this.AccelerationChartY.ChartAreas.Add(chartArea4);
            legend4.Name = "Legend1";
            this.AccelerationChartY.Legends.Add(legend4);
            this.AccelerationChartY.Location = new System.Drawing.Point(1062, 0);
            this.AccelerationChartY.Name = "AccelerationChartY";
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series7.Legend = "Legend1";
            series7.Name = "Accelerated Sensitivity";
            series8.ChartArea = "ChartArea1";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series8.Color = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            series8.Legend = "Legend1";
            series8.Name = "LastAccelVal";
            this.AccelerationChartY.Series.Add(series7);
            this.AccelerationChartY.Series.Add(series8);
            this.AccelerationChartY.Size = new System.Drawing.Size(723, 328);
            this.AccelerationChartY.TabIndex = 31;
            this.AccelerationChartY.Text = "chart1";
            // 
            // VelocityChartY
            // 
            chartArea5.AxisX.Title = "Speed (count/ms)";
            chartArea5.AxisY.Title = "Output Speed (counts/ms)";
            chartArea5.Name = "ChartArea1";
            this.VelocityChartY.ChartAreas.Add(chartArea5);
            legend5.Name = "Legend1";
            this.VelocityChartY.Legends.Add(legend5);
            this.VelocityChartY.Location = new System.Drawing.Point(1062, 334);
            this.VelocityChartY.Name = "VelocityChartY";
            series9.ChartArea = "ChartArea1";
            series9.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series9.Legend = "Legend1";
            series9.Name = "Mouse Velocity";
            series10.ChartArea = "ChartArea1";
            series10.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series10.Color = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            series10.Legend = "Legend1";
            series10.Name = "LastVelocityVal";
            this.VelocityChartY.Series.Add(series9);
            this.VelocityChartY.Series.Add(series10);
            this.VelocityChartY.Size = new System.Drawing.Size(723, 307);
            this.VelocityChartY.TabIndex = 32;
            this.VelocityChartY.Text = "chart1";
            // 
            // GainChartY
            // 
            chartArea6.AxisX.Title = "Speed (counts/ms)";
            chartArea6.AxisY.Title = "Slope of Velocity Chart";
            chartArea6.Name = "ChartArea1";
            this.GainChartY.ChartAreas.Add(chartArea6);
            legend6.Name = "Legend1";
            this.GainChartY.Legends.Add(legend6);
            this.GainChartY.Location = new System.Drawing.Point(1062, 647);
            this.GainChartY.Name = "GainChartY";
            series11.ChartArea = "ChartArea1";
            series11.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series11.Legend = "Legend1";
            series11.Name = "Velocity Gain";
            series12.ChartArea = "ChartArea1";
            series12.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series12.Color = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            series12.Legend = "Legend1";
            series12.Name = "LastGainVal";
            this.GainChartY.Series.Add(series11);
            this.GainChartY.Series.Add(series12);
            this.GainChartY.Size = new System.Drawing.Size(723, 309);
            this.GainChartY.TabIndex = 33;
            this.GainChartY.Text = "chart1";
            // 
            // MouseLabel
            // 
            this.MouseLabel.AutoSize = true;
            this.MouseLabel.Location = new System.Drawing.Point(1, 24);
            this.MouseLabel.Name = "MouseLabel";
            this.MouseLabel.Size = new System.Drawing.Size(80, 13);
            this.MouseLabel.TabIndex = 34;
            this.MouseLabel.Text = "Last (x, y): (x, y)";
            // 
            // ActiveValueTitle
            // 
            this.ActiveValueTitle.AutoSize = true;
            this.ActiveValueTitle.Location = new System.Drawing.Point(248, 30);
            this.ActiveValueTitle.Name = "ActiveValueTitle";
            this.ActiveValueTitle.Size = new System.Drawing.Size(67, 13);
            this.ActiveValueTitle.TabIndex = 35;
            this.ActiveValueTitle.Text = "Active Value";
            // 
            // SensitivityActiveXLabel
            // 
            this.SensitivityActiveXLabel.AutoSize = true;
            this.SensitivityActiveXLabel.Location = new System.Drawing.Point(258, 49);
            this.SensitivityActiveXLabel.Name = "SensitivityActiveXLabel";
            this.SensitivityActiveXLabel.Size = new System.Drawing.Size(14, 13);
            this.SensitivityActiveXLabel.TabIndex = 36;
            this.SensitivityActiveXLabel.Text = "X";
            // 
            // SensitivityActiveYLabel
            // 
            this.SensitivityActiveYLabel.AutoSize = true;
            this.SensitivityActiveYLabel.Location = new System.Drawing.Point(286, 50);
            this.SensitivityActiveYLabel.Name = "SensitivityActiveYLabel";
            this.SensitivityActiveYLabel.Size = new System.Drawing.Size(14, 13);
            this.SensitivityActiveYLabel.TabIndex = 37;
            this.SensitivityActiveYLabel.Text = "Y";
            // 
            // RotationActiveLabel
            // 
            this.RotationActiveLabel.AutoSize = true;
            this.RotationActiveLabel.Location = new System.Drawing.Point(268, 75);
            this.RotationActiveLabel.Name = "RotationActiveLabel";
            this.RotationActiveLabel.Size = new System.Drawing.Size(13, 13);
            this.RotationActiveLabel.TabIndex = 38;
            this.RotationActiveLabel.Text = "0";
            // 
            // AccelTypeActiveLabel
            // 
            this.AccelTypeActiveLabel.AutoSize = true;
            this.AccelTypeActiveLabel.Location = new System.Drawing.Point(258, 98);
            this.AccelTypeActiveLabel.Name = "AccelTypeActiveLabel";
            this.AccelTypeActiveLabel.Size = new System.Drawing.Size(41, 13);
            this.AccelTypeActiveLabel.TabIndex = 39;
            this.AccelTypeActiveLabel.Text = "Default";
            // 
            // AccelerationActiveLabel
            // 
            this.AccelerationActiveLabel.AutoSize = true;
            this.AccelerationActiveLabel.Location = new System.Drawing.Point(268, 128);
            this.AccelerationActiveLabel.Name = "AccelerationActiveLabel";
            this.AccelerationActiveLabel.Size = new System.Drawing.Size(13, 13);
            this.AccelerationActiveLabel.TabIndex = 40;
            this.AccelerationActiveLabel.Text = "0";
            // 
            // CapActiveXLabel
            // 
            this.CapActiveXLabel.AutoSize = true;
            this.CapActiveXLabel.Location = new System.Drawing.Point(259, 151);
            this.CapActiveXLabel.Name = "CapActiveXLabel";
            this.CapActiveXLabel.Size = new System.Drawing.Size(13, 13);
            this.CapActiveXLabel.TabIndex = 41;
            this.CapActiveXLabel.Text = "0";
            // 
            // WeightActiveXLabel
            // 
            this.WeightActiveXLabel.AutoSize = true;
            this.WeightActiveXLabel.Location = new System.Drawing.Point(259, 180);
            this.WeightActiveXLabel.Name = "WeightActiveXLabel";
            this.WeightActiveXLabel.Size = new System.Drawing.Size(13, 13);
            this.WeightActiveXLabel.TabIndex = 42;
            this.WeightActiveXLabel.Text = "0";
            // 
            // WeightActiveYLabel
            // 
            this.WeightActiveYLabel.AutoSize = true;
            this.WeightActiveYLabel.Location = new System.Drawing.Point(286, 180);
            this.WeightActiveYLabel.Name = "WeightActiveYLabel";
            this.WeightActiveYLabel.Size = new System.Drawing.Size(13, 13);
            this.WeightActiveYLabel.TabIndex = 43;
            this.WeightActiveYLabel.Text = "0";
            // 
            // CapActiveYLabel
            // 
            this.CapActiveYLabel.AutoSize = true;
            this.CapActiveYLabel.Location = new System.Drawing.Point(286, 151);
            this.CapActiveYLabel.Name = "CapActiveYLabel";
            this.CapActiveYLabel.Size = new System.Drawing.Size(13, 13);
            this.CapActiveYLabel.TabIndex = 44;
            this.CapActiveYLabel.Text = "0";
            // 
            // OffsetActiveLabel
            // 
            this.OffsetActiveLabel.AutoSize = true;
            this.OffsetActiveLabel.Location = new System.Drawing.Point(268, 206);
            this.OffsetActiveLabel.Name = "OffsetActiveLabel";
            this.OffsetActiveLabel.Size = new System.Drawing.Size(13, 13);
            this.OffsetActiveLabel.TabIndex = 45;
            this.OffsetActiveLabel.Text = "0";
            // 
            // LimitExpActiveLabel
            // 
            this.LimitExpActiveLabel.AutoSize = true;
            this.LimitExpActiveLabel.Location = new System.Drawing.Point(268, 232);
            this.LimitExpActiveLabel.Name = "LimitExpActiveLabel";
            this.LimitExpActiveLabel.Size = new System.Drawing.Size(13, 13);
            this.LimitExpActiveLabel.TabIndex = 46;
            this.LimitExpActiveLabel.Text = "0";
            // 
            // MidpointActiveLabel
            // 
            this.MidpointActiveLabel.AutoSize = true;
            this.MidpointActiveLabel.Location = new System.Drawing.Point(268, 255);
            this.MidpointActiveLabel.Name = "MidpointActiveLabel";
            this.MidpointActiveLabel.Size = new System.Drawing.Size(13, 13);
            this.MidpointActiveLabel.TabIndex = 47;
            this.MidpointActiveLabel.Text = "0";
            // 
            // offsetStyleToolStripMenuItem
            // 
            this.offsetStyleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gainOffsetToolStripMenuItem,
            this.legacyOffsetToolStripMenuItem});
            this.offsetStyleToolStripMenuItem.Name = "offsetStyleToolStripMenuItem";
            this.offsetStyleToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.offsetStyleToolStripMenuItem.Text = "Offset Style";
            // 
            // gainOffsetToolStripMenuItem
            // 
            this.gainOffsetToolStripMenuItem.Name = "gainOffsetToolStripMenuItem";
            this.gainOffsetToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.gainOffsetToolStripMenuItem.Text = "Gain (Default)";
            // 
            // legacyOffsetToolStripMenuItem
            // 
            this.legacyOffsetToolStripMenuItem.Name = "legacyOffsetToolStripMenuItem";
            this.legacyOffsetToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.legacyOffsetToolStripMenuItem.Text = "Legacy";
            // 
            // RawAcceleration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1786, 958);
            this.Controls.Add(this.MidpointActiveLabel);
            this.Controls.Add(this.LimitExpActiveLabel);
            this.Controls.Add(this.OffsetActiveLabel);
            this.Controls.Add(this.CapActiveYLabel);
            this.Controls.Add(this.WeightActiveYLabel);
            this.Controls.Add(this.WeightActiveXLabel);
            this.Controls.Add(this.CapActiveXLabel);
            this.Controls.Add(this.AccelerationActiveLabel);
            this.Controls.Add(this.AccelTypeActiveLabel);
            this.Controls.Add(this.RotationActiveLabel);
            this.Controls.Add(this.SensitivityActiveYLabel);
            this.Controls.Add(this.SensitivityActiveXLabel);
            this.Controls.Add(this.ActiveValueTitle);
            this.Controls.Add(this.MouseLabel);
            this.Controls.Add(this.GainChartY);
            this.Controls.Add(this.VelocityChartY);
            this.Controls.Add(this.AccelerationChartY);
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
            this.Controls.Add(this.menuStrip1);
            this.Name = "RawAcceleration";
            this.Text = "Raw Acceleration Graph";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.RawAcceleration_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.AccelerationChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VelocityChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GainChart)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AccelerationChartY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VelocityChartY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GainChartY)).EndInit();
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
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem graphsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showVelocityGainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem advancedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem capStyleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gainCapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem legacyCapToolStripMenuItem;
        private System.Windows.Forms.DataVisualization.Charting.Chart AccelerationChartY;
        private System.Windows.Forms.DataVisualization.Charting.Chart VelocityChartY;
        private System.Windows.Forms.DataVisualization.Charting.Chart GainChartY;
        private System.Windows.Forms.Label MouseLabel;
        private System.Windows.Forms.ToolStripMenuItem scaleByDPIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dPIToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox DPITextBox;
        private System.Windows.Forms.ToolStripMenuItem pollRateToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox PollRateTextBox;
        private System.Windows.Forms.ToolStripMenuItem ScaleMenuItem;
        private System.Windows.Forms.Label ActiveValueTitle;
        private System.Windows.Forms.Label SensitivityActiveXLabel;
        private System.Windows.Forms.Label SensitivityActiveYLabel;
        private System.Windows.Forms.Label RotationActiveLabel;
        private System.Windows.Forms.Label AccelTypeActiveLabel;
        private System.Windows.Forms.Label AccelerationActiveLabel;
        private System.Windows.Forms.Label CapActiveXLabel;
        private System.Windows.Forms.Label WeightActiveXLabel;
        private System.Windows.Forms.Label WeightActiveYLabel;
        private System.Windows.Forms.Label CapActiveYLabel;
        private System.Windows.Forms.Label OffsetActiveLabel;
        private System.Windows.Forms.Label LimitExpActiveLabel;
        private System.Windows.Forms.Label MidpointActiveLabel;
        private System.Windows.Forms.ToolStripMenuItem startupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AutoWriteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem offsetStyleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gainOffsetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem legacyOffsetToolStripMenuItem;
    }
}

