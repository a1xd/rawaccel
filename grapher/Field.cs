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

        public Field(string defaultText, TextBox box, Form containingForm, double defaultData)
        {
            DefaultText = defaultText;
            Box = box;
            Data = defaultData;
            State = FieldState.Undefined;
            ContainingForm = containingForm;
            box.KeyDown += KeyDown;

            SetToDefault();
        }

        #endregion Constructors

        #region Properties

        TextBox Box { get; }

        Form ContainingForm { get; }

        public double Data { get; private set; }

        public string DefaultText { get; }

        public FieldState State { get; private set; }

        #endregion Properties

        #region Methods

        public void SetToDefault()
        {
            if (State != FieldState.Default)
            {
                Box.BackColor = Color.White;
                Box.ForeColor = Color.Gray;
                State = FieldState.Default;
            }

            Box.Text = DefaultText;
            ContainingForm.ActiveControl = null;
        }

        public void SetToTyping()
        {
            if (State != FieldState.Typing)
            {
                Box.BackColor = Color.White;
                Box.ForeColor = Color.Black;
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
                State = FieldState.Entered;
            }

            ContainingForm.ActiveControl = null;
        }

        public void SetToUnavailable()
        {
            if (State != FieldState.Unavailable)
            {
                Box.BackColor = Color.LightGray;
                Box.ForeColor = Color.LightGray;
                Box.Text = string.Empty;
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
                    break;
                default:
                    break;
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
                    Box.Text = Data.ToString();
                }

                e.Handled = true;
                e.SuppressKeyPress = true;

                SetToEntered();
            }
        }

        #endregion Methods
    }
}
