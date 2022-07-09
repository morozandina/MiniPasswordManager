
using System.Windows.Forms;
using PasswordBl.Model;

namespace PasswordUi
{
    public partial class AddGroup : Form
    {
        public Group Group { get; set; }

        public AddGroup()
        {
            InitializeComponent();
        }

        public void SetPanelParam(PanelType _panelType)
        {
            switch (_panelType)
            {
                case PanelType.Add:
                    Text = "Add Group";
                    titlePanel.Text = "Add Group";
                    subtitleText.Text = "Create a new entry group.";
                    break;
                case PanelType.Edit:
                    Text = "Edit Group";
                    titlePanel.Text = "Edit Group";
                    subtitleText.Text = "Edit properties of the currently selected group.";
                    break;
            }
        }

        private void okButton_Click(object sender, System.EventArgs e)
        {
            Group = new Group()
            {
                Name = nameBox.Text,
                Notes = notesBox.Text,
            };
            Close();
        }

        public void ShowStartValue(Group group)
        {

            nameBox.Text = group.Name;
            notesBox.Text = group.Notes;
        }
    }
}
