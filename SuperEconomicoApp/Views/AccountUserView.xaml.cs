﻿using SuperEconomicoApp.ViewsModels;
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
    public partial class AccountUserView : ContentPage
    {
        public AccountUserView()
        {
            InitializeComponent();
            BindingContext = new UserAccountModels();

        }

        private void ListUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {

        }

        private void CV_Detattles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

       
    }
}