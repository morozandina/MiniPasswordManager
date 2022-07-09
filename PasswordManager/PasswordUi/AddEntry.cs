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
    public partial class AddEntry : Form
    {
        public Entry Entry { get; set; }

        public AddEntry()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Entry = new Entry()
            {
                Title = titleBox.Text,
                UserName = userBox.Text,
                Password = passBox.Text,
                Url = urlBox.Text,
                Notes = notesBox.Text
            };
            Close();
        }

        public void ShowStartValue(Entry entry)
        {

            titleBox.Text = entry.Title;
            userBox.Text = entry.UserName;
            passBox.Text = entry.Password;
            urlBox.Text = entry.Url;
            notesBox.Text = entry.Notes;
        }

        private void repeatBox_TextChanged(object sender, EventArgs e)
        {
            if (repeatBox.Text == passBox.Text)
            {
                repeatBox.BackColor = Color.White;
                okButton.Enabled = true;
            }
            else
            {
                repeatBox.BackColor = Color.LightCoral;
                okButton.Enabled = false;
            }
        }

        private void passBox_TextChanged(object sender, EventArgs e)
        {
            if (repeatBox.Text != passBox.Text)
            {
                repeatBox.BackColor = Color.LightCoral;
                okButton.Enabled = false;
            }
            else
            {
                repeatBox.BackColor = Color.White;
                okButton.Enabled = true;
            }

            chText.Text = passBox.Text.Length + " ch.";
        }

        public void SetPanelParam(PanelType _panelType)
        {
            switch (_panelType)
            {
                case PanelType.Add:
                    Text = "Add Entry";
                    titlePanel.Text = "Add Entry";
                    subtitleText.Text = "Create a new entry.";
                    break;
                case PanelType.Edit:
                    Text = "Edit Entry";
                    titlePanel.Text = "Edit Entry";
                    subtitleText.Text = "You're editing an existing entry.";
                    break;
            }
        }
    }
}
