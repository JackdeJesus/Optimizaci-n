using PoderJudicial.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
namespace PoderJudicial.Data
{
    public class DashboardData
    {
        // Cache del nombre de tabla que contiene cierta columna
        // (por ejemplo "TotDiscosEntregados" -> "CopiasAudiencias").
        // Antes, ObtenerNombreTablaPorColumna recorría TODAS las tablas
        // y para cada una pedía el schema completo de columnas
        // (GetOleDbSchemaTable) — una operación cara que se repetía en
        // cada carga del Dashboard/Home, aunque el resultado es siempre
        // el mismo mientras no cambie la estructura de la BD.
        // ConcurrentDictionary porque varias llamadas podrían compartir
        // esta clase (aunque el acceso a BD ya está serializado por
        // AccesoBD, esto la deja segura de todos modos).
        private static readonly ConcurrentDictionary<string, string>
            _cacheTablaPorColumna = new();

        public int ObtenerTotalAudienciasMes()
        {
            int total = 0;

            DateTime inicioMes =
                new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    1);

            DateTime inicioSiguienteMes =
                inicioMes.AddMonths(1);

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                foreach (string nombreTabla in ObtenerTablasAudiencias(conn))
                {
                    try
                    {
                        string query = $@"
                SELECT COUNT(*)
                FROM [{nombreTabla}]
                WHERE FeAudiencia >= ?
                AND FeAudiencia < ?";

                        using (OleDbCommand cmd =
                            new OleDbCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("?", inicioMes);
                            cmd.Parameters.AddWithValue("?", inicioSiguienteMes);

                            object resultado = cmd.ExecuteScalar();

                            if (resultado != null &&
                                resultado != DBNull.Value)
                            {
                                total += Convert.ToInt32(resultado);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarError(ex.Message, nombreTabla);
                    }
                }
            }

            return total;
        }



        public int ObtenerTotalEjecucionesMes()
        {
            int total = 0;

            DateTime inicioMes =
                new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    1);

            DateTime inicioSiguienteMes =
                inicioMes.AddMonths(1);

            using (OleDbConnection conn =
                Conexion.ObtenerConexion())
            {
                conn.Open();

                string query = @"
            SELECT COUNT(*)
            FROM Ejecucion
            WHERE FechaAudiencia >= ?
            AND FechaAudiencia < ?";

                using (OleDbCommand cmd =
                    new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "?",
                        inicioMes);

                    cmd.Parameters.AddWithValue(
                        "?",
                        inicioSiguienteMes);

                    object resultado =
                       cmd.ExecuteScalar();

                    if (resultado != null &&
                        resultado != DBNull.Value)
                    {
                        total = Convert.ToInt32(resultado);
                    }
                }
            }

