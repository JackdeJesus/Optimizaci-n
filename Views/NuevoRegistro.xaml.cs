using PoderJudicial.Helpers;
using PoderJudicial.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PoderJudicial.Views
{
    public partial class NuevoRegistro : Page
    {
        private DispatcherTimer timer;
        internal NuevoRegistroViewModel VM;

        public NuevoRegistro()
        {
            InitializeComponent();

            VM = new NuevoRegistroViewModel();

            CargarIdVisual();
            IniciarReloj();
            AplicarPlaceholders();
        }

        // ── ID visual ────────────────────────────────────
        internal void CargarIdVisual()
        {
            try
            {
                string tipoCausa = UIHelper.ObtenerValorCombo(CmbTipoCausa);
                TxtId.Text = VM.ObtenerSiguienteId(tipoCausa).ToString();
            }
            catch { TxtId.Text = "---"; }
        }

        // ── Reloj ─────────────────────────────────────────
        private void IniciarReloj()
        {
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, e) => ActualizarFechaHora();
            timer.Start();
            ActualizarFechaHora();
        }

        private void ActualizarFechaHora()
        {
            DateTime ahora = DateTime.Now;
            CultureInfo cultura = new CultureInfo("es-MX");
            TxtHora.Text = ahora.ToString("hh:mm tt");
            TxtFecha.Text = ahora.ToString("dddd, dd MMMM yyyy", cultura);
            TxtFechaRecibo.Text = ahora.ToString("dd/MM/yyyy");
            TxtHoraRecibo.Text = ahora.ToString("hh:mm tt");
        }

        // ── Placeholders ──────────────────────────────────
        private void AplicarPlaceholders()
        {
            PlaceholderHelper.AddPlaceholder(TxtId);
            PlaceholderHelper.AddPlaceholder(TxtNoCausa, "Ej: 123/2024");
            PlaceholderHelper.AddPlaceholder(TxtNUC, "Ej: 12-2024-00567");
            PlaceholderHelper.AddPlaceholder(TxtNoCausaJuicio, "Ej: 89/2024");
            PlaceholderHelper.AddPlaceholder(TxtTipoAudiencia, "Escriba el tipo de audiencia");
            PlaceholderHelper.AddPlaceholder(TxtImputado, "Nombre del imputado");
            PlaceholderHelper.AddPlaceholder(TxtDelito, "Tipo de delito");
            PlaceholderHelper.AddPlaceholder(TxtAgraviado, "Nombre del agraviado");
            PlaceholderHelper.AddPlaceholder(TxtHoraConclusion, "hh:mm");
            PlaceholderHelper.AddPlaceholder(TxtFechaAudiencia, "dd/MM/yyyy");
            PlaceholderHelper.AddPlaceholder(TxtHoraAudiencia, "hh:mm");
            PlaceholderHelper.AddPlaceholder(TxtJuez, "Nombre del juez");
            PlaceholderHelper.AddPlaceholder(TxtExpediente, "Número de expediente");
            PlaceholderHelper.AddPlaceholder(TxtObservaciones, "Observaciones");
        }

        // ── Combos con opción "Otra..." / "Otro..." ───────
        private void CmbJuzgado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtJuzgadoOtro == null) return;
            var item = CmbJuzgado.SelectedItem as ComboBoxItem;
            TxtJuzgadoOtro.Visibility = item?.Content?.ToString() == "Otra..."
                ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CmbTotDiscoAudiencia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtTotDiscoAudienciaOtro == null) return;
            var item = CmbTotDiscoAudiencia.SelectedItem as ComboBoxItem;
            TxtTotDiscoAudienciaOtro.Visibility = item?.Content?.ToString() == "Otro..."
                ? Visibility.Visible : Visibility.Collapsed;
        }

        // ── Tipo causa ────────────────────────────────────
        private void CmbTipoCausa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (CmbTipoCausa.SelectedItem is not ComboBoxItem item) return;

            string tipo = item.Content.ToString();

            PanelNUC.Visibility = Visibility.Visible;
            PanelNoCausaJuicio.Visibility = Visibility.Collapsed;
            PanelCamposEXP.Visibility = Visibility.Collapsed;
            TxtNoCausaJuicio.Text = "";
            TxtExpediente.Text = "";
            TxtObservaciones.Text = "";
            PanelJuecesExtra.Children.Clear();

            switch (tipo)
            {
                case "C":
                case "CP":
                    BtnAgregarJuez.Visibility = Visibility.Collapsed;
                    break;

                case "JO":
                    PanelNoCausaJuicio.Visibility = Visibility.Visible;
                    BtnAgregarJuez.Visibility = Visibility.Visible;
                    if (PanelJuecesExtra.Children.Count == 0)
                        AgregarCampoJuez();
                    break;

                case "EXP":
                    BtnAgregarJuez.Visibility = Visibility.Collapsed;
                    PanelCamposEXP.Visibility = Visibility.Visible;
                    PanelNUC.Visibility = Visibility.Collapsed;
                    TxtObservaciones.Text = SesionActual.Usuario;
                    TxtObservaciones.IsReadOnly = true;
                    break;
            }

            CargarIdVisual();
        }
    }
}