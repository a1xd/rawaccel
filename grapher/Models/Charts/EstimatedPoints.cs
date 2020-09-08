using grapher.Models.Mouse;

namespace grapher.Models.Charts
{
    public class EstimatedPoints
    {
        #region Constructors

        public EstimatedPoints()
        {
            Sensitivity = new PointData();
            Velocity = new PointData();
            Gain = new PointData();
        }

        #endregion Constructors

        #region Properties

        public PointData Sensitivity { get; }

        public PointData Velocity { get; }

        public PointData Gain { get; }

        #endregion Properties
    }
}
