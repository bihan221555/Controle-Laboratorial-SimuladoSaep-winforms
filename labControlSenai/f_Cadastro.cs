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
    public partial class f_Cadastro : Form
    {
        public string Id { get; private set; }
        string linhaDB = "server=localhost;database=labcontrole_db;uid=root;pwd='';";
        public f_Cadastro()
        {
            InitializeComponent();
            buscarTodosMateriais();
        }

        public f_Cadastro(string id)
        {
            InitializeComponent();
            buscarTodosMateriais();
        }

        private void buscarTodosMateriais()
        {
            string sql = "SELECT id as Código," +
                " nome as 'Nome'," +
                " tipo as 'Tipo'," +
                " quantidade as 'Estoque'," +
                " minimo as 'Quantidade Mínima'" +
                " FROM materiais;";
            using (MySqlConnection conexaoDB = new MySqlConnection(linhaDB))
            {
                try
                {
                    conexaoDB.Open();
                    
                    MySqlDataAdapter da = new MySqlDataAdapter(sql, conexaoDB);
                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    dtgMateriais.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao buscar dados: " + ex.Message);
                }
            }
        }

        private void cadastrarMaterial()
        {
            MySqlConnection conexao = new MySqlConnection(linhaDB);
            string nome = txtNome.Text;
            string tipo = txtTipo.Text;
            string qtdMin = txtQtdMin.Text;
            string estoque = txtEstoque.Text;
            string unidadeMedida = txtUnidadeMedida.Text;

            if (nome == "" || tipo == "" || qtdMin == "" || estoque == "")
            {
                MessageBox.Show("Todos os campos devem ser preenchidos.",
                        "ERRO",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                        );
            }
            else if (!qtdMin.All(char.IsDigit) || !estoque.All(char.IsDigit))
            {
                MessageBox.Show("Digite apenas números nos campos de Estoque e Quantidade Mínima",
                        "ERRO",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                        );
            }
            else
            {
                using (conexao)
                {
                    string sql = $"INSERT INTO materiais(nome, tipo, quantidade, minimo, unidade_medida) " +
                                 $"VALUES (@nome, @tipo, @estoque, @qtdMin, @unidadeMedida)";
                    MySqlCommand cmd = new MySqlCommand(sql, conexao);

                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.Parameters.AddWithValue("@tipo", tipo);
                    cmd.Parameters.AddWithValue("@qtdMin", qtdMin);
                    cmd.Parameters.AddWithValue("@estoque", estoque);
                    cmd.Parameters.AddWithValue("@unidadeMedida", unidadeMedida);
                    try
                    {
                        conexao.Open();
                        int resultado = cmd.ExecuteNonQuery();

                        if (resultado > 0)
                        {
                            MessageBox.Show("Cadastro efetuado!",
                                "Concluído",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                                );
                        }
                        else
                        {
                            MessageBox.Show("Erro ao efetuar cadastro!",
                            "ERRO",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message,
                            "ERRO",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                            );
                    }
                }
            }

        }

        private void buscarMaterial()
        {
            string busca = txtBuscar.Text;

            string sql = @"SELECT id AS Código,
                          nome AS Nome,
                          tipo AS Tipo,
                          quantidade AS Estoque,
                          minimo AS 'Quantidade Mínima',
                          unidade_medida AS 'Unidade de Medida'
                   FROM materiais
                   WHERE nome LIKE @busca;";

            using (MySqlConnection conexao = new MySqlConnection(linhaDB))
            using (MySqlCommand cmd = new MySqlCommand(sql, conexao))
            {
                cmd.Parameters.AddWithValue("@busca", $"%{busca}%");

                try
                {
                    conexao.Open();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        dtgMateriais.DataSource = dt;

                        txtID.Text = dt.Rows[0]["Código"].ToString();
                        txtNome.Text = dt.Rows[0]["Nome"].ToString();
                        txtTipo.Text = dt.Rows[0]["Tipo"].ToString();
                        txtQtdMin.Text = dt.Rows[0]["Quantidade Mínima"].ToString();
                        txtEstoque.Text = dt.Rows[0]["Estoque"].ToString();
                        txtUnidadeMedida.Text = dt.Rows[0]["Unidade de Medida"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Material não encontrado", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dtgMateriais.DataSource = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void editarMaterial()
        {
            string nome = txtNome.Text.Trim();
            string tipo = txtTipo.Text.Trim();
            string qtdMin = txtQtdMin.Text.Trim();
            string estoque = txtEstoque.Text.Trim();
            string unidadeMedida = txtUnidadeMedida.Text.Trim();
            string id = txtID.Text.Trim();

            string sql = @"UPDATE materiais 
                   SET nome = @nome,
                       tipo = @tipo,
                       quantidade = @estoque,
                       minimo = @qtdMin,
                       unidade_medida = @unidadeMedida
                   WHERE id = @id;";

            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(tipo) ||
                string.IsNullOrWhiteSpace(qtdMin) ||
                string.IsNullOrWhiteSpace(estoque))
            {
                MessageBox.Show("Todos os campos devem ser preenchidos.",
                                "ERRO",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(qtdMin, out _) || !int.TryParse(estoque, out _))
            {
                MessageBox.Show("Digite apenas números inteiros nos campos de Estoque e Quantidade Mínima.",
                                "ERRO",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection conexao = new MySqlConnection(linhaDB))
            using (MySqlCommand cmd = new MySqlCommand(sql, conexao))
            {
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cmd.Parameters.AddWithValue("@qtdMin", qtdMin);
                cmd.Parameters.AddWithValue("@estoque", estoque);
                cmd.Parameters.AddWithValue("@unidadeMedida", unidadeMedida);
                cmd.Parameters.AddWithValue("@id", id);

                try
                {
                    conexao.Open();
                    int resultado = cmd.ExecuteNonQuery();

                    if (resultado > 0)
                    {
                        MessageBox.Show("Edição efetuada com sucesso!",
                                        "SUCESSO",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Nenhum registro foi atualizado. Verifique o ID informado.",
                                        "ATENÇÃO",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao editar material:\n" + ex.Message,
                                    "ERRO",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
        }

        private void excluirMaterial()
        {
            MySqlConnection conexao = new MySqlConnection(linhaDB);
            DialogResult escolha = MessageBox.Show("Tem certeza que deseja excluir?",
                                                   "Confirmação",
                                                   MessageBoxButtons.YesNo
                                                   );
            if (escolha == DialogResult.Yes)
            {
                using (conexao)
                {

                    string sql = "DELETE FROM materiais where id = @id;";
                    MySqlCommand cmd = new MySqlCommand(sql, conexao);

                    cmd.Parameters.AddWithValue("@id", txtID.Text);


                    try
                    {
                        conexao.Open();
                        int resultado = cmd.ExecuteNonQuery();

                        if (resultado > 0)
                        {
                            MessageBox.Show("Exclusão efetuada!", "SUCESSO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            btnExcluir.Enabled = false;
                        }
                        else
                        {
                            MessageBox.Show("Erro ao efetuar exclusão!", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            cadastrarMaterial();
            buscarTodosMateriais();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            editarMaterial();
            buscarTodosMateriais();
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            excluirMaterial();
            buscarTodosMateriais();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            buscarMaterial();
        }
    }
}
