using grapher.Models.Calculations;
using grapher.Models.Serialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Models.Charts.ChartState
{
    public class ChartStateManager
    {
        public ChartStateManager(
            ChartXY sensitivityChart,
            ChartXY velocityChart,
            ChartXY gainChat,
            AccelCalculator accelCalculator,
            EstimatedPoints combined,
            EstimatedPoints xPoints,
            EstimatedPoints yPoints)
        {
            CombinedState = new CombinedState(
                sensitivityChart,
                velocityChart,
                gainChat,
                combined,
                accelCalculator);

            XYOneGraphState = new XYOneGraphState(
                sensitivityChart,
                velocityChart,
                gainChat,
                xPoints,
                yPoints,
                accelCalculator);

            XYTwoGraphState = new XYTwoGraphState(
                sensitivityChart,
                velocityChart,
                gainChat,
                xPoints,
                yPoints,
                accelCalculator);
        }

        private CombinedState CombinedState { get; }

        private XYOneGraphState XYOneGraphState { get; }

        private XYTwoGraphState XYTwoGraphState { get; }


        public ChartState DetermineState(DriverSettings settings)
        {
            ChartState chartState;

            if (settings.combineMagnitudes)
            {
                if (settings.sensitivity.x != settings.sensitivity.y ||
                    settings.domainArgs.domainXY.x != settings.domainArgs.domainXY.y ||
                    settings.rangeXY.x != settings.rangeXY.y)
                {
                    chartState = XYOneGraphState;
                }
                else
                {
                    chartState = CombinedState;
                }
            }
            else
            {
                chartState = XYTwoGraphState;
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
