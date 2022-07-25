using Acr.UserDialogs;
using SuperEconomicoApp.Model;
using SuperEconomicoApp.ViewsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SuperEconomicoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProductsView : ContentPage
    {
        ProductsViewModel productsViewModel;
        

        public ProductsView()
        {
            InitializeComponent();
            productsViewModel = new ProductsViewModel(CVLatest);
            BindingContext = productsViewModel;
        }

        async void CVLatest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedProduct = e.CurrentSelection.FirstOrDefault() as ProductoItem;
            if (selectedProduct == null)
                return;
            await Navigation.PushModalAsync(new ProductDetailsView(selectedProduct));
            ((CollectionView)sender).SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            productsViewModel.GetQuantityProductsCart();
        }

    }
}