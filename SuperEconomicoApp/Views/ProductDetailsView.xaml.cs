using SuperEconomicoApp.Model;
using SuperEconomicoApp.ViewsModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SuperEconomicoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProductDetailsView : ContentPage
    {
        ProductDetailsViewModel pvm;
        public ProductDetailsView(ProductoItem productoItem)
        {
            InitializeComponent();
            pvm = new ProductDetailsViewModel(productoItem);
            this.BindingContext = pvm;
        }
    }
}