using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitConverter.MainWindow
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Publit properties exposed to view

        public double InputValue
        {
            get => model.Input.Value;
            set
            {
                if(value != model.Input.Value)
                {
                    model.Input.Value = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string InputPrefix
        {
            get => model.Input.Prefix;
            set
            {
                if (value != model.Input.Prefix)
                {
                    model.Input.Prefix = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string InputUnit
        {
            get => model.Input.Unit.UnitSymbol;
            set
            {
                if (value != model.Input.Unit.UnitSymbol)
                {
                    // Do something to set the unit
                    model.SetInputUnit(value);
                    NotifyPropertyChanged();
                }
            }
        }


        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            model = new Conversion();

            // For testing only
            //Measure temperature = new Measure(0, 0, 0, 0, 1, 0, 0, "Temperature");
            //model.AddNewUnit(new Unit(temperature, 1, 0, "Kelvin", "K"));
            //model.AddNewUnit(new Unit(temperature, 1, 273, "Degrees Celcius", "C"));
            // End of testing
        }
        #endregion

        #region private fields

        private Conversion model;
        #endregion

        #region private methods

        

        #endregion

        #region INotifyPropertyChanged Implementation

        /// <summary>
        /// This event notifies UI to update content after the back-end data is changed by program
        /// It is required by the INotifyPropertyChanged interface.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This method is called by the Set accessor of each property to invoke PropertyChanged event.
        /// </summary>
        /// <param name="propertyName"></param>
        /// The CallerMemberName attribute that is applied to the optional propertyName parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
