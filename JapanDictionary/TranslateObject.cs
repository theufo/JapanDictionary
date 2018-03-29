using System;
using System.Collections.Generic;

namespace JapanDictionary
{
    public class TranslateObject
    {
        public int id;
        public string OriginalString;
        public string Pronunciation;
        public List<string> Translation;

        public TranslateObject()
        {
            Translation = new List<string>();
        }
    }

    public class TranslateObjectComparer : IEqualityComparer<TranslateObject>
    {
        public bool Equals(TranslateObject x, TranslateObject y)
        {
            if (x.id == null || y.id == null)
                return false;

            return x.id == y.id;
        }

        public int GetHashCode(TranslateObject obj)
        {
            if (Object.ReferenceEquals(obj, null)) return 0;

            int hashOriginalString = obj.OriginalString == null ? 0 : obj.OriginalString.GetHashCode();

            return hashOriginalString;
        }
    }

}
