using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace SuperEconomicoApp.Model
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
