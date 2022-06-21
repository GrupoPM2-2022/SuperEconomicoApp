using SuperEconomicoApp.Model;
using SuperEconomicoApp.ViewsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SuperEconomicoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchResultsView : ContentPage
    {
        SearchResultsViewModel srvm;
        public SearchResultsView(string searchText)
        {
            InitializeComponent();
            srvm = new SearchResultsViewModel(searchText);
            this.BindingContext = srvm;
            LabelName.Text=$"Bienvenido {Preferences.Get("Username", "Guest")}";
        }

         async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedProduct = e.CurrentSelection.FirstOrDefault() as ProductoItem;
            if(selectedProduct == null)
                return;
            await Navigation.PushModalAsync(new Views.ProductDetailsView(selectedProduct));
            ((CollectionView)sender).SelectedItem = null;
        }

         async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}