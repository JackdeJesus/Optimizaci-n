using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using PoderJudicial.Data;
using PoderJudicial.Helpers;

namespace PoderJudicial.Views
{
    public partial class Login : Window
    {
        bool mostrando = false;

        public Login()
        {
            InitializeComponent();

            
        }

        private void btnIngresar_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string password = mostrando ? passVisible.Text : passOculta.Password;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Ingresa usuario y contraseña.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var repo = new UserRepository();
            bool acceso = repo.Login(usuario, password);

            if (acceso)
            {
                SesionActual.Usuario = usuario;
                Dashboard dashboard = new Dashboard(usuario);
                dashboard.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Acceso denegado",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnMostrar_Click(object sender, RoutedEventArgs e)
        {
            if (!mostrando)
            {
                passVisible.Text = passOculta.Password;
                passVisible.Visibility = Visibility.Visible;
                passOculta.Visibility = Visibility.Collapsed;
                imgOjo.Source = new BitmapImage(
                    new Uri("pack://application:,,,/Resources/eye.png"));
                mostrando = true;
            }
            else
            {
                passOculta.Password = passVisible.Text;
                passVisible.Visibility = Visibility.Collapsed;
                passOculta.Visibility = Visibility.Visible;
                imgOjo.Source = new BitmapImage(
                    new Uri("pack://application:,,,/Resources/eyeClose.png"));
                mostrando = false;
            }
            ActualizarPlaceholderPassword();
        }

        private void txtUsuario_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtPlaceholderUsuario.Visibility =
                string.IsNullOrWhiteSpace(txtUsuario.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void passVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            ActualizarPlaceholderPassword();
        }

        private void passOculta_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ActualizarPlaceholderPassword();
        }

        private void ActualizarPlaceholderPassword()
        {
            string textoPassword = mostrando
                ? passVisible.Text
                : passOculta.Password;
            txtPlaceholderPassword.Visibility =
                string.IsNullOrWhiteSpace(textoPassword)
                ? Visibility.Visible
                : Visibility.Hidden;
        }


        private void txtRegistrate_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var ventana = new CrearUsuario();
            ventana.ShowDialog();
        }


        // activa cursor de campo usuario
       
    }
}