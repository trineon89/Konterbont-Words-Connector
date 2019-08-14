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
        public TwixlAPIForm()
        {
            InitializeComponent();
        }

        private void btnGetCategories_Click(object sender, EventArgs e)
        {
            TwixlAPI twixlAPI = new TwixlAPI();
            twixlAPI.getCategories();
            Console.WriteLine("");
        }

        private void btnGetIssues_Click(object sender, EventArgs e)
        {
            TwixlAPI twixlAPI = new TwixlAPI();
            twixlAPI.getIssues();
        }
    }
}
