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

        public struct SimulatedMouseInput
        {
            public double velocity;
            public double time;
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

        public ReadOnlyCollection<SimulatedMouseInput> SimulatedInputCombined { get; private set; }

        public ReadOnlyCollection<SimulatedMouseInput> SimulatedInputX { get; private set; }

        public ReadOnlyCollection<SimulatedMouseInput> SimulatedInputY { get; private set; }

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

        public void Calculate(AccelChartData data, ManagedAccel accel, double starter, ICollection<SimulatedMouseInput> simulatedInputData)
        {
            double lastInputMagnitude = 0;
            double lastOutputMagnitude = 0;

            double maxRatio = 0.0;
            double minRatio = Double.MaxValue;
            double maxSlope = 0.0;
            double minSlope = Double.MaxValue;

            double log = -2;
            int index = 0;
            int logIndex = 0;

            foreach (var simulatedInputDatum in simulatedInputData)
            {
                if (simulatedInputDatum.velocity <= 0)
                {
                    continue;
                }

                var output = accel.Accelerate(simulatedInputDatum.x, simulatedInputDatum.y, simulatedInputDatum.time);
                var outMagnitude = Magnitude(output.Item1, output.Item2);

                if (!data.VelocityPoints.ContainsKey(simulatedInputDatum.velocity))
                {
                    data.VelocityPoints.Add(simulatedInputDatum.velocity, outMagnitude);
                }
                else
                {
                    continue;
                }

                while (Math.Pow(10,log) < outMagnitude && logIndex < data.LogToIndex.Length)
                {
                    data.LogToIndex[logIndex] = index;
                    log += 0.01;
                    logIndex++;
                }

                var ratio = outMagnitude / simulatedInputDatum.velocity;
                
                if (ratio > maxRatio)
                {
                    maxRatio = ratio;
                }

                if (ratio < minRatio)
                {
                    minRatio = ratio;
                }

                var inDiff = simulatedInputDatum.velocity - lastInputMagnitude;
                var outDiff = outMagnitude - lastOutputMagnitude;
                var slope = inDiff > 0 ? outDiff / inDiff : starter;

                if (slope > maxSlope)
                {
                    maxSlope = slope;
                }

                if (slope < minSlope)
                {
                    minSlope = slope;
                }

                if (!data.AccelPoints.ContainsKey(simulatedInputDatum.velocity))
                {
                    data.AccelPoints.Add(simulatedInputDatum.velocity, ratio);
                }

                if (!data.GainPoints.ContainsKey(simulatedInputDatum.velocity))
                {
                    data.GainPoints.Add(simulatedInputDatum.velocity, slope);
                }

                lastInputMagnitude = simulatedInputDatum.velocity;
                lastOutputMagnitude = outMagnitude;
                index += 1;
            }

            index--;

            while (log <= 5.0)
            {
                data.LogToIndex[logIndex] = index;
                log += 0.01;
                logIndex++;
            }

            data.MaxAccel = maxRatio;
            data.MinAccel = minRatio;
            data.MaxGain = maxSlope;
            data.MinGain = minSlope;
        }

        public void CalculateCombinedDiffSens(AccelData data, ManagedAccel accel, DriverSettings settings, ICollection<SimulatedMouseInput> simulatedInputData)
        {
            double lastInputMagnitude = 0;
            double lastOutputMagnitudeX = 0;
            double lastOutputMagnitudeY = 0;

            double maxRatio = 0.0;
            double minRatio = Double.MaxValue;
            double maxSlope = 0.0;
            double minSlope = Double.MaxValue;


            Sensitivity = GetSens(ref settings);

            double log = -2;
            int index = 0;
            int logIndex = 0;

            foreach (var simulatedInputDatum in simulatedInputData)
            {
                if (simulatedInputDatum.velocity <= 0)
                {
                    continue;
                }

                var output = accel.Accelerate(simulatedInputDatum.x, simulatedInputDatum.y, simulatedInputDatum.time);
                var outputWithoutSens = StripThisSens(output.Item1, output.Item2);
                var magnitudeWithoutSens = Magnitude(outputWithoutSens.Item1, outputWithoutSens.Item2);

                var ratio = magnitudeWithoutSens / simulatedInputDatum.velocity;

                if (!data.Combined.VelocityPoints.ContainsKey(simulatedInputDatum.velocity))
                {
                    data.Combined.VelocityPoints.Add(simulatedInputDatum.velocity, magnitudeWithoutSens);
                }
                else
                {
                    continue;
                }

                while (Math.Pow(10,log) < magnitudeWithoutSens && logIndex < data.Combined.LogToIndex.Length)
                {
                    data.Combined.LogToIndex[logIndex] = index;
                    log += 0.01;
                    logIndex++;
                }

                var xRatio = settings.sensitivity.x * ratio;
                var yRatio = settings.sensitivity.y * ratio;

                if (xRatio > maxRatio)
                {
                    maxRatio = xRatio;
                }

                if (xRatio < minRatio)
                {
                    minRatio = xRatio;
                }

                if (yRatio > maxRatio)
                {
                    maxRatio = yRatio;
                }

                if (yRatio < minRatio)
                {
                    minRatio = yRatio;
                }

                if (!data.X.AccelPoints.ContainsKey(simulatedInputDatum.velocity))
                {
                    data.X.AccelPoints.Add(simulatedInputDatum.velocity, xRatio);
                }

                if (!data.Y.AccelPoints.ContainsKey(simulatedInputDatum.velocity))
                {
                    data.Y.AccelPoints.Add(simulatedInputDatum.velocity, yRatio);
                }

                var xOut = xRatio * simulatedInputDatum.velocity;
                var yOut = yRatio * simulatedInputDatum.velocity;

                var inDiff = simulatedInputDatum.velocity - lastInputMagnitude;
                var xOutDiff = xOut - lastOutputMagnitudeX;
                var yOutDiff = yOut - lastOutputMagnitudeY;
                var xSlope = inDiff > 0 ? xOutDiff / inDiff : settings.sensitivity.x;
                var ySlope = inDiff > 0 ? yOutDiff / inDiff : settings.sensitivity.y;

                if (xSlope > maxSlope)
                {
                    maxSlope = xSlope;
                }

                if (xSlope < minSlope)
                {
                    minSlope = xSlope;
                }

                if (ySlope > maxSlope)
                {
                    maxSlope = ySlope;
                }

                if (ySlope < minSlope)
                {
                    minSlope = ySlope;
                }

                if (!data.X.VelocityPoints.ContainsKey(simulatedInputDatum.velocity))
                {
                    data.X.VelocityPoints.Add(simulatedInputDatum.velocity, xOut);
                }

                if (!data.Y.VelocityPoints.ContainsKey(simulatedInputDatum.velocity))
                {
                    data.Y.VelocityPoints.Add(simulatedInputDatum.velocity, yOut);
                }

                if (!data.X.GainPoints.ContainsKey(simulatedInputDatum.velocity))
                {
                    data.X.GainPoints.Add(simulatedInputDatum.velocity, xSlope);
                }

                if (!data.Y.GainPoints.ContainsKey(simulatedInputDatum.velocity))
                {
                    data.Y.GainPoints.Add(simulatedInputDatum.velocity, ySlope);
                }

                lastInputMagnitude = simulatedInputDatum.velocity;
                lastOutputMagnitudeX = xOut;
                lastOutputMagnitudeY = yOut;
                index += 1;
            }

            index--;

            while (log <= 5.0)
            {
                data.Combined.LogToIndex[logIndex] = index;
                log += 0.01;
                logIndex++;
            }

            data.Combined.MaxAccel = maxRatio;
            data.Combined.MinAccel = minRatio;
            data.Combined.MaxGain = maxSlope;
            data.Combined.MinGain = minSlope;
        }

        public ReadOnlyCollection<SimulatedMouseInput> GetSimulatedInput()
        {
            var magnitudes = new List<SimulatedMouseInput>();
            for (int i = 0; i < CombinedMaxVelocity; i+=Increment)
            {
                for (int j = 0; j <= i; j+=Increment)
                {
                    SimulatedMouseInput mouseInputData;
                    mouseInputData.x = i;
                    mouseInputData.y = j;
                    mouseInputData.time = MeasurementTime;
                    mouseInputData.velocity = Velocity(i, j, mouseInputData.time);
                    magnitudes.Add(mouseInputData);
                }
            }

            magnitudes.Sort((m1, m2) => m1.velocity.CompareTo(m2.velocity));

            return magnitudes.AsReadOnly();
        }

        public ReadOnlyCollection<SimulatedMouseInput> GetSimulatInputX()
        {
            var magnitudes = new List<SimulatedMouseInput>();

            for (int i = 0; i < XYMaxVelocity; i+=Increment)
            {
                SimulatedMouseInput mouseInputData;
                mouseInputData.x = i;
                mouseInputData.y = 0;
                mouseInputData.time = MeasurementTime;
                mouseInputData.velocity = Velocity(i, 0, mouseInputData.time);
                magnitudes.Add(mouseInputData);
            }

            return magnitudes.AsReadOnly();
        }

        public ReadOnlyCollection<SimulatedMouseInput> GetSimulatedInputY()
        {
            var magnitudes = new List<SimulatedMouseInput>();

            for (int i = 0; i < XYMaxVelocity; i+=Increment)
            {
                SimulatedMouseInput mouseInputData;
                mouseInputData.x = 0;
                mouseInputData.y = i;
                mouseInputData.time = MeasurementTime;
                mouseInputData.velocity = Velocity(0, i, mouseInputData.time);
                magnitudes.Add(mouseInputData);
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
            SimulatedInputCombined = GetSimulatedInput();
            SimulatedInputX = GetSimulatInputX();
            SimulatedInputY = GetSimulatedInputY();
        }

        #endregion Methods
    }
}
