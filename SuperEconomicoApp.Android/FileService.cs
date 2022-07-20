using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperEconomicoApp.Model;
using SuperEconomicoApp.Droid;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(FileService))]
namespace SuperEconomicoApp.Droid
{
    public class FileService : IFileService
    {

        public string GetRootPath()
        {
            return Application.Context.GetExternalFilesDir(null).ToString();
        }

        public void CreateFile(string text)
        {
            var filename = "token.txt";

            var destination = Path.Combine(GetRootPath(), filename);

            File.WriteAllText(destination, text);
        }

        public string GetTextFile()
        {
            var filename = "token.txt";

            var destination = Path.Combine(GetRootPath(), filename);

            try
            { return File.ReadAllText(destination); }
            catch { return ""; }

        }
    }

}