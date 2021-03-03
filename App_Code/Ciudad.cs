using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace App_Code
{
    class Ciudad : Base
    {
        private string CODCIU;
       
        private string departamento;


        //Constructores
        public Ciudad()
        {
            this.declaracion();
        }
        public Ciudad(string cod)
        {
            this.declaracion();
            this.Leer(cod);

        }

        //Propiedades Publicas

        public string CodCiu
        {
            get { return this.CODCIU; }
            set { this.CODCIU = value; }
        }
        public string Departamento
        {
            get { return this.departamento; }
            set { this.departamento = value; }
        }

        //Metodos Publicas 
        public void Leer(string cod)
        {
            DataSet oDataSet = new DataSet();
            SqlConnection oConexion = new SqlConnection(this.sql);
            SqlDataAdapter oAdaptador = new SqlDataAdapter(this.sel, oConexion);

            oAdaptador.SelectCommand.Parameters.Add("@cod", SqlDbType.NVarChar).Value =cod;

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
            oComando.Parameters.Add("@cod", SqlDbType.NVarChar).Value = this.CODCIU;
            oComando.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = this.nombre;
            oComando.Parameters.Add("@departamento", SqlDbType.NVarChar).Value = this.departamento;


            // parametro que devuelve el id
            oComando.Parameters.Add("@cod", SqlDbType.Int).Direction = ParameterDirection.Output;

            // Ejecuta la insercion
            try
            {
                oConexion.Open();
                oComando.ExecuteNonQuery();
                this.CODCIU = (string)oComando.Parameters["@cod"].Value;
                oConexion.Close();
                this.err = false;
                this.msg = "Registro insertado.";
            }
            catch // Si ocurre algun problema
            {
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
            oComando.Parameters.Add("@cod", SqlDbType.NVarChar).Value = this.CODCIU;
            oComando.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = this.nombre;
            oComando.Parameters.Add("@departamento", SqlDbType.NVarChar).Value = this.departamento;


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

            oComando.Parameters.Add("@cod", SqlDbType.NVarChar).Value = this.CODCIU;

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
                
               
                this.nombre = (string)oRow["NOMBRE"];
                this.CODCIU = (string)oRow["CODCIU"];
                this.departamento = (string)oRow["DEPARTAMENTO"];

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
            this.tbl = "CIUDAD";
            this.sel =
                "SELECT TOP(1) * " +
                "FROM " + this.tbl + " " +
                "WHERE (" +
                    "CODCIU = @cod);";
            this.ins =
                "INSERT INTO " + this.tbl + " (" +

                    "CODCIU,NOMBRE,DEPARTAMENTO) " +
                "VALUES (" +

                    "@cod,@nombre,@departamento); " +

                "SELECT @cod = SCOPE_IDENTITY() FROM " + this.tbl + ";";

            this.upd =
                "UPDATE " + this.tbl + " " +

                "SET " +
                    "CODCIU                 = @cod,      " +
                    "NOMBRE                 = @nombre,      " +
                    "DEPARTAMENTO                 = @departamento      " +



                "WHERE (" +
                    "CODCIU = @cod);";

            this.del =
                "DELETE FROM " + this.tbl + " " +
                "WHERE (" +
                    "CODCIU = @cod);";

            this.err = false;
            this.msg = "";

          

            this.nombre = "";
            this.CODCIU = "";
            this.departamento = "";
           

        }
    }
}
