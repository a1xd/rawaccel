using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using grapher.Models.Theming;

namespace grapher
{
    public class Field
    {
        #region Enumerations

        public enum FieldState
        {
            Undefined,
            Default,
            Typing,
            Entered,
            Unavailable,
        }

        #endregion Enumerations

        #region Fields

        private double _data;

        #endregion Fields

        #region Constructors

        public Field(TextBox box, Form containingForm, double defaultData,
                                                       double minData = double.MinValue,
                                                       double maxData = double.MaxValue)
        {
            DefaultText = DecimalString(defaultData);
            Box = box;
            _data = defaultData;
            DefaultData = defaultData;
            MinData = minData;
            MaxData = maxData;
            State = FieldState.Undefined;
            ContainingForm = containingForm;
            FormatString = Constants.DefaultFieldFormatString;
            box.KeyDown += new System.Windows.Forms.KeyEventHandler(KeyDown);
            box.Leave += new System.EventHandler(FocusLeave);

            SetToDefault();
        }

        #endregion Constructors

        #region Properties

        public TextBox Box { get; }

        private Form ContainingForm { get; }

        public string FormatString { get; set; }

        public string DefaultText { get; set; }

        public FieldState State { get; private set; }

        public FieldState PreviousState { get; private set; }

        public double Data {
            get
            {
                if (Box.Enabled)
                {
                    return _data;
                }
                else
                {
                    return DefaultData;
                }
            }
        }

        public int Top
        {
            get
            {
                return Box.Top;
            }
            set
            {
                Box.Top = value;
            }
        }

        public int Height
        {
            get
            {
                return Box.Height;
            }
            set
            {
                Box.Height = value;
            }
        }

        public int Left
        {
            get
            {
                return Box.Left;
            }
            set
            {
                Box.Left = value;
            }
        }

        public int Width
        {
            get
            {
                return Box.Width;
            }
            set
            {
                Box.Width = value;
            }
        }

        private double DefaultData { get; set; }

        private double MinData { get; }

        private double MaxData { get; }

        #endregion Properties

        #region Methods

        public void Hide()
        {
            Box.Hide();
            Box.Enabled = false;
        }

        public void Show()
        {
            Box.Show();
            Box.Enabled = true;
        }

        public void SetNewDefault(double newDefault)
        {
            DefaultData = newDefault;
            DefaultText = DecimalString(newDefault);
        }

        public void SetToDefault()
        {
            if (State != FieldState.Default)
            {
                Box.BackColor = Theme.CurrentScheme.Field;
                Box.ForeColor = Theme.CurrentScheme.OnField;

                PreviousState = FieldState.Default;
                State = FieldState.Default;
            }

            _data = DefaultData;
            Box.Text = DefaultText;
        }

        public void SetToTyping()
        {
            if (State != FieldState.Typing)
            {
                Box.BackColor = Theme.CurrentScheme.Field;
                Box.ForeColor = Theme.CurrentScheme.OnFocusedField;

                PreviousState = State;
                State = FieldState.Typing;
            }

            Box.Text = Data.ToString();
        }

        public void SetToEntered()
        {
            if (State != FieldState.Entered)
            {
                Box.BackColor = Theme.CurrentScheme.EditedField;
                Box.ForeColor = Theme.CurrentScheme.OnEditedField;

                PreviousState = State;
                State = FieldState.Entered;
            }
        }

        public void SetToEntered(double value)
        {
            SetToEntered();

            _data = value;
            Box.Text = DecimalString(Data);
        }

        public void SetToUnavailable()
        {
            if (State != FieldState.Unavailable)
            {
                Box.BackColor = Theme.CurrentScheme.DisabledControl;
                Box.ForeColor = Theme.CurrentScheme.OnDisabledControl;
                Box.Text = string.Empty;

                PreviousState = State;
                State = FieldState.Unavailable;
            }
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            switch(State)
            {
                case FieldState.Default:
                // fallthrough
                case FieldState.Entered:
                    if (e.KeyCode == Keys.Enter)
                    {
                        Box.Parent.SelectNextControl(ContainingForm.ActiveControl, true, true, true, true);
                    }
                    else
                    {
                        SetToTyping();
                    }
                    break;
                case FieldState.Typing:
                    HandleTyping(sender, e);
                    break;
                case FieldState.Unavailable:
                    Box.Text = string.Empty;
                    ContainingForm.ActiveControl = null;
                    break;
                default:
                    break;
            }
        }

        public void NewInputToData()
        {
            if (State == FieldState.Typing)
            {
                TextToData();
            }
        }

        private void FocusLeave(object sender, EventArgs e)
        {
            NewInputToData();
        }

        private void HandleTyping(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TextToData();

                e.Handled = true;
                e.SuppressKeyPress = true;
                Box.Parent.SelectNextControl(ContainingForm.ActiveControl, true, true, true, true);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                ContainingForm.ActiveControl = null;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void TextToData()
        {
            if (double.TryParse(Box.Text, out double value) && 
                value <= MaxData && value >= MinData)
            {
                _data = value;
            }

            Box.Text = DecimalString(Data);
            
            if (string.Equals(Box.Text, DefaultText))
            {
                SetToDefault();
            }
            else
            {
                SetToEntered();
            }
        }

        private string DecimalString(double value)
        {
            return value.ToString(FormatString);
        }


        #endregion Methods
    }
}
