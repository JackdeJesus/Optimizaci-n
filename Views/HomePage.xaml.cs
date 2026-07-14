using PoderJudicial.Data;
using PoderJudicial.Helpers;
using PoderJudicial.Models;
using PoderJudicial.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using PoderJudicial.Models;
using System.Windows.Input;
using System.Windows.Media;

using PoderJudicial.Data;

namespace PoderJudicial.Views
{
    /// <summary>
    /// Lógica de interacción para HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private HomePageViewModel vm;
        private DispatcherTimer timer;

        // Brush cacheado para el hover de "Actividad reciente".
        // Antes se creaba un SolidColorBrush + ColorConverter nuevo
        // cada vez que el mouse entraba a una fila (Actividad_MouseEnter),
        // lo cual es trabajo innecesario si el usuario mueve el mouse
        // varias veces sobre la lista.
        private static readonly Brush BrushHoverActividad =
            CrearBrushCongelado("#F8FAFC");

        private static Brush CrearBrushCongelado(string hex)
        {
            var brush = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(hex));

            if (brush.CanFreeze)
                brush.Freeze();

            return brush;
        }

        public HomePage()
        {
            InitializeComponent();
            vm = new HomePageViewModel();
            DataContext = vm;
            IniciarReloj();

            // Antes: CargarDashboard() hacía varias consultas a la BD de
            // forma sincrónica dentro del constructor, bloqueando la UI
            // cada vez que se abría/regresaba a Home. Ahora corre en
            // background.
            _ = CargarDashboardAsync();

            // Cada navegación a Home crea una instancia nueva de HomePage
            // (ver Dashboard.Navegar), y cada una arranca su propio timer.
            // Sin esto, los timers de instancias anteriores nunca se
            // detenían y seguían corriendo (y disparando Tick) para
            // siempre en segundo plano, acumulándose con cada visita a
            // Home. Al detener el timer cuando la página se descarga,
            // evitamos esa fuga y el consumo de CPU/memoria que iba
            // creciendo con el uso prolongado del programa.
            Unloaded += (s, e) => timer?.Stop();
        }

        private Dashboard ObtenerDashboard()
        {
            return Window.GetWindow(this) as Dashboard;
        }

        private void ActualizarFechaHora()
        {
            DateTime ahora = DateTime.Now;
            CultureInfo cultura = new CultureInfo("es-MX");
            TxtHora.Text = ahora.ToString("hh:mm tt");
            TxtFecha.Text = ahora.ToString("dddd, dd MMMM yyyy", cultura);
        }

        private void IniciarReloj()
        {
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, e) => ActualizarFechaHora();
            timer.Start();
            ActualizarFechaHora();
        }

        private async Task CargarDashboardAsync()
        {
            DashboardData dashboard = new DashboardData();

            // Igual que en Dashboard.xaml.cs: se pasa por AccesoBD para
            // que esta consulta no se ejecute al mismo tiempo que otra
            // llamada a OleDb (por ejemplo, Dashboard cargando la lista
            // de tablas justo cuando se abre esta página tras el login).
            // Eso era lo que causaba el SEHException.
            var datos = await AccesoBD.EjecutarAsync(() => new
            {
                TotalAudiencias = dashboard.ObtenerTotalAudienciasMes(),
                TotalEjecuciones = dashboard.ObtenerTotalEjecucionesMes(),
                TotalCopias = dashboard.ObtenerTotalCopiasMes(),
                AudienciasHoy = dashboard.ObtenerAudienciasHoy(),
                Version = dashboard.ObtenerVersionSistema(),
                NombreBD = dashboard.ObtenerNombreBaseDatos(),
                Estado = dashboard.ObtenerEstadoSistema(),
                Actividades = dashboard.ObtenerActividadesRecientes()
            });

            vm.TotalAudienciasMes = datos.TotalAudiencias;
            vm.TotalEjecucionesMes = datos.TotalEjecuciones;
            vm.TotalCopiasMes = datos.TotalCopias;
            vm.AudienciasHoy = datos.AudienciasHoy;
            vm.VersionSistema = datos.Version;
            vm.NombreBaseDatos = datos.NombreBD;
            vm.EstadoSistema = datos.Estado;

            // Temporal mientras no exista la lógica de respaldos
            vm.UltimaCopiaSeguridad = "No disponible";

            vm.Actividades = new ObservableCollection<ActividadReciente>(
                datos.Actividades);
        }

        private void CardNuevoRegistro_Click(
            object sender,
            RoutedEventArgs e)
        {
            ObtenerDashboard()?.AbrirNuevoRegistro();
        }

        private void CardConsultar_Click(
            object sender,
            RoutedEventArgs e)
        {
            ObtenerDashboard()?.AbrirConsultarRegistros();
        }

        private void CardCopias_Click(
            object sender,
            RoutedEventArgs e)
        {
            ObtenerDashboard()?.AbrirRegistroCopias();
        }

        private void CardReportes_Click(
            object sender,
            RoutedEventArgs e)
        {
            ObtenerDashboard()?.AbrirReportes();
        }

        private void CardConfiguracion_Click(
            object sender,
            RoutedEventArgs e)
        {
            ObtenerDashboard()?.AbrirConfiguracion();
        }

        private void Actividad_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = BrushHoverActividad;
            }
        }

        private void Actividad_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = Brushes.Transparent;
            }
        }

        private void Actividad_Click(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if (border == null)
                return;

            ActividadReciente actividad =
                border.DataContext as ActividadReciente;
            if (actividad == null)
                return;

            ObtenerDashboard()?
                .AbrirConsultarRegistros(
                    actividad.TablaDestino);
        }
    }
}