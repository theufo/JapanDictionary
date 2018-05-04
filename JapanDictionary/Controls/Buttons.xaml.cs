using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using static JapanDictionary.Common.DocReader;

namespace JapanDictionary
{
    public partial class Buttons
    {
        private GetWordPlainText _getWordPlainText;
        public string LoadResult;
        public string SaveResult;

        public Buttons()
        {
            InitializeComponent();
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            SelectWordFile();
        }

        private string SelectWordFile()
        {
            string fileName = null;
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                dialog.Filter = "Word document (*.docx)|*.docx";
                dialog.InitialDirectory = Environment.CurrentDirectory;

                // Retore the directory before closing
                dialog.RestoreDirectory = true;
                fileName = dialog.FileName;
                PathText.Text = dialog.FileName;
            }

            return fileName;
        }

        private void ConvertFile(object sender, EventArgs e)
        {
            try
            {
                _getWordPlainText = new GetWordPlainText(PathText.Text);
                PathText.Clear();
                LoadResult = _getWordPlainText.ReadWordDocument();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                LoadResult = string.Empty;
            }
            finally
            {
                _getWordPlainText?.Dispose();
            }
        }

        public void SaveFile(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                FileName = "Dictionary",
                DefaultExt = ".csv",
                Filter = "Csv documents (.csv)|*.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, SaveResult);
                MessageBox.Show("Save Text file successfully");
            }
        }
    }
}