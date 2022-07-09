using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PasswordBl.Model;

namespace PasswordUi
{
    public partial class OpenApplication : Form
    {
        public string DatabasePassword;

        public OpenApplication()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DatabasePassword = textBox1.Text;
        }
    }
}
