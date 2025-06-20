namespace GrupoOSG.Models
{
    public class Proyecto
    {
        private string codigo;
        private string nombre;

        public Proyecto(string codigo)
        {
            Codigo = codigo;
            Nombre = string.Empty;
        }
        public Proyecto(string codigo, string nombre)
        {
            Codigo = codigo;
            Nombre = nombre;
        }

        public string Codigo { get => codigo; set => codigo = value; }
        public string Nombre { get => nombre; set => nombre = value; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Nombre))
            {
                return Codigo;
            }
            return $"{Codigo} - {Nombre}".ToUpper();
        }
    }
}
