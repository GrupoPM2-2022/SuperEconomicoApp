using Acr.UserDialogs;
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
            UserDialogs.Instance.ShowLoading("Cargando");
            ListDirection = await DirectionServiceObject.GetDirectionByUser();
            UserDialogs.Instance.HideLoading();
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
                UserDialogs.Instance.ShowLoading("Cargando");
                try
                {
                    bool response = await DirectionServiceObject.DeleteDirection(direction.id.ToString()); ;
                    if (response)
                    {
                        ListDirection.Remove(direction);
                        UserDialogs.Instance.HideLoading();
                        await Application.Current.MainPage.DisplayAlert("Confirmación", "Dirección Eliminada Correctamente", "Ok");
                    }
                    else
                    {
                        UserDialogs.Instance.HideLoading();
                        await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al eliminar la dirección", "Ok");
                    }
                }
                catch (Exception)
                {
                    UserDialogs.Instance.HideLoading();
                    await Application.Current.MainPage.DisplayAlert("Advertencia::catch", "Se produjo un error al eliminar la dirección", "Ok");
                }
            }
        }

        private async Task EditDirection(Direction direction)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new AddDirectionView("Editar", direction));
        }
        #endregion

        #region COMANDOS

        public ICommand AddDirectionCommand => new Command(async () => await AddDirectionView());
        public ICommand EditDirectionCommand => new Command<Direction>(async (Direction) => await EditDirection(Direction));
        public ICommand DeleteDirectionCommand => new Command<Direction>(async (Direction) => await DeleteDirection(Direction));
        #endregion
    }
}
