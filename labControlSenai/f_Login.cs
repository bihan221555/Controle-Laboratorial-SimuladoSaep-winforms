using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace concessionaria
{
    public partial class f_Login : Form
    {
        string linhaDB = "server=localhost;database=labcontrole_db;uid=root;pwd='';";
        string nome = "";
        string id = "";
        public f_Login()
        {
            InitializeComponent();
        }

        private void validarLogin()
        {
            string loginDigitado = txtLogin.Text;
            string senhaDigitada = txtSenha.Text;

            var conexao = new MySqlConnection(linhaDB);

            

            using (conexao)
            {
                string sql = "SELECT * FROM usuarios WHERE login = @login and senha = @senha;";

                var cmd = new MySqlCommand(sql, conexao);

                cmd.Parameters.AddWithValue("@login", loginDigitado);
                cmd.Parameters.AddWithValue("@senha", senhaDigitada);

                try
                {
                    conexao.Open();
                    var leitor = cmd.ExecuteReader();
                    
                    if (leitor.HasRows)
                    {
                        leitor.Read();
                        nome = leitor["nome"].ToString();
                        id = leitor["id"].ToString();
                        f_Principal f1 = new f_Principal(nome, id);
                        f1.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Falha na autenticação", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtLogin.Text = string.Empty;
                        txtSenha.Text = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            validarLogin();
        }
    }
}