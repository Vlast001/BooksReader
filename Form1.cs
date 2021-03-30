﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BooksReader
{
    public partial class Form1 : Form
    {
        private List<string> booksPath;
        private string filePath = "";
        public Form1()
        {
            InitializeComponent();
            booksPath = new List<string>();

            addBookBtn.Enabled = false;
            closeBookBtn.Enabled = false;
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Parallel.Invoke(ReadFile);

        }

        private void ReadFile()
        {
            string txt = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName.EndsWith(".txt"))
                    {
                        filePath = openFileDialog.FileName;
                        txt = File.ReadAllText(filePath);
                        richTextBox1.Text = txt;
                    }
                    else if (openFileDialog.FileName.EndsWith(".docx") || openFileDialog.FileName.EndsWith(".doc"))
                    {
                        object readOnly = true;
                        object visible = true;
                        object save = false;
                        object fileName = openFileDialog.FileName;
                        filePath = openFileDialog.FileName;
                        object missing = Type.Missing;
                        object newTemplate = false;
                        object docType = 0;
                        Microsoft.Office.Interop.Word._Document oDoc = null;
                        Microsoft.Office.Interop.Word._Application oWord = new Microsoft.Office.Interop.Word.Application() { Visible = false };
                        oDoc = oWord.Documents.Open(
                            ref fileName, ref missing, ref readOnly, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref visible,
                            ref missing, ref missing, ref missing, ref missing);
                        oDoc.ActiveWindow.Selection.WholeStory();
                        oDoc.ActiveWindow.Selection.Copy();
                        IDataObject data = Clipboard.GetDataObject();
                        richTextBox1.Rtf = data.GetData(DataFormats.Rtf).ToString();
                        oWord.Quit(ref missing, ref missing, ref missing);
                    }
                    else
                    {
                        MessageBox.Show("Need open another format");
                    }

                }
            }
        }

        private void ReadFileFromData(string filePath)
        {
            string txt = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.FileName = filePath;
                if (openFileDialog.FileName.EndsWith(".txt"))
                {
                    txt = File.ReadAllText(filePath);
                    richTextBox1.Text = txt;
                }
                else if (openFileDialog.FileName.EndsWith(".docx") || openFileDialog.FileName.EndsWith(".doc"))
                {
                    object readOnly = true;
                    object visible = true;
                    object save = false;
                    object fileName = openFileDialog.FileName;
                    filePath = openFileDialog.FileName;
                    object missing = Type.Missing;
                    object newTemplate = false;
                    object docType = 0;
                    Microsoft.Office.Interop.Word._Document oDoc = null;
                    Microsoft.Office.Interop.Word._Application oWord = new Microsoft.Office.Interop.Word.Application() { Visible = false };
                    oDoc = oWord.Documents.Open(
                        ref fileName, ref missing, ref readOnly, ref missing,
                        ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref visible,
                        ref missing, ref missing, ref missing, ref missing);
                    oDoc.ActiveWindow.Selection.WholeStory();
                    oDoc.ActiveWindow.Selection.Copy();
                    IDataObject data = Clipboard.GetDataObject();
                    richTextBox1.Rtf = data.GetData(DataFormats.Rtf).ToString();
                    oWord.Quit(ref missing, ref missing, ref missing);
                }
                else
                {
                    MessageBox.Show("Need open another format");
                }

            }
        }


        private void closeBookBtn_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void addBookBtn_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text != "" || richTextBox1.Text != null)
            {
                if (!booksPath.Contains(filePath))
                {
                    listBox1.Items.Add(Path.GetFileName(filePath));
                    booksPath.Add(filePath);
                }
                else
                {
                    listBox1.SelectedItem = Path.GetFileName(filePath);
                }
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            string path = @"..\..\Data\log.txt";
            try
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        booksPath.Add(line);
                        listBox1.Items.Add(Path.GetFileName(line));
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
           
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string path = @"..\..\Data\log.txt";
            try
            {
                using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
                {
                    foreach (var t in booksPath)
                    {
                        sw.WriteLine(t);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(richTextBox1.Text))
            {
                addBookBtn.Enabled = false;
                closeBookBtn.Enabled = false;
            }
            else
            {
                addBookBtn.Enabled = true;
                closeBookBtn.Enabled = true;
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            Parallel.Invoke(() =>
            {
                ReadFileFromData(booksPath[listBox1.SelectedIndex]);
            });
        }

        
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Assembly assem = typeof(Form1).Assembly;
            //AssemblyName assemName = assem.GetName();

            //MessageBox.Show($"{ assemName.Version.Major} {assemName.Version.Minor}");

            Assembly assembly = typeof(Form1).Assembly;
            var titleAttribute = assembly.GetCustomAttributes<AssemblyTitleAttribute>().FirstOrDefault();
            var descriptionAttribute = assembly.GetCustomAttributes<AssemblyDescriptionAttribute>().FirstOrDefault();

            MessageBox.Show($"This assembly title is {titleAttribute?.Title}" +
                            $"\n{descriptionAttribute?.Description}" +
                            $"\n{assembly.CodeBase}" +
                            $"\n{assembly.FullName}");
        }

        private void authorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Assembly assembly = typeof(Form1).Assembly;
            var company = assembly.GetCustomAttributes<AssemblyCompanyAttribute>().FirstOrDefault();
            MessageBox.Show($"{company?.Company}");
        }
    }
}