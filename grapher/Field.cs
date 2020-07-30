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

        public Field(string defaultText, TextBox box, double defaultData)
        {
            DefaultText = defaultText;
            Box = box;
            Data = defaultData;
            State = FieldState.Undefined;
            box.KeyDown += KeyDown;

            SetToDefault();
        }

        #endregion Constructors

        #region Properties

        TextBox Box { get; }

        public double Data { get; private set; }

        public string DefaultText { get; }

        public FieldState State { get; private set; }

        #endregion Properties

        #region Methods

        public void SetToDefault()
        {
            if (State != FieldState.Default)
            {
                Box.BackColor = Color.AntiqueWhite;
                Box.ForeColor = Color.Gray;
                Box.Text = DefaultText;
                State = FieldState.Default;
            }
        }

        public void SetToTyping()
        {
            if (State != FieldState.Typing)
            {
                Box.BackColor = Color.White;
                Box.ForeColor = Color.Black;
                State = FieldState.Typing;
            }
        }

        public void SetToEntered()
        {
            if (State != FieldState.Entered)
            {
                Box.BackColor = Color.White;
                Box.ForeColor = Color.DarkGray;
                State = FieldState.Entered;
            }
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
            if (TryHandleWithEnter(e, sender, out double data))
            {
                Data = data;
            }
        }

        private bool TryHandleWithEnter(KeyEventArgs e, object sender, out double data)
        {
            bool validEntry = false;
            data = 0.0;

            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    data = Convert.ToDouble(((TextBox)sender).Text);
                    validEntry = true;
                }
                catch
                {
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            return validEntry;
        }

        #endregion Methods
    }
}
