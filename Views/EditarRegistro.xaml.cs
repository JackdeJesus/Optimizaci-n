
using PoderJudicial.Data;
using PoderJudicial.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
namespace PoderJudicial.Views
{
    public partial class EditarRegistro : Page
    {
        private DispatcherTimer _timer;
        private Audiencia _audiencia;
        private readonly AudienciaData _data = new AudienciaData();

        // ── Listas de autocomplete (mismas que NuevoRegistro) ──
        private readonly List<string> _jueces = new List<string>
        {
            // Agrega aquí los jueces de tu catálogo
        };

        private readonly List<string> _tiposAudiencia = new List<string>
        {
            // Agrega aquí los tipos de audiencia de tu catálogo
        };

        private readonly List<string> _delitos = new List<string>
        {
            // Agrega aquí los delitos de tu catálogo
        };

        public EditarRegistro(Audiencia audiencia)
        {
            InitializeComponent();
            _audiencia = audiencia;
            IniciarReloj();
            CargarDatos();
        }

        // ══════════════════════════════════════════════
        //  CARGA DE DATOS
        // ══════════════════════════════════════════════
        private void CargarDatos()
        {
            // ── ID (solo lectura) ──
            TxtId.Text = _audiencia.Id.ToString();

            // ── Fecha y Hora Audiencia ──
            if (_audiencia.FechaAudiencia.HasValue)
            {
                TxtFechaAudiencia.Text = _audiencia.FechaAudiencia.Value.ToString("dd/MM/yyyy");
                TxtHoraAudiencia.Text = _audiencia.FechaAudiencia.Value.ToString("HH:mm");
            }

            // ── Fecha y Hora Recibo (solo lectura, se auto-rellena igual que en Nuevo) ──
            if (_audiencia.FechaRecibo.HasValue)
            {
                TxtFechaRecibo.Text = _audiencia.FechaRecibo.Value.ToString("dd/MM/yyyy");
                TxtHoraRecibo.Text = _audiencia.FechaRecibo.Value.ToString("HH:mm");
            }

            // ── Total Archivo (CmbTipoDisco) ──
            SeleccionarCombo(CmbTipoDisco, _audiencia.TipoDisco);

            // ── Total Disco Audiencia ──
            if (!string.IsNullOrEmpty(_audiencia.TotDiscoAudiencia))
            {
                bool encontrado = false;
                foreach (ComboBoxItem item in CmbTotDiscoAudiencia.Items)
                {
                    if (item.Content?.ToString() == _audiencia.TotDiscoAudiencia)
                    {
                        CmbTotDiscoAudiencia.SelectedItem = item;
                        encontrado = true;
                        break;
                    }
                }
                // Si no está en las opciones fijas → "Otro..."
                if (!encontrado)
                {
                    // Seleccionar "Otro..."
                    foreach (ComboBoxItem item in CmbTotDiscoAudiencia.Items)
                    {
                        if (item.Content?.ToString() == "Otro...")
                        {
                            CmbTotDiscoAudiencia.SelectedItem = item;
                            break;
                        }
                    }
                    TxtTotDiscoAudienciaOtro.Visibility = Visibility.Visible;
                    TxtTotDiscoAudienciaOtro.Text = _audiencia.TotDiscoAudiencia;
                }
            }

            // ── Juzgado ──
            if (!string.IsNullOrEmpty(_audiencia.Juzgado))
            {
                bool encontrado = false;
                foreach (ComboBoxItem item in CmbJuzgado.Items)
                {
                    if (item.Content?.ToString() == _audiencia.Juzgado)
                    {
                        CmbJuzgado.SelectedItem = item;
                        encontrado = true;
                        break;
                    }
                }
                if (!encontrado)
                {
                    foreach (ComboBoxItem item in CmbJuzgado.Items)
                    {
                        if (item.Content?.ToString() == "Otra...")
                        {
                            CmbJuzgado.SelectedItem = item;
                            break;
                        }
                    }
                    TxtJuzgadoOtro.Visibility = Visibility.Visible;
                    TxtJuzgadoOtro.Text = _audiencia.Juzgado;
                }
            }

            // ── Juez (primer juez en el TextBox principal; extras en el panel) ──
            if (!string.IsNullOrEmpty(_audiencia.Juez))
            {
                var jueces = _audiencia.Juez.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (jueces.Length > 0)
                    txtJuez.Text = jueces[0].Trim();

                for (int i = 1; i < jueces.Length; i++)
                    AgregarFilaExtra(PanelJuecesExtra, jueces[i].Trim(), "juez");
            }

            // ── No. Causa | NUC | Tipo Causa ──
            TxtNoCausa.Text = _audiencia.NoCausa;
            TxtNUC.Text = _audiencia.NUC;
            SeleccionarCombo(CmbTipoCausa, _audiencia.TipoCausa);

            // ── Tipo Audiencia (primer valor en el TextBox; extras en el panel) ──
            if (!string.IsNullOrEmpty(_audiencia.TipoAudiencia))
            {
                var tipos = _audiencia.TipoAudiencia.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (tipos.Length > 0)
                    TxtTipoAudiencia.Text = tipos[0].Trim();

                for (int i = 1; i < tipos.Length; i++)
                    AgregarFilaExtra(PanelAudienciaExtra, tipos[i].Trim(), "audiencia");
            }

            // ── Hora Conclusión ──
            if (_audiencia.HoraConclusion.HasValue)
                TxtHoraConclusion.Text = _audiencia.HoraConclusion.Value.ToString("HH:mm");

            // ── Imputado ──
            TxtImputado.Text = _audiencia.Imputado;

            // ── Delito (primer valor en el TextBox; extras en el panel) ──
            if (!string.IsNullOrEmpty(_audiencia.Delito))
            {
                var delitos = _audiencia.Delito.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (delitos.Length > 0)
                    TxtDelito.Text = delitos[0].Trim();

                for (int i = 1; i < delitos.Length; i++)
                    AgregarFilaExtra(PanelDelitoExtra, delitos[i].Trim(), "delito");
            }

            // ── Agraviado | Sala | No. Causa Juicio ──
            TxtAgraviado.Text = _audiencia.Agraviado;
            SeleccionarCombo(CmbSala, _audiencia.Sala);
            TxtNoCausaJuicio.Text = _audiencia.NoCausaJuicio;
        }

