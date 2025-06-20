using System.Windows;

namespace GrupoOSG.Views
{
    public partial class InicioView : Window
    {
        public InicioView()
        {
            InitializeComponent();
        }

        private void btnAbrirMaterialesPorProyecto_Click(object sender, RoutedEventArgs e)
        {
            MaterialesView materialesView = new MaterialesView();
            materialesView.Show();
        }
    }
}
