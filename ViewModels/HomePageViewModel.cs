using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoderJudicial.Models;
using PoderJudicial.Data;

namespace PoderJudicial.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        private int _totalAudienciasMes;
        public int TotalAudienciasMes
        {
            get => _totalAudienciasMes;
            set => SetProperty(ref _totalAudienciasMes, value);
        }

        private ObservableCollection<ActividadReciente> _actividades;
        public ObservableCollection<ActividadReciente> Actividades
        {
            get => _actividades;
            set => SetProperty(ref _actividades, value);
        }

        private int _totalEjecucionesMes;
        public int TotalEjecucionesMes
        {
            get => _totalEjecucionesMes;
            set => SetProperty(ref _totalEjecucionesMes, value);
        }

        private int _totalCopiasMes;
        public int TotalCopiasMes
        {
            get => _totalCopiasMes;
            set => SetProperty(ref _totalCopiasMes, value);
        }

        private int _audienciasHoy;
        public int AudienciasHoy
        {
            get => _audienciasHoy;
            set => SetProperty(ref _audienciasHoy, value);
        }

        private string _versionSistema;
        public string VersionSistema
        {
            get => _versionSistema;
            set => SetProperty(ref _versionSistema, value);
        }

        private string _nombreBaseDatos;
        public string NombreBaseDatos
        {
            get => _nombreBaseDatos;
            set => SetProperty(ref _nombreBaseDatos, value);
        }

        private string _ultimaCopiaSeguridad;
        public string UltimaCopiaSeguridad
        {
            get => _ultimaCopiaSeguridad;
            set => SetProperty(ref _ultimaCopiaSeguridad, value);
        }

        private string _estadoSistema;
        public string EstadoSistema
        {
            get => _estadoSistema;
            set => SetProperty(ref _estadoSistema, value);
        }
    }
}