using GrupoOSG.Model;
using GrupoOSG.Models;
using GrupoOSG.Security;
using GrupoOSG.Services.Interfaces;
using System.Data.SqlClient;

namespace GrupoOSG.Services
{
    public class MaterialesService : IMaterialesService
    {
        private readonly string connectionString;

        public MaterialesService()
        {
            connectionString = EncryptedConnectionMaterials.LoadConnectionString();
        }

        public List<Proyecto> GetProyectos()
        {
            List<Proyecto> proyectos = new List<Proyecto>();
            string queryMateriales = "SELECT * FROM VIS_CO_PenentRebre";

            //Conexión a la base de datos
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(queryMateriales, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    //Leer los datos y llenar la lista de Lineas
                    while (reader.Read())
                    {
                        //Obtener los valores de cada columna
                        string CodigoProyecto = reader["CodigoProyecto"].ToString();

                        //Filtramos los códigos de proyecto que empiezan con "OSG" o "OP"
                        if (!string.IsNullOrEmpty(CodigoProyecto) && (CodigoProyecto.ToUpper().StartsWith("OSG") || CodigoProyecto.ToUpper().StartsWith("OP")))
                        {
                            //Añadir el código del proyecto a la colección de códigos de proyectos si no existe
                            if (!proyectos.Any(p => p.Codigo == CodigoProyecto))
                            {
                                //Aquí solo guardamos el código, luego hacemos otra consulta para traer el nombre
                                proyectos.Add(new Proyecto(CodigoProyecto));
                            }
                        }
                    }
                }

                if (proyectos.Count > 0)
                {
                    //Ahora que tenemos los códigos de proyectos, hacemos una segunda consulta para obtener los nombres
                    List<string> codigos = proyectos.Select(p => p.Codigo).ToList();
                    string inParams = string.Join(",", codigos.Select((_, i) => $"@codigo{i}"));

                    //Consulta para obtener los nombres de los proyectos
                    string queryNombres = $@"
                    SELECT CodigoProyecto, Proyecto 
                    FROM VIS_CO_ProyectosPendientes 
                    WHERE CodigoProyecto IN ({inParams})";

                    //Ejecutar la consulta para obtener los nombres de los proyectos
                    using (SqlCommand cmd = new SqlCommand(queryNombres, conn))
                    {
                        //Añadir los parámetros dinámicamente
                        for (int i = 0; i < codigos.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@codigo{i}", codigos[i]);
                        }

                        //Leer los resultados y actualizar los nombres de los proyectos
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            //Crear un diccionario para mapear códigos a proyectos
                            Dictionary<string, Proyecto> proyectosDict = proyectos.ToDictionary(p => p.Codigo, p => p);
                            while (reader.Read())
                            {
                                //Obtener el código y el nombre del proyecto
                                string codigo = reader["CodigoProyecto"].ToString();
                                string nombre = reader["Proyecto"].ToString();

                                //Si el código existe en el diccionario, actualizar el nombre del proyecto
                                if (proyectosDict.TryGetValue(codigo, out Proyecto proyecto))
                                {
                                    proyecto.Nombre = nombre;
                                }
                            }
                        }
                    }
                }
            }
            return proyectos;
        }

        public List<LineaMateriales> GetLineasPorProyecto(string codigoProyecto)
        {
            string query = "SELECT * FROM VIS_CO_PenentRebre WHERE CodigoProyecto = @CodigoProyecto";
            List<LineaMateriales> lineasFiltradas = new List<LineaMateriales>();

            //Conexión a la base de datos
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CodigoProyecto", codigoProyecto);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<LineaMateriales> lineas = new List<LineaMateriales>();

                        //Leer los datos y llenar la lista de Lineas
                        while (reader.Read())
                        {
                            //Obtener los valores de cada columna
                            string codigoArticulo = reader["CodigoArticulo"].ToString();
                            string descripcionArticulo = reader["DescripcionArticulo"].ToString();
                            int numeroPedido = reader["NumeroPedido"] != DBNull.Value ? Convert.ToInt32(reader["NumeroPedido"]) : 0;
                            string nombre = reader["Nombre"]?.ToString() ?? string.Empty;
                            string observaciones = reader["Observaciones"]?.ToString() ?? string.Empty;
                            string recibidoStr = reader["RecibidoPedProv"]?.ToString()?.Trim().ToLower();
                            bool recibidoPedProv = recibidoStr == "sí" || recibidoStr == "si";
                            double unidadesPedidas = Convert.ToDouble(reader["UnidadesPedidas"]);
                            double unidadesRecibidas = Convert.ToDouble(reader["UnidadesRecibidas"]);
                            double unidadesPendientes = Convert.ToDouble(reader["UnidadesPendientes"]);
                            DateOnly fechaPedido = reader["FechaPedido"] != DBNull.Value ? DateOnly.FromDateTime(Convert.ToDateTime(reader["FechaPedido"])) : DateOnly.MinValue;
                            DateOnly fechaRecepcion = reader["FechaRecepcion"] != DBNull.Value ? DateOnly.FromDateTime(Convert.ToDateTime(reader["FechaRecepcion"])) : DateOnly.MinValue;
                            int codigoEmpresa = reader["CodigoEmpresa"] != DBNull.Value ? Convert.ToInt32(reader["CodigoEmpresa"]) : 0;

                            //Crear una nueva instancia de LineaMateriales
                            LineaMateriales lineaMateriales = new LineaMateriales(
                                codigoProyecto,
                                codigoArticulo,
                                descripcionArticulo,
                                numeroPedido,
                                nombre,
                                observaciones,
                                recibidoPedProv,
                                unidadesPedidas,
                                unidadesRecibidas,
                                unidadesPendientes,
                                fechaPedido,
                                fechaRecepcion,
                                codigoEmpresa
                            );

                            //Añadir la línea a la lista de Lineas
                            lineas.Add(lineaMateriales);
                        }

                        if (!lineas.Any()) return lineasFiltradas;

                        //Filtrar las líneas por el código del proyecto seleccionado
                        IEnumerable<LineaMateriales> filtradas = lineas
                        .Where(l => l.CodigoProyecto == codigoProyecto)
                        .OrderBy(l => l.NombreProveedor)  //A → Z
                        .ThenBy(l => l.Recibido)          //false → true
                        .ThenBy(l => l.FechaRecepcion);   //más pronto → más tarde

                        //Añadir las líneas filtradas a la colección
                        foreach (LineaMateriales linea in filtradas)
                        {
                            lineasFiltradas.Add(linea);
                        }
                    }
                }
            }

            return lineasFiltradas;
        }
    }
}
