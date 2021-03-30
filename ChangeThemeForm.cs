using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BooksReader
{
    public partial class ChangeThemeForm : Form
    {
        public ChangeThemeForm()
        {
            InitializeComponent();
        }

        private void txtBtn_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                label1.ForeColor = colorDialog1.Color;
            }
        }

        private void backgroundBtn_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                panel1.BackColor = colorDialog1.Color;
            }
        }

        public List<Color> SetNewColors()
        {
            List<Color> colors = new List<Color>();
            
            colors.Add(panel1.BackColor);
            colors.Add(label1.ForeColor);
            
            return colors;
        }

        private void doneBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
