using GrupoOSG.Services;
using GrupoOSG.ViewModel;
using System.Windows;

namespace GrupoOSG
{
    public partial class MaterialesView : Window
    {
        public MaterialesView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MaterialesService materialesService = new MaterialesService();
                MaterialesViewModel vm = new MaterialesViewModel(materialesService);
                this.DataContext = vm;

                vm.RecargarListadoProyectos();
            }
            catch (Exception ex)
            {
                //Mostrar un mensaje de error si ocurre una excepción al inicializar los materiales
                MessageBox.Show($"Error al cargar la aplicación: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnActualizarInformacion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.DataContext is MaterialesViewModel vm)
                {
                    vm.RecargarListadoProyectos();
                    vm.CargarLineasMateriales(vm.ProyectoSeleccionado);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar la información: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /*private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }*/
    }
}