using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Windows;

namespace JapanDictionary
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Buttons.ConvertButton.Click += OnConvertClicked;
        }

        private void OnConvertClicked(object sender, RoutedEventArgs e)
        {
            TextView.InputText.Text = Buttons.Result;
        }

        private void TranslateFile(object sender, RoutedEventArgs e)
        {
            GetTranslation();
        }

        public async void GetTranslation()
        {
            ApiHelper apiHelper = new ApiHelper();
            var link = "http://www.jardic.ru/search/search_r.php?q=" +
                       TextView.InputText.Text + "&pg=0&dic_jardic=1&dic_warodai=1&dic_unihan=1&dic_edict=1&dic_enamdict=1&dic_kanjidic=1&dic_tatoeba=1&dic_chekhov=1&dic_japaneselaw=1&dic_medic=1&sw=1920";
            try
            {
                var resultHtml = await apiHelper.GetAsync(link);

                var htmlParser = new HtmlParser(resultHtml);

                DictionaryView.OutPutText.Text = htmlParser.Parse();
            }
            catch (Exception e)
            {
                DictionaryView.OutPutText.Text = e.Message;
            }
        }
    }

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