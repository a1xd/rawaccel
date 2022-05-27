using grapher.Models.Calculations;
using System.Windows.Forms;

namespace grapher.Models.Charts.ChartState
{
    public class ChartStateManager
    {
        public ChartStateManager(
            ChartXY sensitivityChart,
            ChartXY velocityChart,
            ChartXY gainChart,
            TableLayoutPanel chartContainer,
            AccelCalculator accelCalculator,
            EstimatedPoints combined,
            EstimatedPoints xPoints,
            EstimatedPoints yPoints)
        {
            CombinedState = new CombinedState(
                sensitivityChart,
                velocityChart,
                gainChart,
                chartContainer,
                combined,
                accelCalculator);

            XYOneGraphState = new XYOneGraphState(
                sensitivityChart,
                velocityChart,
                gainChart,
                chartContainer,
                xPoints,
                yPoints,
                accelCalculator);

            XYTwoGraphState = new XYTwoGraphState(
                sensitivityChart,
                velocityChart,
                gainChart,
                chartContainer,
                xPoints,
                yPoints,
                accelCalculator);
        }

        private CombinedState CombinedState { get; }

        private XYOneGraphState XYOneGraphState { get; }

        private XYTwoGraphState XYTwoGraphState { get; }


        public ChartState DetermineState(Profile settings)
        {
            ChartState chartState;

            if (settings.combineMagnitudes)
            {
                if (settings.yxSensRatio != 1 ||
                    settings.domainXY.x != settings.domainXY.y ||
                    settings.rangeXY.x != settings.rangeXY.y)
                {
                    chartState = XYOneGraphState;
                }
                else
                {
                    chartState = CombinedState;
                }

                chartState.ChartContainer.ColumnCount = Constants.CombinedChartColumnCount;
                chartState.ChartContainer.ColumnStyles[0].Width = Constants.CombinedChartColumnWidth;
            }
            else
            {
                chartState = XYTwoGraphState;
                chartState.ChartContainer.ColumnCount = Constants.SeparateChartColumnCount;
                chartState.ChartContainer.ColumnStyles[0].Width = Constants.SeparateChartColumnWidth;
                chartState.ChartContainer.ColumnStyles[1].Width = Constants.SeparateChartColumnWidth;
            }

            chartState.Settings = settings;
            return chartState;
        }

        public ChartState InitialState()
        {
            return CombinedState;
        }
    }
}
