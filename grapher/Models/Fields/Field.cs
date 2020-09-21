using System;
using System.Drawing;
using System.Windows.Forms;

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

        public Field(TextBox box, Form containingForm, double defaultData)
        {
            DefaultText = DecimalString(defaultData);
            Box = box;
            _data = defaultData;
            DefaultData = defaultData;
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

        public string DefaultText { get; }

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

        private double DefaultData { get; }

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

        public void SetToDefault()
        {
            if (State != FieldState.Default)
            {
                Box.BackColor = Color.White;
                Box.ForeColor = Color.Gray;
                State = FieldState.Default;
                PreviousState = FieldState.Default;
            }

            _data = DefaultData;
            Box.Text = DefaultText;
            ContainingForm.ActiveControl = null;
        }

        public void SetToTyping()
        {
            if (State != FieldState.Typing)
            {
                Box.BackColor = Color.White;
                Box.ForeColor = Color.Black;

                PreviousState = State;
                State = FieldState.Typing;
            }

            Box.Text = Data.ToString();
        }

        public void SetToEntered()
        {
            if (State != FieldState.Entered)
            {
                Box.BackColor = Color.AntiqueWhite;
                Box.ForeColor = Color.DarkGray;

                PreviousState = State;
                State = FieldState.Entered;
            }

            ContainingForm.ActiveControl = null;
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
                Box.BackColor = Color.LightGray;
                Box.ForeColor = Color.LightGray;
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
                    if (e.KeyCode == Keys.Enter)
                    {
                        SetToDefault();
                    }
                    else
                    {
                        SetToTyping();
                    }
                    break;

                case FieldState.Entered:
                    if (e.KeyCode != Keys.Enter)
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

        private void FocusLeave(object sender, EventArgs e)
        {
            if (State == FieldState.Typing)
            {
                TextToData();
            }
        }

        private void HandleTyping(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TextToData();

                e.Handled = true;
                e.SuppressKeyPress = true;
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
            try
            {
                _data = Convert.ToDouble(Box.Text);
            }
            catch
            {
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
