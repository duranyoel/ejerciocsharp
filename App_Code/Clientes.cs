using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace App_Code
{
    class Clientes : Base
    {
        private string codcliente;
        private string direccion;
        private string telefono;
        private int cupo;
        private string canal;
        private string vendedor;
        private string ciudad;
        private string padre;


        //Constructores
        public Clientes()
        {
            this.declaracion();
        }
        public Clientes(string cod)
        {
            this.declaracion();
            this.Leer(cod);

        }

        //Propiedades Publicas

        public string CodCliente
        {
            get { return this.codcliente; }
            set { this.codcliente = value; }
        }
        public string Direccion
        {
            get { return this.direccion; }
            set { this.direccion = value; }
        }

        public string Telefono
        {
            get { return this.telefono; }
            set { this.telefono = value; }
        }
        public int Cupo
        {
            get { return this.cupo; }
            set { this.cupo = value; }
        }
        public string Canal
        {
            get { return this.canal; }
            set { this.canal = value; }
        }
        public string Vendedor
        {
            get { return this.vendedor; }
            set { this.vendedor = value; }
        }

        public string Ciudad
        {
            get { return this.ciudad; }
            set { this.ciudad = value; }
        }

        public string Padre
        {
            get { return this.padre; }
            set { this.padre = value; }
        }

        //Metodos Publicas 
        public new void Leer(string cod)
        {
            DataSet oDataSet = new DataSet();
            SqlConnection oConexion = new SqlConnection(this.sql);
            SqlDataAdapter oAdaptador = new SqlDataAdapter(this.sel, oConexion);

            oAdaptador.SelectCommand.Parameters.Add("@cod", SqlDbType.NVarChar).Value = cod;

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
            oComando.Parameters.Add("@cod", SqlDbType.NVarChar).Value = this.codcliente;
            oComando.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = this.nombre;
            oComando.Parameters.Add("@direccion", SqlDbType.NVarChar).Value = this.direccion;
            oComando.Parameters.Add("@telefono", SqlDbType.NVarChar).Value = this.telefono;
            oComando.Parameters.Add("@cupo", SqlDbType.Int).Value = this.cupo;
            oComando.Parameters.Add("@fechacreacion", SqlDbType.DateTime).Value = this.creado;
            oComando.Parameters.Add("@canal", SqlDbType.NVarChar).Value = this.canal;
            oComando.Parameters.Add("@vendedor", SqlDbType.NVarChar).Value = this.vendedor;
            oComando.Parameters.Add("@ciudad", SqlDbType.NVarChar).Value = this.ciudad;
            oComando.Parameters.Add("@padre", SqlDbType.NVarChar).Value = this.padre;

            // parametro que devuelve el id
            oComando.Parameters.Add("@cod", SqlDbType.Int).Direction = ParameterDirection.Output;

            // Ejecuta la insercion
            try
            {
                oConexion.Open();
                oComando.ExecuteNonQuery();
                this.codcliente = (string)oComando.Parameters["@cod"].Value;
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
            oComando.Parameters.Add("@cod", SqlDbType.NVarChar).Value = this.codcliente;
            oComando.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = this.nombre;
            oComando.Parameters.Add("@direccion", SqlDbType.NVarChar).Value = this.direccion;
            oComando.Parameters.Add("@telefono", SqlDbType.NVarChar).Value = this.telefono;
            oComando.Parameters.Add("@cupo", SqlDbType.Int).Value = this.cupo;
            oComando.Parameters.Add("@fechacreacion", SqlDbType.DateTime).Value = this.creado;
            oComando.Parameters.Add("@canal", SqlDbType.NVarChar).Value = this.canal;
            oComando.Parameters.Add("@vendedor", SqlDbType.NVarChar).Value = this.vendedor;
            oComando.Parameters.Add("@ciudad", SqlDbType.NVarChar).Value = this.ciudad;
            oComando.Parameters.Add("@padre", SqlDbType.NVarChar).Value = this.padre;


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

            oComando.Parameters.Add("@cod", SqlDbType.NVarChar).Value = this.codcliente;

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
                this.codcliente = (string)oRow["CODCLI"];
               
                this.nombre = (string)oRow["NOMBRE"];
                this.direccion = (string)oRow["DIRECCION"];
                this.telefono = (string)oRow["TELEFONO"];
                this.cupo = (int)oRow["CUPO"];
                this.creado = (DateTime)oRow["FECHACREACION"];
                this.canal = (string)oRow["CANAL"];
                this.vendedor = (string)oRow["VENDEDOR"];
                this.ciudad = (string)oRow["CIUDAD"];
                //this.padre = (null)oRow["PADRE"];

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
            this.tbl = "CLIENTE";
            this.sel =
                "SELECT TOP(1) * " +
                "FROM " + this.tbl + " " +
                "WHERE (" +
                    "CODCLI = @cod);";
            this.ins =
                "INSERT INTO " + this.tbl + " (" +

                    "CODCLI,NOMBRE,DIRECCION,TELEFONO,CUPO,FECHACREACION,CANAL,VENDEDOR,CIUDAD,PADRE) " +
                "VALUES (" +

                    "@cod,@nombre,@direccion,@telefono,@cupo,@fechacreacion,@canal,@vendedor,@ciudad,@padre); " +

                "SELECT @cod = SCOPE_IDENTITY() FROM " + this.tbl + ";";

            this.upd =
                "UPDATE " + this.tbl + " " +

                "SET " +
                    "CODCLI               = @cod,      " +
                    "NOMBRE                 = @nombre      " +

              

                "WHERE (" +
                    "CODCLI = @cod);";

            this.del =
                "DELETE FROM " + this.tbl + " " +
                "WHERE (" +
                    "CODCLI = @cod);";

            this.err = false;
            this.msg = "";

           
            this.codcliente = "";
            this.nombre = "";
            this.direccion = "";
            this.telefono = "";
            this.cupo = 0;
            this.canal = "";
            this.vendedor = "";
            this.ciudad = "";
            this.padre = "";
           

        }
    }
}
