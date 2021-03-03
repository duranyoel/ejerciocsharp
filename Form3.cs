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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            this.Nuevo();
          
            this.ConsultaProductos();

            
        }

        private void Nuevo()
        {
            
            DataTable oTabla = new DataTable();
            DataSet oDataSet = new DataSet();
            SqlConnection oConexion = new SqlConnection(new App_Code.Base().Sql);
            SqlDataAdapter oAdaptador = new SqlDataAdapter(
                "SELECT  MAX(CONVERT(int,NUMPEDIDO,150)) as r FROM PEDIDO ", oConexion);
           
            oConexion.Open();
            oAdaptador.Fill(oDataSet, "tabla");
            oTabla = oDataSet.Tables["tabla"];
            oConexion.Close();

            int numero = int.Parse(oTabla.Rows[0]["r"].ToString()) + 1;
            this.txtCodigoPedido.Text = numero.ToString();
            this.txtCodigoPedido.ReadOnly = true;

            this.txtNombreCliente.ReadOnly = true;
            this.txtVendedor.ReadOnly = true;
            this.txtCodigoPro.ReadOnly = true;
        }
        

       

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
           

        }

        private void ventasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 oFrm = new Form1();
            this.Close();
            oFrm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmClientes oClientes = new frmClientes();
            oClientes.ShowDialog();
            //this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmVendedor oVendedor = new frmVendedor();
            oVendedor.ShowDialog();
           // this.Hide();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        public void ConsultaProductos()
        {

            DataTable oTabla = new DataTable();
            DataSet oDataSet = new DataSet();
            SqlConnection oConexion = new SqlConnection(new App_Code.Base().Sql);
            SqlDataAdapter oAdaptador = new SqlDataAdapter(
                "SELECT CODPROD,NOMBRE,FAMILIA,PRECIO FROM PRODUCTO " +
                "WHERE " +
                    "CODPROD LIKE '%" + this.txtBuscarProductos.Text + "%' OR " +
                    "NOMBRE LIKE '%" + this.txtBuscarProductos.Text + "%' OR " +
                    "FAMILIA LIKE '%" + this.txtBuscarProductos.Text + "%'" +
                "", oConexion);

            //oAdaptador.SelectCommand.Parameters.Add("@buscar", SqlDbType.NVarChar).Value = this.textBox1.Text;
            oConexion.Open();
            oAdaptador.Fill(oDataSet, "tabla");
            oTabla = oDataSet.Tables["tabla"];
            oConexion.Close();
            this.dgvProductos.DataSource = oTabla;

            //Encabezado
            this.dgvProductos.Columns[0].HeaderText = "Codigo";
            this.dgvProductos.Columns[1].HeaderText = "Nombre";
            this.dgvProductos.Columns[2].HeaderText = "Familia";
            this.dgvProductos.Columns[3].HeaderText = "Precio";
            this.dgvProductos.Columns[3].DefaultCellStyle.Format = "N2";

        }

        private void txtBuscarProductos_KeyUp(object sender, KeyEventArgs e)
        {
            this.ConsultaProductos();
        }

        private void Presentar (string cod)
        {
            App_Code.Producto oProducto = new App_Code.Producto(cod);

            this.txtCodigoPro.Text = oProducto.Codigo.ToString();
            

        }
        private void dgvProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.Presentar(this.dgvProductos.Rows[int.Parse(e.RowIndex.ToString())].Cells[0].Value.ToString());
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            App_Code.Producto oProducto = new App_Code.Producto(this.txtCodigoPro.Text.ToString());
            App_Code.Items oRegistro = new App_Code.Items(this.txtCodigoPedido.Text,this.txtCodigoPro.Text);
            App_Code.Pedido oPedido = new App_Code.Pedido(this.txtCodigoPedido.Text);
            oPedido.NumeroPedido = this.txtCodigoPedido.Text;
            oPedido.Cliente = this.codCliente.Text;
            oPedido.Vendedor = this.codVendedor.Text;
            oPedido.Fecha = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"));

            oRegistro.Producto = oProducto.Codigo;
            oRegistro.NumeroPedido = this.txtCodigoPedido.Text;
            oRegistro.Precio = decimal.Parse(oProducto.Precio.ToString());
            oRegistro.Cantidad = decimal.Parse(this.txtCantidadProducto.Text.ToString());

            try
            {
                if (!Validar())
                {
                    if (oPedido.Err)
                    {
                        oPedido.Insertar();
                    }
                    else
                    {
                        //oPedido.Actualizar();
                    }
                    if (oRegistro.Err)
                    {
                        oRegistro.Insertar();
                    }
                    else
                    {
                        oRegistro.Actualizar();
                    }
                    

                    this.ConsultaProductosItems();



                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        private void ConsultaProductosItems()
        {
            DataTable oTabla = new DataTable();
            DataSet oDataSet = new DataSet();
            SqlConnection oConexion = new SqlConnection(new App_Code.Base().Sql);
            SqlDataAdapter oAdaptador = new SqlDataAdapter(
                "SELECT PRODUCTO.NOMBRE, " +
                "PRODUCTO.FAMILIA, " +
                "ITEMS.CANTIDAD, " +
               
                "ITEMS.PRECIO, " +
                "ITEMS.SUBTOTAL " +
                "FROM ITEMS " +
                "INNER JOIN PRODUCTO ON ITEMS.PRODUCTO = PRODUCTO.CODPROD " +
                "WHERE ITEMS.NUMPEDIDO = '"+ this.txtCodigoPedido.Text + "'", oConexion);

           
            oConexion.Open();
            oAdaptador.Fill(oDataSet, "tabla");
            oTabla = oDataSet.Tables["tabla"];
            oConexion.Close();
            this.dgvItems.DataSource = oTabla;

            //Encabezado
            this.dgvItems.Columns[0].HeaderText = "Producto";
            this.dgvItems.Columns[1].HeaderText = "Familia";
            this.dgvItems.Columns[2].HeaderText = "Cantidad";
            this.dgvItems.Columns[3].HeaderText = "Precio";
            this.dgvItems.Columns[3].DefaultCellStyle.Format = "N2";
            this.dgvItems.Columns[4].HeaderText = "Subtotal";
            this.dgvItems.Columns[4].DefaultCellStyle.Format = "N2";
        }

        private void comisionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 oFrm = new Form2();
            oFrm.Show();
            this.Hide();
        }

        private bool Validar()
        {
            if (this.txtCodigoPedido.Text.Length == 0)
            {
                this.txtCodigoPedido.BackColor = Color.Red;
                MessageBox.Show("Disculpe error en codigo pedido", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            if (this.txtNombreCliente.Text.Length == 0)
            {
                this.txtNombreCliente.BackColor = Color.Red;
                MessageBox.Show("Disculpe error en nombre de cliente", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            if (this.txtVendedor.Text.Length == 0)
            {
                this.txtVendedor.BackColor = Color.Red;
                MessageBox.Show("Disculpe error en nombre de vendedor", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            if (this.txtCodigoPro.Text.Length == 0)
            {
                this.txtCodigoPro.BackColor = Color.Red;
                MessageBox.Show("Disculpe error en codigo producto", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            return false;
        }
    }
}
