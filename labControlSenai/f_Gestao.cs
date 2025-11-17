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
    public partial class f_Gestao : Form
    {
        public string Id { get; private set; }
        string linhaDB = "server=localhost;database=labControle_db;uid=root;pwd='';";
        public f_Gestao(string id)
        {
            InitializeComponent();
            buscarTodosMateriais();
            MateriaisAbaixoEstoque();
            Id = id;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            buscarMaterial();
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
                   WHERE id LIKE @busca;";

            using (MySqlConnection conexao = new MySqlConnection(linhaDB))
            {
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

        private void MateriaisAbaixoEstoque()
        {
            string sql = "SELECT id as Código," +
                " nome as 'Nome'," +
                " tipo as 'Tipo'," +
                " quantidade as 'Estoque'," +
                " minimo as 'Quantidade Mínima'" +
                " FROM materiais where quantidade < minimo;";
            using (MySqlConnection conexaoDB = new MySqlConnection(linhaDB))
            {
                try
                {
                    conexaoDB.Open();

                    MySqlDataAdapter da = new MySqlDataAdapter(sql, conexaoDB);
                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    dtgMateriaisEstqMin.DataSource = dt;
                    if (dt.Rows.Count > 0)
                    {
                        lblAviso.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao buscar dados: " + ex.Message);
                }
            }
        }

        private void cadastrarMovimentacoes()
        {
            MySqlConnection conexao = new MySqlConnection(linhaDB);
            string idMaterial = txtBuscar.Text;
            string idUsuario = Id;
            string quantidade = txtQuantidade.Text;
            string data = dtpData.Value.ToString("yyyy-MM-dd");
            string tipo = "";
            string sqlUpdate = "";

            if (rdbEntrada.Checked)
            {
                tipo = "Entrada";
                sqlUpdate = @"UPDATE materiais 
                   SET quantidade = quantidade + @quantidade
                   WHERE id = @idMaterial;";

            }
            else if (rdbSaida.Checked)
            {
                tipo = "Saída";
                sqlUpdate = @"UPDATE materiais 
                   SET quantidade = quantidade - @quantidade
                   WHERE id = @idMaterial;";
            }
            else
            {
                MessageBox.Show("Selecione ao menos um tipo de movimentação.",
                                        "ERRO",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning
                                        );
            }

            if (quantidade == "")
            {
                MessageBox.Show("Todos os campos devem ser preenchidos.",
                        "ERRO",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                        );
            }
            else if (!quantidade.All(char.IsDigit))
            {
                MessageBox.Show("Digite apenas números nos campo de quantidade",
                        "ERRO",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                        );
            }
            else
            {
                using (conexao)
                {
                    string sql = $"INSERT INTO movimentacoes(id_material, id_usuario, tipo_movimentacao, quantidade, data_movimentacao) " +
                                 $"VALUES (@idMaterial, @idUsuario, @tipo, @quantidade, @data);" + sqlUpdate;
                    MySqlCommand cmd = new MySqlCommand(sql, conexao);

                    cmd.Parameters.AddWithValue("@idMaterial", idMaterial );
                    cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@tipo", tipo);
                    cmd.Parameters.AddWithValue("@quantidade", quantidade);
                    cmd.Parameters.AddWithValue("@data", data);
                    try
                    {
                        conexao.Open();
                        int resultado = cmd.ExecuteNonQuery();

                        if (resultado > 0)
                        {
                            MessageBox.Show("Movimentação efetuada!",
                                "Concluído",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                                );
                            buscarTodosMateriais();
                            MateriaisAbaixoEstoque();
                        }
                        else
                        {
                            MessageBox.Show("Erro ao efetuar Movimentação!",
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

        private void btnMovimentacao_Click(object sender, EventArgs e)
        {
            cadastrarMovimentacoes();
        }
    }
}
