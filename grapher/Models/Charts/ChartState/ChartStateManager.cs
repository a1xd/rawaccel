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
            AccelData accelData,
            AccelCalculator accelCalculator)
        {
            CombinedState = new CombinedState(
                sensitivityChart,
                velocityChart,
                gainChat,
                accelData,
                accelCalculator);

            XYOneGraphState = new XYOneGraphState(
                sensitivityChart,
                velocityChart,
                gainChat,
                accelData,
                accelCalculator);

            XYTwoGraphState = new XYTwoGraphState(
                sensitivityChart,
                velocityChart,
                gainChat,
                accelData,
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
                if (settings.sensitivity.x != settings.sensitivity.y)
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
