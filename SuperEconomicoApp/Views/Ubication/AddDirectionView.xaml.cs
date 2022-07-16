using SuperEconomicoApp.Model;
using SuperEconomicoApp.ViewsModels.Ubication;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SuperEconomicoApp.Views.Ubication
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddDirectionView : ContentPage
    {
        public AddDirectionView(string action, Direction direction)
        {
            InitializeComponent();
            BindingContext = new AddDirectionViewModel(map, action, direction);
        }
    }
}