﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KonterbontLODConnector
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class RichTextWPF : UserControl
    {
        public RichTextWPF()
        {
            InitializeComponent();
        }

        public void richTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            frmMainProgram.getInstance().RichTextBox_Click(sender, e);
        }
    }
}
