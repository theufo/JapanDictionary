using System.Collections.Generic;

namespace JapanDictionary.Common
{
    public class TranslateObject
    {
        public int Id;
        public string OriginalString;
        public List<KeyValuePair<List<string>, string>> Translation;

        public TranslateObject()
        {
            Translation = new List<KeyValuePair<List<string>, string>>();
        }
    }

    public class TranslateObjectComparer : IEqualityComparer<TranslateObject>
    {
        public bool Equals(TranslateObject x, TranslateObject y)
        {
            return y != null && x != null && x.Id == y.Id;
        }

        public int GetHashCode(TranslateObject obj)
        {
            var hashOriginalString = obj.OriginalString == null ? 0 : obj.OriginalString.GetHashCode();

            return hashOriginalString;
        }
    }
}