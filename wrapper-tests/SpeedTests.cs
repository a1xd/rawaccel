using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
        public void Given_Input_Smooths()
        {
            var speedCalc = new SpeedCalculator();
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

            double expected = sum / 100;

            Assert.AreEqual(expected, speed);
        }

        public static double Magnitude (double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }
    }
}
