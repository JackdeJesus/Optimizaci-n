using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text.RegularExpressions;

namespace PoderJudicial.Data
{
    public static class TableDetector
    {
        private static List<string> _todasLasTablas;
        private static string _tablaActual;
        private static DateTime _expiracion = DateTime.MinValue;
        private static readonly TimeSpan Vigencia = TimeSpan.FromMinutes(30);
        private const string Prefijo = "Audiencias ";

        /// <summary>
        /// Tabla más reciente — usada para INSERT, UPDATE, DELETE.
        /// </summary>
        public static string TablaActual
        {
            get
            {
                RefrescarSiExpiro();
                return _tablaActual;
            }
        }

        /// <summary>
        /// Todas las tablas de audiencias — usadas para autocompletado histórico.
        /// </summary>
        public static List<string> TodasLasTablas
        {
            get
            {
                RefrescarSiExpiro();
                return _todasLasTablas;
            }
        }

        /// <summary>
        /// Llama esto después de guardar un registro nuevo,
        /// por si se creó una tabla nueva en la BD.
        /// </summary>
        public static void InvalidarCache()
        {
            _expiracion = DateTime.MinValue;
        }

        private static void RefrescarSiExpiro()
        {
            if (DateTime.Now < _expiracion && _todasLasTablas != null)
                return;

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                // OleDb expone el esquema sin queries frágiles
                DataTable esquema = conn.GetSchema("Tables");

                _todasLasTablas = esquema.AsEnumerable()
                    .Select(r => r["TABLE_NAME"].ToString())
                    .Where(n => n.StartsWith(Prefijo,
                                StringComparison.OrdinalIgnoreCase))
                    .OrderBy(n => ExtraerAnoFinal(n))
                    .ToList();

                if (_todasLasTablas.Count == 0)
                    throw new InvalidOperationException(
                        "No se encontró ninguna tabla de Audiencias en la base de datos.");

                // La más reciente = año final más alto = última de la lista
                _tablaActual = _todasLasTablas.Last();
            }

            _expiracion = DateTime.Now.Add(Vigencia);
        }

        private static int ExtraerAnoFinal(string nombreTabla)
        {
            // Extrae el segundo año de "Audiencias YYYY-YYYY"
            Match m = Regex.Match(nombreTabla, @"\d{4}-(\d{4})");
            return m.Success ? int.Parse(m.Groups[1].Value) : 0;
        }


        public static string ObtenerTabla(string prefijo)
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                DataTable esquema = conn.GetSchema("Tables");

                return esquema.AsEnumerable()
                    .Select(r => r["TABLE_NAME"].ToString())
                    .FirstOrDefault(t =>
                        !t.StartsWith("MSys") &&
                        t.StartsWith(prefijo,
                            StringComparison.OrdinalIgnoreCase));
            }
        }


    }
}