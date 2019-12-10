using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KonterbontLODConnector
{
    public partial class frmMainProgram : Form
    {
        private static Settings settings;

        private static frmMainProgram instance = null;
        public static frmMainProgram getInstance()
        {
            return instance;
        }

        public frmMainProgram()
        {
            InitializeComponent();
            instance = this;
            settings = new Settings();
        }

        private void artikelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //New Article Click
        }

        private void opmachenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open Article Click
        }

        private void magazinnOpmachenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open Magazine Click
        }

        private void zoumachenxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Close Click
            Application.Exit();
        }

        private void programmAstellungenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open Settings
            settings.ShowDialog();
        }
    }

    public class Helpers
    {
       //
    }
}
