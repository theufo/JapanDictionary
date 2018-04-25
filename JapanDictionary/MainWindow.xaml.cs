using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using JapanDictionary.Properties;

namespace JapanDictionary
{
    public partial class MainWindow
    {
        private readonly ApiHelper _apiHelper;

        public List<TranslateObject> DictionaryResult;

        public MainWindow()
        {
            InitializeComponent();

            DictionaryResult = new List<TranslateObject>();
            _apiHelper = new ApiHelper();

            Buttons.ConvertButton.Click += OnConvertClicked;
        }

        #region Button events
        private void OnConvertClicked(object sender, RoutedEventArgs e)
        {
            TextView.InputText.Text = Buttons.LoadResult;
        }

        private void OnSaveClicked()
        {
            string result = "\uFEFF\n";

            foreach (var translateObject in DictionaryResult)
            {
                result += translateObject.OriginalString + ";";

                for (int i = 0; i < translateObject.Translation.Count && i < Settings.Default.MaxTranslations; i++)
                {
                    if (i != 0)
                        result += "\n ;";
                    for (int j = 0; j < translateObject.Translation[i].Key.Count; j++)
                    {
                        result += translateObject.Translation[i].Key[j];
                        if (j < translateObject.Translation[i].Key.Count)
                            result += " / ";
                    }
                    result += ";" + translateObject.Translation[i].Value;
                }
                result += "\n";
            }
            Buttons.SaveResult = result;
        }

        #endregion

        private void TranslateFile(object sender, RoutedEventArgs e)
        {
            ParseForKanji();
        }

        private async void ParseForKanji()
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            StatusTextBlock.Text = "Parsing for kanji";

            long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            DictionaryResult = new List<TranslateObject>();

            var inputText = TextView.InputText.Text + " ";
            var charArray = inputText.ToCharArray();

            List<string> kanjiList = new List<string>();

            int kanjiStartValue = int.Parse(Settings.Default.KanjiStartValue, System.Globalization.NumberStyles.HexNumber);
            int kanjiEndValue = int.Parse(Settings.Default.KanjiEndValue, System.Globalization.NumberStyles.HexNumber);
            int hiraganaStartValue = int.Parse(Settings.Default.HiraganaStartValue, System.Globalization.NumberStyles.HexNumber);
            int hiraganaEndValue = int.Parse(Settings.Default.HiraganaEndValue, System.Globalization.NumberStyles.HexNumber);
            int katakanaStartValue = int.Parse(Settings.Default.KatakanaStartValue, System.Globalization.NumberStyles.HexNumber);
            int katakanaEndValue = int.Parse(Settings.Default.KatakanaEndValue, System.Globalization.NumberStyles.HexNumber);

            var str = String.Empty;

            foreach (var charItem in charArray)
            {
                var i = (int)charItem;

                if (i >= kanjiStartValue && i <= kanjiEndValue)
                {
                    str += charItem;
                }
                else
                {
                    if (str != string.Empty)
                    {
                        if ((i >= hiraganaStartValue && i <= hiraganaEndValue) || (i >= katakanaStartValue && i <= katakanaEndValue))
                            if (str.Length < 2) str += charItem;
                        if(!(kanjiList.Contains(str))) kanjiList.Add(str);
                        str = string.Empty;
                    }
                }
            }

            StatusTextBlock.Text = "Translating kanji";

            for (int i = 0; i < kanjiList.Count; i++)
            {
                int output = i + 1;
                StatusTextBlock.Text = "Trainslating kanji № " + output + "\\" + kanjiList.Count;
                DictionaryResult.AddRange(await GetTranslation(kanjiList[i], i + 1));
            }

            milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds() - milliseconds;
            StatusTextBlock.Text = "Translate successfull. Elapsed: " + TimeSpan.FromMilliseconds(milliseconds);

            //TextView.InputText.Text = newText;
            DictionaryView.OutPutText.Text = OutputTranslation();

            OnSaveClicked();

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        public string OutputTranslation()
        {
            string result = String.Empty;

            foreach (var translateObject in DictionaryResult)
            {
                result += "-----------------------(" + translateObject.id + ")-----------------------" + "\n";
                result += translateObject.OriginalString + "\n";

                for (int i = 0; i < translateObject.Translation.Count && i < Settings.Default.MaxTranslations; i++)
                {
                    result += (i + 1) + ") ";
                    for (int j = 0; j < translateObject.Translation[i].Key.Count; j++)
                    {
                        result += translateObject.Translation[i].Key[j] + ";";
                    }
                    result += "\n" + translateObject.Translation[i].Value + "\n";
                }
                result += "\n";
            }
            return result;
        }

        public async Task<List<TranslateObject>> GetTranslation(string kanjiItem, int i)
        {
            var result = new List<TranslateObject>();
            var link = "http://www.jardic.ru/search/search_r.php?q=" + kanjiItem + "&pg=0";
            if (Settings.Default.Jardic)
                link += "&dic_jardic=1";
            if (Settings.Default.Warodai)
                link += "&dic_warodai=1";
            link += "&sw = 1920";

            Thread.Sleep(100);

            try
            {
                var resultHtml = await _apiHelper.GetAsync(link);

                var htmlParser = new HtmlParser(resultHtml, i);

                result = htmlParser.Parse();
            }
            catch (Exception e)
            {
                DictionaryView.OutPutText.Text = e.Message;
            }
            return result;
        }

        #region Translations output box
        private void NumericOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text);
        }

        private static bool IsTextNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[^0-9]");
            return reg.IsMatch(str);
        }

        #endregion

    }
}