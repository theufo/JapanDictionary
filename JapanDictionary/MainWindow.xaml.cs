﻿using System;
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
            ParseForKanji();
        }

        private async void ParseForKanji()
        {
            DictionaryResult = new List<TranslateObject>();

            var inputText = TextView.InputText.Text + " ";
            var charArray = inputText.ToCharArray();

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
                    if (isPreviousKanji)
                    {
                        kanjiList[kanjiList.Count - 1] += charItem.ToString();
                        isPreviousKanji = false;

                        newText += "(" + kanjiNumber + ") ";
                        kanjiNumber++;
                    }
                    else
                    {
                        kanjiList.Add(charItem.ToString());
                        isPreviousKanji = true;
                    }

                }
                else
                {
                    if (isPreviousKanji)
                    {
                        newText += "(" + kanjiNumber + ") ";
                        kanjiNumber++;
                    }
                    isPreviousKanji = false;
                }
            }

            for(int i = 0; i < kanjiList.Count; i++)
            {
                DictionaryResult.AddRange(await GetTranslation(kanjiList[i], i+1));
            }

            TextView.InputText.Text = newText;

            DictionaryView.OutPutText.Text = OutputTranslation();
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
                    result += (i + 1) + ") " + translateObject.Translation[i].Key + "\n" + translateObject.Translation[i].Value + "\n";
                }
                result += "\n";
            }
            return result;
        }

        public async Task<List<TranslateObject>> GetTranslation(string kanjiItem, int i)
        {
            ApiHelper apiHelper = new ApiHelper();
            var result = new List<TranslateObject>();
            var link = "http://www.jardic.ru/search/search_r.php?q=" + kanjiItem + "&pg=0&dic_jardic=" + Settings.Default.Jardic + "&dic_warodai=" + Settings.Default.Warodai + " & sw=1920"; //TODO to checkboxes
            Thread.Sleep(100);
            try
            {
                var resultHtml = await apiHelper.GetAsync(link);

                var htmlParser = new HtmlParser(resultHtml, i);

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