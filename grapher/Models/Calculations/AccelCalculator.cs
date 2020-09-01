using grapher.Constants;
using grapher.Models.Serialized;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher.Models.Calculations
{
    public class AccelCalculator
    {
        #region Structs

        public struct MagnitudeData
        {
            public double magnitude;
            public int x;
            public int y;
        }

        #endregion Structs

        #region Constructors

        public AccelCalculator(Field dpi, Field pollRate)
        {
            DPI = dpi;
            PollRate = pollRate;
        }

        #endregion Constructors

        #region Properties

        public ReadOnlyCollection<MagnitudeData> MagnitudesCombined { get; private set; }

        public ReadOnlyCollection<MagnitudeData> MagnitudesX { get; private set; }

        public ReadOnlyCollection<MagnitudeData> MagnitudesY { get; private set; }

        public Field DPI { get; private set; }

        public Field PollRate { get; private set; }

        private double CombinedMaxVelocity { get; set; }

        private double XYMaxVelocity { get; set; }

        private int Increment { get; set; }

        #endregion Fields

        #region Methods

        public void Calculate(AccelData data, ManagedAccel accel, DriverSettings settings)
        {
            ScaleByMouseSettings();

            data.Clear();

            Calculate(data.Combined, accel, settings.sensitivity.x, MagnitudesCombined);
            Calculate(data.X, accel, settings.sensitivity.x, MagnitudesX);
            Calculate(data.Y, accel, settings.sensitivity.y, MagnitudesY);
        }

        public static void Calculate(AccelChartData data, ManagedAccel accel, double starter, ICollection<MagnitudeData> magnitudeData)
        {
            double lastInputMagnitude = 0;
            double lastOutputMagnitude = 0;

            foreach (var magnitudeDatum in magnitudeData)
            {
                var output = accel.Accelerate(magnitudeDatum.x, magnitudeDatum.y, 1);

                var outMagnitude = Magnitude(output.Item1, output.Item2);
                var ratio = magnitudeDatum.magnitude > 0 ? outMagnitude / magnitudeDatum.magnitude : starter;

                var inDiff = magnitudeDatum.magnitude - lastInputMagnitude;
                var outDiff = outMagnitude - lastOutputMagnitude;
                var slope = inDiff > 0 ? outDiff / inDiff : starter;

                if (!data.AccelPoints.ContainsKey(magnitudeDatum.magnitude))
                {
                    data.AccelPoints.Add(magnitudeDatum.magnitude, ratio);
                }

                if (!data.VelocityPoints.ContainsKey(magnitudeDatum.magnitude))
                {
                    data.VelocityPoints.Add(magnitudeDatum.magnitude, outMagnitude);
                }

                if (!data.GainPoints.ContainsKey(magnitudeDatum.magnitude))
                {
                    data.GainPoints.Add(magnitudeDatum.magnitude, slope);
                }

                lastInputMagnitude = magnitudeDatum.magnitude;
                lastOutputMagnitude = outMagnitude;
            }

            data.OrderedVelocityPointsList.AddRange(data.VelocityPoints.Values.ToList());
        }

        public ReadOnlyCollection<MagnitudeData> GetMagnitudes()
        {
            var magnitudes = new List<MagnitudeData>();
            for (int i = 0; i < CombinedMaxVelocity; i+=Increment)
            {
                for (int j = 0; j <= i; j+=Increment)
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

        public ReadOnlyCollection<MagnitudeData> GetMagnitudesX()
        {
            var magnitudes = new List<MagnitudeData>();

            for (int i = 0; i < XYMaxVelocity; i+=Increment)
            {
                MagnitudeData magnitudeData;
                magnitudeData.magnitude = i;
                magnitudeData.x = i;
                magnitudeData.y = 0;
                magnitudes.Add(magnitudeData);
            }

            return magnitudes.AsReadOnly();
        }

        public ReadOnlyCollection<MagnitudeData> GetMagnitudesY()
        {
            var magnitudes = new List<MagnitudeData>();

            for (int i = 0; i < XYMaxVelocity; i+=Increment)
            {
                MagnitudeData magnitudeData;
                magnitudeData.magnitude = i;
                magnitudeData.x = 0;
                magnitudeData.y = i;
                magnitudes.Add(magnitudeData);
            }

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

        public static double Velocity(int x, int y, double time)
        {
            return Magnitude(x, y) / time;
        }

        public static double Velocity(double x, double y, double time)
        {
            return Magnitude(x, y) / time;
        }

        public void ScaleByMouseSettings()
        {
            var dpiPollFactor = DPI.Data / PollRate.Data;
            CombinedMaxVelocity = dpiPollFactor * AccelGUIConstants.MaxMultiplier;
            Increment = (int) Math.Floor(CombinedMaxVelocity / AccelGUIConstants.Resolution);
            XYMaxVelocity = CombinedMaxVelocity * AccelGUIConstants.XYToCombinedRatio;
            MagnitudesCombined = GetMagnitudes();
            MagnitudesX = GetMagnitudesX();
            MagnitudesY = GetMagnitudesY();
        }

        #endregion Methods
    }
}
