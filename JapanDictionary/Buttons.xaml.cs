using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using static JapanDictionary.DocReader;

namespace JapanDictionary
{
    public partial class Buttons : UserControl
    {

        public string Result;
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
                Result = getWordPlainText.ReadWordDocument();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                Result = String.Empty;
            }
            finally
            {
                if (getWordPlainText != null)
                {
                    getWordPlainText.Dispose();
                }
            }
        }

        /// <summary>
        ///  Save the text to text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void btnSaveAs_Click(object sender, EventArgs e)
        //{
        //    using (SaveFileDialog savefileDialog = new SaveFileDialog())
        //    {
        //        savefileDialog.Filter = "txt document(*.txt)|*.txt";

        //        // default file name extension
        //        savefileDialog.DefaultExt = ".txt";

        //        // Retore the directory before closing
        //        savefileDialog.RestoreDirectory = true;
        //        if (savefileDialog.ShowDialog() == DialogResult.OK)
        //        {
        //            string filename = savefileDialog.FileName;
        //            rtbText.SaveFile(filename, RichTextBoxStreamType.PlainText);
        //            MessageBox.Show("Save Text file successfully, the file path is： " + filename);
        //        }
        //    }
        //}

    }
}
