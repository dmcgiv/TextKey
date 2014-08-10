using System;

namespace McGiv.TextKey
{
    public class CaseProcessor : IKeyProcessor
    {
        public string Process(string text)
        {
            if ( text == null)
            {
                return text;
            }

            if (Argument == null)
            {
                throw new InvalidOperationException(string.Format("CaseProcessor : Argument is required.", this.Argument));
            }

            if (this.Argument.StartsWith("upper", StringComparison.OrdinalIgnoreCase))
            {
                return text.ToUpper();
            }
            else if (this.Argument.StartsWith("lower", StringComparison.OrdinalIgnoreCase))
            {
                 return text.ToLower();
            }

            throw new InvalidOperationException(string.Format("CaseProcessor : Argument '{0}' is not supported.", this.Argument));
        }

        public string Argument { get; set; }

        public string Key {
            get
            {
                return "case";
            }
        }
    }
}
