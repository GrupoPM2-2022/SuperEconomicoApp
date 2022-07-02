using SuperEconomicoApp.Model;
using SuperEconomicoApp.Services;
using SuperEconomicoApp.Views.Ubication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SuperEconomicoApp.ViewsModels.Ubication
{
    public class ListDirectionViewModel : BaseViewModel
    {

        #region VARIABLES
        private string _Texto;
        private ObservableCollection<Direction> _ListDirection;
        #endregion

        #region CONSTRUCTOR
        public ListDirectionViewModel()
        {
            DirectionServiceObject = new DirectionService();
            GetAllDirections();
        }
        #endregion

        #region OBJETOS
        public string Texto
        {
            get { return _Texto; }
            set { _Texto = value; }
        }

        public ObservableCollection<Direction> ListDirection
        {
            get { return _ListDirection; }
            set
            {
                _ListDirection = value;
                OnPropertyChanged();
            }
        }

        public DirectionService DirectionServiceObject { get; set; }
        #endregion

        #region PROCESOS
        public async void GetAllDirections()
        {
            ListDirection = await DirectionServiceObject.GetDirectionByUser();
        }

        public async Task AddDirectionView()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new AddDirectionView("Guardar", null));
        }

        private async Task DeleteDirection(Direction direction)
        {
            bool confirmation = await Application.Current.MainPage.DisplayAlert("Advertencia", "¿Está seguro de eliminar " + direction.description + "?", "Si", "No");
            if (confirmation)
            {
                try
                {
                    bool response = await DirectionServiceObject.DeleteDirection(direction.id.ToString()); ;
                    if (response)
                    {
                        await Application.Current.MainPage.DisplayAlert("Confirmación", "Dirección Eliminada Correctamente", "Ok");
                        ListDirection.Remove(direction);
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al eliminar la dirección", "Ok");
                    }
                }
                catch (Exception)
                {

                    await Application.Current.MainPage.DisplayAlert("Advertencia::catch", "Se produjo un error al eliminar la dirección", "Ok");
                }
            }
        }

        private async Task EditDirection(Direction direction)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new AddDirectionView("Editar", direction));
        }

        public void ProcesoSimple()
        {

        }
        #endregion

        #region COMANDOS

        public ICommand AddDirectionCommand => new Command(async () => await AddDirectionView());
        public ICommand EditDirectionCommand => new Command<Direction>(async (Direction) => await EditDirection(Direction));
        public ICommand DeleteDirectionCommand => new Command<Direction>(async (Direction) => await DeleteDirection(Direction));
        public ICommand ProcesoSimpleCommand => new Command(ProcesoSimple);
        #endregion
    }
}
