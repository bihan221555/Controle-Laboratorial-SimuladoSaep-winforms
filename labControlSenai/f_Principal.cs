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
    public partial class f_Principal : Form
    {
        public string Id { get; private set; }
        private Form frmAberto;
        public f_Principal()
        {
            InitializeComponent();
            abrirFormulario(new f_Inicio());
        }

        public f_Principal(string nome, string id)
        {
            Id = id;
            InitializeComponent();
            abrirFormulario(new f_Inicio());
            lblNome.Text = nome;

        }
        private void abrirFormulario(Form frm)
        {
            frmAberto?.Close();

            frmAberto = frm;
            frm.TopLevel = false;
            pnlCentral.Controls.Clear();
            pnlCentral.Controls.Add(frm);
            frm.BringToFront();
            frm.Show();
        }
        

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
            f_Login f1 = new f_Login();
            f1.Show();
        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
            abrirFormulario(new f_Inicio());
        }

        private void btnCadMateriais_Click(object sender, EventArgs e)
        {
            abrirFormulario(new f_Cadastro(Id));
        }

        private void btnMovimentacao_Click(object sender, EventArgs e)
        {
            abrirFormulario(new f_Gestao(Id));
        }
    }
}
