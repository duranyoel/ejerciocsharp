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
    public partial class frmClientes : Form
    {
        public frmClientes()
        {
            InitializeComponent();
        }

        private void Clientes_Load(object sender, EventArgs e)
        {
            this.Consulta();
        }
        private void Presentar(string cod)
        {
            Form3 ofrm = new Form3();
            App_Code.Clientes oCliente = new App_Code.Clientes(cod);
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.Name== "Form3")
                {
                    ofrm = (Form3)frm;
                    ofrm.codCliente.Text = oCliente.CodCliente.ToString();
                    ofrm.txtNombreCliente.Text = oCliente.Nombre;

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
                "SELECT " +
                    "CLIENTE.CODCLI, " +
                    "CLIENTE.NOMBRE, " +
                    "CLIENTE.TELEFONO, " +
                    "CIUDAD.NOMBRE AS ciudad, " +
                    "DEPARTAMENTO.NOMBRE AS departamento" +
                " FROM CLIENTE " +
                    "INNER JOIN CIUDAD ON CIUDAD.CODCIU = CLIENTE.CIUDAD " +
                    "INNER JOIN DEPARTAMENTO ON CIUDAD.DEPARTAMENTO = DEPARTAMENTO.CODDEP "+
                "WHERE " +
                    "CLIENTE.CODCLI LIKE '%" + this.textBox1.Text + "%' OR " +
                    "CLIENTE.NOMBRE LIKE '%" + this.textBox1.Text + "%'" +
                "", oConexion );
            
            //oAdaptador.SelectCommand.Parameters.Add("@buscar", SqlDbType.NVarChar).Value = this.textBox1.Text;
            oConexion.Open();
            oAdaptador.Fill(oDataSet, "tabla");
            oTabla = oDataSet.Tables["tabla"];
            oConexion.Close();
            this.dgResultados.DataSource = oTabla;

            //Encabezado
            this.dgResultados.Columns[0].HeaderText = "Codigo";
            this.dgResultados.Columns[1].HeaderText = "Nombre";
            this.dgResultados.Columns[2].HeaderText = "Telefono";
            this.dgResultados.Columns[3].HeaderText = "Ciudad";
            this.dgResultados.Columns[4].HeaderText = "Departamento";

            //this.dgListadoCuentasBancarias.Columns[7].HeaderText = "Numero";


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.Consulta();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            this.Consulta();
        }

        private void dgResultados_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.Presentar(this.dgResultados.Rows[int.Parse(e.RowIndex.ToString())].Cells[0].Value.ToString());

        }

        private void dgResultados_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.Presentar(this.dgResultados.Rows[int.Parse(e.RowIndex.ToString())].Cells[0].Value.ToString());

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
