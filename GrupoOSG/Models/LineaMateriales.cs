namespace GrupoOSG.Model
{
    public class LineaMateriales
    {
        private string codigoProyecto;
        private string codigoArticulo;
        private string descripcionArticulo;
        private int numeroPedido;
        private string nombreProveedor;
        private string observaciones;
        private bool recibido;
        private double unidadesPedidas;
        private double unidadesRecibidas;
        private double unidadesPendientes;
        private DateOnly fechaPedido;
        private DateOnly fechaRecepcion;
        private int codigoEmpresa;

        public LineaMateriales() { }

        public LineaMateriales(string codigoProyecto, string codigoArticulo, string descripcionArticulo, int numeroPedido, string nombreProveedor, string observaciones, bool recibido, double unidadesPedidas, double unidadesRecibidas, double unidadesPendientes, DateOnly fechaPedido, DateOnly fechaRecepcion, int codigoEmpresa)
        {
            CodigoProyecto = codigoProyecto;
            CodigoArticulo = codigoArticulo;
            DescripcionArticulo = descripcionArticulo;
            NumeroPedido = numeroPedido;
            NombreProveedor = nombreProveedor;
            Observaciones = observaciones;
            Recibido = recibido;
            UnidadesPedidas = unidadesPedidas;
            UnidadesRecibidas = unidadesRecibidas;
            UnidadesPendientes = unidadesPendientes;
            FechaPedido = fechaPedido;
            FechaRecepcion = fechaRecepcion;
            CodigoEmpresa = codigoEmpresa;
        }

        public string CodigoProyecto { get => codigoProyecto; set => codigoProyecto = value; }
        public string CodigoArticulo { get => codigoArticulo; set => codigoArticulo = value; }
        public string DescripcionArticulo { get => descripcionArticulo; set => descripcionArticulo = value; }
        public int NumeroPedido { get => numeroPedido; set => numeroPedido = value; }
        public string NombreProveedor { get => nombreProveedor; set => nombreProveedor = value; }
        public string Observaciones { get => observaciones; set => observaciones = value; }
        public bool Recibido { get => recibido; set => recibido = value; }
        public double UnidadesPedidas { get => unidadesPedidas; set => unidadesPedidas = value; }
        public double UnidadesRecibidas { get => unidadesRecibidas; set => unidadesRecibidas = value; }
        public double UnidadesPendientes { get => unidadesPendientes; set => unidadesPendientes = value; }
        public DateOnly FechaPedido { get => fechaPedido; set => fechaPedido = value; }
        public DateOnly FechaRecepcion { get => fechaRecepcion; set => fechaRecepcion = value; }
        public int CodigoEmpresa { get => codigoEmpresa; set => codigoEmpresa = value; }
    }
}
