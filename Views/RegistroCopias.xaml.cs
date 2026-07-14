using PoderJudicial.Data;
using PoderJudicial.Helpers;
using PoderJudicial.Models;
using PoderJudicial.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media;


namespace PoderJudicial.Views
{
    public partial class RegistroCopias : Page
    {
        // ──────────────────────────────────────────
        //  CAMPOS PRIVADOS
        // ──────────────────────────────────────────
        private readonly RegistroCopiasViewModel _vm;
        private DispatcherTimer _timer;
        private bool _actualizandoHora = false;

        // ──────────────────────────────────────────
        //  CONSTRUCTOR
        // ──────────────────────────────────────────
        public RegistroCopias()
        {
            InitializeComponent();

            _vm = new RegistroCopiasViewModel();
            DataContext = _vm;

            CargarIdVisual();
            IniciarReloj();
            RegistrarPlaceholders();
        }

        // ──────────────────────────────────────────
        //  ID VISUAL
        // ──────────────────────────────────────────
        private void CargarIdVisual()
        {
            try
            {
                int id =
                    new CopiasData()
                    .ObtenerSiguienteIdVisual();

                TxtId.Text = id.ToString();
            }
            catch
            {
                TxtId.Text = "---";
            }
        }

        // ──────────────────────────────────────────
        //  RELOJ
        // ──────────────────────────────────────────
        private void IniciarReloj()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
            ActualizarFechaHora();
        }

        private void Timer_Tick(object? sender, EventArgs e)
            => ActualizarFechaHora();

        private void ActualizarFechaHora()
        {
            DateTime ahora = DateTime.Now;
            CultureInfo cultura = new CultureInfo("es-MX");

            TxtHora.Text = ahora.ToString("hh:mm tt");
            TxtFecha.Text = ahora.ToString("dddd, dd MMMM yyyy", cultura);

            // Fecha recibo es automática (solo lectura)
            TxtFeRecibo.Text = ahora.ToString("dd/MM/yyyy");
        }

       
        //  PLACEHOLDERS

        private void RegistrarPlaceholders()
        {
            PlaceholderHelper.AddPlaceholder(TxtId);
            PlaceholderHelper.AddPlaceholder(TxtFeAudiencia, "dd/MM/yyyy");
            PlaceholderHelper.AddPlaceholder(TxtFeRecibo, "dd/MM/yyyy");
            PlaceholderHelper.AddPlaceholder(TxtNoCausa, "Ej: 123/2024");
            PlaceholderHelper.AddPlaceholder(TxtNUC, "Ej: 12-2024-00567");
            
            PlaceholderHelper.AddPlaceholder(TxtAQuienSeEntrega, "Nombre de quien recibe");
            PlaceholderHelper.AddPlaceholder(TxtObservaciones, "Escriba observaciones adicionales...");
        }

        //  FORMATO FECHA AUDIENCIA
        
        private void TxtFeAudiencia_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = (TextBox)sender;

            if (PlaceholderHelper.IsPlaceholder(txt))
                return;

            string numeros = new string(
                txt.Text.Where(char.IsDigit).ToArray());

            if (numeros.Length != 8)
                return;

