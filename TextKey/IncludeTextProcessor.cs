using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McGiv.TextKey
{


    /// <summary>
    /// Adds ability to have key/value insert within key values.
    /// 
    /// Insert key format : $(key)
    /// 
    /// e.g
    /// product_name = "Washing Machine"
    /// product_price = "$1200"
    /// product_details = "$(product_name) costs $(product_cost)"
    /// 
    /// product_details when processed with result in "Washing Machine costs $1200"
    /// 
    /// </summary>
    public class IncludeTextProcessor : ITextProcessor
    {
        private readonly ITextRepository _textRepository;

        public IncludeTextProcessor(ITextRepository textRepository)
        {
            if (textRepository == null)
            {
                throw new ArgumentNullException("textRepository");
            }

            this._textRepository = textRepository;
        }

        public string Process(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }


            return InnerProcess(key);

        }

        string InnerProcess(string key, IList<string> parentKeys = null)
        {

            var value = _textRepository.FindValueByKey(key);

            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (parentKeys != null)
            {

                if (parentKeys.Contains(key))
                {
                    parentKeys.Add(key);
                    throw new InvalidOperationException("Circular key reference found for the key path : " + String.Join(" > ", parentKeys));
                }

                parentKeys.Add(key);
            }

            StringBuilder sb = null;
            var start = 0;
            const string insertTail = "$(";
            while (true)
            {


                // find start of key - format : $(key)
                var keyStart = value.IndexOf(insertTail, start);

                if (keyStart == -1)
                {
                    if (sb == null)
                    {
                        if (start == 0)
                        {
                            return value;
                        }
                        else
                        {
                            sb = new StringBuilder();
                        }
                    }

                    sb.Append(value.Substring(start));
                    return sb.ToString();
                }

                var keyEnd = value.IndexOf(')', keyStart);
                var nextKeyStart = value.IndexOf(insertTail, keyStart + 2);

                if (keyEnd == -1 || (nextKeyStart != -1 && nextKeyStart < keyEnd))
                {
                    if (sb == null)
                    {
                        sb = new StringBuilder();
                    }

                    sb.Append(value.Substring(start, keyStart - start + 2));
                    start = keyStart + 2;
                    continue;
                }

                if (sb == null)
                {
                    sb = new StringBuilder();
                }

                var keyValue = value.Substring(keyStart + 2, keyEnd - keyStart - 2);
                var innerValue = this.InnerProcess(keyValue, (parentKeys ?? new List<string> { key }).ToList());

                if (innerValue == null)
                {
                    // key not found 
                    sb.Append(value.Substring(start, keyEnd - start + 1));
                }
                else
                {

                    sb.Append(value.Substring(start, keyStart - start));
                    sb.Append(innerValue);
                }



                start = keyEnd + 1;

            }
        }


    }
}
