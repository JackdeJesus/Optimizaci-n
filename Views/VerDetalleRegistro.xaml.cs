using PoderJudicial.Models;
using System;
using System.Collections.Generic;
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

namespace PoderJudicial.Views
{
    public partial class VerDetalleRegistro : Window
    {
        public VerDetalleRegistro()
        {
            InitializeComponent();
        }

        public void CargarDatos(
            string Id, string noCausa, string nuc, string fechaAudiencia,
            string fechaRecibo, string horaConclusion, string tipoAudiencia,
            string tipoCausa, string juzgado, string juez, string sala,
            string totalDiscos, string tipoDisco, string totalDiscoAudiencia,
            string imputado, string delito, string agraviado,
            string noCausaJuicio, string diferida, string quienRealiza)
        {
            TxtID.Text = Id.ToString();
            TxtNoCausa.Text = noCausa;
            TxtNUC.Text = nuc;
            TxtFechaAudiencia.Text = fechaAudiencia;
            TxtFechaRecibo.Text = fechaRecibo;
            TxtHoraConclusion.Text = horaConclusion;
            TxtTipoAudiencia.Text = tipoAudiencia;
            TxtTipoCausa.Text = tipoCausa;
            TxtJuzgado.Text = juzgado;
            TxtJuez.Text = juez;
            TxtSala.Text = sala;
            TxtTotalDiscos.Text = totalDiscos;
            TxtTipoDisco.Text = tipoDisco;
            TxtTotalDiscoAudiencia.Text = totalDiscoAudiencia;
            TxtImputado.Text = imputado;
            TxtDelito.Text = delito;
            TxtAgraviado.Text = agraviado;
            TxtNoCausaJuicio.Text = noCausaJuicio;
           
            TxtQuienRealiza.Text = quienRealiza;
        }

        
        // Permite arrastrar la ventana desde cualquier parte
        protected override void OnMouseLeftButtonDown(
            System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    }


   
}
