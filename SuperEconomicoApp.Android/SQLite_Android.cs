using System;
using System.IO;
using SuperEconomicoApp.Model;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(SuperEconomicoApp.Droid.SQLite_Android))]

namespace SuperEconomicoApp.Droid
{
    public class SQLite_Android : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFileName = "MyDatabase.db3";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, sqliteFileName);
            var cn = new SQLiteConnection(path);
            return cn;
        }
    }
}
