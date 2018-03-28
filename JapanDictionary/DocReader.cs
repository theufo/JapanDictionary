using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.IO;
using System.Text;

namespace JapanDictionary
{
    public class DocReader
    {
    public class GetWordPlainText : IDisposable
        {
            // Specify whether the instance is disposed.
            private bool disposed = false;

            // The word package
            private WordprocessingDocument package = null;

            private string FileName = string.Empty;

            public GetWordPlainText(string filepath)
            {
                this.FileName = filepath;
                if (string.IsNullOrEmpty(filepath) || !File.Exists(filepath))
                {
                    throw new Exception("The file is invalid. Please select an existing file again");
                }
                this.package = WordprocessingDocument.Open(filepath, true);
            }

            public string ReadWordDocument()
            {
                StringBuilder sb = new StringBuilder();
                OpenXmlElement element = package.MainDocumentPart.Document.Body;
                if (element == null)
                {
                    return string.Empty;
                }

                sb.Append(GetPlainText(element));
                return sb.ToString();
            }

            public string GetPlainText(OpenXmlElement element)
            {
                StringBuilder PlainTextInWord = new StringBuilder();
                foreach (OpenXmlElement section in element.Elements())
                {
                    switch (section.LocalName)
                    {
                        // Text
                        case "t":
                            PlainTextInWord.Append(section.InnerText);
                            break;

                        case "cr":                          // Carriage return
                        case "br":                          // Page break
                            PlainTextInWord.Append(Environment.NewLine);
                            break;

                        // Tab
                        case "tab":
                            PlainTextInWord.Append("\t");
                            break;

                        // Paragraph
                        case "p":
                            PlainTextInWord.Append(GetPlainText(section));
                            PlainTextInWord.AppendLine(Environment.NewLine);
                            break;

                        default:
                            PlainTextInWord.Append(GetPlainText(section));
                            break;
                    }
                }

                return PlainTextInWord.ToString();
            }

            #region IDisposable interface

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                // Protect from being called multiple times.
                if (disposed)
                {
                    return;
                }

                if (disposing)
                {
                    // Clean up all managed resources.
                    if (this.package != null)
                    {
                        this.package.Dispose();
                    }
                }
                disposed = true;
            }
            #endregion
        }
    }
}
