using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SuperEconomicoApp.ViewsModels
{
    public class ViewModelPatron : BaseViewModel
    {
        #region VARIABLES
        string _Texto;
        #endregion
        
        #region CONSTRUCTOR
        public ViewModelPatron()
        {
        }
        #endregion

        #region OBJETOS
        public string Texto
        {
            get { return _Texto; }
            set { _Texto = value; }
        }
        #endregion
        #region PROCESOS
        public void ProcesoSimple()
        {

        }
        #endregion
        #region COMANDOS
        public ICommand ProcesoSimpcommand => new Command(ProcesoSimple);
        #endregion
    }
}
