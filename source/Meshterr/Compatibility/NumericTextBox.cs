using System.Windows.Forms;

namespace NumericTextBox
{
    public class NumericTextBox : TextBox
    {
        public bool AllowControls { get; set; }
        public bool AllowDecimalSeparator { get; set; }
        public bool AllowDigits { get; set; }
        public bool AllowLowers { get; set; }
        public bool AllowSymbols { get; set; }
        public bool AllowUppers { get; set; }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) && AllowControls)
            {
                base.OnKeyPress(e);
                return;
            }

            if (char.IsDigit(e.KeyChar) && AllowDigits)
            {
                base.OnKeyPress(e);
                return;
            }

            if ((e.KeyChar == '.' || e.KeyChar == ',') && AllowDecimalSeparator)
            {
                base.OnKeyPress(e);
                return;
            }

            if (char.IsLower(e.KeyChar) && AllowLowers)
            {
                base.OnKeyPress(e);
                return;
            }

            if (char.IsUpper(e.KeyChar) && AllowUppers)
            {
                base.OnKeyPress(e);
                return;
            }

            if ((char.IsSymbol(e.KeyChar) || char.IsPunctuation(e.KeyChar)) && AllowSymbols)
            {
                base.OnKeyPress(e);
                return;
            }

            e.Handled = true;
        }
    }
}
