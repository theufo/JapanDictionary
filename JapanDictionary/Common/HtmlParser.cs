using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using JapanDictionary.Properties;

namespace JapanDictionary.Common
{
    //TODO попроавить добавление лишних произношений, добавить проверки на перевод всех частей составного канзи(чтоб все были выделены красным)
    public class HtmlParser
    {
        public string HtmlBody;
        public int translateObjectId;

        public StatusService StatusService { get; }

        public List<TranslateObject> TranslateObjects;

        public HtmlParser(string resultHtml, int i)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(resultHtml);

            HtmlBody = htmlDoc.GetElementbyId("tabContent").OuterHtml;
            TranslateObjects = new List<TranslateObject>();
            StatusService = StatusService.Instance;

            translateObjectId = i;
        }

        public bool CheckForAttributes(HtmlNode node)
        {
            if (node.HasAttributes)
                if (node.Attributes[0].Value == "background-color: #DDDDDD; ")
                    return true;
            return false;
        }

        public List<TranslateObject> Parse()
        {
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(HtmlBody);

                var trCollection = htmlDoc.DocumentNode.SelectNodes("//tr");

                foreach (var node in trCollection)
                {
                    if (node.Id.Contains("pos") || CheckForAttributes(node)) //getting tr tag with pos
                    {
                        var translateObject = new TranslateObject {Id = translateObjectId};

                        var translateObjectComparer = new TranslateObjectComparer();

                        var isSameKanji = false;

                        if (TranslateObjects.Contains(translateObject, translateObjectComparer))
                        {
                            translateObject = TranslateObjects.First(x => x.Id == translateObject.Id);
                            isSameKanji = true;
                        }

                        HtmlNode headerNode = null;
                        if (translateObject.OriginalString == null)
                            headerNode = node.SelectSingleNode(node.XPath + "//span");
                        if (headerNode != null)
                            translateObject.OriginalString = node.SelectSingleNode(node.XPath + "//span").InnerText; //selecting first span for getting header hieroglyph if there are more than one
                        else
                            translateObject.OriginalString = node.InnerText;

                        if (!isSameKanji)
                            TranslateObjects.Add(translateObject);
                    }
                    if (!node.HasAttributes) //getting <tr> tag without pos thus this <tr> is part of previous one
                    {
                        var num = TranslateObjects.Count - 1;
                        if (num >= 0)
                            if (TranslateObjects[num].Translation.Count <= Settings.Default.MaxTranslations)
                            {
                                var spans = node.SelectNodes(node.XPath + "//span"); //getting all <span>'s

                                KeyValuePair<List<string>, string> keyValuePair;

                                List<string> pronunciation = new List<string>();
                                string translation = string.Empty;

                                if (spans != null)
                                    foreach (var span in spans) //selecting specific spans based on style color
                                    {
                                        if (span.Attributes[0].Value.ToLower() == "color: #7f0000;") //pronunciation span
                                            pronunciation.Add(span.InnerText);
                                        else
                                        if (span.Attributes[0].Value.ToLower() == "color: #00007f;") //pronunciation kanji span
                                        {
                                            var separator = "<br>";
                                            List<string> list;
                                            if (span.InnerHtml.Contains(separator))
                                            {
                                                list = span.InnerHtml.Split(new string[] { separator }, StringSplitOptions.None).ToList();

                                                List<string> newList = new List<string>();

                                                foreach (var item in list)
                                                {
                                                        HtmlDocument htmlDocument = new HtmlDocument();
                                                        htmlDocument.LoadHtml(item);
                                                        newList.Add(htmlDocument.DocumentNode.InnerText);

                                                }
                                                pronunciation.AddRange(newList);
                                            }
                                            else
                                                pronunciation.Add(span.InnerText);
                                        }
                                        else
                                            if (span.Attributes[0].Value == "color: #000000;") //translation span
                                        {
                                            translation = span.InnerText;
                                            translation = translation.Replace(";", "\t");
                                        }
                                    }
                                keyValuePair = new KeyValuePair<List<string>, string>(pronunciation, translation);
                                TranslateObjects[num].Translation.Add(keyValuePair);
                            }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return TranslateObjects;
        }
    }
}