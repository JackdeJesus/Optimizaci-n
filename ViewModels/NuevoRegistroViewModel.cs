using PoderJudicial.Data;
using PoderJudicial.Helpers;
using PoderJudicial.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PoderJudicial.ViewModels
{
    public class NuevoRegistroViewModel : BaseViewModel
    {
        // ── Historial para autocomplete ──────────────────
        public List<Audiencia> HistorialAudiencias { get; private set; }
        public List<Ejecucion> HistorialEjecuciones { get; private set; }
        public List<string> Jueces { get; private set; }

        public NuevoRegistroViewModel()
        {
            HistorialAudiencias = new AudienciaData().ObtenerAudiencias();
            HistorialEjecuciones = new EjecucionData().ObtenerEjecuciones();
            Jueces = new JuezRepository().ObtenerJueces();
        }

        // ── Siguiente ID visual ──────────────────────────
        public int ObtenerSiguienteId(string tipoCausa)
        {
            return tipoCausa == "EXP"
                ? new EjecucionData().ObtenerSiguienteId()
                : new AudienciaData().ObtenerSiguienteIdVisual();
        }

        // ── Delitos filtrados por tipo de causa ──────────
        public List<string> ObtenerDelitosFiltrados(string tipoCausa)
        {
            if (tipoCausa == "EXP")
                return HistorialEjecuciones
                    .Where(x => !string.IsNullOrWhiteSpace(x.Delito))
                    .Select(x => x.Delito).Distinct().ToList();

            return HistorialAudiencias
                .Where(x => x.TipoCausa == tipoCausa &&
                            !string.IsNullOrWhiteSpace(x.Delito))
                .Select(x => x.Delito).Distinct().ToList();
        }

        // ── Tipos de audiencia filtrados ─────────────────
        public List<string> ObtenerAudienciasFiltradas(string tipoCausa)
        {
            if (tipoCausa == "EXP")
                return HistorialEjecuciones
                    .Where(x => !string.IsNullOrWhiteSpace(x.TipoAudiencia))
                    .Select(x => x.TipoAudiencia).Distinct().ToList();

            return HistorialAudiencias
                .Where(x => x.TipoCausa == tipoCausa &&
                            !string.IsNullOrWhiteSpace(x.TipoAudiencia))
                .Select(x => x.TipoAudiencia).Distinct().ToList();
        }

        // ── Guardar Audiencia ────────────────────────────
        public void GuardarAudiencia(Audiencia registro)
        {
            new AudienciaData().Insertar(registro);
        }

        // ── Guardar Ejecucion ────────────────────────────
        public void GuardarEjecucion(Ejecucion expediente)
        {
            new EjecucionData().Insertar(expediente);
        }

        // ── Construir modelo Audiencia ───────────────────
        public Audiencia ConstruirAudiencia(
            int id, string noCausa, string nuc,
            DateTime? fechaAudiencia, DateTime? fechaRecibo,
            string tipoAudiencia, string tipoCausa,
            string juzgado, string juez, string imputado,
            string delito, string agraviado, string sala,
            DateTime? horaConclusion, string noCausaJuicio,
            int? totDiscos, string totDiscoAudiencia)
        {
            return new Audiencia
            {
                Id = id,
                NoCausa = noCausa,
                NUC = nuc,
                FechaAudiencia = fechaAudiencia,
                FechaRecibo = fechaRecibo,
                TipoAudiencia = tipoAudiencia,
                TipoCausa = tipoCausa,
                Juzgado = juzgado,
                Juez = juez,
                Imputado = imputado,
                Delito = delito,
                Agraviado = agraviado,
                Sala = sala,
                HoraConclusion = horaConclusion,
                NoCausaJuicio = noCausaJuicio,
                Diferida = string.Empty,
                QuienRealiza = SesionActual.Usuario,
                TotDiscos = totDiscos,
                TipoDisco = "Archivo",
                TotDiscoAudiencia = totDiscoAudiencia
            };
        }

        // ── Construir modelo Ejecucion ───────────────────
        public Ejecucion ConstruirEjecucion(
            int id, DateTime? fechaAudiencia,
            string totalDiscos, string juez,
            string expedienteNumero, string causa,
            string tipoAudiencia, string horaTermino,
            string imputado, string delito,
            string victima, string sala)
        {
            return new Ejecucion
            {
                Id = id,
                FechaAudiencia = fechaAudiencia,
                TotalDiscos = totalDiscos,
                Juez = juez,
                ExpedienteNumero = expedienteNumero,
                Causa = causa,
                TipoAudiencia = tipoAudiencia,
                HoraTermino = horaTermino,
                Imputado = imputado,
                Delito = delito,
                Victima = victima,
                Sala = sala,
                Observaciones = SesionActual.Usuario
            };
        }
    }
}