using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Models.Calculations
{
    public static class AccelCalculator
    {
        public const int MaxCombined = 100;
        public const int MaxXY = 150;

        public struct MagnitudeData
        {
            public double magnitude;
            public int x;
            public int y;
        }

        public static ReadOnlyCollection<MagnitudeData> MagnitudesCombined = GetMagnitudes();
        public static ReadOnlyCollection<MagnitudeData> MagnitudesX = GetMagnitudesX();
        public static ReadOnlyCollection<MagnitudeData> MagnitudesY = GetMagnitudesY();

        public static void Calculate(AccelData data, ManagedAccel accel, double starter)
        {
            data.Clear();

            Calculate(data.Combined, accel, starter, MagnitudesCombined);
            Calculate(data.X, accel, starter, MagnitudesX);
            Calculate(data.Y, accel, starter, MagnitudesY);
        }

        public static void Calculate(AccelChartData data, ManagedAccel accel, double starter, ICollection<MagnitudeData> magnitudeData)
        {
            double lastInputMagnitude = 0;
            double lastOutputMagnitude = 0;

            foreach (var magnitudeDatum in MagnitudesCombined)
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
        }

        public static ReadOnlyCollection<MagnitudeData> GetMagnitudes()
        {
            var magnitudes = new List<MagnitudeData>();
            for (int i = 0; i < MaxCombined; i++)
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

        public static ReadOnlyCollection<MagnitudeData> GetMagnitudesX()
        {
            var magnitudes = new List<MagnitudeData>();

            for (int i = 0; i < MaxXY; i++)
            {
                MagnitudeData magnitudeData;
                magnitudeData.magnitude = i;
                magnitudeData.x = i;
                magnitudeData.y = 0;
                magnitudes.Add(magnitudeData);
            }

            return magnitudes.AsReadOnly();
        }

        public static ReadOnlyCollection<MagnitudeData> GetMagnitudesY()
        {
            var magnitudes = new List<MagnitudeData>();

            for (int i = 0; i < MaxXY; i++)
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

    }
}
