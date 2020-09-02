using grapher.Models.Calculations;
using grapher.Models.Mouse;
using grapher.Models.Options;
using grapher.Models.Serialized;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace grapher
{
    public class AccelGUI
    {

        #region Constructors

        public AccelGUI(
            RawAcceleration accelForm,
            AccelCalculator accelCalculator,
            AccelCharts accelCharts,
            SettingsManager settings,
            ApplyOptions applyOptions,
            OptionXY sensitivity,
            Option rotation,
            AccelOptionSet optionSetX,
            AccelOptionSet optionSetY,
            Button writeButton,
            Label mouseMoveLabel,
            ToolStripMenuItem scaleMenuItem)
        {
            AccelForm = accelForm;
            AccelCalculator = accelCalculator;
            AccelCharts = accelCharts;
            ApplyOptions = applyOptions;
            Sensitivity = sensitivity;
            Rotation = rotation;
            WriteButton = writeButton;
            ScaleMenuItem = scaleMenuItem;
            Settings = settings;
            OptionSetX = optionSetX;
            OptionSetY = optionSetY;
            Settings.Startup();
            RefreshOnRead();

            MouseWatcher = new MouseWatcher(AccelForm, mouseMoveLabel, AccelCharts);

            ScaleMenuItem.Click += new System.EventHandler(OnScaleMenuItemClick);
        }

        #endregion Constructors

        #region Properties

        public RawAcceleration AccelForm { get; }

        public AccelCalculator AccelCalculator { get; }

        public AccelCharts AccelCharts { get; }

        public SettingsManager Settings { get; }

        public ApplyOptions ApplyOptions { get; }

        public OptionXY Sensitivity { get; }

        public Option Rotation { get; }

        public AccelOptionSet OptionSetX { get; }

        public AccelOptionSet OptionSetY { get; }

        public Button WriteButton { get; }

        public MouseWatcher MouseWatcher { get; }

        public ToolStripMenuItem ScaleMenuItem { get; }

        #endregion Properties

        #region Methods

        public void UpdateActiveSettingsFromFields()
        {
            Settings.UpdateActiveSettings(new DriverSettings
            {
                rotation = Rotation.Field.Data,
                sensitivity = new Vec2<double>
                {
                    x = Sensitivity.Fields.X,
                    y = Sensitivity.Fields.Y
                },
                combineMagnitudes = ApplyOptions.IsWhole,
                modes = new Vec2<AccelMode>
                {
                    x = (AccelMode)OptionSetX.AccelTypeOptions.AccelerationIndex,
                    y = (AccelMode)OptionSetY.AccelTypeOptions.AccelerationIndex
                },
                args = new Vec2<AccelArgs>
                {
                    x = OptionSetX.GenerateArgs(),
                    y = OptionSetY.GenerateArgs()
                },
                minimumTime = .4
            });
            RefreshOnRead();
        }

        public void RefreshOnRead()
        {
            UpdateGraph();
            UpdateShownActiveValues();
        }

        public void UpdateGraph()
        {
            AccelCalculator.Calculate(
                AccelCharts.AccelData, 
                Settings.ActiveAccel, 
                Settings.RawAccelSettings.AccelerationSettings);
            AccelCharts.Bind();
        }

        public void UpdateShownActiveValues()
        {
            var settings = Settings.RawAccelSettings.AccelerationSettings;

            Sensitivity.SetActiveValues(settings.sensitivity.x, settings.sensitivity.y);
            Rotation.SetActiveValue(settings.rotation);
            ApplyOptions.SetActive(settings.combineMagnitudes);
            OptionSetX.SetActiveValues((int)settings.modes.x, settings.args.x);
            OptionSetY.SetActiveValues((int)settings.modes.y, settings.args.y);

            AccelCharts.RefreshXY(settings.combineMagnitudes);
        }

        private void OnScaleMenuItemClick(object sender, EventArgs e)
        {
            UpdateGraph();
        }

        #endregion Methods
    }

}