            return total;
        }



        public int ObtenerTotalCopiasMes()
        {
            int total = 0;

            DateTime inicioMes =
                new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    1);

            DateTime inicioSiguienteMes =
                inicioMes.AddMonths(1);

            using (OleDbConnection conn =
                Conexion.ObtenerConexion())
            {
                conn.Open();

                string query = @"
            SELECT SUM(Val(TotDiscosEntregados))
            FROM CopiasAudiencias
            WHERE FeRecibo >= ?
            AND FeRecibo < ?";

                using (OleDbCommand cmd =
                    new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "?",
                        inicioMes);

                    cmd.Parameters.AddWithValue(
                        "?",
                        inicioSiguienteMes);

                    object resultado =
                        cmd.ExecuteScalar();

                    if (resultado != null &&
                        resultado != DBNull.Value)
                    {
                        total = Convert.ToInt32(resultado);
                    }
                }
            }

            return total;
        }


        public int ObtenerAudienciasHoy()
        {
            int total = 0;

            DateTime inicioDia = DateTime.Today;
            DateTime inicioSiguienteDia = inicioDia.AddDays(1);

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                foreach (string nombreTabla in ObtenerTablasAudiencias(conn))
                {
                    try
                    {
                        string query = @"
                    SELECT COUNT(*)
                    FROM [" + nombreTabla + @"]
                    WHERE FeAudiencia >= ?
                    AND FeAudiencia < ?";

                        using (OleDbCommand cmd = new OleDbCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("?", inicioDia);
                            cmd.Parameters.AddWithValue("?", inicioSiguienteDia);

                            object resultado = cmd.ExecuteScalar();

                            if (resultado != null &&
                                resultado != DBNull.Value)
                            {
                                total += Convert.ToInt32(resultado);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarError(ex.Message, nombreTabla);
                    }
                }
            }

            return total;
        }


        public string ObtenerVersionSistema()
        {
            Version version =
                Assembly.GetExecutingAssembly().GetName().Version;

            return $"v{version.Major}.{version.Minor}.{version.Build}";
        }

        public string ObtenerEstadoSistema()
        {
            try
            {
                using (var cn = Conexion.ObtenerConexion())
                {
                    cn.Open();
                }

                return "Operativo";
            }
            catch
            {
                return "Sin conexión";
            }
        }


        public string ObtenerNombreBaseDatos()
        {
            return Path.GetFileName(Conexion.RutaBD);
        }


        public List<ActividadReciente> ObtenerActividadesRecientes()
        {
            List<ActividadReciente> actividades = new List<ActividadReciente>();

            actividades.AddRange(ObtenerActividadesAudiencias());

            actividades.AddRange(ObtenerActividadesCopias());

            actividades.AddRange(ObtenerActividadesEjecuciones());

            return actividades
                .OrderByDescending(x => x.FechaHora)
                .Take(8)
                .ToList();
        }

        private List<ActividadReciente> ObtenerActividadesAudiencias()
        {
            List<ActividadReciente> lista = new List<ActividadReciente>();

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                foreach (string nombreTabla in ObtenerTablasAudiencias(conn))
                {
                    try
                    {
                        string query = $@"
                    SELECT TOP 10
                        Id,
                        FeRecibo,
                        NUC,
                        NoCausa,
                        [Quien Realiza]
                    FROM [{nombreTabla}]
                    WHERE FeRecibo IS NOT NULL
                    ORDER BY FeRecibo DESC";

                        using (OleDbCommand cmd = new OleDbCommand(query, conn))
                        using (OleDbDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                lista.Add(new ActividadReciente
                                {
                                    FechaHora = Convert.ToDateTime(dr["FeRecibo"]),

                                    Icono = "⚖",

                                    TipoActividad = "Registro de audiencia",

                                    Descripcion =
        $"NUC: {dr["NUC"]} | Causa: {dr["NoCausa"]}",

                                    Usuario = dr["Quien Realiza"].ToString(),

                                    IdRegistro = Convert.ToInt32(dr["Id"]),

                                    TablaDestino = nombreTabla,
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarError(ex.Message, nombreTabla);
                    }
                }
            }

            return lista;
        }

        private List<ActividadReciente> ObtenerActividadesCopias()
        {
            List<ActividadReciente> lista = new List<ActividadReciente>();

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();


                string nombreTabla =
    ObtenerNombreTablaPorColumna(
        conn,
        "TotDiscosEntregados");

                string query = $@"
SELECT TOP 10
    Id,
    FeRecibo,
    NUC,
    NoCausa,
    TotDiscosEntregados,
    [Quien Realiza]
FROM [{nombreTabla}]
WHERE FeRecibo IS NOT NULL
ORDER BY FeRecibo DESC";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                using (OleDbDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string nuc = dr["NUC"]?.ToString() ?? "";
                        string causa = dr["NoCausa"]?.ToString() ?? "";

                        string descripcion = "";

                        if (!string.IsNullOrWhiteSpace(nuc))
                            descripcion = $"NUC: {nuc}";

                        if (!string.IsNullOrWhiteSpace(causa))
                        {
                            if (descripcion != "")
                                descripcion += " | ";

                            descripcion += $"Causa: {causa}";
                        }

                        lista.Add(new ActividadReciente
                        {
                            FechaHora = Convert.ToDateTime(dr["FeRecibo"]),
                            Icono = "💿",
                            TipoActividad = "Entrega de copias",
                            Descripcion = descripcion,
                            Usuario = dr["Quien Realiza"].ToString(),
                            IdRegistro = Convert.ToInt32(dr["Id"]),

                            TablaDestino = nombreTabla,
                        });
                    }
                }
            }

            return lista;
        }

        private List<ActividadReciente> ObtenerActividadesEjecuciones()
        {
            List<ActividadReciente> lista = new List<ActividadReciente>();

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                string nombreTabla =
    ObtenerNombreTablaPorColumna(
        conn,
        "Expediente");

                string query = $@"
SELECT TOP 10
Id,
    FechaAudiencia,
    Expediente,
    Causa,
    Observaciones
FROM [{nombreTabla}]
WHERE FechaAudiencia IS NOT NULL
ORDER BY FechaAudiencia DESC";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                using (OleDbDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string expediente = dr["Expediente"]?.ToString() ?? "";
                        string causa = dr["Causa"]?.ToString() ?? "";

                        string descripcion = "";

                        if (!string.IsNullOrWhiteSpace(expediente))
                            descripcion = $"Expediente: {expediente}";

                        if (!string.IsNullOrWhiteSpace(causa))
                        {
                            if (descripcion != "")
                                descripcion += " | ";

                            descripcion += $"Causa: {causa}";
                        }

                        lista.Add(new ActividadReciente
                        {
                            FechaHora = Convert.ToDateTime(dr["FechaAudiencia"]),
                            Icono = "✔",
                            TipoActividad = "Registro de ejecución",
                            Descripcion = descripcion,
                            Usuario = dr["Observaciones"].ToString(),
                            IdRegistro = Convert.ToInt32(dr["Id"]),
                            TablaDestino = nombreTabla,
                        });
                    }
                }
            }

            return lista;
        }


        private List<string> ObtenerTablasAudiencias(OleDbConnection conn)
        {
            List<string> tablas = new List<string>();

            DataTable schema = conn.GetSchema("Tables");

            foreach (DataRow row in schema.Rows)
            {
                string nombreTabla = row["TABLE_NAME"].ToString();

                if (nombreTabla.StartsWith("MSys"))
                    continue;

                if (nombreTabla.StartsWith(
                    "Audiencias ",
                    StringComparison.OrdinalIgnoreCase))
                {
                    tablas.Add(nombreTabla);
                }
            }

            return tablas;
        }


        private string ObtenerNombreTablaPorColumna(
    OleDbConnection conn,
    string columna)
        {
            // Antes: escaneaba TODAS las tablas y pedía el schema de
            // columnas de cada una en CADA llamada, aunque el resultado
            // no cambia mientras la estructura de la BD no cambie.
            // Ahora se cachea por nombre de columna la primera vez que
            // se calcula, y las siguientes veces (por ejemplo, cada vez
            // que se abre Home) se devuelve directo sin volver a barrer
            // el schema completo.
            if (_cacheTablaPorColumna.TryGetValue(columna, out string tablaCacheada))
            {
                return tablaCacheada;
            }

            DataTable schema = conn.GetSchema("Tables");

            foreach (DataRow row in schema.Rows)
            {
                string nombreTabla = row["TABLE_NAME"].ToString();

                if (nombreTabla.StartsWith("MSys"))
                    continue;

                try
                {
                    DataTable columnas =
                        conn.GetOleDbSchemaTable(
                            OleDbSchemaGuid.Columns,
                            new object[] { null, null, nombreTabla, null });

                    foreach (DataRow columnaRow in columnas.Rows)
                    {
                        if (columnaRow["COLUMN_NAME"].ToString()
                            .Equals(columna,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            _cacheTablaPorColumna[columna] = nombreTabla;
                            return nombreTabla;
                        }
                    }
                }
                catch
                {
                }
            }

            return "";
        }

        // Antes, los catch de este archivo llamaban directo a
        // MessageBox.Show(...). Ahora que estos métodos se ejecutan en
        // background (vía AccesoBD.EjecutarAsync desde Dashboard/HomePage),
        // eso significa abrir un MessageBox desde un hilo que NO es el de
        // UI — lo cual en WPF puede tronar con SEHException
        // ("External component has thrown an exception"), justo el error
        // que estabas viendo.
        // Este helper detecta si estamos fuera del hilo de UI y, de ser
        // así, manda el MessageBox a ese hilo con el Dispatcher en vez de
        // mostrarlo directo desde el hilo de background.
        private static void MostrarError(string mensaje, string titulo)
        {
            var dispatcher = System.Windows.Application.Current?.Dispatcher;

            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(new Action(
                    () => MessageBox.Show(mensaje, titulo)));
            }
            else
            {
                MessageBox.Show(mensaje, titulo);
            }
        }
    }
}