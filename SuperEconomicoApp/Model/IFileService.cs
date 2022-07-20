using System;
using System.Collections.Generic;
using System.Text;

namespace SuperEconomicoApp.Model
{
    public interface IFileService
    {
        void CreateFile(string text);
        string GetTextFile();
    }
}
