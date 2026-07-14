using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using PoderJudicial.Data;
using PoderJudicial.Models;
using PoderJudicial.Views;

namespace PoderJudicial.ViewModels
{
    public class ConsultarRegistrosViewModel : BaseViewModel
    {
        
        private List<Audiencia> _listaCompleta = new List<Audiencia>();
        private DispatcherTimer _reloj;
        private string _tablaActual;


        //  PROPIEDADES

        private ObservableCollection<Audiencia> _audiencias;
        public ObservableCollection<Audiencia> Audiencias
        {
            get => _audiencias;
            set { _audiencias = value; OnPropertyChanged(); }
        }

        private List<string> _sugerencias;
        public List<string> Sugerencias
        {
            get => _sugerencias;
            set { _sugerencias = value; OnPropertyChanged(); }
        }

        private string _textoBusqueda = "";
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                _textoBusqueda = value;
                OnPropertyChanged();
                Filtrar();
                ActualizarSugerencias();
            }
        }

        private string _totalRegistros;

        private string _totalDiscosBusqueda;
        public string TotalDiscosBusqueda
        {
            get => _totalDiscosBusqueda;
            set
            {
                _totalDiscosBusqueda = value;
                OnPropertyChanged();
            }
        }

        public string TotalRegistros
        {
            get => _totalRegistros;
            set { _totalRegistros = value; OnPropertyChanged(); }
        }

        private string _hora;
        public string Hora
        {
            get => _hora;
            set { _hora = value; OnPropertyChanged(); }
        }

        private string _fecha;
        public string Fecha
        {
            get => _fecha;
            set { _fecha = value; OnPropertyChanged(); }
        }

      
        //  COMANDOS
      
        public ICommand VerCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand EliminarCommand { get; }


        //  CONSTRUCTOR

        public ConsultarRegistrosViewModel(
    string tabla)
        {
            _tablaActual = tabla;

            VerCommand =
                new RelayCommand(EjecutarVer);

            EditarCommand =
                new RelayCommand(EjecutarEditar);

            EliminarCommand =
                new RelayCommand(EjecutarEliminar);

            IniciarReloj();

            CargarDatos();
        }

        //  RELOJ

        private void IniciarReloj()
        {
            _reloj = new DispatcherTimer();
            _reloj.Interval = TimeSpan.FromSeconds(1);
            _reloj.Tick += (s, e) => ActualizarHora();
            _reloj.Start();
            ActualizarHora();
        }

        private void ActualizarHora()
        {
            Hora = DateTime.Now.ToString("hh:mm tt");
            Fecha = DateTime.Now.ToString("dddd, dd MMMM yyyy");
        }

        
        //  DATOS
       
        private void CargarDatos()
        {
            try
            {
                AudienciaData data = new AudienciaData();
                _listaCompleta = data
                    .ObtenerAudiencias(_tablaActual)
                    .OrderByDescending(a => a.Id)
    .ToList();

                Audiencias = new ObservableCollection<Audiencia>(
                    _listaCompleta.Take(10)
                );
                TotalRegistros = $"{_listaCompleta.Count} registro(s) en total";
                int totalDiscosGeneral = _listaCompleta.Sum(a =>
                {
                    if (string.IsNullOrWhiteSpace(a.TotDiscoAudiencia))
                        return 0;

                    string numeros = new string(
                        a.TotDiscoAudiencia
                        .Where(char.IsDigit)
                        .ToArray()
                    );

                    if (int.TryParse(numeros, out int valor))
                        return valor;

                    return 0;
                });

                TotalDiscosBusqueda =
                    $"Total discos audiencia: {totalDiscosGeneral}";

                CargarSugerencias();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message);
            }
        }

        private void CargarSugerencias()
        {
            Sugerencias = _listaCompleta.SelectMany(x => new[] {x.NoCausa, x.NUC, x.Imputado,x.FechaAudiencia?.ToString("dd/MM/yyyy HH:mm")
                }).Where(x => !string.IsNullOrWhiteSpace(x)) .Distinct().ToList();
        }


        //  FILTRADO Y SUGERENCIAS

        private void Filtrar()
        {
            string texto = _textoBusqueda.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(texto))
            {
                Audiencias = new ObservableCollection<Audiencia>(
                    _listaCompleta.Take(10)
                );

                TotalRegistros = $"{_listaCompleta.Count} registro(s) en total";

                int totalDiscosGeneral = _listaCompleta.Sum(a =>
                {
                    if (string.IsNullOrWhiteSpace(a.TotDiscoAudiencia))
                        return 0;

                    string numeros = new string(
                        a.TotDiscoAudiencia
                        .Where(char.IsDigit)
                        .ToArray()
                    );

                    if (int.TryParse(numeros, out int valor))
                        return valor;

                    return 0;
                });

                TotalDiscosBusqueda =
                    $"Total discos audiencia: {totalDiscosGeneral}";

                return;
            }

            // Intentar convertir a fecha
            DateTime fechaBuscada;
            bool esFecha = DateTime.TryParse(texto, out fechaBuscada);

            var filtrados = _listaCompleta.Where(a =>

                // =========================
                // No. Causa EXACTO
                // =========================
                (!string.IsNullOrWhiteSpace(a.NoCausa) &&
                 a.NoCausa.Trim().ToLower() == texto)

                ||

                // =========================
                // NUC EXACTO
                // =========================
                (!string.IsNullOrWhiteSpace(a.NUC) &&
                 a.NUC.Trim().ToLower() == texto)

                ||

                // =========================
                // FECHA EXACTA
                // =========================
                (esFecha &&
                 a.FechaAudiencia.HasValue &&
                 a.FechaAudiencia.Value.Date == fechaBuscada.Date)

                ||

               // =========================
               // TIPO CAUSA
               // =========================
               (!string.IsNullOrWhiteSpace(a.TipoCausa) &&
                 a.TipoCausa.Trim().ToLower() == texto)

                ||

                // =========================
                // TIPO AUDIENCIA / JUICIO
                // =========================
                (!string.IsNullOrWhiteSpace(a.TipoAudiencia) &&
                 a.TipoAudiencia.ToLower().Contains(texto))

                ||

                // =========================
                // IMPUTADO
                // =========================
                (!string.IsNullOrWhiteSpace(a.Imputado) &&
                 a.Imputado.ToLower().Contains(texto))

                ||

                // =========================
                // AGRAVIADO / VICTIMA
                // =========================
                (!string.IsNullOrWhiteSpace(a.Agraviado) &&
                 a.Agraviado.ToLower().Contains(texto))

            ).ToList();

            Audiencias = new ObservableCollection<Audiencia>(filtrados);

            TotalRegistros = $"{filtrados.Count} registro(s) encontrado(s)";

            int totalDiscosAudiencia = filtrados.Sum(a =>
            {
                if (string.IsNullOrWhiteSpace(a.TotDiscoAudiencia))
                    return 0;

                string numeros = new string(
                    a.TotDiscoAudiencia
                    .Where(char.IsDigit)
                    .ToArray()
                );

                if (int.TryParse(numeros, out int valor))
                    return valor;

                return 0;
            });

            TotalDiscosBusqueda =
                $"Total discos audiencia: {totalDiscosAudiencia}";


        }

        private void ActualizarSugerencias()
        {
            string texto = _textoBusqueda.Trim();

            if (string.IsNullOrWhiteSpace(texto))
            {
                Sugerencias = new List<string>();
                return;
            }

            var sugerencias = new List<string>();

            sugerencias.AddRange(
                _listaCompleta
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.NoCausa) &&
                    x.NoCausa.Contains(texto, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.NoCausa)
            );

            sugerencias.AddRange(
                _listaCompleta
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.NUC) &&
                    x.NUC.Contains(texto, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.NUC)
            );

            sugerencias.AddRange(
                _listaCompleta
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.Imputado) &&
                    x.Imputado.Contains(texto, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Imputado)
            );

            Sugerencias = sugerencias
                .Distinct()
                .Take(8)
                .ToList();
        }

        //  ACCIONES

        private void EjecutarVer(object param)
        {
            if (param is Audiencia audiencia)
            {
                try
                {
                    AudienciaData data = new AudienciaData();
                    Audiencia detalle =
     data.ObtenerAudienciaPorId(
         audiencia.Id,
         _tablaActual);

                    if (detalle != null)
                    {
                        VerDetalleRegistro ventana = new VerDetalleRegistro();
                        ventana.CargarDatos(
    Id: detalle.Id.ToString(),

    noCausa: detalle.NoCausa,
    nuc: detalle.NUC,

    fechaAudiencia:
        detalle.FechaAudiencia?
        .ToString("dd/MM/yyyy HH:mm") ?? "",

    fechaRecibo:
        detalle.FechaRecibo?
        .ToString("dd/MM/yyyy HH:mm") ?? "",

    horaConclusion:
        detalle.HoraConclusion?
        .ToString("HH:mm") ?? "",

    tipoAudiencia: detalle.TipoAudiencia,
    tipoCausa: detalle.TipoCausa,
    juzgado: detalle.Juzgado,
    juez: detalle.Juez,
    sala: detalle.Sala,

    totalDiscos:
        detalle.TotDiscos?
        .ToString() ?? "",

    tipoDisco: detalle.TipoDisco,

    totalDiscoAudiencia:
        detalle.TotDiscoAudiencia,

    imputado: detalle.Imputado,
    delito: detalle.Delito,
    agraviado: detalle.Agraviado,
    noCausaJuicio: detalle.NoCausaJuicio,
    diferida: detalle.Diferida,
    quienRealiza: detalle.QuienRealiza
);
                        ventana.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el registro.", "Aviso",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar detalle: " + ex.Message);
                }
            }
        }


        private void EjecutarEditar(object param)
        {
            if (param is Audiencia audiencia)
            {
                Dashboard dashboard = Application.Current.Windows
                    .OfType<Dashboard>()
                    .FirstOrDefault();

                if (dashboard != null)
                {
                    dashboard.FramePrincipal.Navigate(
                        new EditarRegistro(audiencia));
                }
            }
        }
        private async void EjecutarEliminar(object param)
        {
            if (param is Audiencia audiencia)
            {
                var resultado = MessageBox.Show(
                    $"¿Eliminar el registro {audiencia.NoCausa}?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    if (await EliminarDeBaseDeDatos(audiencia))
                    {
                        _listaCompleta.Remove(audiencia);
                        Filtrar();
                    }
                }
            }
        }

        private async Task<bool> EliminarDeBaseDeDatos(Audiencia audiencia)
        {
            try
            {
                using (var connection = Conexion.ObtenerConexion())
                {
                    await connection.OpenAsync();

                    string tabla = _tablaActual;
                    string query = $"DELETE FROM [{tabla}] WHERE NoCausa = @NoCausa";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NoCausa", audiencia.NoCausa);
                        int filasAfectadas = await command.ExecuteNonQueryAsync();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al eliminar: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
        }
    }
}