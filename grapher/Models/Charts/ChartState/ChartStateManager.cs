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
            AccelData accelData)
        {
            CombinedState = new CombinedState(
                sensitivityChart,
                velocityChart,
                gainChat,
                accelData);

            XYOneGraphState = new XYOneGraphState(
                sensitivityChart,
                velocityChart,
                gainChat,
                accelData);

            XYTwoGraphState = new XYTwoGraphState(
                sensitivityChart,
                velocityChart,
                gainChat,
                accelData);
        }

        private CombinedState CombinedState { get; }

        private XYOneGraphState XYOneGraphState { get; }

        private XYTwoGraphState XYTwoGraphState { get; }

        public ChartState DetermineState(DriverSettings settings)
        {
            if (settings.combineMagnitudes)
            {
                if (settings.sensitivity.x != settings.sensitivity.y)
                {
                    return XYOneGraphState;
                }
                else
                {
                    return CombinedState;
                }
            }
            else
            {
                return XYTwoGraphState;
            }
        }

        public ChartState InitialState()
        {
            return CombinedState;
        }
    }
}
