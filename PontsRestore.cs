using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace PontsRestore
{
    public partial class PontsRestore : Form
    {
        public PontsRestore()
        {
            InitializeComponent();
        }

        private void lblOpenDirectory_Click_1(object sender, EventArgs e)
        {
            FileInfo fi = new FileInfo(@"C:\Caixa Fácil\Imagens do sistema\Sem título.jpg");

            txtDirectory.Text = fi.FullName.ToString();
        }
    }
}
