using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace App_Code
{
    class Items : Base
    {
        private string numero;
        private string producto;
        private decimal precio;
        private decimal cantidad;
        private decimal subtotal;
        StringBuilder errorMessages = new StringBuilder();
        //Constructores
        public Items()
        {
            this.declaracion();
        }
        public Items(string cod)
        {
            this.declaracion();
            this.Leer(cod);

        }
        public Items(string cod, string producto)
        {
            this.declaracion();
            this.Leer(cod, producto);
        }
        //Propiedades Publicas

        public string NumeroPedido
        {
            get { return this.numero; }
            set { this.numero = value; }
        }
        public string Producto
        {
            get { return this.producto; }
            set { this.producto = value; }
        }

        public decimal Precio
        {
            get { return this.precio; }
            set { this.precio = value; }
        }
        public decimal Cantidad
        {
            get { return this.cantidad; }
            set { this.cantidad = value; }
        }
       

        //Metodos Publicas 
        public void Leer(string cod)
        {
            DataSet oDataSet = new DataSet();
            SqlConnection oConexion = new SqlConnection(this.sql);
            SqlDataAdapter oAdaptador = new SqlDataAdapter(this.sel, oConexion);

            oAdaptador.SelectCommand.Parameters.Add("@numero", SqlDbType.NVarChar).Value =cod;

            oConexion.Open();
            oAdaptador.Fill(oDataSet, "tabla");
            oConexion.Close();

            this.campos(oDataSet);
        }
        public void Leer(string cod, string pro)
        {
            DataSet oDataSet = new DataSet();
            SqlConnection oConexion = new SqlConnection(this.sql);

            SqlDataAdapter oAdaptador = new SqlDataAdapter(
                "SELECT TOP(1) * " +
                "FROM " + this.tbl + " " +
                "WHERE (" +
                    "NUMPEDIDO = @numero) AND (" +
                    "PRODUCTO = @producto);"
                , oConexion);


            oAdaptador.SelectCommand.Parameters.Add("@numero", SqlDbType.NVarChar).Value = cod;
            oAdaptador.SelectCommand.Parameters.Add("@producto", SqlDbType.NVarChar).Value = pro;

            oConexion.Open();
            oAdaptador.Fill(oDataSet, "tabla");
            oConexion.Close();

            this.campos(oDataSet);
        }

        public void Insertar()
        {
            SqlConnection oConexion = new SqlConnection(this.sql);
            SqlCommand oComando = new SqlCommand(this.ins, oConexion);

            // Parametros para insertar
            oComando.Parameters.Add("@numero", SqlDbType.NVarChar).Value = this.numero;
            oComando.Parameters.Add("@producto", SqlDbType.NVarChar).Value = this.producto;
            oComando.Parameters.Add("@precio", SqlDbType.Decimal).Value = this.precio;
            oComando.Parameters.Add("@cantidad", SqlDbType.Decimal).Value = this.cantidad;
            //oComando.Parameters.Add("@subtotal", SqlDbType.NVarChar).Value = this.subtotal;

            // parametro que devuelve el id
            oComando.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;

            // Ejecuta la insercion
            try
            {
                oConexion.Open();
                oComando.ExecuteNonQuery();
                this.numero = (string)oComando.Parameters["@numero"].Value;
                oConexion.Close();
                this.err = false;
                this.msg = "Registro insertado.";
            }
            catch (SqlException ex)// Si ocurre algun problema
            {
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                Console.WriteLine(errorMessages.ToString());

                this.err = true;
                this.msg = "Registro no pudo ser insertado.";
                return;
            }

        }
        public void Actualizar()
        {
            SqlConnection oConexion = new SqlConnection(this.sql);
            SqlCommand oComando = new SqlCommand(this.upd, oConexion);

            // Parametros para actualizar
            oComando.Parameters.Add("@numero", SqlDbType.NVarChar).Value = this.numero;
            oComando.Parameters.Add("@producto", SqlDbType.NVarChar).Value = this.producto;
            oComando.Parameters.Add("@precio", SqlDbType.Decimal).Value = this.precio;
            oComando.Parameters.Add("@cantidad", SqlDbType.Decimal).Value = this.cantidad;
            //oComando.Parameters.Add("@subtotal", SqlDbType.NVarChar).Value = this.subtotal;



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

            oComando.Parameters.Add("@numero", SqlDbType.NVarChar).Value = this.numero;

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
                
               
                this.numero = (string)oRow["NUMPEDIDO"];
                this.producto = (string)oRow["PRODUCTO"];
                this.precio = (decimal)oRow["PRECIO"];
                this.cantidad = (decimal)oRow["CANTIDAD"];
                this.subtotal = (decimal)oRow["SUBTOTAL"];


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
            this.tbl = "ITEMS";
            this.sel =
                "SELECT TOP(1) * " +
                "FROM " + this.tbl + " " +
                "WHERE (" +
                    "NUMPEDIDO = @numero);";
            this.ins =
                "INSERT INTO " + this.tbl + " (" +

                    "NUMPEDIDO,PRODUCTO,PRECIO,CANTIDAD) " +
                "VALUES (" +

                    "@numero,@producto,@precio,@cantidad); " +

                "SELECT @numero = SCOPE_IDENTITY() FROM " + this.tbl + ";";

            this.upd =
                "UPDATE " + this.tbl + " " +

                "SET " +
                    "NUMPEDIDO                 = @numero,      " +
                    "PRODUCTO                 = @producto,      " +
                    "PRECIO = @precio, "+
                     "CANTIDAD= @cantidad " +
                      



                "WHERE (" +
                    "NUMPEDIDO = @numero);";

            this.del =
                "DELETE FROM " + this.tbl + " " +
                "WHERE (" +
                    "NUMPEDIDO = @numero);";

            this.err = false;
            this.msg = "";

          

            this.numero = "";
            this.producto = "";
            this.precio = 0;
            this.cantidad=0;
            this.subtotal = 0;

           

        }
    }
}
