using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace wrapper_tests
{
    [TestClass]
    public class SynchronousAccelTests
    {
        [TestMethod]
        public void GivenSpeeds_SynchronousAccel_YieldsCorrectSens()
        {
            double syncSpeed = 20;
            double gamma = 0.5;
            double motivity = 1.3;
            double smooth = 0.5;

            var profile = new Profile();
            profile.outputDPI = 1000;
            profile.argsX.mode = AccelMode.synchronous;
            profile.argsX.gain = false;
            profile.argsX.syncSpeed = syncSpeed;
            profile.argsX.gamma = gamma;
            profile.argsX.motivity = motivity;
            profile.argsX.smooth = smooth;
            var accel = new ManagedAccel(profile);
            var accelSimulator = new SynchronousAccelSimulator(syncSpeed, motivity, gamma, smooth);

            List<int> inputs = new List<int>()
            {
                1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181,
            };

            foreach (int input in inputs)
            {
                Tuple<double, double> output = accel.Accelerate(input, 0, 1, 10);
                double expectedOutput = input * accelSimulator.Accelerate(input / 10.0);
                Assert.AreEqual(expectedOutput, output.Item1, expectedOutput * 0.0001);
            }
        }

        [TestMethod]
        public void GivenSpeeds_SynchronousAccelWithNonDefaultSmooth_YieldsCorrectSens()
        {
            double syncSpeed = 20;
            double gamma = 0.5;
            double motivity = 1.3;
            double smooth = 1.0;

            var profile = new Profile();
            profile.outputDPI = 1000;
            profile.argsX.mode = AccelMode.synchronous;
            profile.argsX.gain = false;
            profile.argsX.syncSpeed = syncSpeed;
            profile.argsX.gamma = gamma;
            profile.argsX.motivity = motivity;
            profile.argsX.smooth = smooth;
            var accel = new ManagedAccel(profile);
            var accelSimulator = new SynchronousAccelSimulator(syncSpeed, motivity, gamma, smooth);

            List<int> inputs = new List<int>()
            {
                1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181,
            };

            foreach (int input in inputs)
            {
                Tuple<double, double> output = accel.Accelerate(input, 0, 1, 10);
                double expectedOutput = input * accelSimulator.Accelerate(input / 10.0);
                Assert.AreEqual(expectedOutput, output.Item1, expectedOutput * 0.0001);
            }
        }

	}

    /// <summary>
    /// Contains definition of how synchronous accel is expected to accelerate inputs
    /// No optimization tricks are used for clarity of behavior.
    /// </summary>
    public class SynchronousAccelSimulator
    {
        public SynchronousAccelSimulator(
            double syncSpeed,
            double motivity,
            double gamma,
            double smooth)
        {
            SyncSpeed = syncSpeed;
            Motivity = motivity;
            Gamma = gamma;
            Sharpness = smooth <= 0 ? 16 : 0.5 / smooth;
        }

        public double SyncSpeed { get; }

        public double Motivity { get; }

        public double Gamma { get; }

        public double Sharpness { get; }

        public double Accelerate(double inputSpeed)
        {
            double logSpace = CalculateLogSpace(inputSpeed);
            double activation = ActivationFunction(logSpace);
            return Math.Pow(Motivity, activation);
        }

        public double CalculateLogSpace(double x)
        {
            double syncRatio = x / SyncSpeed;
            double logSpaceUnadjusted = Math.Log(syncRatio, Motivity);
            double gammaAdjusted = Gamma * logSpaceUnadjusted;
            return gammaAdjusted;
        }

        public double ActivationFunction(double x)
        {
            if (Sharpness >= 16)
            {
                return Math.Min(1, Math.Max(-1, x));
            }

            return Math.Sign(x) * Math.Pow(Math.Tanh(Math.Pow(Math.Abs(x), Sharpness)), 1 / Sharpness);
        }
    }
}