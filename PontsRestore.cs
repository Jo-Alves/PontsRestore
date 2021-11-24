using System;
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

        private void lblOpenDirectory_Click(object sender, System.EventArgs e)
        {
            DialogResult dr = folderBrowserDialog.ShowDialog();
            if(dr == DialogResult.OK)
            {
                txtDirectory.Text = folderBrowserDialog.SelectedPath;
                ListFiles(folderBrowserDialog.SelectedPath);
            }
        }

        private void ListFiles(string direct)
        {
            try
            {
                if (Directory.Exists(direct))
                {
                    string[] files = Directory.GetFiles(direct, "*bak", SearchOption.TopDirectoryOnly);

                    FileInfo fileInfo = null;
                    dgvFiles.Rows.Clear();
                    foreach (string file in files)
                    {
                        fileInfo = new FileInfo(file);

                        dgvFiles.Rows.Add(fileInfo.Name, fileInfo.CreationTime, file);
                    }
                }
                else
                {
                    MessageBox.Show("Diretório não existe!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    dgvFiles.Rows.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtDirectory_Leave(object sender, System.EventArgs e)
        {
            ListFiles(txtDirectory.Text.Trim());
        }
    }
}
