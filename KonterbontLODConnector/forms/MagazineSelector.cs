using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KonterbontLODConnector.forms
{
    public partial class MagazineSelector : Form
    {
        private List<classes.ArticleFile> activeMagazineFiles = new List<classes.ArticleFile>();
        public classes.ArticleFile activeMagazineFile;

        public MagazineSelector()
        {
            InitializeComponent();
        }

        public void ClearItemsInView()
        {
            listView1.Items.Clear();
        }

        public void AddItemToView(classes.ArticleFile articleFile)
        {
            if (articleFile == null) return;
            ListViewItem listViewItem = new ListViewItem();
            listViewItem.Text = articleFile.ArticleId + " " + articleFile.ArticleName;
            listViewItem.ImageIndex = 0;
            listViewItem.Tag = articleFile.ArticlePath;
            listViewItem.Name = articleFile.ArticleFileName;
            listView1.Items.Add(listViewItem);

            activeMagazineFiles.Add(articleFile);
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems != null)
            {
                DialogResult = DialogResult.OK;
                activeMagazineFile = activeMagazineFiles[listView1.SelectedItems[0].Index];
            } else
            {
                DialogResult = DialogResult.Cancel;
            }
            Close();
        }
    }
}
