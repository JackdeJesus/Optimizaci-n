using System;
using System.Linq;
using System.Windows.Controls;

namespace PoderJudicial.Helpers
{
    public static class ValidationHelper
    {
        // ──────────────────────────────────────────
        // SOLO NÚMEROS
        // ──────────────────────────────────────────
        public static bool SoloNumeros(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return false;

            return texto.All(char.IsDigit);
        }

        // ──────────────────────────────────────────
        // NÚMEROS Y "/"
        // ──────────────────────────────────────────
        public static bool NumerosYDiagonal(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return false;

            return texto.All(c =>
                char.IsDigit(c) || c == '/');
        }






        // ──────────────────────────────────────────
        // FECHA VÁLIDA
        // ──────────────────────────────────────────
        public static bool FechaValida(string fecha)
        {
            return DateTime.TryParse(fecha, out _);
        }

        // ──────────────────────────────────────────
        // HORA VÁLIDA
        // ──────────────────────────────────────────
        public static bool HoraValida(string hora)
        {
            return DateTime.TryParse(hora, out _);
        }

        // ──────────────────────────────────────────
        // OBLIGATORIO
        // ──────────────────────────────────────────
        public static bool CampoVacio(TextBox txt)
        {
            return PlaceholderHelper.IsPlaceholder(txt)
                || string.IsNullOrWhiteSpace(txt.Text);
        }
    }
}