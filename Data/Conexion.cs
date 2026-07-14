using System;
using System.Data.OleDb;
using System.IO;

namespace PoderJudicial.Data
{
    public static class Conexion
    {
        public static readonly string RutaBD =
            @"\\ANTONIOS_LAPTOP\Database\p.accdb";

        public static OleDbConnection ObtenerConexion()
        {
            string connectionString =
                $@"Provider=Microsoft.ACE.OLEDB.12.0;
                   Data Source={RutaBD};";

            return new OleDbConnection(connectionString);
        }
    }
} 


