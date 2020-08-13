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
        public struct MagnitudeData
        {
            public double magnitude;
            public int x;
            public int y;
        }

        public static ReadOnlyCollection<MagnitudeData> Magnitudes = GetMagnitudes();

        public static void Calculate(AccelData data, ManagedAccel accel, double starter)
        {
            data.Clear();

            double lastInputMagnitude = 0;
            double lastOutputMagnitude = 0;

            foreach (var magnitudeData in Magnitudes)
            {
                var output = accel.Accelerate(magnitudeData.x, magnitudeData.y, 1);

                var outMagnitude = Magnitude(output.Item1, output.Item2);
                var ratio = magnitudeData.magnitude > 0 ? outMagnitude / magnitudeData.magnitude : starter;

                var inDiff = magnitudeData.magnitude - lastInputMagnitude;
                var outDiff = outMagnitude - lastOutputMagnitude;
                var slope = inDiff > 0 ? outDiff / inDiff : starter;

                if (!data.OrderedAccelPoints.ContainsKey(magnitudeData.magnitude))
                {
                    data.OrderedAccelPoints.Add(magnitudeData.magnitude, ratio);
                }

                if (!data.OrderedVelocityPoints.ContainsKey(magnitudeData.magnitude))
                {
                    data.OrderedVelocityPoints.Add(magnitudeData.magnitude, outMagnitude);
                }

                if (!data.OrderedGainPoints.ContainsKey(magnitudeData.magnitude))
                {
                    data.OrderedGainPoints.Add(magnitudeData.magnitude, slope);
                }

                lastInputMagnitude = magnitudeData.magnitude;
                lastOutputMagnitude = outMagnitude;
            }

        }

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

    }
}
