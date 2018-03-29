using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using JapanDictionary.Properties;

namespace JapanDictionary
{
    public partial class MainWindow
    {
        public List<TranslateObject> DictionaryResult;
        public MainWindow()
        {
            InitializeComponent();
            DictionaryResult = new List<TranslateObject>();

            Buttons.ConvertButton.Click += OnConvertClicked;
        }

        private void OnConvertClicked(object sender, RoutedEventArgs e)
        {
            TextView.InputText.Text = Buttons.Result;
        }

        private void TranslateFile(object sender, RoutedEventArgs e)
        {
            //GetTranslation();
            ParseForKanji();
        }

        private async void ParseForKanji()
        {
            var charArray = TextView.InputText.Text.ToCharArray();
            string newText = ""; 
            List<string> kanjiList = new List<string>();

            int kanjiStartValue = int.Parse(Settings.Default.KanjiStartValue, System.Globalization.NumberStyles.HexNumber);
            int kanjiEndValue = int.Parse(Settings.Default.KanjiEndValue, System.Globalization.NumberStyles.HexNumber);

            var kanjiNumber = 1;

            bool isPreviousKanji = false;

            foreach (var charItem in charArray)
            {
                var i = (int)charItem;
                newText += charItem;

                if (i >= kanjiStartValue && i <= kanjiEndValue)
                {
                    newText += "(" + kanjiNumber + ") ";
                    kanjiNumber++;
                    //if (isPreviousKanji)
                    //{
                    //    kanjiList[kanjiList.Count-1] += charItem.ToString();
                    //}
                    //else
                        kanjiList.Add(charItem.ToString());

                    isPreviousKanji = true;
                }
                else
                    isPreviousKanji = false;
            }



            //var output = string.Empty;

            //foreach (var kanji in kanjiList)
            //{
            //    output += kanji + "\n";
            //}

            //DictionaryView.OutPutText.Text = newText;

            foreach (var kanjiItem in kanjiList)
            {
                DictionaryResult.AddRange(await GetTranslation(kanjiItem));
            }

            TextView.InputText.Text = newText;
            DictionaryView.OutPutText.Text = OutputTranslation();
        }

        public string OutputTranslation()
        {
            string result = String.Empty;

            int number = 1;
            foreach (var translateObject in DictionaryResult)
            {
                //TODO result += "(" + translateObject.id + ")" + "\n";
                result += "(" + number + ")" + "\n";
                number++;
                result += translateObject.OriginalString + "\n";
                result += translateObject.Pronunciation + "\n";
                for (int i = 0; i < translateObject.Translation.Count && i < Settings.Default.MaxTranslations; i++)
                {
                    result += (i + 1) + ") " + translateObject.Translation[i] + "\n";
                }
                result += "\n";
            }
            return result;
        }

        public async Task<List<TranslateObject>> GetTranslation(string kanjiItem)
        {
            ApiHelper apiHelper = new ApiHelper();
            var result = new List<TranslateObject>();
            var link = "http://www.jardic.ru/search/search_r.php?q=" +
                       kanjiItem + "&pg=0&dic_jardic=1&dic_warodai=1&dic_unihan=1&dic_edict=1&dic_enamdict=1&dic_kanjidic=1&dic_tatoeba=1&dic_chekhov=1&dic_japaneselaw=1&dic_medic=1&sw=1920"; //TODO to checkboxes
            Thread.Sleep(100);
            try
            {
                var resultHtml = await apiHelper.GetAsync(link);

                var htmlParser = new HtmlParser(resultHtml);

                result = htmlParser.Parse();
            }
            catch (Exception e)
            {
                DictionaryView.OutPutText.Text = e.Message;
            }
            return result;
        }
    }
}