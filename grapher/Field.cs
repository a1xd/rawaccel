using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grapher
{
    public class Field
    {
        #region Enums

        public enum FieldState
        {
            Undefined,
            Default,
            Typing,
            Entered,
            Unavailable,
        }

        #endregion Enums


        #region Constructors

        public Field(TextBox box, Form containingForm, double defaultData)
        {
            DefaultText = DecimalString(defaultData);
            Box = box;
            Data = defaultData;
            DefaultData = defaultData;
            State = FieldState.Undefined;
            ContainingForm = containingForm;
            box.KeyDown += new System.Windows.Forms.KeyEventHandler(KeyDown);
            box.Leave += new System.EventHandler(FocusLeave);

            SetToDefault();
        }

        #endregion Constructors

        #region Properties

        TextBox Box { get; }

        Form ContainingForm { get; }

        public double Data { get; private set; }

        public string DefaultText { get; }

        public FieldState State { get; private set; }

        public FieldState PreviousState { get; private set; }

        private double DefaultData { get; }

        #endregion Properties

        #region Methods

        public void SetToDefault()
        {
            if (State != FieldState.Default)
            {
                Box.BackColor = Color.White;
                Box.ForeColor = Color.Gray;
                State = FieldState.Default;
                PreviousState = FieldState.Default;
            }

            Data = DefaultData;
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

            Box.Text = string.Empty;
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

            Data = value;
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
                if (PreviousState == FieldState.Default)
                {
                    SetToDefault();
                }
                else if (PreviousState == FieldState.Entered)
                {
                    SetToEntered();
                    Box.Text = DecimalString(Data);
                }
            }
        }

        private void HandleTyping(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    Data = Convert.ToDouble(((TextBox)sender).Text);
                }
                catch
                {
                }

                Box.Text = DecimalString(Data);
                e.Handled = true;
                e.SuppressKeyPress = true;

                SetToEntered();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                ContainingForm.ActiveControl = null;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private static string DecimalString(double value)
        {
            return value.ToString("N2");
        }

        #endregion Methods
    }
}
