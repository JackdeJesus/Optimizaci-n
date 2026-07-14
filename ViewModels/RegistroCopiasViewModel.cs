using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PoderJudicial.ViewModels
{
    public class RegistroCopiasViewModel : INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // ──────────────────────────────────────────
        //  PROPIEDADES
        // ──────────────────────────────────────────
        private int _id;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        private DateTime? _feAudiencia;
        public DateTime? FeAudiencia
        {
            get => _feAudiencia;
            set { _feAudiencia = value; OnPropertyChanged(); }
        }

        private DateTime? _feRecibo;
        public DateTime? FeRecibo
        {
            get => _feRecibo;
            set { _feRecibo = value; OnPropertyChanged(); }
        }

        private int? _totDiscosEntregados;
        public int? TotDiscosEntregados
        {
            get => _totDiscosEntregados;
            set { _totDiscosEntregados = value; OnPropertyChanged(); }
        }

        private string _tipoDisco = string.Empty;
        public string TipoDisco
        {
            get => _tipoDisco;
            set { _tipoDisco = value; OnPropertyChanged(); }
        }

        private string _noCausa = string.Empty;
        public string NoCausa
        {
            get => _noCausa;
            set { _noCausa = value; OnPropertyChanged(); }
        }

        private string _nuc = string.Empty;
        public string NUC
        {
            get => _nuc;
            set { _nuc = value; OnPropertyChanged(); }
        }

        private string _tipoCausa = string.Empty;
        public string TipoCausa
        {
            get => _tipoCausa;
            set { _tipoCausa = value; OnPropertyChanged(); }
        }

        private int? _discosExternos;
        public int? DiscosExternos
        {
            get => _discosExternos;
            set { _discosExternos = value; OnPropertyChanged(); }
        }

        private int? _etiquetasEntregadas;
        public int? EtiquetasEntregadas
        {
            get => _etiquetasEntregadas;
            set { _etiquetasEntregadas = value; OnPropertyChanged(); }
        }

        private string _aQuienSeEntrega = string.Empty;
        public string AQuienSeEntrega
        {
            get => _aQuienSeEntrega;
            set { _aQuienSeEntrega = value; OnPropertyChanged(); }
        }

        private string _observaciones = string.Empty;
        public string Observaciones
        {
            get => _observaciones;
            set { _observaciones = value; OnPropertyChanged(); }
        }

        private string _quienRegistra = string.Empty;
        public string QuienRegistra
        {
            get => _quienRegistra;
            set { _quienRegistra = value; OnPropertyChanged(); }
        }
    }
}