            txt.Text = numeros.Insert(2, "/").Insert(5, "/");
            txt.CaretIndex = txt.Text.Length;
        }

        
        //  TIPO CAUSA → visibilidad de controles
        
        private void CmbTipoCausa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // No hacer nada si la UI aún no cargó
            if (!IsLoaded) return;
        }

        // ──────────────────────────────────────────
        //  SOLO NÚMEROS
        // ──────────────────────────────────────────
        private void SoloNumeros_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => e.Handled = !e.Text.All(char.IsDigit);

        private void NoCausa_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = e.Text.FirstOrDefault();
            e.Handled = !(char.IsDigit(c) || c == '/');
        }

        //  GUARDAR
        
      
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario())
                return;

            // ── Recopilar valores ──────────────────────────
            DateTime? fechaAudiencia = null;
            if (DateTime.TryParseExact(
                    ObtenerTexto(TxtFeAudiencia),
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime fAud))
            {
                fechaAudiencia = fAud;
            }

            DateTime? fechaRecibo = null;
            if (DateTime.TryParseExact(
                    TxtFeRecibo.Text,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime fRec))
            {
                fechaRecibo = fRec;
            }

            int? totDiscos = null;
            string totTexto = ObtenerValorCombo(CmbTotDiscosEntregados);
            string cantidadStr = totTexto.Split(' ')[0];
            if (int.TryParse(cantidadStr, out int discos))
                totDiscos = discos;

            int? discosExternos = null;
            string extTexto =
                ObtenerValorCombo(CmbDiscosExternos);
            if (!string.IsNullOrWhiteSpace(extTexto))
            {
                string extStr =
                    extTexto.Split(' ')[0];
                if (int.TryParse(extStr, out int ext))
                    discosExternos = ext;
            }

            int? etiquetas = null;
            string etqTexto = ObtenerValorCombo(CmbEtiquetasEntregadas);
            if (!string.IsNullOrWhiteSpace(etqTexto))
            { 
                string etqStr = etqTexto.Split(' ')[0];
            if (int.TryParse(etqStr, out int etq))
                etiquetas = etq;
            }
            // ── Construir modelo ───────────────────────────
            var registro = new RegistroCopia
            {
                Id = int.TryParse(TxtId.Text, out int id) ? id : 0,
                FeAudiencia = fechaAudiencia,
                FeRecibo = fechaRecibo,
                TotDiscosEntregados = totDiscos,
                TipoDisco = ObtenerValorCombo(CmbTipoDisco),
                NoCausa = ObtenerTexto(TxtNoCausa),
                NUC = ObtenerTexto(TxtNUC),
                TipoCausa = ObtenerValorCombo(CmbTipoCausa),

                DiscosExternos =
        ObtenerValorCombo(CmbDiscosExternos),

                EtiquetasEntregadas =
        ObtenerValorCombo(CmbEtiquetasEntregadas),

                AQuienSeEntrega = ObtenerTexto(TxtAQuienSeEntrega),
                Observaciones = ObtenerTexto(TxtObservaciones),
                QuienRegistra = SesionActual.Usuario
            };

            // ── Insertar ───────────────────────────────────
            try
            {
                new CopiasData().Insertar(registro);

                MessageBox.Show(
                    "Registro de copia guardado correctamente.",
                    "Éxito",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                LimpiarFormulario();
                CargarIdVisual();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al guardar:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ──────────────────────────────────────────
        //  VALIDACIÓN
        // ──────────────────────────────────────────
        private bool ValidarFormulario()
        {
            // ID
            if (string.IsNullOrWhiteSpace(TxtId.Text) || TxtId.Text == "---")
            {
                Alerta("El campo 'Id' es obligatorio.");
                return false;
            }

            // Fecha Audiencia
            if (!ValidationHelper.FechaValida(ObtenerTexto(TxtFeAudiencia)))
            {
                Alerta("La fecha de audiencia no es válida (dd/MM/yyyy).");
                return false;
            }

            // Total Discos
            if (CmbTotDiscosEntregados.SelectedIndex == 0)
            {
                Alerta("Debe seleccionar el total de discos entregados.");
                return false;
            }

           

            // No. Causa
            string noCausa = ObtenerTexto(TxtNoCausa);
            if (string.IsNullOrWhiteSpace(noCausa))
            {
                Alerta("El campo 'No. Causa' es obligatorio.");
                return false;
            }
            if (!ValidationHelper.NumerosYDiagonal(noCausa))
            {
                Alerta("El campo 'No. Causa' solo permite números y '/'.");
                return false;
            }

            // NUC
            if (string.IsNullOrWhiteSpace(ObtenerTexto(TxtNUC)))
            {
                Alerta("El campo 'NUC' es obligatorio.");
                return false;
            }

            if (CmbTipoDisco.SelectedIndex == 0)
            {
                Alerta("Debe seleccionar el tipo de disco.");
                return false;
            }

            // Tipo Causa
            if (CmbTipoCausa.SelectedIndex == 0)
            {
                Alerta("Debe seleccionar el tipo de causa.");
                return false;
            }

            // A Quien se Entrega
            if (string.IsNullOrWhiteSpace(ObtenerTexto(TxtAQuienSeEntrega)))
            {
                Alerta("El campo 'A Quien se Entrega' es obligatorio.");
                return false;
            }

            return true;
        }

        private static void Alerta(string mensaje)
            => MessageBox.Show(mensaje, "Validación",
               MessageBoxButton.OK, MessageBoxImage.Warning);

        // ──────────────────────────────────────────
        //  LIMPIAR FORMULARIO
        // ──────────────────────────────────────────
        private void LimpiarFormulario()
        {
            TxtId.Text = string.Empty;
            TxtFeAudiencia.Text = string.Empty;
            
            TxtNoCausa.Text = string.Empty;
            TxtNUC.Text = string.Empty;
            TxtAQuienSeEntrega.Text = string.Empty;
            TxtObservaciones.Text = string.Empty;
            CmbTipoDisco.SelectedIndex = 0;
            CmbTotDiscosEntregados.SelectedIndex = 0;
            
            CmbTipoCausa.SelectedIndex = 0;
            CmbDiscosExternos.SelectedIndex = 0;
            CmbEtiquetasEntregadas.SelectedIndex = 0;

            RegistrarPlaceholders();

            
        }

        // ──────────────────────────────────────────
        //  HELPERS
        // ──────────────────────────────────────────
        private string ObtenerTexto(TextBox txt)
        {
            if (txt == null) return string.Empty;

            if (PlaceholderHelper.IsPlaceholder(txt))
                return string.Empty;

            string[] placeholders =
            {
                "dd/MM/yyyy",
                "Ej: 123/2024",
                "Ej: 12-2024-00567",
                "Nombre de quien recibe",
                "Escriba observaciones adicionales..."
            };

            string texto = txt.Text?.Trim() ?? string.Empty;
            return placeholders.Contains(texto) ? string.Empty : texto;
        }

        private string ObtenerValorCombo(ComboBox combo)
        {
            var item = combo.SelectedItem as ComboBoxItem;
            if (item == null) return string.Empty;

            string content = item.Content?.ToString() ?? string.Empty;
            return content.StartsWith("Seleccione") || content == "Ninguno" || content == "Ninguna"
                ? string.Empty
                : content;
        }


        private void TxtNoCausa_LostFocus(
    object sender,
    RoutedEventArgs e)
        {
            string causa =
                ObtenerTexto(TxtNoCausa);

            if (string.IsNullOrWhiteSpace(causa))
                return;

            string nuc =
                new AudienciaData()
                    .ObtenerNUCPorNoCausa(causa);

            if (!string.IsNullOrWhiteSpace(nuc))
            {
                TxtNUC.Text = nuc;

                TxtNUC.Foreground =
                    (Brush)Application.Current.Resources["PrimaryTextBrush"];
            }
        }


    }
}