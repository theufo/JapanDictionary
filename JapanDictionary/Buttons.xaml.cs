using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using static JapanDictionary.DocReader;

namespace JapanDictionary
{
    public partial class Buttons : UserControl
    {
        public string LoadResult;
        public string SaveResult;

        GetWordPlainText getWordPlainText = null;

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
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                dialog.Filter = "Word document (*.docx)|*.docx";
                dialog.InitialDirectory = Environment.CurrentDirectory;

                // Retore the directory before closing
                dialog.RestoreDirectory = true;
                    fileName = dialog.FileName;
                    PathText.Text = dialog.FileName;
                    //rtbText.Clear();
            }
            return fileName;
        }

        private void ConvertFile(object sender, EventArgs e)
        {
            try
            {
                getWordPlainText = new GetWordPlainText(PathText.Text);
                this.PathText.Clear();
                LoadResult = getWordPlainText.ReadWordDocument();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                LoadResult = String.Empty;
            }
            finally
            {
                if (getWordPlainText != null)
                {
                    getWordPlainText.Dispose();
                }
            }
        }

        public void SaveFile(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "Dictionary";
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = "Csv documents (.csv)|*.csv";

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, SaveResult);
                MessageBox.Show("Save Text file successfully");
            }
        }
    }
}
