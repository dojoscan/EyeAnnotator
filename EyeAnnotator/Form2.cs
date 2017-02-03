using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Enter class box pops up after drawing box

namespace EyeAnnotator
{
    public partial class Form2 : Form
    {
        public string objectClass { get; set; }

        public Form2()
        {
            InitializeComponent();
            
        }

        public void continueBtn_Click(object sender, EventArgs e)
        {
            objectClass = listBox1.GetItemText(listBox1.SelectedItem);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
