using GrupoOSG.Core;
using GrupoOSG.Model;
using GrupoOSG.Models;
using GrupoOSG.Services.Interfaces;
using System.Collections.ObjectModel;

namespace GrupoOSG.ViewModel
{
    public class MaterialesViewModel : BaseViewModel
    {
        private readonly IMaterialesService materialesService;
        
        public ObservableCollection<Proyecto> Proyectos { get; set; } = new ObservableCollection<Proyecto>();
        public ObservableCollection<LineaMateriales> LineasFiltradas { get; set; } = new ObservableCollection<LineaMateriales>();
        
        private Proyecto proyectoSeleccionado;

        public MaterialesViewModel(IMaterialesService materialesService)
        {
            this.materialesService = materialesService;
        }

        public void RecargarListadoProyectos()
        {
            string codigoAnteriorSeleccionado = ProyectoSeleccionado?.Codigo;

            Proyectos.Clear();

            //Cargar los proyectos desde el servicio
            List<Proyecto> proyectos = materialesService.GetProyectos();

            //Añadir los proyectos a la colección
            foreach (Proyecto proyecto in proyectos.OrderBy(p => p.Codigo))
            {
                Proyectos.Add(proyecto);
            }

            //Seleccionamos el proyecto que ya estaba seleccionado, si existe
            if (!string.IsNullOrEmpty(codigoAnteriorSeleccionado))
            {
                Proyecto proyectoExistente = Proyectos.FirstOrDefault(p => p.Codigo == codigoAnteriorSeleccionado);
                if (proyectoExistente != null)
                {
                    ProyectoSeleccionado = proyectoExistente;
                }
                else if (Proyectos.Count > 0)
                {
                    ProyectoSeleccionado = Proyectos.Last();
                }
            }
            else if (Proyectos.Count > 0)
            {
                //Si no había un proyecto seleccionado, seleccionamos el último de la lista
                ProyectoSeleccionado = Proyectos.Last();
            }
        }

        public void CargarLineasMateriales(Proyecto proyecto)
        {
            LineasFiltradas.Clear();

            if (proyecto != null)
            {
                List<LineaMateriales> lineas = materialesService.GetLineasPorProyecto(proyecto.Codigo);

                foreach (LineaMateriales linea in lineas)
                {
                    LineasFiltradas.Add(linea);
                }
            }
        }

        public Proyecto ProyectoSeleccionado
        {
            get => proyectoSeleccionado;
            set
            {
                if (SetProperty(ref proyectoSeleccionado, value))
                {
                    CargarLineasMateriales(value);
                }
            }
        }
    }
}
