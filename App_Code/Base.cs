using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Globalization;

namespace App_Code
{
    class Base
    {
        //propiedades privadas
        protected string sql;
        
        protected string tbl;
        protected string sel;
        protected string ins;
        protected string upd;
        protected string del;
        protected bool err;
        protected string msg;
       
        protected string cod;
        protected DateTime creado;
        protected DateTime modificado;
       
        protected string nombre;
        protected bool invalid = false;
        protected bool borrado;
        

        //Constructores

        public Base()
        {
            this.declaracion();
        }
        public Base(string cod)
        {

            this.declaracion();
            this.Leer(cod);
        }

        //Propiedades Publicas 
        public string Sql
        {
            get { return this.sql; }
        }
     
        public bool Err
        {
            get { return this.err; }
            set { this.err = value; }
        }
        public string Msg
        {
            get { return this.msg; }
            set { this.msg = value; }
        }
      
        

        public string Nombre
        {
            get { return this.nombre; }
            set { this.nombre = value; }
        }
        public string Codigo
        {
            get { return this.cod; }
            set { this.cod = value; }
        }
       

       
        public DateTime Creado
        {
            get { return this.creado; }
            set { this.creado = value; }
        }
       
        public bool Borrado
        {
            get { return this.borrado; }
            set { this.borrado = value; }
        }

        // Metodos Publicos
        public void Nuevo()
        {
            this.declaracion();
        }

        public void Leer(string cod)
        {
            DataSet oDataSet = new DataSet();
            SqlConnection oConexion = new SqlConnection(this.sql);
            SqlDataAdapter oAdaptador = new SqlDataAdapter(this.sel, oConexion);

            oAdaptador.SelectCommand.Parameters.Add("@cod", SqlDbType.Int).Value = cod;

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
            oComando.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = this.nombre;

   
            oComando.Parameters.Add("@creado", SqlDbType.DateTime).Value = this.creado;
            oComando.Parameters.Add("@modificado", SqlDbType.DateTime).Value = this.modificado;
            oComando.Parameters.Add("@borrado", SqlDbType.Bit).Value = this.borrado;

            // parametro que devuelve el id
            oComando.Parameters.Add("@cod", SqlDbType.Int).Direction = ParameterDirection.Output;

            // Ejecuta la insercion
            try
            {
                oConexion.Open();
                oComando.ExecuteScalar();
                this.cod = (string)oComando.Parameters["@cod"].Value;
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
            oComando.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = this.nombre;

           
            oComando.Parameters.Add("@creado", SqlDbType.DateTime).Value = this.creado;
            oComando.Parameters.Add("@modificado", SqlDbType.DateTime).Value = this.modificado;
            oComando.Parameters.Add("@borrado", SqlDbType.Bit).Value = this.borrado;


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
            SqlCommand oComando = new SqlCommand(this.ins, oConexion);

            oComando.Parameters.Add("@cod", SqlDbType.Int).Value = this.cod;

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

        public DataTable Buscar(int cantidad, string campos, string filtro, string orden)
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
                this.cod = (string)oRow["cod"];

                this.nombre = (string)oRow["nombre"];

              
                this.creado = (DateTime)oRow["creado"];
                this.modificado = (DateTime)oRow["modificado"];
               
                this.borrado=(bool)oRow["borrado"];

                this.err = false;
                this.msg = "Registro leido correctamente.";
            }
            else
            {
                this.err = true;
                this.msg = "No hay registro.";
            }
        }

        public void declaracion()
        {

            this.sql = ConfigurationManager.ConnectionStrings["pruebacsharp"].ToString();
            this.tbl = "tabla";
            this.sel =
                "SELECT TOP(1) * " +
                "FROM " + this.tbl + " " +
                "WHERE (" +
                    "COD = @id);";
            this.ins =
                "INSERT INTO " + this.tbl + " (" +
                    " nombre, borrado,creado,modificado) " +
                "VALUES (" +
                    "@nombre,@borrado,@creado,@modificado); " +

                "SELECT @id = SCOPE_IDENTITY() FROM " + this.tbl + ";";

            this.upd =
                "UPDATE " + this.tbl + " " +

                "SET " +
                   
                    "nombre     = @nombre,     " +
                   
                    "borrado    = @borrado,    " +
                    "creado     = @creado,     " +
                    "modificado = @modificado  " +

                "WHERE (" +
                    "id = @id);";

            this.del =
                "DELETE FROM " + this.tbl + " " +
                "WHERE (" +
                    "id = @id);";

            this.err = false;
            this.msg = "";

            this.cod = "";
         
            this.nombre = "";

          
            this.borrado = false;
            this.creado = DateTime.Now;
            this.modificado = DateTime.Now;

        }

    }
}
