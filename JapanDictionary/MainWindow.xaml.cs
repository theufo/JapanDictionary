using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using JapanDictionary.Common;
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
            DataContext = this;

            DictionaryResult = new List<TranslateObject>();
            _apiHelper = new ApiHelper();

            StatusService = StatusService.Instance;
            StatusService.SetReady();

            Buttons.ConvertButton.Click += OnConvertClicked;
        }

        public StatusService StatusService { get; }

        private async void TranslateFile(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var kanjiParser = new KanjiParser();
            var milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var kanjiList = kanjiParser.ParseForKanji(TextView.InputText.Text + " ");

            DictionaryResult = await MakeDictionary(kanjiList);

            DictionaryView.OutPutText.Text = OutputTranslation();

            milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds() - milliseconds;
            StatusService.Text = "Translate successfull. Elapsed: " + TimeSpan.FromMilliseconds(milliseconds);

            OnSaveClicked(); //TODO

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private async Task<List<TranslateObject>> MakeDictionary(List<string> kanjiList)
        {
            var dictionaryResult = new List<TranslateObject>();

            StatusService.Text = "Translating kanji";

            for (var i = 0; i < kanjiList.Count; i++)
            {
                var output = i + 1;
                StatusService.Text = "Trainslating kanji № " + output + "\\" + kanjiList.Count;
                dictionaryResult.AddRange(await GetTranslation(kanjiList[i], i + 1));
            }

            return dictionaryResult;
        }

        public string OutputTranslation()
        {
            var result = string.Empty;

            foreach (var translateObject in DictionaryResult)
            {
                result += "-----------------------(" + translateObject.Id + ")-----------------------" + "\n";
                result += translateObject.OriginalString + "\n";

                for (var i = 0; i < translateObject.Translation.Count && i < Settings.Default.MaxTranslations; i++)
                {
                    result += i + 1 + ") ";
                    for (var j = 0; j < translateObject.Translation[i].Key.Count; j++) result += translateObject.Translation[i].Key[j] + ";";
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

            Thread.Sleep(Settings.Default.RequestDelay);

            try
            {
                var resultHtml = await _apiHelper.GetAsync(link);
                var htmlParser = new HtmlParser(resultHtml, i);

                result = htmlParser.Parse();
            }
            catch (Exception e)
            {
                StatusService.Text = e.Message;
                DictionaryView.OutPutText.Text = e.Message;
            }

            return result;
        }

        #region Button events

        private void OnConvertClicked(object sender, RoutedEventArgs e)
        {
            TextView.InputText.Text = Buttons.LoadResult;
        }

        private void OnSaveClicked()
        {
            var result = "\uFEFF\n";

            foreach (var translateObject in DictionaryResult)
            {
                result += translateObject.OriginalString + ";";

                for (var i = 0; i < translateObject.Translation.Count && i < Settings.Default.MaxTranslations; i++)
                {
                    if (i != 0)
                        result += "\n ;";
                    for (var j = 0; j < translateObject.Translation[i].Key.Count; j++)
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

        #region Translations output box 

        //TODO move to settings control
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text);
        }

        private static bool IsTextNumeric(string str)
        {
            var reg = new Regex("[^0-9]");
            return reg.IsMatch(str);
        }

        #endregion
    }
}