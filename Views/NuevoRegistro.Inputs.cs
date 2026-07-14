using ClosedXML.Excel;
using PoderJudicial.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PoderJudicial.Views
{
    public partial class NuevoRegistro : Page
    {
        // ── Formato automático fecha ──────────────────────
        private void TxtFechaAudiencia_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            if (PlaceholderHelper.IsPlaceholder(txt)) return;
            string numeros = new string(txt.Text.Where(char.IsDigit).ToArray());
            if (numeros.Length != 8) return;
            txt.Text = $"{numeros[..2]}/{numeros[2..4]}/{numeros[4..]}";
            txt.CaretIndex = txt.Text.Length;
        }

        // ── Formato automático hora ───────────────────────
        private void TxtHoraAudiencia_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            if (PlaceholderHelper.IsPlaceholder(txt)) return;
            string numeros = new string(txt.Text.Where(char.IsDigit).ToArray());
            if (numeros.Length != 4) return;
            int h = int.Parse(numeros[..2]), m = int.Parse(numeros[2..]);
            if (h > 23 || m > 59) return;
            txt.Text = $"{h:D2}:{m:D2}";
            txt.CaretIndex = txt.Text.Length;
        }

        private bool actualizandoHora = false;

        private void TxtHoraConclusion_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PlaceholderHelper.IsPlaceholder(TxtHoraConclusion) || actualizandoHora) return;
            string numeros = new string(TxtHoraConclusion.Text.Where(char.IsDigit).ToArray());
            if (numeros.Length != 4) return;
            int h = int.Parse(numeros[..2]), m = int.Parse(numeros[2..]);
            if (h > 23 || m > 59) return;
            actualizandoHora = true;
            TxtHoraConclusion.Text = new DateTime(1, 1, 1, h, m, 0).ToString("hh:mm tt");
            TxtHoraConclusion.SelectionStart = TxtHoraConclusion.Text.Length;
            actualizandoHora = false;
        }

        // ── Autocomplete juez ─────────────────────────────
        private void txtJuez_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = UIHelper.ObtenerTexto((TextBox)sender);
            if (string.IsNullOrWhiteSpace(texto)) return;
            AutocompleteHelper.FiltrarDesdeSender(sender, VM.Jueces);
        }

        // ── Autocomplete delito ───────────────────────────
        private void TxtDelito_TextChanged(object sender, TextChangedEventArgs e)
        {
            string tipoCausa = UIHelper.ObtenerValorCombo(CmbTipoCausa);
            AutocompleteHelper.FiltrarDesdeSender(sender, VM.ObtenerDelitosFiltrados(tipoCausa));
        }

        // ── Autocomplete tipo audiencia ───────────────────
        private void TxtTipoAudiencia_TextChanged(object sender, TextChangedEventArgs e)
        {
            string tipoCausa = UIHelper.ObtenerValorCombo(CmbTipoCausa);
            AutocompleteHelper.FiltrarDesdeSender(sender, VM.ObtenerAudienciasFiltradas(tipoCausa));
        }

        // ── Teclado listbox autocomplete ──────────────────
        private void TxtAutocomplete_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            ListBox lst = ((StackPanel)txt.Parent).Children.OfType<ListBox>().First();
            if (e.Key == Key.Down) { lst.Focus(); lst.SelectedIndex = 0; e.Handled = true; }
            if (e.Key == Key.Escape) { lst.Visibility = Visibility.Collapsed; e.Handled = true; }
        }

        private void lstAutocomplete_PreviewKeyDown(object sender, KeyEventArgs e)
            => AutocompleteHelper.ManejarTecladoListBox((ListBox)sender, e);

        private void lstAutocomplete_MouseClick(object sender, MouseButtonEventArgs e)
            => AutocompleteHelper.ManejarClickMouse((ListBox)sender);

        // ── Solo números / números y diagonal ────────────
        private void SoloNumeros_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => e.Handled = !e.Text.All(char.IsDigit);

        private void NoCausa_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = e.Text.FirstOrDefault();
            e.Handled = !(char.IsDigit(c) || c == '/');
        }

        // ── Botones agregar campos dinámicos ─────────────
        private void BtnAgregarJuez_Click(object sender, RoutedEventArgs e)
            => AgregarCampoJuez();

        private void BtnAgregarDelito_Click(object sender, RoutedEventArgs e)
            => CrearCampoDinamico(PanelDelitoExtra, "Tipo de delito", TxtDelito_TextChanged);

        private void BtnAgregarAudiencia_Click(object sender, RoutedEventArgs e)
            => CrearCampoDinamico(PanelAudienciaExtra, "Tipo de Audiencia", TxtTipoAudiencia_TextChanged);

        private void AgregarCampoJuez()
            => CrearCampoDinamico(PanelJuecesExtra, "Escriba el nombre del juez", txtJuez_TextChanged);

        private void CrearCampoDinamico(StackPanel panel, string placeholder,
            TextChangedEventHandler textChanged)
        {
            DynamicFieldFactory.CrearCampoAutocomplete(
                panel, placeholder,
                textChanged,
                TxtAutocomplete_PreviewKeyDown,
                lstAutocomplete_PreviewKeyDown,
                lstAutocomplete_MouseClick,
                (s, ev) =>
                {
                    Button btn = (Button)s;
                    panel.Children.Remove((Grid)btn.Parent);
                },
                (Style)FindResource("InputStyle"));
        }
    }
}