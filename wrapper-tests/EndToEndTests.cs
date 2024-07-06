using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace wrapper_tests
{
    [TestClass]
    public class EndToEndTests
    {
        [TestMethod]
        public void ModifyInput_Default_DoesNotChangeInput()
        {
            var accel = new ManagedAccel();
            (int x, int y) input = (1, 1);
            var output = accel.Accelerate(input.x, input.y, 1, 1);

            Assert.AreEqual(input.x, output.Item1);
            Assert.AreEqual(input.y, output.Item2);
        }

        [TestMethod]
        public void ModifyInput_WithOutputDPI_HasCorrectFactor()
        {
            double expectedFactor = 2;
            double expectedNormalizedDPI = 1000;
            (int x, int y) input = (1, 1);

            var profile = new Profile();
            profile.outputDPI = expectedFactor * expectedNormalizedDPI;
            var accel = new ManagedAccel(profile);

            var output = accel.Accelerate(input.x, input.y, 1, 1);

            (double x, double y) expectedOutput = (expectedFactor * input.x, expectedFactor * input.y);
            Assert.AreEqual(expectedOutput.x, output.Item1);
            Assert.AreEqual(expectedOutput.y, output.Item2);
        }
    }
}
