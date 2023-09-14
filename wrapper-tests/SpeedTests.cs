using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace wrapper_tests
{
    [TestClass]
    public class SpeedTests
    {
        [TestMethod]
        public void Given_ZeroVector_ReturnsZero()
        {
            var speedCalc = new SpeedCalculator();
            var speed = speedCalc.CalculateSpeed(0, 0, 1);
            Assert.AreEqual(0, speed);
        }
        
        [TestMethod]
        public void Given_Input_DefaultCalculator_ReturnsUnsmoothed()
        {
            var speedCalc = new SpeedCalculator();
            var inputs = new[]
            {
                (0,0),
                (1,1),
                (2,2),
                (3,3),
            };

            double timePerInput = 1;
            double speed = 0;
            double sum = 0;

            foreach (var input in inputs)
            {
                speed = speedCalc.CalculateSpeed(input.Item1, input.Item2, timePerInput);
                sum += Magnitude(input.Item1, input.Item2);
            }

            double expected = Magnitude(inputs[inputs.Length-1].Item1, inputs[inputs.Length-1].Item2) / timePerInput;

            Assert.AreEqual(expected, speed, 0.0001);
        }

        [TestMethod]
        public void Given_Input_SmoothCalculator_Smooths()
        {
            double smooth_window = 100;

            var speedArgs = new SpeedCalculatorArgs(
                lp_norm: 2,
                should_smooth: true,
                smooth_window: smooth_window,
                should_cutoff: false,
                cutoff_window: 0);

            var speedCalc = new SpeedCalculator();
            speedCalc.Init(speedArgs);

            var inputs = new[]
            {
                (0,0),
                (1,1),
                (2,2),
                (3,3),
            };

            double speed = 0;
            double sum = 0;

            foreach (var input in inputs)
            {
                speed = speedCalc.CalculateSpeed(input.Item1, input.Item2, 1);
                sum += Magnitude(input.Item1, input.Item2);
            }

            double expected = sum / smooth_window;

            Assert.AreEqual(expected, speed, 0.0001);
        }

        [TestMethod]
        public void Given_InputWithCutoff_SmoothCalculator_Smooths()
        {
            double smooth_window = 100;
            double cutoff_window = 10;

            var speedArgs = new SpeedCalculatorArgs(
                lp_norm: 2,
                should_smooth: true,
                smooth_window: smooth_window,
                should_cutoff: true,
                cutoff_window: cutoff_window);

            var speedCalc = new SpeedCalculator();
            speedCalc.Init(speedArgs);

            var inputs = new List<(int, int)>();
            double cutoff_sum = 0;

            for (int i = 100; i > 0; i--)
            {
                inputs.Add((i, i));

                if (i <= (cutoff_window + 1))
                {
                    cutoff_sum += Magnitude(i, i);
                }
            }

            double speed = 0;

            foreach (var input in inputs)
            {
                speed = speedCalc.CalculateSpeed(input.Item1, input.Item2, 1);
            }

            double expected = cutoff_sum / cutoff_window;

            Assert.AreEqual(expected, speed, 0.0001);
        }

        public static double Magnitude (double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }
    }
}
