using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.PresentationFramework.Filters;

namespace Caliburn.WPF.Actions
{
    public class Calculator
    {
        public decimal Divide(string left, string right)
        {
            decimal dividend;
            decimal divisor;
            decimal.TryParse(left, out dividend); 
            decimal.TryParse(right, out divisor);
            return dividend/divisor;
        }

        public bool CanDivide(string left, string right)
        {
            decimal number;
            return decimal.TryParse(left, out number) && decimal.TryParse(right, out number);
        }
    }
}
