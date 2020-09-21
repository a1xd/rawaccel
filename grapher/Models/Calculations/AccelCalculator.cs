using grapher.Models.Serialized;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
        
        private double MeasurementTime { get; set; }

        private (double, double) RotationVector { get; set; } 

        private (double, double) Sensitivity { get; set; }

        #endregion Fields

        #region Methods

        public void Calculate(AccelChartData data, ManagedAccel accel, double starter, ICollection<MagnitudeData> magnitudeData)
        {
            double lastInputMagnitude = 0;
            double lastOutputMagnitude = 0;

            foreach (var magnitudeDatum in magnitudeData)
            {
                var output = accel.Accelerate(magnitudeDatum.x, magnitudeDatum.y, MeasurementTime);
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

        public void CalculateCombinedDiffSens(AccelData data, ManagedAccel accel, DriverSettings settings, ICollection<MagnitudeData> magnitudeData)
        {
            double lastInputMagnitude = 0;
            double lastOutputMagnitudeX = 0;
            double lastOutputMagnitudeY = 0;

            Sensitivity = GetSens(ref settings);

            foreach (var magnitudeDatum in magnitudeData)
            {
                var output = accel.Accelerate(magnitudeDatum.x, magnitudeDatum.y, MeasurementTime);
                var outputWithoutSens = StripThisSens(output.Item1, output.Item2);
                var magnitudeWithoutSens = Magnitude(outputWithoutSens.Item1, outputWithoutSens.Item2);

                var ratio = magnitudeDatum.magnitude > 0 ? magnitudeWithoutSens / magnitudeDatum.magnitude : 1;

                if (!data.Combined.VelocityPoints.ContainsKey(magnitudeDatum.magnitude))
                {
                    data.Combined.VelocityPoints.Add(magnitudeDatum.magnitude, magnitudeWithoutSens);
                }

                var xRatio = settings.sensitivity.x * ratio;
                var yRatio = settings.sensitivity.y * ratio;

                if (!data.X.AccelPoints.ContainsKey(magnitudeDatum.magnitude))
                {
                    data.X.AccelPoints.Add(magnitudeDatum.magnitude, xRatio);
                }

                if (!data.Y.AccelPoints.ContainsKey(magnitudeDatum.magnitude))
                {
                    data.Y.AccelPoints.Add(magnitudeDatum.magnitude, yRatio);
                }

                var xOut = xRatio * magnitudeDatum.magnitude;
                var yOut = yRatio * magnitudeDatum.magnitude;

                var inDiff = magnitudeDatum.magnitude - lastInputMagnitude;
                var xOutDiff = xOut - lastOutputMagnitudeX;
                var yOutDiff = yOut - lastOutputMagnitudeY;
                var xSlope = inDiff > 0 ? xOutDiff / inDiff : settings.sensitivity.x;
                var ySlope = inDiff > 0 ? yOutDiff / inDiff : settings.sensitivity.y;

                if (!data.X.VelocityPoints.ContainsKey(magnitudeDatum.magnitude))
                {
                    data.X.VelocityPoints.Add(magnitudeDatum.magnitude, xOut);
                }

                if (!data.Y.VelocityPoints.ContainsKey(magnitudeDatum.magnitude))
                {
                    data.Y.VelocityPoints.Add(magnitudeDatum.magnitude, yOut);
                }

                if (!data.X.GainPoints.ContainsKey(magnitudeDatum.magnitude))
                {
                    data.X.GainPoints.Add(magnitudeDatum.magnitude, xSlope);
                }

                if (!data.Y.GainPoints.ContainsKey(magnitudeDatum.magnitude))
                {
                    data.Y.GainPoints.Add(magnitudeDatum.magnitude, ySlope);
                }

                lastInputMagnitude = magnitudeDatum.magnitude;
                lastOutputMagnitudeX = xOut;
                lastOutputMagnitudeY = yOut;
            }

            data.Combined.OrderedVelocityPointsList.AddRange(data.Combined.VelocityPoints.Values.ToList());
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

        public static bool ShouldStripSens(ref DriverSettings settings) =>
            settings.sensitivity.x != settings.sensitivity.y;

        public static bool ShouldStripRot(ref DriverSettings settings) =>
            settings.rotation > 0;

        public static (double, double) GetSens(ref DriverSettings settings) =>
            (settings.sensitivity.x, settings.sensitivity.y);

        public static (double, double) GetRotVector(ref DriverSettings settings) =>
            (Math.Cos(settings.rotation), Math.Sin(settings.rotation));

        public static (double, double) StripSens(double outputX, double outputY, double sensitivityX, double sensitivityY) =>
            (outputX / sensitivityX, outputY / sensitivityY);

        public (double, double) StripRot(double outputX, double outputY, double rotX, double rotY) =>
            (outputX * rotX + outputY * rotY, outputX * rotY - outputY * rotX);

        public (double, double) StripThisSens(double outputX, double outputY) =>
            StripSens(outputX, outputY, Sensitivity.Item1, Sensitivity.Item2);

        public (double, double) StripThisRot(double outputX, double outputY) =>
            StripRot(outputX, outputY, RotationVector.Item1, RotationVector.Item2);

        public void ScaleByMouseSettings()
        {
            var dpiPollFactor = DPI.Data / PollRate.Data;
            CombinedMaxVelocity = dpiPollFactor * Constants.MaxMultiplier;
            var ratio = CombinedMaxVelocity / Constants.Resolution;
            Increment = ratio > 1 ? (int) Math.Floor(ratio) : 1;
            MeasurementTime = Increment == 1 ? 1 / ratio : 1;
            XYMaxVelocity = CombinedMaxVelocity * Constants.XYToCombinedRatio;
            MagnitudesCombined = GetMagnitudes();
            MagnitudesX = GetMagnitudesX();
            MagnitudesY = GetMagnitudesY();
        }

        #endregion Methods
    }
}
