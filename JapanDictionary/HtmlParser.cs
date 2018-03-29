using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;
using JapanDictionary.Properties;

namespace JapanDictionary
{
    public class HtmlParser
    {
        public string HtmlBody;

        public List<TranslateObject> TranslateObjects;

        public HtmlParser(string resultHtml)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(resultHtml);

            HtmlBody = htmlDoc.GetElementbyId("tabContent").OuterHtml;
            TranslateObjects = new List<TranslateObject>();
        }

        public string Parse()
        {
            string result = "";

            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(HtmlBody);

                var trCollection = htmlDoc.DocumentNode.SelectNodes("//tr");

                foreach (var node in trCollection)
                {
                    if (node.Id.Contains("pos")) //getting tr tag with pos
                    {
                        var translateObject = new TranslateObject();
                        translateObject.id = Int32.Parse(node.Id.Substring(3));
                        var translateObjectComparer = new TranslateObjectComparer();

                        var copy = false;

                        if (TranslateObjects.Contains(translateObject, translateObjectComparer))
                        {
                            translateObject = TranslateObjects.First(x => x.id == translateObject.id);
                            copy = true;
                        }

                        HtmlNode headerNode = null;
                        if (translateObject.OriginalString == null)
                            headerNode = node.SelectSingleNode(node.XPath + "//span");
                        if (headerNode != null)
                        translateObject.OriginalString = node.SelectSingleNode(node.XPath + "//span").InnerText; //selecting first span for getting header hieroglyph if there are more than one
                        else
                        translateObject.OriginalString = node.InnerText;

                        if (!copy)
                            TranslateObjects.Add(translateObject);
                    }
                    if (!node.HasAttributes) //getting <tr> tag without pos thus this <tr> is part of previous one
                    {
                        var num = TranslateObjects.Count - 1;
                        if (num >= 0)
                        {
                            var spans = node.SelectNodes(node.XPath + "//span"); //getting all <span>'s
                            foreach (var span in spans)
                            {
                                if (span.Attributes[0].Value.ToLower() == "color: #7f0000;") //selecting specific spans based on style color
                                    TranslateObjects[num].Pronunciation = span.InnerText;
                                if (span.Attributes[0].Value == "color: #000000;")
                                {
                                    //var separator = "<br>";
                                    //List<string> list;
                                    //if (span.InnerHtml.Contains(separator))
                                    //{
                                    //    list = span.InnerHtml.Split(new string[] {separator}, StringSplitOptions.RemoveEmptyEntries).ToList();
                                    //    foreach (var item in list)
                                    //    {
                                    //        HtmlDocument htmlDocument = new HtmlDocument();
                                    //        htmlDocument.LoadHtml(item);
                                    //        item = htmlDoc.DocumentNode.InnerText;
                                    //    }
                                    //    else
                                        //list = span.InnerText.Split(new string[] {separator}, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        //TranslateObjects[num].Translation.AddRange(list);
                                        TranslateObjects[num].Translation.Add(span.InnerText);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                foreach (var translateObject in TranslateObjects)
                {
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
        }
    }