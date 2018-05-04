using System.Collections.Generic;
using System.Globalization;
using JapanDictionary.Properties;

namespace JapanDictionary.Common
{
    public class KanjiParser
    {
        public KanjiParser()
        {
            StatusService = StatusService.Instance;
        }

        public StatusService StatusService { get; }

        public List<string> ParseForKanji(string inputText)
        {
            StatusService.Text = "Parsing for kanji";

            var charArray = inputText.ToCharArray();

            var kanjiList = new List<string>();

            var kanjiStartValue = int.Parse(Settings.Default.KanjiStartValue, NumberStyles.HexNumber);
            var kanjiEndValue = int.Parse(Settings.Default.KanjiEndValue, NumberStyles.HexNumber);
            var hiraganaStartValue = int.Parse(Settings.Default.HiraganaStartValue, NumberStyles.HexNumber);
            var hiraganaEndValue = int.Parse(Settings.Default.HiraganaEndValue, NumberStyles.HexNumber);
            var katakanaStartValue = int.Parse(Settings.Default.KatakanaStartValue, NumberStyles.HexNumber);
            var katakanaEndValue = int.Parse(Settings.Default.KatakanaEndValue, NumberStyles.HexNumber);

            var str = string.Empty;

            foreach (var charItem in charArray)
            {
                var i = (int) charItem;

                if (i >= kanjiStartValue && i <= kanjiEndValue)
                {
                    str += charItem;
                }
                else
                {
                    if (str != string.Empty)
                    {
                        if (i >= hiraganaStartValue && i <= hiraganaEndValue || i >= katakanaStartValue && i <= katakanaEndValue)
                            if (str.Length < 2)
                                str += charItem;
                        if (!kanjiList.Contains(str)) kanjiList.Add(str);
                        str = string.Empty;
                    }
                }
            }

            return kanjiList;
        }
    }
}