        // ══════════════════════════════════════════════
        //  ACTUALIZAR
        // ══════════════════════════════════════════════
        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            // ── Fecha Audiencia ──
            if (DateTime.TryParseExact(
                    TxtFechaAudiencia.Text.Trim() + " " + TxtHoraAudiencia.Text.Trim(),
                    "dd/MM/yyyy HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime fechaAud))
                _audiencia.FechaAudiencia = fechaAud;
            else
                _audiencia.FechaAudiencia = null;

            // ── Total Archivo ──
            _audiencia.TipoDisco = (CmbTipoDisco.SelectedItem as ComboBoxItem)?.Content?.ToString();

            // ── Total Disco Audiencia ──
            var selDisco = (CmbTotDiscoAudiencia.SelectedItem as ComboBoxItem)?.Content?.ToString();
            _audiencia.TotDiscoAudiencia = selDisco == "Otro..."
                ? TxtTotDiscoAudienciaOtro.Text.Trim()
                : selDisco;

            // ── Juzgado ──
            var selJuzgado = (CmbJuzgado.SelectedItem as ComboBoxItem)?.Content?.ToString();
            _audiencia.Juzgado = selJuzgado == "Otra..."
                ? TxtJuzgadoOtro.Text.Trim()
                : selJuzgado;

            // ── Juez (concatenar todos) ──
            _audiencia.Juez = RecolectarValoresPanel(txtJuez.Text, PanelJuecesExtra);

            // ── No. Causa | NUC | Tipo Causa ──
            _audiencia.NoCausa = TxtNoCausa.Text.Trim();
            _audiencia.NUC = TxtNUC.Text.Trim();
            _audiencia.TipoCausa = (CmbTipoCausa.SelectedItem as ComboBoxItem)?.Content?.ToString();

            // ── Tipo Audiencia ──
            _audiencia.TipoAudiencia = RecolectarValoresPanel(TxtTipoAudiencia.Text, PanelAudienciaExtra);

            // ── Hora Conclusión ──
            if (DateTime.TryParseExact(
                    TxtHoraConclusion.Text.Trim(),
                    "HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime horaConc))
                _audiencia.HoraConclusion = horaConc;
            else
                _audiencia.HoraConclusion = null;

            // ── Imputado ──
            _audiencia.Imputado = TxtImputado.Text.Trim();

            // ── Delito ──
            _audiencia.Delito = RecolectarValoresPanel(TxtDelito.Text, PanelDelitoExtra);

            // ── Agraviado | Sala | No. Causa Juicio ──
            _audiencia.Agraviado = TxtAgraviado.Text.Trim();
            _audiencia.Sala = (CmbSala.SelectedItem as ComboBoxItem)?.Content?.ToString();
            _audiencia.NoCausaJuicio = TxtNoCausaJuicio.Text.Trim();

            try
            {
                _data.Actualizar(_audiencia);

                MessageBox.Show(
                    "Registro actualizado correctamente.",
                    "Éxito",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al actualizar:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ══════════════════════════════════════════════
        //  CANCELAR
        // ══════════════════════════════════════════════
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
            => NavigationService?.GoBack();

        // ══════════════════════════════════════════════
        //  HELPERS: combos y paneles dinámicos
        // ══════════════════════════════════════════════

        private void SeleccionarCombo(ComboBox combo, string valor)
        {
            if (string.IsNullOrEmpty(valor)) return;
            foreach (ComboBoxItem item in combo.Items)
            {
                if (item.Content?.ToString() == valor)
                {
                    combo.SelectedItem = item;
                    return;
                }
            }
        }

        /// <summary>
        /// Devuelve el valor del TextBox principal + los TextBoxes extras del panel,
        /// separados por " | ".
        /// </summary>
        private string RecolectarValoresPanel(string valorPrincipal, StackPanel panel)
        {
            var partes = new List<string>();

            if (!string.IsNullOrWhiteSpace(valorPrincipal))
                partes.Add(valorPrincipal.Trim());

            foreach (var child in panel.Children)
            {
                if (child is Grid g)
                {
                    foreach (var gc in g.Children)
                    {
                        if (gc is TextBox tb && !string.IsNullOrWhiteSpace(tb.Text))
                            partes.Add(tb.Text.Trim());
                    }
                }
            }

            return string.Join(" | ", partes);
        }

        /// <summary>
        /// Agrega una fila extra en el panel dinámico (igual que en NuevoRegistro).
        /// tipo: "juez" | "audiencia" | "delito"
        /// </summary>
        private void AgregarFilaExtra(StackPanel panel, string valorInicial, string tipo)
        {
            var grid = new Grid { Margin = new Thickness(0, 4, 0, 0) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(42) });

            var txt = new TextBox
            {
                Style = (Style)FindResource("InputStyle"),
                Text = valorInicial
            };
            Grid.SetColumn(txt, 0);

            var btnEliminar = new Button
            {
                Content = "−",
                Width = 42,
                Height = 42,
                FontSize = 20,
                Cursor = Cursors.Hand,
                Background = System.Windows.Media.Brushes.White,
                Foreground = System.Windows.Media.Brushes.Red,
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                BorderThickness = new Thickness(1)
            };
            Grid.SetColumn(btnEliminar, 2);
            btnEliminar.Click += (s, e) => panel.Children.Remove(grid);

            grid.Children.Add(txt);
            grid.Children.Add(btnEliminar);
            panel.Children.Add(grid);
        }

        // ══════════════════════════════════════════════
        //  EVENTOS: ComboBox con "Otro..." / "Otra..."
        // ══════════════════════════════════════════════

        private void CmbTotDiscoAudiencia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtTotDiscoAudienciaOtro == null) return;
            var sel = (CmbTotDiscoAudiencia.SelectedItem as ComboBoxItem)?.Content?.ToString();
            TxtTotDiscoAudienciaOtro.Visibility =
                sel == "Otro..." ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CmbJuzgado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtJuzgadoOtro == null) return;
            var sel = (CmbJuzgado.SelectedItem as ComboBoxItem)?.Content?.ToString();
            TxtJuzgadoOtro.Visibility =
                sel == "Otra..." ? Visibility.Visible : Visibility.Collapsed;
        }

        // ══════════════════════════════════════════════
        //  BOTONES + (agregar juez / audiencia / delito)
        // ══════════════════════════════════════════════

        private void BtnAgregarJuez_Click(object sender, RoutedEventArgs e)
            => AgregarFilaExtra(PanelJuecesExtra, string.Empty, "juez");

        private void BtnAgregarAudiencia_Click(object sender, RoutedEventArgs e)
            => AgregarFilaExtra(PanelAudienciaExtra, string.Empty, "audiencia");

        private void BtnAgregarDelito_Click(object sender, RoutedEventArgs e)
            => AgregarFilaExtra(PanelDelitoExtra, string.Empty, "delito");

        // ══════════════════════════════════════════════
        //  AUTOCOMPLETE — Juez
        // ══════════════════════════════════════════════

        private void txtJuez_TextChanged(object sender, TextChangedEventArgs e)
            => MostrarSugerencias(txtJuez.Text, lstSugerencias, _jueces);

        // ══════════════════════════════════════════════
        //  AUTOCOMPLETE — Tipo Audiencia
        // ══════════════════════════════════════════════

        private void TxtTipoAudiencia_TextChanged(object sender, TextChangedEventArgs e)
            => MostrarSugerencias(TxtTipoAudiencia.Text, lstAudiencia, _tiposAudiencia);

        // ══════════════════════════════════════════════
        //  AUTOCOMPLETE — Delito
        // ══════════════════════════════════════════════

        private void TxtDelito_TextChanged(object sender, TextChangedEventArgs e)
            => MostrarSugerencias(TxtDelito.Text, lstDelito, _delitos);

        // ══════════════════════════════════════════════
        //  AUTOCOMPLETE — teclado (compartido)
        // ══════════════════════════════════════════════

        private void TxtAutocomplete_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ListBox lista = null;
            if (sender == txtJuez) lista = lstSugerencias;
            else if (sender == TxtTipoAudiencia) lista = lstAudiencia;
            else if (sender == TxtDelito) lista = lstDelito;

            if (lista == null || lista.Visibility != Visibility.Visible) return;

            if (e.Key == Key.Down)
            {
                lista.Focus();
                lista.SelectedIndex = 0;
                e.Handled = true;
            }
        }

        private void lstAutocomplete_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            var lb = (ListBox)sender;
            if (lb.SelectedItem == null) return;

            SeleccionarSugerencia(lb);
            e.Handled = true;
        }

