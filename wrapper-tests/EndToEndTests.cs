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
    }
}
