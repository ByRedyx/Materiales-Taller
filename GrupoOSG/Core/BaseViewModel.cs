using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GrupoOSG.Core
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private bool isBusy;

        /// <summary>
        /// Lanza el evento PropertyChanged para actualizar la UI.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Asigna un valor a una propiedad y lanza OnPropertyChanged si cambió.
        /// </summary>
        protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(backingField, value))
            {
                return false;
            }  

            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Indica si hay una operación en curso (para mostrar spinner, desactivar botones, etc.)
        /// </summary>
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }
    }
}
