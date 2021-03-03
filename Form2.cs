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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Consulta();
            //this.dateTimePicker1.Format = "";
            this.comboBox1.DisplayMember = "Text";
            this.comboBox1.ValueMember = "Value";
            var items = new[] {
                new { Text = "Enero", Value = "1" },
                new { Text = "Febrero", Value = "2" },
                new { Text = "Marzo", Value = "3" },
                new { Text = "Abril", Value = "4" },
                new { Text = "Mayo", Value = "5" } ,
                new { Text = "Junio", Value = "6" } ,
                new { Text = "Julio", Value = "7" },
                new { Text = "Agosto", Value = "8" },
                new { Text = "Septiembre", Value = "9" },
                new { Text = "Octubre", Value = "10" },
                new { Text = "Noviembre", Value = "11" },
                new { Text = "Diciembre", Value = "12" }};
                this.comboBox1.DataSource = items;
                this.label1.Text = "Meses";

        
        }

        private void Consulta()
{
    DataTable oTabla = new DataTable();
    DataSet oDataSet = new DataSet();
    SqlConnection oConexion = new SqlConnection(new App_Code.Base().Sql);
    SqlDataAdapter oAdaptador = new SqlDataAdapter(
        "SELECT " +
        "VENDEDOR.NOMBRE,PEDIDO.FECHA,(SUM(ITEMS.SUBTOTAL)*10)/100  as total " +
        "FROM ITEMS " +
        "INNER JOIN PEDIDO ON ITEMS.NUMPEDIDO = PEDIDO.NUMPEDIDO " +
        "INNER JOIN VENDEDOR ON PEDIDO.VENDEDOR = VENDEDOR.CODVEND " +
        "WHERE MONTH(PEDIDO.FECHA)= '" + this.comboBox1.SelectedValue + "' " +
        "GROUP BY VENDEDOR.NOMBRE, PEDIDO.FECHA", oConexion);


    oConexion.Open();
    oAdaptador.Fill(oDataSet, "tabla");
    oTabla = oDataSet.Tables["tabla"];
    oConexion.Close();
    this.dgvConsulta.DataSource = oTabla;

    //Encabezado
    this.dgvConsulta.Columns[0].HeaderText = "Vendedor";
    this.dgvConsulta.Columns[1].HeaderText = "Fecha";

    this.dgvConsulta.Columns[2].HeaderText = "Total";
    this.dgvConsulta.Columns[2].DefaultCellStyle.Format = "N2";
}

private void ventasToolStripMenuItem_Click(object sender, EventArgs e)
{
    Form1 oFrm = new Form1();
    this.Close();
    oFrm.Show();
}

private void nuevoPedidoToolStripMenuItem_Click(object sender, EventArgs e)
{
    Form3 oFrm = new Form3();
    oFrm.Show();
    this.Hide();
}

private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
{
    this.Consulta();
}
    }
}
