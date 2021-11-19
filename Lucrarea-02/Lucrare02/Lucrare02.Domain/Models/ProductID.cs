using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lucrarea01.Domain
{
    public record ProductID
    {
        private static readonly Regex ValidPattern = new("^LM[0-9]{5}$");

        public string Value { get; }

        private ProductID(string value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidProductID("WRONG INPUT");
            }
        }

        public static bool IsValid(string value) => ValidPattern.IsMatch(stringValue);

        public override string ToString()
        {
            return Value;
        }

        public static bool TryParse(string stringValue, out ProductID productId)
        {
            bool isValid = false;
            productId = null;

            if(IsValid(stringValue))
            {
                isValid = true;
                productId = new(stringValue);
            }

            return isValid;
        }
    }
}
