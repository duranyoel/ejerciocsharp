﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace App_Code
{
    class Producto : Base
    {
        
       
        private string familia;
        private decimal precio;

        //Constructores
        public Producto()
        {
            this.declaracion();
        }
        public Producto(string cod)
        {
            this.declaracion();
            this.Leer(cod);

        }

        //Propiedades Publicas

        
        public string Familia
        {
            get { return this.familia; }
            set { this.familia = value; }
        }
        public decimal Precio
        {
            get { return this.precio; }
            set { this.precio = value; }
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
            oComando.Parameters.Add("@cod", SqlDbType.NVarChar).Value = this.cod;
            oComando.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = this.nombre;
            oComando.Parameters.Add("@familia", SqlDbType.NVarChar).Value = this.familia;
            oComando.Parameters.Add("@precio", SqlDbType.Float).Value = this.precio;


            // parametro que devuelve el id
            oComando.Parameters.Add("@cod", SqlDbType.Int).Direction = ParameterDirection.Output;

            // Ejecuta la insercion
            try
            {
                oConexion.Open();
                oComando.ExecuteNonQuery();
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
            oComando.Parameters.Add("@cod", SqlDbType.NVarChar).Value = this.cod;
            oComando.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = this.nombre;
            oComando.Parameters.Add("@familia", SqlDbType.NVarChar).Value = this.familia;
            oComando.Parameters.Add("@precio", SqlDbType.Float).Value = this.precio;


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

            oComando.Parameters.Add("@cod", SqlDbType.NVarChar).Value = this.cod;

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
                this.cod = (string)oRow["CODPROD"];
                this.familia = (string)oRow["FAMILIA"];
                this.precio = (decimal)oRow["PRECIO"];

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
            this.tbl = "PRODUCTO";
            this.sel =
                "SELECT TOP(1) * " +
                "FROM " + this.tbl + " " +
                "WHERE (" +
                    "CODPROD = @cod);";
          

            this.err = false;
            this.msg = "";

            

            this.nombre = "";
            this.cod = "";
            this.familia= "";
            this.precio = 0;
            
           

        }
    }
}
