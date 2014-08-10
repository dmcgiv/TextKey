using System;
using System.Collections.Generic;
using System.Linq;


namespace McGiv.TextKey
{
    public class DictionaryTextRepository : ITextRepository
    {
        private readonly Dictionary<string, TextItem> _data;


        public DictionaryTextRepository()
        {
            this._data = new Dictionary<string, TextItem>(StringComparer.OrdinalIgnoreCase);
        }

        public DictionaryTextRepository(IEnumerable<TextItem> items)
            : this()
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }


            foreach (var item in items)
            {
                if (item == null)
                {
                    throw new ArgumentException("The data contains a null TextItem", "data");
                }

                _data.Add(item.Key, item);
            }
        }

        public IQueryable<TextItem> All()
        {
            return _data.Values.AsQueryable();
        }

        public TextItem FindByKey(string key)
        {
            TextItem item;
            if (!_data.TryGetValue(key, out item))
            {
                return null;
            }

            return item;
        }

        public string FindValueByKey(string key)
        {
            var item = this.FindByKey(key);

            if (item != null)
            {
                return item.Value;
            }

            return null;
        }
    }
}
