using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace App_Code
{
    class Pedido : Base
    {
        private string numeropedido;
        private string cliente;
        private DateTime fecha;
        private string vendedor;
        StringBuilder errorMessages = new StringBuilder();

        //Constructores
        public Pedido()
        {
            this.declaracion();
        }
        public Pedido(string cod)
        {
            this.declaracion();
            this.Leer(cod);

        }

        //Propiedades Publicas

        public string NumeroPedido
        {
            get { return this.numeropedido; }
            set { this.numeropedido = value; }
        }
        public string Cliente
        {
            get { return this.cliente; }
            set { this.cliente = value; }
        }
        public DateTime Fecha
        {
            get { return this.fecha; }
            set { this.fecha = value; }
        }
        public string Vendedor
        {
            get { return this.vendedor; }
            set { this.vendedor = value; }
        }
        //Metodos Publicas 
        public new void Leer(string cod)
        {
            DataSet oDataSet = new DataSet();
            SqlConnection oConexion = new SqlConnection(this.sql);
            SqlDataAdapter oAdaptador = new SqlDataAdapter(this.sel, oConexion);

            oAdaptador.SelectCommand.Parameters.Add("@numero", SqlDbType.NVarChar).Value = cod;

            oConexion.Open();
            oAdaptador.Fill(oDataSet, "tabla");
            oConexion.Close();

            this.campos(oDataSet);
        }

        public new void Insertar()
        {
            SqlConnection oConexion = new SqlConnection(this.sql);
            SqlCommand oComando = new SqlCommand(this.ins, oConexion);

            // Parametros para insertar
            oComando.Parameters.Add("@numero", SqlDbType.NVarChar).Value = this.numeropedido;
            oComando.Parameters.Add("@cliente", SqlDbType.NVarChar).Value = this.cliente;
            oComando.Parameters.Add("@fecha", SqlDbType.DateTime).Value = this.fecha;
            oComando.Parameters.Add("@vendedor", SqlDbType.NVarChar).Value = this.vendedor;

            // parametro que devuelve el id
            oComando.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;

            // Ejecuta la insercion
            try
            {
                oConexion.Open();
                oComando.ExecuteNonQuery();
                this.numeropedido = (string)oComando.Parameters["@numero"].Value;
                oConexion.Close();
                this.err = false;
                this.msg = "Registro insertado.";
            }
            catch (SqlException ex)// Si ocurre algun problema
            {
                //for (int i = 0; i < ex.Errors.Count; i++)
                //{
                //    errorMessages.Append("Index #" + i + "\n" +
                //        "Message: " + ex.Errors[i].Message + "\n" +
                //        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                //        "Source: " + ex.Errors[i].Source + "\n" +
                //        "Procedure: " + ex.Errors[i].Procedure + "\n");
                //}
                //Console.WriteLine(errorMessages.ToString());

                this.err = true;
                this.msg = "Registro no pudo ser insertado.";
                return;
            }

        }
        public new void Actualizar()
        {
            SqlConnection oConexion = new SqlConnection(this.sql);
            SqlCommand oComando = new SqlCommand(this.upd, oConexion);

            // Parametros para actualizar
            oComando.Parameters.Add("@numero", SqlDbType.NVarChar).Value = this.numeropedido;
            oComando.Parameters.Add("@cliente", SqlDbType.NVarChar).Value = this.cliente;
            oComando.Parameters.Add("@fecha", SqlDbType.DateTime).Value = this.fecha;
            oComando.Parameters.Add("@vendedor", SqlDbType.NVarChar).Value = this.vendedor;



            try
            {
                oConexion.Open();
                oComando.ExecuteNonQuery();
                oConexion.Close();
                this.err = false;
                this.msg = "Registro actualizado.";
            }
            catch
            {
                this.err = true;
                this.msg = "Registro no pudo ser actualizado.";
                return;
            }

        }
        public void Eliminar()
        {
            SqlConnection oConexion = new SqlConnection(this.sql);
            SqlCommand oComando = new SqlCommand(this.del, oConexion);

            oComando.Parameters.Add("@numero", SqlDbType.NVarChar).Value = this.numeropedido;

            try
            {
                oConexion.Open();
                oComando.ExecuteNonQuery();
                oConexion.Close();

                this.err = false;
                this.msg = "Registro eliminado.";
            }
            catch
            {
                this.err = true;
                this.msg = "Registro no pudo ser eliminado.";
            }
        }
        public new DataTable Buscar(int cantidad, string campos, string filtro, string orden)
        {
            DataSet oDataSet = new DataSet();
            SqlConnection oConexion = new SqlConnection(this.sql);
            SqlDataAdapter oAdaptador = new SqlDataAdapter(
                "SELECT " +
                    (cantidad > 0 ? "TOP(" + cantidad.ToString("0") + ")" : "") + " " +
                    (campos.Length > 0 ? campos : "*") + " " +
                "FROM " + this.tbl + " " +
                    (filtro.Length > 0 ? "WHERE " + filtro : "") + " " +
                    (orden.Length > 0 ? "ORDER BY " + orden : "") +
                ";", oConexion);

            oConexion.Open();
            oAdaptador.Fill(oDataSet, "tabla");
            oConexion.Close();

            return oDataSet.Tables["tabla"];
        }

        // Metodos Privados
        private void campos(DataSet oDataSet)
        {
            if (oDataSet.Tables["tabla"].Rows.Count != 0)
            {
                DataRow oRow = oDataSet.Tables["tabla"].Rows[0];

                this.numeropedido = (string)oRow["NUMPEDIDO"];
                this.cliente = (string)oRow["CLIENTE"];
                this.fecha = (DateTime)oRow["FECHA"];
                this.vendedor = (string)oRow["VENDEDOR"];


                this.err = false;
                this.msg = "Registro leido correctamente.";
            }
            else
            {
                this.err = true;
                this.msg = "No hay registro.";
            }
        }
        private void declaracion()
        {
            this.tbl = "PEDIDO";
            this.sel =
                "SELECT TOP(1) * " +
                "FROM " + this.tbl + " " +
                "WHERE (" +
                    "NUMPEDIDO = @numero);";
            this.ins =
                "INSERT INTO " + this.tbl + " (" +

                    "NUMPEDIDO,CLIENTE,FECHA,VENDEDOR) " +
                "VALUES (" +

                    "@numero,@cliente,@fecha,@vendedor); " +

                "SELECT @numero = SCOPE_IDENTITY() FROM " + this.tbl + ";";

            this.upd =
                "UPDATE " + this.tbl + " " +

                "SET " +
                    "NUMPEDIDO                 = @coddep,      " +
                    "CLIENTE                 = @cliente,      " +
                    "FECHA                 = @fecha,    " +
                    "VENDEDOR                 = @vendedor      " +



                "WHERE (" +
                    "NUMPEDIDO = @numero);";

            this.del =
                "DELETE FROM " + this.tbl + " " +
                "WHERE (" +
                    "NUMPEDIDO = @numero);";

            this.err = false;
            this.msg = "";

           

            this.numeropedido = "";
            this.cliente = "";
            this.fecha = DateTime.Now;
            this.vendedor = "";


        }
    }
}
