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
    public partial class f_Inicio : Form
    {
        string linhaDB = "server=localhost;database=concessionaria;uid=root;pwd='';";
        public f_Inicio()
        {
            InitializeComponent();
            
        }
    }
}