        private void lstAutocomplete_MouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
            => SeleccionarSugerencia((ListBox)sender);

        private void SeleccionarSugerencia(ListBox lb)
        {
            var valor = lb.SelectedItem?.ToString();
            if (valor == null) return;

            if (lb == lstSugerencias) { txtJuez.Text = valor; lstSugerencias.Visibility = Visibility.Collapsed; txtJuez.CaretIndex = valor.Length; }
            else if (lb == lstAudiencia) { TxtTipoAudiencia.Text = valor; lstAudiencia.Visibility = Visibility.Collapsed; TxtTipoAudiencia.CaretIndex = valor.Length; }
            else if (lb == lstDelito) { TxtDelito.Text = valor; lstDelito.Visibility = Visibility.Collapsed; TxtDelito.CaretIndex = valor.Length; }
        }

        private void MostrarSugerencias(string texto, ListBox lista, List<string> catalogo)
        {
            lista.Items.Clear();
            if (string.IsNullOrWhiteSpace(texto)) { lista.Visibility = Visibility.Collapsed; return; }

            var filtro = catalogo
                .Where(x => x.Contains(texto, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (filtro.Count == 0) { lista.Visibility = Visibility.Collapsed; return; }

            foreach (var item in filtro) lista.Items.Add(item);
            lista.Visibility = Visibility.Visible;
        }

        // ══════════════════════════════════════════════
        //  FORMATO HORA (HH:mm auto-formato)
        // ══════════════════════════════════════════════

        private void TxtHoraAudiencia_TextChanged(object sender, TextChangedEventArgs e)
            => FormatearHora(TxtHoraAudiencia);

        private void TxtHoraConclusion_TextChanged(object sender, TextChangedEventArgs e)
            => FormatearHora(TxtHoraConclusion);

        private void FormatearHora(TextBox txt)
        {
            string raw = txt.Text.Replace(":", "");
            if (raw.Length == 4 && !txt.Text.Contains(":"))
            {
                txt.TextChanged -= TxtHoraAudiencia_TextChanged;
                txt.TextChanged -= TxtHoraConclusion_TextChanged;

                txt.Text = raw.Insert(2, ":");
                txt.CaretIndex = txt.Text.Length;

                txt.TextChanged += TxtHoraAudiencia_TextChanged;
                txt.TextChanged += TxtHoraConclusion_TextChanged;
            }
        }

        // ══════════════════════════════════════════════
        //  FORMATO FECHA (dd/MM/yyyy auto-formato)
        // ══════════════════════════════════════════════

        private void TxtFechaAudiencia_TextChanged(object sender, TextChangedEventArgs e)
            => FormatearFecha(TxtFechaAudiencia);

        private void FormatearFecha(TextBox txt)
        {
            string raw = txt.Text.Replace("/", "");
            if (raw.Length == 8 && !txt.Text.Contains("/"))
            {
                txt.TextChanged -= TxtFechaAudiencia_TextChanged;
                txt.Text = raw.Insert(2, "/").Insert(5, "/");
                txt.CaretIndex = txt.Text.Length;
                txt.TextChanged += TxtFechaAudiencia_TextChanged;
            }
        }

        // ══════════════════════════════════════════════
        //  RELOJ
        // ══════════════════════════════════════════════

        private void IniciarReloj()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) => ActualizarFechaHora();
            _timer.Start();
            ActualizarFechaHora();
        }

        private void ActualizarFechaHora()
        {
            var ahora = DateTime.Now;
            var cultura = new CultureInfo("es-MX");
            TxtHora.Text = ahora.ToString("hh:mm tt");
            TxtFecha.Text = ahora.ToString("dddd, dd MMMM yyyy", cultura);
        }
    }
}