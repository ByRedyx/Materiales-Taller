using Materiales.Model;
using Materiales.Models;
using Materiales.Security;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Materiales.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        //Event for property change notification
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string nombre = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));

        //Collections to hold project codes and filtered lines
        public ObservableCollection<Proyecto> Proyectos { get; set; } = new ObservableCollection<Proyecto>();
        public ObservableCollection<LineaMateriales> LineasFiltradas { get; set; } = new ObservableCollection<LineaMateriales>();
        private readonly List<LineaMateriales> Lineas = new List<LineaMateriales>();

        //Property to hold the selected project code
        private Proyecto proyectoSeleccionado;

        public MainViewModel()
        {
            try
            {
                InicializarMateriales();
            }
            catch (Exception ex)
            {
                //Mostrar un mensaje de error si ocurre una excepción al inicializar los materiales
                MessageBox.Show($"Error al cargar la aplicación: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Carga los datos de la base de datos y los almacena en las colecciones.
        /// </summary>
        public void InicializarMateriales()
        {
            Proyectos.Clear();
            Lineas.Clear();

            //Cadena de conexión
            //string connectionString = "Server=servidor;Database=db;User Id=admin;Password=admin123;";
            string connectionString = SecureConnectionManager.LoadConnectionString();

            //Consulta SQL de la vista que contiene toda la información de los materiales
            string queryMaterials = "SELECT * FROM VIS_CO_PenentRebre";
            string queryNombre = "SELECT Proyecto FROM VIS_CO_ProyectosPendientes WHERE CodigoProyecto = @CodigoProyecto";

            //Conexión a la base de datos
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(queryMaterials, conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    //Leer los datos y llenar la lista de Lineas
                    while (reader.Read())
                    {
                        //Obtener los valores de cada columna
                        string CodigoProyecto = reader["CodigoProyecto"].ToString();

                        //Filtrem el codi del projecte per a només tenir en compte els projectes OSG i OT
                        if (!string.IsNullOrEmpty(CodigoProyecto) && (CodigoProyecto.ToUpper().StartsWith("OSG") || CodigoProyecto.ToUpper().StartsWith("OP")))
                        {
                            string CodigoArticulo = reader["CodigoArticulo"].ToString();
                            string DescripcionArticulo = reader["DescripcionArticulo"].ToString();
                            int NumeroPedido = reader["NumeroPedido"] != DBNull.Value ? Convert.ToInt32(reader["NumeroPedido"]) : 0;
                            string Nombre = reader["Nombre"]?.ToString() ?? string.Empty;
                            string Observaciones = reader["Observaciones"]?.ToString() ?? string.Empty;
                            string recibidoStr = reader["RecibidoPedProv"]?.ToString()?.Trim().ToLower();
                            bool RecibidoPedProv = recibidoStr == "sí" || recibidoStr == "si";
                            double UnidadesPedidas = Convert.ToDouble(reader["UnidadesPedidas"]);
                            double UnidadesRecibidas = Convert.ToDouble(reader["UnidadesRecibidas"]);
                            double UnidadesPendientes = Convert.ToDouble(reader["UnidadesPendientes"]);
                            DateOnly FechaPedido = reader["FechaPedido"] != DBNull.Value ? DateOnly.FromDateTime(Convert.ToDateTime(reader["FechaPedido"])) : DateOnly.MinValue;
                            DateOnly FechaRecepcion = reader["FechaRecepcion"] != DBNull.Value ? DateOnly.FromDateTime(Convert.ToDateTime(reader["FechaRecepcion"])) : DateOnly.MinValue;
                            int CodigoEmpresa = reader["CodigoEmpresa"] != DBNull.Value ? Convert.ToInt32(reader["CodigoEmpresa"]) : 0;

                            //Crear una nueva instancia de LineaMateriales
                            LineaMateriales lineaMateriales = new LineaMateriales(
                                CodigoProyecto,
                                CodigoArticulo,
                                DescripcionArticulo,
                                NumeroPedido,
                                Nombre,
                                Observaciones,
                                RecibidoPedProv,
                                UnidadesPedidas,
                                UnidadesRecibidas,
                                UnidadesPendientes,
                                FechaPedido,
                                FechaRecepcion,
                                CodigoEmpresa
                            );

                            //Añadir la línea a la lista de Lineas
                            Lineas.Add(lineaMateriales);

                            //Añadir el código del proyecto a la colección de códigos de proyectos si no existe
                            if (!Proyectos.Any(p => p.Codigo == CodigoProyecto))
                            {
                                //Aquí solo guardamos el código, luego hacemos otra consulta para traer el nombre
                                Proyectos.Add(new Proyecto(CodigoProyecto));
                            }
                        }
                    }
                }

                //Ahora que tenemos los códigos de proyectos, hacemos una segunda consulta para obtener los nombres
                foreach (Proyecto proyecto in Proyectos)
                {
                    SqlCommand cmdNombre = new SqlCommand(queryNombre, conn);
                    cmdNombre.Parameters.AddWithValue("@CodigoProyecto", proyecto.Codigo);
                    using (SqlDataReader nombreReader = cmdNombre.ExecuteReader())
                    {
                        if (nombreReader.Read())
                        {
                            proyecto.Nombre = nombreReader["Proyecto"].ToString();
                        }
                    }
                }
            }

            //Ordenar el ComboBox de códigos de proyectos
            Proyectos = new ObservableCollection<Proyecto>(Proyectos.OrderBy(p => p.Codigo));
            OnPropertyChanged(nameof(Proyectos));

            //Seleccionamos el último proyecto si hay alguno
            if (Proyectos.Count > 0)
            {
                ProyectoSeleccionado = Proyectos.Last();
            }
        }

        /// <summary>
        /// Método para filtrar las líneas de materiales según el código del proyecto seleccionado.
        /// </summary>
        private void FiltrarLineas()
        {
            LineasFiltradas.Clear();

            if (Lineas == null || Lineas.Count == 0 || ProyectoSeleccionado == null) return;

            //Filtrar las líneas por el código del proyecto seleccionado
            IEnumerable<LineaMateriales> filtradas = Lineas
            .Where(l => l.CodigoProyecto == ProyectoSeleccionado.Codigo)
            .OrderBy(l => l.NombreProveedor)  //A → Z
            .ThenBy(l => l.Recibido)          //false → true
            .ThenBy(l => l.FechaRecepcion);   //más pronto → más tarde

            //Añadir las líneas filtradas a la colección
            foreach (LineaMateriales linea in filtradas)
            {
                LineasFiltradas.Add(linea);
            }
        }

        public Proyecto ProyectoSeleccionado
        {
            get => proyectoSeleccionado;
            set
            {
                if (proyectoSeleccionado != value)
                {
                    proyectoSeleccionado = value;
                    OnPropertyChanged();
                    FiltrarLineas();
                }
            }
        }
    }
}
