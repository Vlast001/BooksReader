using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using eBdb.EpubReader;

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

            #region Нарисовать кнопку поиска круглой

            //System.Drawing.Drawing2D.GraphicsPath myPath = new System.Drawing.Drawing2D.GraphicsPath();

            //myPath.AddEllipse(1, 1, searchBtn.Width, searchBtn.Height);
            //// создаем с помощью элипса ту область формы, которую мы хотим видеть
            //Region myRegion = new Region(myPath);
            //// устанавливаем видимую область
            //searchBtn.Region = myRegion;
            

            #endregion
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Parallel.Invoke(ReadFile);
        }

        private void ReadFile()
        {
            richTextBox1.ReadOnly = false;
            string txt = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog(){Filter = "txt files (*.txt)|*.txt|rtf books|*.rtf|ePub books|*.epub|All files (*.*)|*.*", Multiselect = false})
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName.EndsWith(".txt"))
                    {
                        filePath = openFileDialog.FileName;
                        txt = File.ReadAllText(filePath);
                        richTextBox1.Text = txt;
                    }
                    else if(openFileDialog.FileName.EndsWith(".rtf"))
                    {
                        filePath = openFileDialog.FileName;
                        richTextBox1.LoadFile(filePath);
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
                    else if (openFileDialog.FileName.EndsWith(".epub"))
                    {
                        filePath = openFileDialog.FileName;
                        Epub epub = new Epub(filePath);

                        richTextBox1.Text = epub.GetContentAsPlainText();
                    }
                    else
                    {
                        MessageBox.Show("Need open another format");
                    }

                    richTextBox1.ReadOnly = true;
                }
            }
        }

        private void ReadFileFromData(string filePath)
        {
            richTextBox1.ReadOnly = false;
            string txt = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.FileName = filePath;
                if (openFileDialog.FileName.EndsWith(".txt"))
                {
                    txt = File.ReadAllText(filePath);
                    richTextBox1.Text = txt;
                }
                else if (openFileDialog.FileName.EndsWith(".rtf"))
                {
                    richTextBox1.LoadFile(filePath);
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
                else if (openFileDialog.FileName.EndsWith(".epub"))
                {
                    Epub epub = new Epub(filePath);

                    richTextBox1.Text = epub.GetContentAsPlainText();
                }
                else
                {
                    MessageBox.Show("Need open another format");
                }
                richTextBox1.ReadOnly = true;

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

        private void Form1_Load(object sender, EventArgs e)
        {
           
           LoadLocalLibrary();
        }

        private async void LoadLocalLibrary()
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

            MessageBox.Show($"This assembly title is {titleAttribute?.Title}\n" +
                            $"\nDescription: {descriptionAttribute?.Description}" +
                            $"\n\n{assembly.CodeBase}" +
                            $"\n\n{assembly.FullName}");
        }

        private void authorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Assembly assembly = typeof(Form1).Assembly;
            var company = assembly.GetCustomAttributes<AssemblyCompanyAttribute>().FirstOrDefault();
            MessageBox.Show($"{company?.Company}");
        }

        private void changeThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeThemeForm theme = new ChangeThemeForm();
            if (theme.ShowDialog() == DialogResult.OK)
            {
                var themColors = theme.SetNewColors();
                this.BackColor = themColors[0];
                this.ForeColor = themColors[1];
                groupBox1.BackColor = themColors[0];
                groupBox2.BackColor = themColors[0];

                groupBox1.ForeColor = themColors[1];
                groupBox2.ForeColor = themColors[1];
                listBox1.ForeColor = themColors[1];
            }
        }

        private void defaultThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;

            groupBox1.BackColor = SystemColors.Control;
            groupBox2.BackColor = SystemColors.Control;

            groupBox1.ForeColor = SystemColors.ControlText;
            groupBox2.ForeColor = SystemColors.ControlText;
            listBox1.ForeColor = SystemColors.WindowText;
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            int start = 0;
            int end = richTextBox1.Text.LastIndexOf(textBox1.Text);

            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = SystemColors.Control;

            while (start < end)
            {
                richTextBox1.Find(textBox1.Text, start, richTextBox1.TextLength, RichTextBoxFinds.MatchCase);
                richTextBox1.SelectionBackColor = Color.Chartreuse;

                start = richTextBox1.Text.IndexOf(textBox1.Text, start) + 1;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.ZoomFactor += 0.5f;
        }

        private void zoomMinusBtn_Click(object sender, EventArgs e)
        {
            richTextBox1.ZoomFactor -= 0.5f;
        }

        private void changeLocalLibraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeLogFile();
        }

        private void ChangeLogFile()
        {
            string localLibraryPath = @"..\..\Data\log.txt";
            MessageBox.Show("После внесения изменений в файл локальной библиотеки перезапустите приложение");
            process1 = Process.Start(localLibraryPath);
        }

        private void process1_Exited(object sender, EventArgs e)
        {
            if (process1.HasExited)
            {
                MessageBox.Show("True");
            }
            listBox1.Items.Clear();
            LoadLocalLibrary();
        }
    }
}
