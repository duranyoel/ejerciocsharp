using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace pruebaschar
{
    public partial class frmVendedor : Form
    {
        public frmVendedor()
        {
            InitializeComponent();
        }

        private void frmVendedor_Load(object sender, EventArgs e)
        {
            this.Consulta();
        }
        private void Presentar(string cod)
        {
            Form3 ofrm = new Form3();
                      
            App_Code.Clientes oCliente = new App_Code.Clientes(cod);
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.Name == "Form3")
                {
                    ofrm = (Form3)frm;
                    App_Code.Vendedor oVendedor = new App_Code.Vendedor(cod);
                    ofrm.codVendedor.Text = oVendedor.Codigo;
                    ofrm.txtVendedor.Text = oVendedor.Nombre;
                    this.Close();
                    break;

                }
            }


        }
        public void Consulta()
        {

            DataTable oTabla = new DataTable();
            DataSet oDataSet = new DataSet();
            SqlConnection oConexion = new SqlConnection(new App_Code.Base().Sql);
            SqlDataAdapter oAdaptador = new SqlDataAdapter(
                "SELECT CODVEND,NOMBRE FROM VENDEDOR " +
                "WHERE " +
                    "CODVEND LIKE '%" + this.textBox1.Text + "%' OR " +
                    "NOMBRE LIKE '%" + this.textBox1.Text + "%'" +
                "", oConexion);

            
            oConexion.Open();
            oAdaptador.Fill(oDataSet, "tabla");
            oTabla = oDataSet.Tables["tabla"];
            oConexion.Close();
            this.dgResultados.DataSource = oTabla;

            //Encabezado
            this.dgResultados.Columns[0].HeaderText = "Codigo";
            this.dgResultados.Columns[1].HeaderText = "Nombre";
           


        }

        private void dgResultados_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.Presentar(this.dgResultados.Rows[int.Parse(e.RowIndex.ToString())].Cells[0].Value.ToString());
        }
    }
}
