using System;
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
                       TextView.InputText.Text + "&pg=0&dic_jardic=1&dic_warodai=1&dic_unihan=1&dic_edict=1&dic_enamdict=1&dic_kanjidic=1&dic_tatoeba=1&dic_chekhov=1&dic_japaneselaw=1&dic_medic=1&sw=1920"; //TODO to checkboxes
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
}