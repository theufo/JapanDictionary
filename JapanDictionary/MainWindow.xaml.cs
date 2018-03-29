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

                var response = new Response(resultHtml);

                DictionaryView.OutPutText.Text = response.HtmlBody.OuterHtml;
            }
            catch (Exception e)
            {
                DictionaryView.OutPutText.Text = e.Message;
            }
        }
    }

    public class Response
    {
        public HtmlNode HtmlBody;
        public string OriginalWord;
        public string Pronunciation;
        public List<string> Translation;

        public Response(string resultHtml)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(resultHtml);

            HtmlBody = htmlDoc.GetElementbyId("tabContent");
        }
    }
}