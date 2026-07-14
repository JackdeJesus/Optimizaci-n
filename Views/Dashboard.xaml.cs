 using PoderJudicial.Data;
using PoderJudicial.Helpers;
using PoderJudicial.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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
using System.Windows.Shapes;
using PoderJudicial.Data;

namespace PoderJudicial.Views
{
    public partial class Dashboard : Window
    {
        private bool _submenuConsultasVisible = false;
        private Button _tablaSeleccionada = null;

        // ── Brushes cacheados ────────────────────────────────────────────
        // Antes se creaban con ColorConverter.ConvertFromString cada vez que
        // se hacía clic en un botón o se seleccionaba una tabla. Ahora se
        // crean una única vez y se "congelan" (Freeze) para que WPF las
        // reutilice sin recalcular nada. Esto reduce el trabajo en cada
        // clic y evita micro-congelamientos perceptibles en la UI.
        private static readonly Brush BrushTransparente = Brushes.Transparent;

        private static readonly Brush BrushFondoActivo =
            CrearBrushCongelado("#2A3147");

        private static readonly Brush BrushTextoActivo =
            CrearBrushCongelado("#2ECC8F");

        private static readonly Brush BrushTextoInactivo =
            CrearBrushCongelado("#8B92A5");

        private static readonly Brush BrushTextoTablaInactiva =
            CrearBrushCongelado("#B8C1D1");

        private static Brush CrearBrushCongelado(string hex)
        {
            var brush = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(hex));

            if (brush.CanFreeze)
                brush.Freeze();

            return brush;
        }

        public Dashboard(string usuario)
        {
            InitializeComponent();

            MainFrame.Navigate(new HomePage());

            ActivarBoton(BtnHome);

            // Antes: CargarTablasBD() bloqueaba el hilo de UI mientras
            // se conectaba a la BD y leía el schema (I/O sincrónico).
            // Ahora se dispara en background y no congela la ventana
            // al abrir el Dashboard.
            _ = CargarTablasBDAsync();

            txtAvatar.Text = usuario.Substring(0, 1).ToUpper();
            txtNombreUsuario.Text = usuario;
        }

        public Frame FramePrincipal => MainFrame;

        // ── Carga de tablas (asíncrona) ─────────────────────────────────
        private async Task CargarTablasBDAsync()
        {
            // Se usa AccesoBD.EjecutarAsync en lugar de Task.Run directo
            // para que esta operación nunca se solape con otra llamada a
            // OleDb (por ejemplo, la de HomePage cargando sus totales al
            // mismo tiempo justo después del login). El driver ACE OLEDB
            // no soporta accesos concurrentes y truena con SEHException
            // si dos operaciones le pegan a la vez.
            List<string> tablas = await AccesoBD.EjecutarAsync(() =>
            {
                List<string> resultado = new();

                using (OleDbConnection conn = Conexion.ObtenerConexion())
                {
                    conn.Open();

                    DataTable schema = conn.GetSchema("Tables");

                    foreach (DataRow row in schema.Rows)
                    {
                        string nombreTabla = row["TABLE_NAME"].ToString();

                        // IGNORAR tablas del sistema
                        if (nombreTabla.StartsWith("MSys"))
                            continue;

                        // IGNORAR tablas temporales
                        if (nombreTabla.StartsWith("~"))
                            continue;

                        resultado.Add(nombreTabla);
                    }
                }

                return resultado
                    .OrderByDescending(x => x)
                    .ToList();
            });

            // Volvemos al hilo de UI para actualizar el ItemsSource
            PanelTablas.ItemsSource = tablas;
        }

        private void BtnTablaDinamica_Click(
            object sender,
            RoutedEventArgs e)
        {
            ActivarBoton(BtnConsultar);

            Button btn = (Button)sender;
            string nombreTabla = btn.Content.ToString();

            SeleccionarTabla(btn);

            MainFrame.Navigate(
                new ConsultarRegistros(nombreTabla));
        }

        // ── Selección / deselección de tabla en el panel lateral ────────
        // Extraído de los 6 lugares donde se repetía este mismo bloque.
        private void DeseleccionarTabla()
        {
            if (_tablaSeleccionada == null)
                return;

            _tablaSeleccionada.Background = BrushTransparente;
            _tablaSeleccionada.Foreground = BrushTextoTablaInactiva;
            _tablaSeleccionada = null;
        }

