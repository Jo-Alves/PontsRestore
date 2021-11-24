using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using PontsRestore.Properties;

namespace PontsRestore
{
    public partial class PontsRestore : Form
    {
        public PontsRestore()
        {
            InitializeComponent();
        }

        string fullPath, conn = @"Data Source=LOCALHOST\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";

        private void lblOpenDirectory_Click(object sender, System.EventArgs e)
        {
            DialogResult dr = folderBrowserDialog.ShowDialog();
            if (dr == DialogResult.OK)
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
                        dgvFiles.ClearSelection();
                    }

                    if (files.Length > 0)
                    {
                        Settings.Default["Directory"] = direct;
                        Settings.Default.Save();
                    }
                    else
                        MessageBox.Show("Não foi encontrado nenhum arquivo de backup neste diretório.", "Ponts Restore", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
            if(string.IsNullOrWhiteSpace(txtDirectory.Text))
            {
                MessageBox.Show("Diretório Vazio. Especifique um caminho correto.", "Ponts Restore", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            ListFiles(txtDirectory.Text.Trim());            
        }

        private void PontsRestore_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Settings.Default["Directory"].ToString()))
            {
                txtDirectory.Text = Settings.Default["Directory"].ToString();
                ListFiles(txtDirectory.Text.Trim());
            }
        }

        private void dgvFiles_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                fullPath = dgvFiles.CurrentRow.Cells["colFullPath"].Value.ToString();
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(fullPath))
            {
                try
                {
                    if (ExistDataBase())
                        DropDatabase();

                    RestoreDatabase();
                    dgvFiles.ClearSelection();
                    fullPath = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                MessageBox.Show("Selecione o arquivo para a restauração do Banco.", "Ponts Restore", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void DropDatabase()
        {
            using (SqlConnection connection = new SqlConnection(conn))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("", connection);
                    command.CommandText = "DROP DATABASE dbControleVenda";
                    command.ExecuteNonQuery();
                }
                catch 
                {
                    throw;
                }
            }
        } 
        
        private void RestoreDatabase()
        {
            using (SqlConnection connection = new SqlConnection(conn))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("", connection);
                    command.CommandText = $"RESTORE DATABASE dbControleVenda FROM DISK = '{fullPath}'";
                    command.ExecuteNonQuery();
                }
                catch 
                {
                    throw;
                }
            }
        }

        private void PontsRestore_KeyDown(object sender, KeyEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.O)
                lblOpenDirectory_Click(sender, e);
            else if (e.KeyCode == Keys.Enter)
                btnRestore_Click(sender, e);

        }

        private bool ExistDataBase()
        {
            bool existDataBase = false;
            using (SqlConnection conexao = new SqlConnection(conn))
            {
                SqlCommand command = new SqlCommand("", conexao);
                command.CommandText = "SELECT * from SYS.DATABASES WHERE name = 'dbControleVenda'";
                try
                {
                    conexao.Open();
                    command.ExecuteNonQuery();
                    SqlDataReader dr = command.ExecuteReader();
                    if (dr.Read())
                    {
                        existDataBase = true;
                    }
                }
                catch
                {
                    throw;
                }
            }

            return existDataBase;
        }
    }
}