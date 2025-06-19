using Materiales.Models;
using Materiales.ViewModel;
using System.Windows;

namespace Materiales
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new MainViewModel();
        }

        private void btnActualizarInformacion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.DataContext is MainViewModel vm)
                {
                    Proyecto proyectoActual = vm.ProyectoSeleccionado;
                    vm.InicializarMateriales();

                    //Intenta reasignar la selección si sigue existiendo
                    if (proyectoActual != null)
                    {
                        Proyecto proyectoEnLista = vm.Proyectos.FirstOrDefault(p => p.Codigo == proyectoActual.Codigo);
                        if (proyectoEnLista != null)
                        {
                            vm.ProyectoSeleccionado = proyectoEnLista;
                        }
                        else if (vm.Proyectos.Count > 0)
                        {
                            vm.ProyectoSeleccionado = vm.Proyectos.First();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar la información: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }

    }
}