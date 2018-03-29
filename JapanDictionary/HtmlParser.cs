using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;

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
                    if (node.Id.Contains("pos"))
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

                        if (translateObject.OriginalString == null)
                            translateObject.OriginalString = node.InnerText; //select right word <span bf0000="">年度</span> <a>予算</a>

                        if (!copy)
                        TranslateObjects.Add(translateObject);

                    }
                    if (!node.HasAttributes)
                    {
                        var num = TranslateObjects.Count - 1;
                        if (num>=0)
                        TranslateObjects[num].Translation.Add(node.InnerText);
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
                foreach (var translation in translateObject.Translation)
                {
                    result += translation + "\n";
                }
                result += "\n";
            }
            return result;
        }
    }
}
