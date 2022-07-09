using System;
using System.Windows.Forms;
using PasswordBl.Model;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;
using System.Linq;

namespace PasswordUi
{
    [Serializable]
    public enum PanelType
    {
        Add = 0,
        Edit = 1,
    }

    public partial class Main : Form
    {
        public List<Group> Group { get; set; } = new List<Group>();
        public Database Database { get; set; } = new Database();

        public static FileInfo sourceFile;


        private Group _currentGroup;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            ShowSelectedStatusDebug();
            startTreeView();

        }

        private void SetButtonStart(bool _state)
        {
            addGroupToolStripMenuItem.Enabled = _state;
            editGroupToolStripMenuItem.Enabled = _state;
            addEntryToolStripMenuItem.Enabled = _state;
            selectAllToolStripMenuItem.Enabled = _state;
        }
        /// ////////////////////////////////////////////////////////////////////////////////////////////
        /// List View
        /// ////////////////////////////////////////////////////////////////////////////////////////////
        
        // Add to list view
        private void addEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new AddEntry();
            form.SetPanelParam(PanelType.Add);

            if (form.ShowDialog() == DialogResult.OK)
            {
                var listViewItem = new ListViewItem(RowComponents(form.Entry));
                form.Entry.Group = GetIndexOfTreeview(treeView1.SelectedNode);
                _currentGroup.Entries.Add(form.Entry);


                dbList.Items.Add(listViewItem);
                ShowSelectedStatusDebug();
            }
        }

        // Delete a row from list
        private void deleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dbList.SelectedItems.Count > 0)
                if (MessageBox.Show($"Are you sure you want to remove selected entry ?\r\n\r\n-{_currentGroup.Entries[dbList.SelectedItems[0].Index]}", "Remove Entry", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    foreach (ListViewItem item in dbList.SelectedItems)
                        item.Remove();

            UpdateListView();
        }

        // On select a row
        private void dbList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtonsState();
            UpdateListView();
        }

        // Open edit panel and save changes
        private void editEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new AddEntry();
            form.ShowStartValue(_currentGroup.Entries[dbList.SelectedItems[0].Index]);
            form.SetPanelParam(PanelType.Edit);

            if (form.ShowDialog() == DialogResult.OK)
            {
                var itemsList = RowComponents(form.Entry);

                dbList.SelectedItems[0].SubItems.Clear();
                dbList.SelectedItems[0].Text = form.Entry.Title;
                for (var i = 1; i < itemsList.Length; i++)
                {
                    dbList.SelectedItems[0].SubItems.Add(itemsList[i]);
                }
            }
        }

        // From entry to string
        private string[] RowComponents(Entry entry)
        {
            string passwordChar = "";
            for (int i = 0; i < entry.Password.Length; i++)
                passwordChar += "*";

            string[] row = { entry.Title, entry.UserName, passwordChar, entry.Url, entry.Notes };
            return row;
        }

        // Update
        private void UpdateListView()
        {
            ShowSelectedDebug();
            ShowSelectedStatusDebug();
        }
        private void SetButtonsState()
        {
            deleteEntryToolStripMenuItem.Enabled = dbList.SelectedItems.Count > 0;
            editEntryToolStripMenuItem.Enabled = dbList.SelectedItems.Count == 1;
            copyUserNameToolStripMenuItem.Enabled = HasCell(1);
            copyPasswordToolStripMenuItem.Enabled = HasCell(2);
            uRLsToolStripMenuItem.Enabled = HasCell(3);
        }
        private bool HasCell(int _cellNr)
        {
            return dbList.SelectedItems.Count == 1 && !string.IsNullOrEmpty(dbList.SelectedItems[0].SubItems[_cellNr].Text);
        }
        /// ////////////////////////////////////////////////////////////////////////////////////////////
        /// Debug
        /// ////////////////////////////////////////////////////////////////////////////////////////////
        private void ShowSelectedDebug()
        {
            string debug = "";
            string passwordChar = "";
            int _index = dbList.FocusedItem.Index;
            var _currentEntry = _currentGroup.Entries[_index];
            
            debug += $"Group: {_currentGroup.Name}, ";

            if (!string.IsNullOrEmpty(_currentEntry.Title))
                debug += $"Title: {_currentEntry.Title}, ";

            if (!string.IsNullOrEmpty(_currentEntry.UserName))
                debug += $"UserName: {_currentEntry.UserName}, ";

            if (!string.IsNullOrEmpty(_currentEntry.Password))
                for (int i = 0; i < _currentEntry.Password.Length; i++)
                    passwordChar += "*";
            debug += $"Password: {passwordChar}, ";

            if (!string.IsNullOrEmpty(_currentEntry.Url))
                debug += $"URL: {_currentEntry.Url}, ";

            if (!string.IsNullOrEmpty(_currentEntry.Notes))
                debug += $"\r\n\r\n{_currentEntry.Notes}"; 

            DebugText.Text = debug;
        }
        private void ShowSelectedStatusDebug()
        {
            DebugSelected.Text = $"{dbList.SelectedItems.Count} of {dbList.Items.Count} selected";
        }
        /// ////////////////////////////////////////////////////////////////////////////////////////////
        /// Select All
        /// ////////////////////////////////////////////////////////////////////////////////////////////
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in dbList.Items)
                item.Selected = true;
        }
        /// ////////////////////////////////////////////////////////////////////////////////////////////
        /// Copy in clipboard and open Link
        /// ////////////////////////////////////////////////////////////////////////////////////////////
        private int counter;
        private void copyUserNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, dbList.SelectedItems[0].SubItems[1].Text);
            StartTimer();
        }
        private void copyPasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, dbList.SelectedItems[0].SubItems[2].Text);
            StartTimer();
        }
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, dbList.SelectedItems[0].SubItems[3].Text);
            StartTimer();
        }
        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var url = _currentGroup.Entries[dbList.SelectedItems[0].Index].Url;
            try
            {
                Process.Start(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(dbList.SelectedItems[0].SubItems[3].Text + "\r\n\r\n" + ex.Message, "URL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Countdown progress bar
        private void StartTimer()
        {
            counter = 12;
            progressBar1.Visible = true;
            progressBar1.Value = counter;
            timer1 = new Timer();
            timer1.Tick += new EventHandler(count_down);
            timer1.Interval = 1000;
            timer1.Start();
            DebugStatus.Text = "Data copied to clipboard. Clipboard will be cleared in 12 seconds.";
        }
        private void count_down(object sender, EventArgs e)
        {
            counter--;
            progressBar1.Increment(-1);
            if (counter == 0)
            {
                timer1.Stop();
                progressBar1.Value = counter;
                progressBar1.Visible = false;
                Clipboard.Clear();
                DebugStatus.Text = "Ready.";
            }
        }

        /// ////////////////////////////////////////////////////////////////////////////////////////////
        /// Tree View
        /// ////////////////////////////////////////////////////////////////////////////////////////////
        private void startTreeView()
        {


            treeView1.ExpandAll();
            treeView1.BeforeCollapse += ((object sender, TreeViewCancelEventArgs e) => { e.Cancel = true; });

            // Add event handlers for the required drag events.  
            treeView1.ItemDrag += new ItemDragEventHandler(treeView1_ItemDrag);
            treeView1.DragEnter += new DragEventHandler(treeView1_DragEnter);
            treeView1.DragOver += new DragEventHandler(treeView1_DragOver);
            treeView1.DragDrop += new DragEventHandler(treeView1_DragDrop);
        }

        // Drag And Drop Operation
        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Move the dragged node when the left mouse button is used.  
            if (e.Button == MouseButtons.Left)
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }
        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }
        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the mouse position.  
            Point targetPoint = treeView1.PointToClient(new Point(e.X, e.Y));

            // Select the node at the mouse position.  
            treeView1.SelectedNode = treeView1.GetNodeAt(targetPoint);
        }
        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            Point targetPoint = treeView1.PointToClient(new Point(e.X, e.Y));

            TreeNode targetNode = treeView1.GetNodeAt(targetPoint);

            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
 
            if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode))
            {
                if (e.Effect == DragDropEffects.Move)
                {
                    draggedNode.Remove();
                    targetNode.Nodes.Add(draggedNode);
                }
                else if (e.Effect == DragDropEffects.Copy)
                {
                    targetNode.Nodes.Add((TreeNode)draggedNode.Clone());
                }
                targetNode.Expand();
            }
        }
        private bool ContainsNode(TreeNode node1, TreeNode node2)
        {  
            if (node2.Parent == null) return false;
            if (node2.Parent.Equals(node1)) return true;

            return ContainsNode(node1, node2.Parent);
        }
        
        // Add to tree view
        private void addGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new AddGroup();
            form.SetPanelParam(PanelType.Add);

            if (form.ShowDialog() == DialogResult.OK)
            {
                var nodeToAdd = treeView1.SelectedNode.Nodes.Add(form.Group.Name);
                form.Group.GroupId = GetIndexOfTreeview(nodeToAdd);
                Group.Add(form.Group);
                treeView1.ExpandAll();
            }
        }

        // Edit group
        private void editGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new AddGroup();
            var _group = Group.Find(item => item.GroupId == GetIndexOfTreeview(treeView1.SelectedNode));
            form.ShowStartValue(_group);
            form.SetPanelParam(PanelType.Edit);

            if (form.ShowDialog() == DialogResult.OK)
            {
                treeView1.SelectedNode.Text = form.Group.Name;
                _group.Name = form.Group.Name;
                _group.Notes = form.Group.Notes;

                treeView1.ExpandAll();
            }
        }

        // Remove group
        private void removeGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to remove selected group ?", "Remove Entry", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                treeView1.SelectedNode.Remove();
        }

        // Button control
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _currentGroup = Group.Find(item => item.GroupId == GetIndexOfTreeview(treeView1.SelectedNode));
            ShowListView();
            DebugText.Text = "";

            if (treeView1.SelectedNode == treeView1.Nodes[0])
                removeGroupToolStripMenuItem.Enabled = false;
            else
                removeGroupToolStripMenuItem.Enabled = true;
        }

        // Get index of object
        private int GetIndexOfTreeview(TreeNode node)
        {
            var tempNumber = node.Level + "" + node.Index;
            int index = int.Parse(tempNumber);

            return index;
        }

        // Show list view by tree view
        private void ShowListView()
        {
            dbList.Items.Clear();

            if (_currentGroup.Entries.Count > 0)
                foreach (var item in _currentGroup.Entries)
                {
                    var listViewItem = new ListViewItem(RowComponents(item));
                    dbList.Items.Add(listViewItem);
                }

            ShowSelectedStatusDebug();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var jsonString = JsonConvert.SerializeObject(Group);
            var fileNameLocation = Database.Location;
            sourceFile = new FileInfo(fileNameLocation);
            File.WriteAllText(fileNameLocation, jsonString);
            Encrypt(sourceFile, "1234");
            
        }

        /// ////////////////////////////////////////////////////////////////////////////////////////////
        /// Encrypt / Decrypt
        /// ////////////////////////////////////////////////////////////////////////////////////////////
        private const int SaltSize = 8;

        public static void Encrypt(FileInfo targetFile, string password)
        {
            var keyGenerator = new Rfc2898DeriveBytes(password, SaltSize);
            var rijndael = Rijndael.Create();

            // BlockSize, KeySize in bit --> divide by 8
            rijndael.IV = keyGenerator.GetBytes(rijndael.BlockSize / 8);
            rijndael.Key = keyGenerator.GetBytes(rijndael.KeySize / 8);

            using (var fileStream = targetFile.Create())
            {
                // write random salt
                fileStream.Write(keyGenerator.Salt, 0, SaltSize);

                using (var cryptoStream = new CryptoStream(fileStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    
                }
            }
        }

        public static void Decrypt(FileInfo sourceFile, string password)
        {
            // read salt
            var fileStream = sourceFile.OpenRead();
            var salt = new byte[SaltSize];
            fileStream.Read(salt, 0, SaltSize);

            // initialize algorithm with salt
            var keyGenerator = new Rfc2898DeriveBytes(password, salt);
            var rijndael = Rijndael.Create();
            rijndael.IV = keyGenerator.GetBytes(rijndael.BlockSize / 8);
            rijndael.Key = keyGenerator.GetBytes(rijndael.KeySize / 8);

            // decrypt
            using (var cryptoStream = new CryptoStream(fileStream, rijndael.CreateDecryptor(), CryptoStreamMode.Read))
            {
                using (var streamReader = new StreamReader(cryptoStream))
                {

                    MessageBox.Show($"{streamReader.ReadToEnd()}", "Information", MessageBoxButtons.OK);
                }

            }
        }

        /// ////////////////////////////////////////////////////////////////////////////////////////////
        /// First Password
        /// ////////////////////////////////////////////////////////////////////////////////////////////

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog _patch = new OpenFileDialog();
            _patch.Filter = "Password manager files (*.pmf)|*.pmf";
            _patch.InitialDirectory = "C:/Users/User/Documents/";
            _patch.FilterIndex = 1;
            _patch.RestoreDirectory = true;

            if (_patch.ShowDialog() == DialogResult.OK)
            {
                var form = new OpenApplication();

                if (form.ShowDialog() == DialogResult.OK)
                {
                    Decrypt(sourceFile, form.DatabasePassword);
                }
            }
        }

        /// ////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions
        /// ////////////////////////////////////////////////////////////////////////////////////////////
        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite, Formatting.Indented);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(fileContents);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// ////////////////////////////////////////////////////////////////////////////////////////////
        /// Default DataBase & Create new file
        /// ////////////////////////////////////////////////////////////////////////////////////////////
        /// 
        private Database DefaultList()
        {
            Database database = new Database()
            {
                Name = "",
                Location = "",
                Password = "1234",

                Groups = new List<Group>()
                {
                    new Group
                    {
                        GroupId = 0,
                        Name = "Database",
                        Notes = "",

                        Entries = new List<Entry>()
                        {
                            new Entry()
                            {
                                Group = 0,
                                Title = "Sample Entry",
                                UserName = "User Name",
                                Password = "Password",
                                Url = "https://www.google.com",
                                Notes = "Notes"
                            },
                            new Entry()
                            {
                                Group = 0,
                                Title = "Sample Entry #2",
                                UserName = "Michael321",
                                Password = "12345",
                                Url = "https://www.google.com",
                                Notes = "Notes"
                            }
                        }
                    },
                    new Group
                    {
                        GroupId = 10,
                        Name = "General",
                        Notes = "",

                        Entries= new List<Entry>(),
                    },
                    new Group
                    {
                        GroupId = 11,
                        Name = "Windows",
                        Notes = "",

                        Entries= new List<Entry>(),
                    },
                    new Group
                    {
                        GroupId = 12,
                        Name = "Network",
                        Notes = "",

                        Entries= new List<Entry>(),
                    },
                    new Group
                    {
                        GroupId = 13,
                        Name = "Internet",
                        Notes = "",

                        Entries= new List<Entry>(),
                    },
                    new Group
                    {
                        GroupId = 14,
                        Name = "eMail",
                        Notes = "",

                        Entries= new List<Entry>(),
                    }
                }
            };
            return database;
        }

        private void newFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog _patch = new SaveFileDialog();
            _patch.Filter = "Password manager files (*.pmf)|*.pmf";
            _patch.InitialDirectory = "C:/Users/User/Documents/";
            _patch.FileName = "Database";
            _patch.FilterIndex = 1;
            _patch.RestoreDirectory = true;

            if (_patch.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(_patch.FileName);
                Database = DefaultList();
                Database.Name = fileInfo.Name;
                Database.Location = fileInfo.FullName;
                WriteToJsonFile(Database.Location, Database);

                PutDatabaseInList();
            }
        }

        private void PutDatabaseInList()
        {
            // Group
            treeView1.Nodes.Add(Database.Groups[0].Name);
            Group.Add(Database.Groups[0]);
            for (var i = 1; i < Database.Groups.Count; i++)
            {
                var index = (int)(Database.Groups[i].GroupId.ToString()[0]) - 48;

                if (index <= 1)
                    treeView1.Nodes[0].Nodes.Add(Database.Groups[i].Name);
                else
                    treeView1.Nodes[index].Nodes.Add(Database.Groups[i].Name);

                Group.Add(Database.Groups[i]);
            }

            startTreeView();

            treeView1.SelectedNode = treeView1.Nodes[0];
            _currentGroup = Group[0];

            SetButtonStart(true);
            ShowListView();
            ShowSelectedStatusDebug();
        }
    }
}
