using PoderJudicial.Models;
using PoderJudicial.Views;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Reflection;
using System;
using System.Data.OleDb;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PoderJudicial.Data
{
    public class AudienciaData
    {
        // ──────────────────────────────────────────
        //  Mapeo centralizado — un solo lugar para
        //  leer columnas del reader al modelo
        //
        //  OPTIMIZACIÓN: ahora recibe el set de columnas
        //  ya calculado (una vez por consulta, no por fila).
        //  La lógica de negocio (valores, fallbacks, orden)
        //  es IDÉNTICA a la versión original.
        // ──────────────────────────────────────────
        private static Audiencia MapearDesdeReader(
            OleDbDataReader reader,
            HashSet<string> columnas)
        {
            Audiencia a = new Audiencia
            {
                // ─────────────────────────────
                // ID
                // ─────────────────────────────
                Id = columnas.Contains("Id") &&
                     reader["Id"] != DBNull.Value
                    ? Convert.ToInt32(reader["Id"])
                    : 0,

                // ─────────────────────────────
                // FECHA AUDIENCIA
                // ─────────────────────────────




                FechaAudiencia =
                    columnas.Contains("FeAudiencia") &&
                    DateTime.TryParse(
                        reader["FeAudiencia"]?.ToString(),
                        out DateTime fechaAud1)

                        ? fechaAud1

                        : columnas.Contains("FechaAudiencia") &&
                          DateTime.TryParse(
                              reader["FechaAudiencia"]?.ToString(),
                              out DateTime fechaAud2)

                            ? fechaAud2
                            : null,



                // ─────────────────────────────
                // FECHA RECIBO
                // ─────────────────────────────
                FechaRecibo =
                    columnas.Contains("FeRecibo") &&
                    DateTime.TryParse(
                        reader["FeRecibo"]?.ToString(),
                        out DateTime fechaRec)

                        ? fechaRec

                        : null,

                // ─────────────────────────────
                // TOTAL DISCOS
                // ─────────────────────────────
                TotDiscos =
                    columnas.Contains("TotDiscos") &&
                    int.TryParse(
                        reader["TotDiscos"]?.ToString(),
                        out int discosAud)
                        ? discosAud

                    : columnas.Contains("TotalDiscos") &&
                      int.TryParse(
                          new string(
                              reader["TotalDiscos"]
                                  ?.ToString()
                                  .Where(char.IsDigit)
                                  .ToArray()),
                          out int discosEjec)
                        ? discosEjec

                    : columnas.Contains("TotDiscosEntregados") &&
                      int.TryParse(
                          reader["TotDiscosEntregados"]?.ToString(),
                          out int discosCopias)
                        ? discosCopias

                    : null,

                // ─────────────────────────────
                // TIPO DISCO
                // ─────────────────────────────
                TipoDisco =
                    columnas.Contains("TipoDisco")
                        ? reader["TipoDisco"]?.ToString()
                        : "",

                // ─────────────────────────────
                // JUZGADO
                // ─────────────────────────────
                Juzgado =
                    columnas.Contains("Juzgado")
                        ? reader["Juzgado"]?.ToString()
                        : "",

                // ─────────────────────────────
                // TOTAL DISCO AUDIENCIA
                // ─────────────────────────────
                TotDiscoAudiencia =
                    columnas.Contains("TotDiscoAudiencia")
                        ? reader["TotDiscoAudiencia"]?.ToString()

                    : columnas.Contains("DiscosExternos")
                        ? reader["DiscosExternos"]?.ToString()

                    : "",

                // ─────────────────────────────
                // JUEZ
                // ─────────────────────────────
                Juez =
                    columnas.Contains("Juez")
                        ? reader["Juez"]?.ToString()
                        : "",

                // ─────────────────────────────
                // NO CAUSA
                // ─────────────────────────────
                NoCausa =
                    columnas.Contains("NoCausa")
                        ? reader["NoCausa"]?.ToString()
                        : columnas.Contains("Causa")
                            ? reader["Causa"]?.ToString()
                            : "",

                // ─────────────────────────────
                // NUC
                // ─────────────────────────────
                NUC =
                    columnas.Contains("NUC")
                        ? reader["NUC"]?.ToString()
                        : "",

                // ─────────────────────────────
                // TIPO CAUSA
                // ─────────────────────────────
                TipoCausa =
                    columnas.Contains("TipoCausa")
                        ? reader["TipoCausa"]?.ToString()
                        : "EXP",

                // ─────────────────────────────
                // TIPO AUDIENCIA
                // ─────────────────────────────
                TipoAudiencia =
                    columnas.Contains("TipoAudiencia")
                        ? reader["TipoAudiencia"]?.ToString()
                        : "",

                // ─────────────────────────────
                // HORA CONCLUSION / TERMINO
                // ─────────────────────────────
                HoraConclusion =
                    columnas.Contains("Hora conclusion") &&
                    DateTime.TryParse(
                        reader["Hora conclusion"]?.ToString(),
                        out DateTime hora1)

                        ? hora1

                        : columnas.Contains("HoraTermino") &&
                          DateTime.TryParse(
                              reader["HoraTermino"]?.ToString(),
                              out DateTime hora2)

                            ? hora2

                            : null,

                // ─────────────────────────────
                // IMPUTADO
                // ─────────────────────────────
                Imputado =
                    columnas.Contains("Imputado")
                        ? reader["Imputado"]?.ToString()
                        : "",

                // ─────────────────────────────
                // DELITO
                // ─────────────────────────────
                Delito =
                    columnas.Contains("Delito")
                        ? reader["Delito"]?.ToString()
                        : "",

                // ─────────────────────────────
                // AGRAVIADO / VICTIMA
                // ─────────────────────────────
                Agraviado =
                    columnas.Contains("Agraviado")
                        ? reader["Agraviado"]?.ToString()
                        : columnas.Contains("Victima")
                            ? reader["Victima"]?.ToString()
                            : "",

                // ─────────────────────────────
                // SALA
                // ─────────────────────────────
                Sala =
                    columnas.Contains("Sala")
                        ? reader["Sala"]?.ToString()
                        : "",

                // ─────────────────────────────
                // NO CAUSA JUICIO
                // ─────────────────────────────
                NoCausaJuicio =
                    columnas.Contains("NoCausaJuicio")
                        ? reader["NoCausaJuicio"]?.ToString()
                        : "",

                // ─────────────────────────────
                // DIFERIDA
                // ─────────────────────────────
                Diferida =
                    columnas.Contains("Diferida")
                        ? reader["Diferida"]?.ToString()
                        : "",

                // ─────────────────────────────
                // QUIEN REALIZA
                // ─────────────────────────────
                QuienRealiza =
                    columnas.Contains("QuienRealiza")
                        ? reader["QuienRealiza"]?.ToString()

                    : columnas.Contains("Quien Realiza")
                        ? reader["Quien Realiza"]?.ToString()

                    : columnas.Contains("Observaciones")
                        ? reader["Observaciones"]?.ToString()

                    : "",

                // ─────────────────────────────
                // OBSERVACIONES
                // ─────────────────────────────
                Observaciones =
                    columnas.Contains("Observaciones")
                        ? reader["Observaciones"]?.ToString()
                        : "",

                //
                //EXPEDIENTE (NO EN TODOS LOS REGISTROS)  Y ETIQUETAS ENTREGADAS (NO EN TODOS LOS REGISTROS)
                //

                Expediente =
                    columnas.Contains("Expediente")
                        ? reader["Expediente"]?.ToString()
                        : "",

                DiscosExternos =
                    columnas.Contains("DiscosExternos")
                        ? reader["DiscosExternos"]?.ToString()
                        : "",

                EtiquetasEntregadas =
                    columnas.Contains("Etiquetas entregadas")
                        ? reader["Etiquetas entregadas"]?.ToString()
                        : "",

                AQuienEntrega =
                    columnas.Contains("A quien se entraga")
                        ? reader["A quien se entraga"]?.ToString()
                        : "",
            };

            // ÍNDICE DE BÚSQUEDA OPTIMIZADO
            a.TextoBusqueda = string.Join(" ", new[]
                {
                    a.Id.ToString(),
                    a.NoCausa,
                    a.NUC,
                    a.Imputado,
                    a.Delito,
                    a.Juez,
                    a.Juzgado,
                    a.TipoAudiencia,
                    a.TipoCausa,
                    a.Agraviado,
                    a.Sala,
                    a.NoCausaJuicio,
                    a.QuienRealiza,
                    a.FechaAudiencia?.ToString("dd/MM/yyyy"),
                    a.FechaRecibo?.ToString("dd/MM/yyyy")
                }
                .Where(x => !string.IsNullOrWhiteSpace(x)))
                .ToLower();

            return a;
        }

        // ──────────────────────────────────────────
        //  NUEVO: calcula el set de columnas UNA VEZ
        //  por consulta (no por fila). Esta es la
        //  optimización principal contra el "congelado".
        // ──────────────────────────────────────────
        private static HashSet<string> ObtenerColumnasReader(OleDbDataReader reader)
        {
            var columnas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < reader.FieldCount; i++)
                columnas.Add(reader.GetName(i));

            return columnas;
        }

        // ──────────────────────────────────────────
        //  NUEVO: recorre el reader completo y arma
        //  la lista, usando el set de columnas cacheado.
        //  filtrarVacios reproduce el comportamiento
        //  original de ObtenerAudiencias() (sin tabla).
        // ──────────────────────────────────────────
        private static List<Audiencia> LeerListaAudiencias(
            OleDbDataReader reader,
            bool filtrarVacios)
        {
            List<Audiencia> lista = new List<Audiencia>();
            HashSet<string> columnas = ObtenerColumnasReader(reader);

            while (reader.Read())
            {
                if (filtrarVacios &&
                    string.IsNullOrWhiteSpace(reader["NoCausa"]?.ToString()) &&
                    string.IsNullOrWhiteSpace(reader["NUC"]?.ToString()))
                    continue;

                lista.Add(MapearDesdeReader(reader, columnas));
            }

            return lista;
        }

        // ──────────────────────────────────────────
        //  Parámetros centralizados — mismo orden
        //  que el INSERT y el UPDATE (sin cambios)
        // ──────────────────────────────────────────
        private static void AgregarParametros(OleDbCommand cmd, Audiencia a)
        {
            cmd.Parameters.AddWithValue("@Id", a.Id);
            cmd.Parameters.AddWithValue("@FeAudiencia", a.FechaAudiencia.HasValue ? a.FechaAudiencia.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@FeRecibo", a.FechaRecibo.HasValue ? a.FechaRecibo.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@TotDiscos", a.TotDiscos.HasValue ? a.TotDiscos.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@TipoDisco", a.TipoDisco ?? string.Empty);
            cmd.Parameters.AddWithValue("@Juzgado", a.Juzgado ?? string.Empty);
            cmd.Parameters.AddWithValue("@TotDiscoAudiencia", a.TotDiscoAudiencia ?? string.Empty);
            cmd.Parameters.AddWithValue("@Juez", a.Juez ?? string.Empty);
            cmd.Parameters.AddWithValue("@NoCausa", a.NoCausa ?? string.Empty);
            cmd.Parameters.AddWithValue("@NUC", a.NUC ?? string.Empty);
            cmd.Parameters.AddWithValue("@TipoCausa", a.TipoCausa ?? string.Empty);
            cmd.Parameters.AddWithValue("@TipoAudiencia", a.TipoAudiencia ?? string.Empty);
            cmd.Parameters.AddWithValue("@HoraConclusion", a.HoraConclusion.HasValue ? a.HoraConclusion.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@Imputado", a.Imputado ?? string.Empty);
            cmd.Parameters.AddWithValue("@Delito", a.Delito ?? string.Empty);
            cmd.Parameters.AddWithValue("@Agraviado", a.Agraviado ?? string.Empty);
            cmd.Parameters.AddWithValue("@Sala", a.Sala ?? string.Empty);
            cmd.Parameters.AddWithValue("@NoCausaJuicio", a.NoCausaJuicio ?? string.Empty);
            cmd.Parameters.AddWithValue("@Diferida", a.Diferida ?? string.Empty);
            cmd.Parameters.AddWithValue("@QuienRealiza", a.QuienRealiza ?? string.Empty);
        }

        
        public List<Audiencia> ObtenerAudiencias()
        {
            return ObtenerAudienciasInterno(TableDetector.TablaActual, filtrarVacios: true);
        }

        // ──────────────────────────────────────────
        //  OBTENER TODOS DE TABLA ESPECÍFICA
        //  (firma pública sin cambios)
        // ──────────────────────────────────────────
        public List<Audiencia> ObtenerAudiencias(string tabla)
        {
            return ObtenerAudienciasInterno(tabla, filtrarVacios: false);
        }

        // NUEVO: lógica compartida entre las dos sobrecargas de arriba
        private List<Audiencia> ObtenerAudienciasInterno(string tabla, bool filtrarVacios)
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();
                string query = $"SELECT * FROM [{tabla}]";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    return LeerListaAudiencias(reader, filtrarVacios);
                }
            }
        }

        // ──────────────────────────────────────────
        //  OBTENER UNO POR NoCausa (sin cambios de firma)
        // ──────────────────────────────────────────
        public Audiencia ObtenerAudienciaPorNoCausa(string noCausa)
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();
                string query = $"SELECT * FROM [{TableDetector.TablaActual}] WHERE NoCausa = ?";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", noCausa);

                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var columnas = ObtenerColumnasReader(reader);
                            return MapearDesdeReader(reader, columnas);
                        }
                    }
                }
            }

            return null;
        }

        // ──────────────────────────────────────────
        //  OBTENER UNO POR ID (firma pública sin cambios)
        // ──────────────────────────────────────────
        public Audiencia ObtenerAudienciaPorId(int id)
        {
            return ObtenerAudienciaPorId(id, TableDetector.TablaActual);
        }

        // ──────────────────────────────────────────
        //  OBTENER UNO POR ID + TABLA (firma pública sin cambios)
        // ──────────────────────────────────────────
        public Audiencia ObtenerAudienciaPorId(int id, string tabla)
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                string query = $"SELECT * FROM [{tabla}] WHERE Id = ?";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", id);

                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var columnas = ObtenerColumnasReader(reader);
                            return MapearDesdeReader(reader, columnas);
                        }
                    }
                }
            }

            return null;
        }

        // ──────────────────────────────────────────
        //  INSERT (sin cambios de lógica)
        // ──────────────────────────────────────────
        public void Insertar(Audiencia a)
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();
                string tabla = TableDetector.TablaActual;

                string sql = $@"
                    INSERT INTO [{tabla}] (Id,
                        FeAudiencia, FeRecibo,
                        TotDiscos, TipoDisco,
                        Juzgado, TotDiscoAudiencia,
                        Juez, NoCausa, NUC,
                        TipoCausa, TipoAudiencia,
                        [Hora conclusion],
                        Imputado, Delito, Agraviado,
                        Sala, NoCausaJuicio,
                        Diferida, [Quien Realiza]
                    ) VALUES (
                       ?, ?, ?,
                        ?, ?,
                        ?, ?,
                        ?, ?, ?,
                        ?, ?,
                        ?,
                        ?, ?, ?,
                        ?, ?,
                        ?, ?
                    )";

                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    AgregarParametros(cmd, a);
                    cmd.ExecuteNonQuery();
                }
            }

            TableDetector.InvalidarCache();
        }

        public int ObtenerSiguienteIdVisual()
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                string tabla = TableDetector.TablaActual;
                string sql = $"SELECT MAX(Id) FROM [{tabla}]";

                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    object resultado = cmd.ExecuteScalar();

                    if (resultado == DBNull.Value || resultado == null)
                        return 1;

                    return Convert.ToInt32(resultado) + 1;
                }
            }
        }

        //  ACTUALIZAR (sin cambios de lógica)
        public void Actualizar(Audiencia a)
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();
                string tabla = TableDetector.TablaActual;

                string sql = $@"
                    UPDATE [{tabla}] SET
                        FeAudiencia        = ?,
                        FeRecibo           = ?,
                        TotDiscos          = ?,
                        TipoDisco          = ?,
                        Juzgado            = ?,
                        TotDiscoAudiencia  = ?,
                        Juez               = ?,
                        NoCausa            = ?,
                        NUC                = ?,
                        TipoCausa          = ?,
                        TipoAudiencia      = ?,
                        [Hora conclusion]  = ?,
                        Imputado           = ?,
                        Delito             = ?,
                        Agraviado          = ?,
                        Sala               = ?,
                        NoCausaJuicio      = ?,
                        Diferida           = ?,
                        [Quien Realiza]    = ?
                    WHERE Id = ?";

                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@FeAudiencia", a.FechaAudiencia.HasValue ? (object)a.FechaAudiencia.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@FeRecibo", a.FechaRecibo.HasValue ? (object)a.FechaRecibo.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@TotDiscos", a.TotDiscos.HasValue ? (object)a.TotDiscos.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@TipoDisco", a.TipoDisco ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Juzgado", a.Juzgado ?? string.Empty);
                    cmd.Parameters.AddWithValue("@TotDiscoAudiencia", a.TotDiscoAudiencia ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Juez", a.Juez ?? string.Empty);
                    cmd.Parameters.AddWithValue("@NoCausa", a.NoCausa ?? string.Empty);
                    cmd.Parameters.AddWithValue("@NUC", a.NUC ?? string.Empty);
                    cmd.Parameters.AddWithValue("@TipoCausa", a.TipoCausa ?? string.Empty);
                    cmd.Parameters.AddWithValue("@TipoAudiencia", a.TipoAudiencia ?? string.Empty);
                    cmd.Parameters.AddWithValue("@HoraConclusion", a.HoraConclusion.HasValue ? (object)a.HoraConclusion.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Imputado", a.Imputado ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Delito", a.Delito ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Agraviado", a.Agraviado ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Sala", a.Sala ?? string.Empty);
                    cmd.Parameters.AddWithValue("@NoCausaJuicio", a.NoCausaJuicio ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Diferida", a.Diferida ?? string.Empty);
                    cmd.Parameters.AddWithValue("@QuienRealiza", a.QuienRealiza ?? string.Empty);
                    // WHERE Id = ? — va AL FINAL en OleDb
                    cmd.Parameters.AddWithValue("@Id", a.Id);

                    cmd.ExecuteNonQuery();
                }
            }

            TableDetector.InvalidarCache();
        }

        public string ObtenerNUCPorNoCausa(string noCausa)
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                DataTable schema = conn.GetSchema("Tables");

                foreach (DataRow row in schema.Rows)
                {
                    string nombreTabla = row["TABLE_NAME"].ToString();

                    if (!nombreTabla.Contains("Audiencias"))
                        continue;

                    if (nombreTabla.StartsWith("MSys"))
                        continue;

                    string query = $"SELECT TOP 1 NUC FROM [{nombreTabla}] WHERE NoCausa = ?";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", noCausa);

                        object resultado = cmd.ExecuteScalar();

                        if (resultado != null && resultado != DBNull.Value)
                        {
                            return resultado.ToString();
                        }
                    }
                }
            }

            return "";
        }

        public string ObtenerVersionSistema()
        {
            return Assembly
                .GetExecutingAssembly()
                .GetName()
                .Version
                ?.ToString() ?? "1.0.0";
        }

        // ══════════════════════════════════════════
        //  NUEVO (OPCIONAL): versiones asíncronas.
        //  No reemplazan a las síncronas, así que nada
        //  de lo que ya llama a AudienciaData se rompe.
        //  Úsalas desde tus ViewModels con `await` para
        //  que la carga NO congele la UI.
        // ══════════════════════════════════════════
        public Task<List<Audiencia>> ObtenerAudienciasAsync()
            => Task.Run(() => ObtenerAudiencias());

        public Task<List<Audiencia>> ObtenerAudienciasAsync(string tabla)
            => Task.Run(() => ObtenerAudiencias(tabla));

        public Task<Audiencia> ObtenerAudienciaPorIdAsync(int id)
            => Task.Run(() => ObtenerAudienciaPorId(id));

        public Task<Audiencia> ObtenerAudienciaPorNoCausaAsync(string noCausa)
            => Task.Run(() => ObtenerAudienciaPorNoCausa(noCausa));

        public Task InsertarAsync(Audiencia a)
            => Task.Run(() => Insertar(a));

        public Task ActualizarAsync(Audiencia a)
            => Task.Run(() => Actualizar(a));
    }
}