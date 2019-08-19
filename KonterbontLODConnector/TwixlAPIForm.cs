using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KonterbontLODConnector
{
    public partial class TwixlAPIForm : Form
    {
        public static TwixlAPI _twixlAPI;

        public TwixlAPIForm()
        {
            InitializeComponent();
        }

        private void btnGetCategories_Click(object sender, EventArgs e)
        {
            frmTwixlCategorySelector _frmTwixlCategorySelector = new frmTwixlCategorySelector();
            _twixlAPI = new TwixlAPI();

            int _issueid = 55071;

            _frmTwixlCategorySelector.cbCategories.Items.Clear();
            foreach (TwixlCategory cat in _twixlAPI._twixlCategories.categories)
            {
                
                _frmTwixlCategorySelector.cbCategories.Items.Add(cat.name, _twixlAPI.IsInCategory(_issueid, cat.id));
            }

           if (_frmTwixlCategorySelector.ShowDialog()== DialogResult.OK)
           {

           }

            if (_twixlAPI == null)
            {
                MessageBox.Show("Api ass net verbonn!");
                return;
            }
            //_twixlAPI.getCategories(_twixlAPIAppData);
        }

        private void btnGetIssues_Click(object sender, EventArgs e)
        {
            //TwixlAPI twixlAPI = new TwixlAPI();
            //twixlAPI.getIssues();
        }

        private void BtnLoadAppData_Click(object sender, EventArgs e)
        {
            //Load AppData
            _twixlAPI = new TwixlAPI();
            Console.WriteLine("");
        }
    }
}
