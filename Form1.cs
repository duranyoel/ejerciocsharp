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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Nuevo();
            this.Departamentos();
            this.ConsultaResultados();
        }

        private void Nuevo()
        {
            this.lblMonto.Text = "0";
            this.dtpFechaInicio.Text = "01/01/2016";
            this.dtpFechaFinal.Text = DateTime.Now.ToString("dd/MM/yyyy");
            this.dgResultados.DataBindings.Clear();
            this.dgResultados.Refresh();
            this.dgResultados.ClearSelection();


        }
        public void ConsultaResultados()
        {

            DataTable oTabla = new DataTable();
            DataSet oDataSet = new DataSet();
            SqlConnection oConexion = new SqlConnection(new App_Code.Base().Sql);
            SqlDataAdapter oAdaptador = new SqlDataAdapter(
                "SELECT " +

                    "DEPARTAMENTO.NOMBRE AS departamento, " +
                    "PRODUCTO.NOMBRE AS producto, " +
                    "PRODUCTO.CODPROD AS codproducto, " +
                    "PRODUCTO.FAMILIA AS familia, " +
                    "PRODUCTO.PRECIO AS precio_producto," +
                    "ITEMS.CANTIDAD AS cantidad, " +
                    "SUM(ITEMS.SUBTOTAL) AS total, " +
                    "PEDIDO.FECHA as fecha " +
                "FROM CIUDAD " +
                    "INNER JOIN CLIENTE ON CIUDAD.CODCIU = CLIENTE.CIUDAD " +
                    "INNER JOIN DEPARTAMENTO ON CIUDAD.DEPARTAMENTO = DEPARTAMENTO.CODDEP " +
                    "INNER JOIN PEDIDO ON CLIENTE.CODCLI = PEDIDO.CLIENTE " +
                    "INNER JOIN ITEMS ON PEDIDO.NUMPEDIDO = ITEMS.NUMPEDIDO " +
                    "INNER JOIN PRODUCTO ON ITEMS.PRODUCTO = PRODUCTO.CODPROD " +
                "WHERE " +
                "(PEDIDO.FECHA BETWEEN  convert(datetime,@inicio,105) AND convert(datetime,@final,105)) AND " +
                "(DEPARTAMENTO.CODDEP = @departamento) " +
                "GROUP BY " +

                    "DEPARTAMENTO.NOMBRE, " +
                    "PRODUCTO.NOMBRE, " +
                    "PRODUCTO.CODPROD, " +
                    "PRODUCTO.FAMILIA, " +
                    "PRODUCTO.PRECIO," +
                    "ITEMS.CANTIDAD, " +
                    "PEDIDO.FECHA " +
                "ORDER BY PEDIDO.FECHA DESC", oConexion
                    );
           
            oAdaptador.SelectCommand.Parameters.Add("@inicio", SqlDbType.DateTime).Value = DateTime.Parse(this.dtpFechaInicio.Value.ToString());
            oAdaptador.SelectCommand.Parameters.Add("@final", SqlDbType.DateTime).Value = DateTime.Parse(this.dtpFechaFinal.Value.ToString());
            oAdaptador.SelectCommand.Parameters.Add("@departamento", SqlDbType.Char).Value = this.cboDepartamentos.SelectedValue;
            oConexion.Open();
            oAdaptador.Fill(oDataSet, "tabla");
            oTabla = oDataSet.Tables["tabla"];
            oConexion.Close();
            this.dgResultados.DataSource = oTabla;

            //Encabezado
            this.dgResultados.Columns[0].HeaderText = "Departamento";
            this.dgResultados.Columns[1].HeaderText = "Nombre Producto";
            this.dgResultados.Columns[2].HeaderText = "Codigo Producto";
            this.dgResultados.Columns[3].HeaderText = "Familia";
            this.dgResultados.Columns[4].HeaderText = "Precio Producto";
            this.dgResultados.Columns[5].HeaderText = "Cantidad";
            this.dgResultados.Columns[6].HeaderText = "Monto";
            this.dgResultados.Columns[6].DefaultCellStyle.Format = "N2";
            this.dgResultados.Columns[7].HeaderText = "Fecha Pedido";
            //this.dgListadoCuentasBancarias.Columns[7].HeaderText = "Numero";

            this.TotalGeneral();
        }
        private void Departamentos()
        {
            
            App_Code.Departamentos oDepartamento = new App_Code.Departamentos();
           
            this.cboDepartamentos.DataSource = oDepartamento.Buscar(0,
                    "CODDEP,NOMBRE",
                    "",
                    "NOMBRE");
            
            this.cboDepartamentos.DisplayMember = "NOMBRE";
            this.cboDepartamentos.ValueMember = "CODDEP";
            
        }
        private void TotalGeneral()
        {
            double total = 0;
            foreach (DataGridViewRow row in dgResultados.Rows)
            {
                total += Convert.ToDouble(row.Cells[6].Value);
            }
            this.lblMonto.Text = total.ToString("#,##0.00");
        }
        private void cboDepartamentos_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.ConsultaResultados();

        }

        private void nuevoPedidoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 oFrm = new Form3();
            oFrm.Show();
            this.Hide();
        }

        private void comisionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 oFrm = new Form2();
            oFrm.Show();
            this.Hide();
        }
    }
}
