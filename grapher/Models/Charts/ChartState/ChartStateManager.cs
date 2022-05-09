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

                chartState.ChartContainer.ColumnCount = 1;
                chartState.ChartContainer.ColumnStyles[0].Width = 100;
            }
            else
            {
                chartState = XYTwoGraphState;
                chartState.ChartContainer.ColumnCount = 2;
                chartState.ChartContainer.ColumnStyles[0].Width = 50;
                chartState.ChartContainer.ColumnStyles[1].Width = 50;
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
