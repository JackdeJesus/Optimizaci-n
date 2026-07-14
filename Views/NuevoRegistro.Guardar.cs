using ClosedXML.Excel;
using PoderJudicial.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PoderJudicial.Views
{
    public partial class NuevoRegistro : Page
    {
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario()) return;

            string tipoCausa = UIHelper.ObtenerValorCombo(CmbTipoCausa);

            if (tipoCausa == "EXP") { EjecutarGuardadoEjecucion(); return; }

            EjecutarGuardadoAudiencia(tipoCausa);
        }

        private void EjecutarGuardadoAudiencia(string tipoCausa)
        {
            try
            {
                string tipoDiscoTexto = UIHelper.ObtenerValorCombo(CmbTipoDisco);
                int? totalDiscos = int.TryParse(tipoDiscoTexto.Split(' ')[0], out int d)
                    ? d : null;

                var registro = VM.ConstruirAudiencia(
                    id: int.Parse(TxtId.Text),
                    noCausa: UIHelper.ObtenerTexto(TxtNoCausa),
                    nuc: UIHelper.ObtenerTexto(TxtNUC),
                    fechaAudiencia: ParsearFechaHora(TxtFechaAudiencia, TxtHoraAudiencia),
                    fechaRecibo: ParsearFechaHora(TxtFechaRecibo, TxtHoraRecibo, esRecibo: true),
                    tipoAudiencia: UIHelper.ObtenerTextosPanelDinamico(PanelAudienciaExtra, TxtTipoAudiencia),
                    tipoCausa: tipoCausa,
                    juzgado: UIHelper.ObtenerValorComboOtro(CmbJuzgado, TxtJuzgadoOtro),
                    juez: UIHelper.ObtenerTextosPanelDinamico(PanelJuecesExtra, TxtJuez),
                    imputado: UIHelper.ObtenerTexto(TxtImputado),
                    delito: UIHelper.ObtenerTextosPanelDinamico(PanelDelitoExtra, TxtDelito),
                    agraviado: UIHelper.ObtenerTexto(TxtAgraviado),
                    sala: UIHelper.ObtenerValorCombo(CmbSala),
                    horaConclusion: ParsearHora(TxtHoraConclusion),
                    noCausaJuicio: UIHelper.ObtenerTexto(TxtNoCausaJuicio),
                    totDiscos: totalDiscos,
                    totDiscoAudiencia: UIHelper.ObtenerValorComboOtro(CmbTotDiscoAudiencia, TxtTotDiscoAudienciaOtro)
                );

                VM.GuardarAudiencia(registro);
                MessageBox.Show("Registro guardado correctamente.", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                LimpiarFormulario();
                CargarIdVisual();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar:\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EjecutarGuardadoEjecucion()
        {
            try
            {
                var expediente = VM.ConstruirEjecucion(
                    id: int.Parse(TxtId.Text),
                    fechaAudiencia: ParsearFechaHora(TxtFechaAudiencia, TxtHoraAudiencia),
                    totalDiscos: UIHelper.ObtenerValorComboOtro(CmbTotDiscoAudiencia, TxtTotDiscoAudienciaOtro),
                    juez: UIHelper.ObtenerTextosPanelDinamico(PanelJuecesExtra, TxtJuez),
                    expedienteNumero: UIHelper.ObtenerTexto(TxtExpediente),
                    causa: UIHelper.ObtenerTexto(TxtNoCausa),
                    tipoAudiencia: UIHelper.ObtenerTextosPanelDinamico(PanelAudienciaExtra, TxtTipoAudiencia),
                    horaTermino: UIHelper.ObtenerTexto(TxtHoraConclusion),
                    imputado: UIHelper.ObtenerTexto(TxtImputado),
                    delito: UIHelper.ObtenerTextosPanelDinamico(PanelDelitoExtra, TxtDelito),
                    victima: UIHelper.ObtenerTexto(TxtAgraviado),
                    sala: UIHelper.ObtenerValorCombo(CmbSala)
                );

                VM.GuardarEjecucion(expediente);
                MessageBox.Show("Expediente guardado correctamente.", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                LimpiarFormulario();
                CargarIdVisual();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar expediente:\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Parsers de fecha/hora ─────────────────────────
        private DateTime? ParsearFechaHora(TextBox txtFecha, TextBox txtHora, bool esRecibo = false)
        {
            string texto = esRecibo
                ? $"{txtFecha.Text} {txtHora.Text}"
                : $"{UIHelper.ObtenerTexto(txtFecha)} {UIHelper.ObtenerTexto(txtHora)}";
            return DateTime.TryParse(texto, out DateTime dt) ? dt : null;
        }

        private DateTime? ParsearHora(TextBox txtHora)
            => DateTime.TryParse(UIHelper.ObtenerTexto(txtHora), out DateTime h) ? h : null;

        // ── Limpiar formulario ────────────────────────────
        private void LimpiarFormulario()
        {
            TxtId.Text = TxtNoCausa.Text = TxtNUC.Text = TxtFechaAudiencia.Text =
            TxtTipoAudiencia.Text = TxtImputado.Text = TxtDelito.Text =
            TxtAgraviado.Text = TxtHoraConclusion.Text = TxtNoCausaJuicio.Text =
            TxtJuez.Text = TxtHoraAudiencia.Text = TxtExpediente.Text =
            TxtObservaciones.Text = string.Empty;

            CmbJuzgado.SelectedIndex = 1;
            CmbTotDiscoAudiencia.SelectedIndex = 1;
            CmbSala.SelectedIndex = 1;
            CmbTipoCausa.SelectedIndex = 1;
            CmbTipoDisco.SelectedIndex = 0;

            PanelJuecesExtra.Children.Clear();
            PanelDelitoExtra.Children.Clear();
            PanelAudienciaExtra.Children.Clear();

            AplicarPlaceholders();
        }

        // ── Validaciones ──────────────────────────────────
        private bool ValidarFormulario()
        {
            string tipoCausa = UIHelper.ObtenerValorCombo(CmbTipoCausa);

            if (!Validar(ValidationHelper.CampoVacio(TxtId),
                "El campo 'Id' es obligatorio.")) return false;

            string noCausa = UIHelper.ObtenerTexto(TxtNoCausa);
            if (!Validar(!ValidationHelper.NumerosYDiagonal(noCausa),
                "El campo 'No. Causa' solo permite números y '/'.")) return false;

            if (!Validar(ValidationHelper.CampoVacio(TxtNoCausa),
                "El campo 'No. Causa' es obligatorio.")) return false;

            if (tipoCausa != "EXP" &&
                !Validar(string.IsNullOrWhiteSpace(UIHelper.ObtenerTexto(TxtNUC)),
                "El campo NUC es obligatorio.")) return false;

            var juzgadoItem = CmbJuzgado.SelectedItem as ComboBoxItem;
            if (!Validar(juzgadoItem == null || juzgadoItem.Content.ToString() == "Seleccione juzgado",
                "El campo 'Juzgado' es obligatorio.")) return false;

            if (!Validar(!ValidationHelper.FechaValida(UIHelper.ObtenerTexto(TxtFechaAudiencia)),
                "La fecha de audiencia no es válida.")) return false;

            if (!Validar(!ValidationHelper.HoraValida(UIHelper.ObtenerTexto(TxtHoraAudiencia)),
                "La hora de audiencia no es válida.")) return false;

            if (!Validar(!ValidationHelper.HoraValida(UIHelper.ObtenerTexto(TxtHoraConclusion)),
                "La hora de conclusión no es válida.")) return false;

            string juez = UIHelper.ObtenerTextosPanelDinamico(PanelJuecesExtra, TxtJuez);
            if (!Validar(string.IsNullOrWhiteSpace(juez),
                "Debe capturar al menos un juez.")) return false;

            string audiencia = UIHelper.ObtenerTextosPanelDinamico(PanelAudienciaExtra, TxtTipoAudiencia);
            if (!Validar(string.IsNullOrWhiteSpace(audiencia),
                "El tipo de audiencia es obligatorio.")) return false;

            if (tipoCausa != "CP")
            {
                if (!Validar(string.IsNullOrWhiteSpace(UIHelper.ObtenerTexto(TxtImputado)),
                    "El imputado es obligatorio.")) return false;

                string delito = UIHelper.ObtenerTextosPanelDinamico(PanelDelitoExtra, TxtDelito);
                if (!Validar(string.IsNullOrWhiteSpace(delito),
                    "El delito es obligatorio.")) return false;

                if (!Validar(string.IsNullOrWhiteSpace(UIHelper.ObtenerTexto(TxtAgraviado)),
                    "El agraviado es obligatorio.")) return false;
            }

            if (tipoCausa == "JO")
            {
                string noCausaJuicio = UIHelper.ObtenerTexto(TxtNoCausaJuicio);
                if (!Validar(string.IsNullOrWhiteSpace(noCausaJuicio),
                    "El No. Causa Juicio es obligatorio para JO.")) return false;

                if (!Validar(!ValidationHelper.NumerosYDiagonal(noCausaJuicio),
                    "No. Causa Juicio solo permite números y '/'.")) return false;
            }

            return true;
        }

        // ── Helper para mostrar mensaje y cortar validación ─
        private static bool Validar(bool condicion, string mensaje)
        {
            if (!condicion) return true;
            MessageBox.Show(mensaje, "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
    }
}