using grapher.Models.Options;

namespace grapher.Layouts
{
    public class OptionLayout
    {
        #region Constructors

        public OptionLayout(bool show, string name)
        {
            Show = show;
            Name = name;
        }

        #endregion Constructors

        #region Properties

        private bool Show { get; }

        private string Name { get; }

        #endregion Properties

        #region Methods

        public void Layout(IOption option)
        {
            if (Show)
            {
                option.Show(Name);
            }
            else
            {
                option.Hide();
            }
        }

        #endregion Methods
    }
}
