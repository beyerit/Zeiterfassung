using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zeiterfassung
{
    public partial class FormAbout : Form
    {
        string buildVersion = "10010j";

        public FormAbout()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            //load version number
            lblVersion.Text = "Version: " + Application.ProductVersion;

            //get copyright year
            if (DateTime.Now.Year > 2023)
            {
                lblCopyright.Text = "Copyright © 2022 - " + DateTime.Now.Year + " Christoph Beyer, Schindler AG";
            }
        }

        private void lblVersion_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show("Build version " + buildVersion);
        }
    }
}
