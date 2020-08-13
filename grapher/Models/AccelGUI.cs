using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace grapher
{
    public class AccelGUI
    {        
        public struct MagnitudeData
        {
            public double magnitude;
            public int x;
            public int y;
        }

        public static ReadOnlyCollection<MagnitudeData> Magnitudes = GetMagnitudes();

        #region constructors

        public AccelGUI(
            RawAcceleration accelForm,
            AccelCharts accelCharts,
            ManagedAccel managedAccel,
            AccelOptions accelOptions,
            OptionXY sensitivity,
            Option rotation,
            OptionXY weight,
            CapOptions cap,
            Option offset,
            Option acceleration,
            Option limtOrExp,
            Option midpoint,
            Button writeButton)
        {
            AccelForm = accelForm;
            AccelCharts = accelCharts;
            ManagedAcceleration = managedAccel;
            AccelerationOptions = accelOptions;
            Sensitivity = sensitivity;
            Rotation = rotation;
            Weight = weight;
            Cap = cap;
            Offset = offset;
            Acceleration = acceleration;
            LimitOrExponent = limtOrExp;
            Midpoint = midpoint;
            WriteButton = writeButton;

            OrderedAccelPoints = new SortedDictionary<double, double>();
            OrderedVelocityPoints = new SortedDictionary<double, double>();
            OrderedGainPoints = new SortedDictionary<double, double>();


            ManagedAcceleration.ReadFromDriver();
            UpdateGraph();
        }

        #endregion constructors

        #region properties

        public RawAcceleration AccelForm { get; }

        public AccelCharts AccelCharts { get; }

        public ManagedAccel ManagedAcceleration { get; }

        public AccelOptions AccelerationOptions { get; }

        public OptionXY Sensitivity { get; }

        public Option Rotation { get; }

        public OptionXY Weight { get; }

        public CapOptions Cap { get; }

        public Option Offset { get; }

        public Option Acceleration { get; }

        public Option LimitOrExponent { get; }

        public Option Midpoint { get; }

        public Button WriteButton { get; }

        public SortedDictionary<double, double> OrderedAccelPoints { get; }

        public SortedDictionary<double, double> OrderedVelocityPoints { get; }

        public SortedDictionary<double, double> OrderedGainPoints { get; }

        #endregion properties

        #region methods

        public static ReadOnlyCollection<MagnitudeData> GetMagnitudes()
        {
            var magnitudes = new List<MagnitudeData>();
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    MagnitudeData magnitudeData;
                    magnitudeData.magnitude = Magnitude(i, j);
                    magnitudeData.x = i;
                    magnitudeData.y = j;
                    magnitudes.Add(magnitudeData);
                }
            }

            magnitudes.Sort((m1, m2) => m1.magnitude.CompareTo(m2.magnitude));

            return magnitudes.AsReadOnly();
        }

        public static double Magnitude(int x, int y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        public static double Magnitude(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        public void UpdateGraph()
        {
            OrderedAccelPoints.Clear();
            OrderedVelocityPoints.Clear();
            OrderedGainPoints.Clear();

            double lastInputMagnitude = 0;
            double lastOutputMagnitude = 0;

            foreach (var magnitudeData in Magnitudes)
            {
                var output = ManagedAcceleration.Accelerate(magnitudeData.x, magnitudeData.y, 1);

                var outMagnitude = Magnitude(output.Item1, output.Item2);
                var ratio = magnitudeData.magnitude > 0 ? outMagnitude / magnitudeData.magnitude : Sensitivity.Fields.X;

                var inDiff = magnitudeData.magnitude - lastInputMagnitude;
                var outDiff = outMagnitude - lastOutputMagnitude;
                var slope = inDiff > 0 ? outDiff / inDiff : Sensitivity.Fields.X;

                if (!OrderedAccelPoints.ContainsKey(magnitudeData.magnitude))
                {
                    OrderedAccelPoints.Add(magnitudeData.magnitude, ratio);
                }

                if (!OrderedVelocityPoints.ContainsKey(magnitudeData.magnitude))
                {
                    OrderedVelocityPoints.Add(magnitudeData.magnitude, outMagnitude);
                }

                if (!OrderedGainPoints.ContainsKey(magnitudeData.magnitude))
                {
                    OrderedGainPoints.Add(magnitudeData.magnitude, slope);
                }

                lastInputMagnitude = magnitudeData.magnitude;
                lastOutputMagnitude = outMagnitude;
            }

            AccelCharts.SensitivityChart.ChartX.Series[0].Points.DataBindXY(OrderedAccelPoints.Keys, OrderedAccelPoints.Values);
            AccelCharts.VelocityChart.ChartX.Series[0].Points.DataBindXY(OrderedVelocityPoints.Keys, OrderedVelocityPoints.Values);
            AccelCharts.GainChart.ChartX.Series[0].Points.DataBindXY(OrderedGainPoints.Keys, OrderedGainPoints.Values);
        }

        #endregion methods
    }

}
