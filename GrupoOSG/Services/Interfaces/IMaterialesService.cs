using GrupoOSG.Model;
using GrupoOSG.Models;

namespace GrupoOSG.Services.Interfaces
{
    /// <summary>
    /// Interfaz para obtener proyectos y líneas de materiales.
    /// </summary>
    public interface IMaterialesService
    {
        /// <summary>
        /// Obtiene todos los proyectos válidos (OSG u OP).
        /// </summary>
        List<Proyecto> GetProyectos();

        /// <summary>
        /// Obtiene todas las líneas de materiales para un proyecto dado.
        /// </summary>
        List<LineaMateriales> GetLineasPorProyecto(string codigoProyecto);
    }
}