        private void SeleccionarTabla(Button btn)
        {
            DeseleccionarTabla();

            _tablaSeleccionada = btn;
            _tablaSeleccionada.Background = BrushFondoActivo;
            _tablaSeleccionada.Foreground = BrushTextoActivo;
        }

        // ACTIVAR BOTÓN
        private void ActivarBoton(Button botonActivo)
        {
            // TODOS LOS BOTONES
            Button[] botones =
            {
                BtnConsultar,
                BtnNuevo,
                BtnCopias,
                BtnReportes,
                BtnConfig,
                BtnHome
            };

            // Desactivar todos
            foreach (Button btn in botones)
            {
                btn.Background = BrushTransparente;
                btn.Foreground = BrushTextoInactivo;
            }

            // activar solo uno
            botonActivo.Background = BrushFondoActivo;
            botonActivo.Foreground = BrushTextoActivo;
        }

        private void Navegar(Page pagina, Button boton)
        {
            ActivarBoton(boton);
            DeseleccionarTabla();

            MainFrame.Navigate(pagina);
        }

        // Consultar
        private void BtnConsultar_Click(
            object sender,
            RoutedEventArgs e)
        {
            _submenuConsultasVisible = !_submenuConsultasVisible;

            PanelTablas.Visibility =
                _submenuConsultasVisible
                ? Visibility.Visible
                : Visibility.Collapsed;

            TxtFlechaConsultar.Text =
                _submenuConsultasVisible
                ? "▲"
                : "▼";
        }

        // Nuevo
        private void BtnNuevo_Click(
            object sender,
            RoutedEventArgs e)
        {
            Navegar(new NuevoRegistro(), BtnNuevo);
        }

        // Copias
        private void BtnCopias_Click(
            object sender,
            RoutedEventArgs e)
        {
            Navegar(new RegistroCopias(), BtnCopias);
        }

        // REPORTES
        private void BtnReportes_Click(
            object sender,
            RoutedEventArgs e)
        {
            Navegar(new ReportesView(), BtnReportes);
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            SesionActual.Usuario = string.Empty;
            Login login = new Login();
            login.Show();

            this.Close();
        }

        // Config
        private void BtnConfig_Click(
            object sender,
            RoutedEventArgs e)
        {
            AbrirMenuConfiguracion();
        }

        // HOME
        private void BtnHome_Click(
            object sender,
            RoutedEventArgs e)
        {
            Navegar(new HomePage(), BtnHome);
        }

        public void AbrirNuevoRegistro()
        {
            Navegar(new NuevoRegistro(), BtnNuevo);
        }

        public void AbrirRegistroCopias()
        {
            Navegar(new RegistroCopias(), BtnCopias);
        }

        public void AbrirReportes()
        {
            Navegar(new ReportesView(), BtnReportes);
        }

        public void AbrirHome()
        {
            Navegar(new HomePage(), BtnHome);
        }

        public void AbrirConfiguracion()
        {
            AbrirMenuConfiguracion();
        }

        // BtnConfig_Click y AbrirConfiguracion hacían exactamente lo mismo;
        // ambos ahora delegan aquí.
        private void AbrirMenuConfiguracion()
        {
            ActivarBoton(BtnConfig);
            DeseleccionarTabla();

            BtnConfig.ContextMenu.PlacementTarget = BtnConfig;
            BtnConfig.ContextMenu.IsOpen = true;
        }

        public void AbrirConsultarRegistros()
        {
            AbrirConsultarRegistros(TableDetector.TablaActual);
        }

        public void AbrirConsultarRegistros(string tabla)
        {
            ActivarBoton(BtnConsultar);
            DeseleccionarTabla();

            MainFrame.Navigate(new ConsultarRegistros(tabla));
        }

        private void ModoClaro_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.CambiarTema("Light");
        }

        private void ModoOscuro_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.CambiarTema("Dark");
        }

        private void ModoDescanso_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.CambiarTema("EyeCare");
        }
    }
}