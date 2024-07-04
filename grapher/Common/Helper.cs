namespace grapher.Common
{
    public static class Helper
    {
        public static double GetSensitivityFactor(Profile profile) => GetSensitivityFactor(profile.outputDPI);

        public static double GetSensitivityFactor(double outputDPI) => outputDPI / Constants.DriverNormalizedDPI;

        public static double CalculatOutputDPI(double sensitivity) => sensitivity * Constants.DriverNormalizedDPI;
    }
}
