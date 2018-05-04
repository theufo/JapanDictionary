using System;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

namespace JapanDictionary.Common
{
    public class DocReader
    {
        public class GetWordPlainText : IDisposable
        {
            private readonly WordprocessingDocument _package;
            private bool _disposed;
            private string _fileName;

            public GetWordPlainText(string filepath)
            {
                _fileName = filepath;
                if (string.IsNullOrEmpty(filepath) || !File.Exists(filepath)) throw new Exception("The file is invalid");
                _package = WordprocessingDocument.Open(filepath, true);
            }

            public string ReadWordDocument()
            {
                var sb = new StringBuilder();
                OpenXmlElement element = _package.MainDocumentPart.Document.Body;
                if (element == null) return string.Empty;

                sb.Append(GetPlainText(element));
                return sb.ToString();
            }

            public string GetPlainText(OpenXmlElement element)
            {
                var plainTextInWord = new StringBuilder();
                foreach (var section in element.Elements())
                    switch (section.LocalName)
                    {
                        // Text
                        case "t":
                            plainTextInWord.Append(section.InnerText);
                            break;

                        case "cr": // Carriage return
                        case "br": // Page break
                            plainTextInWord.Append(Environment.NewLine);
                            break;

                        // Tab
                        case "tab":
                            plainTextInWord.Append("\t");
                            break;

                        // Paragraph
                        case "p":
                            plainTextInWord.Append(GetPlainText(section));
                            plainTextInWord.AppendLine(Environment.NewLine);
                            break;

                        default:
                            plainTextInWord.Append(GetPlainText(section));
                            break;
                    }

                return plainTextInWord.ToString();
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
                if (_disposed) return;

                if (disposing)
                    if (_package != null)
                        _package.Dispose();
                _disposed = true;
            }

            #endregion
        }
    }
}