﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitConverter.MainWindow
{
    /// <summary>
    /// View model connects the model (Conversion) and the view (MainWindowView).
    /// The view model will be responsible for providing search suggestions for the Unit input box
    /// </summary>
    class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Publit properties exposed to view

        public double InputValue
        {
            get => model.Input.Value;
            set
            {
                if (value != model.Input.Value)
                {
                    model.Input.Value = value;
                    model.CalculateOutput();
                    //NotifyPropertyChanged();
                    NotifyPropertyChanged("Results");
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
                    model.CalculateOutput();
                    //NotifyPropertyChanged();
                    NotifyPropertyChanged("Results");
                }
            }
        }

        /// <summary>
        /// This property is binded to the Text of the unit input box.
        /// It takes user input, and is used to find possible units.
        /// </summary>
        public string InputUnitStr
        {
            get => inputUnitStr;
            set
            {
                if (value != inputUnitStr)
                {
                    // Update the PossibleUnits list by performing a search
                    inputUnitStr = value;
                    SearchPossibleUnits(inputUnitStr);
                    // Also send it to model (Conversion) to update results
                    model.SetInputUnit(inputUnitStr);
                    NotifyPropertyChanged("Results");
                }
            }
        }

        public VariableWithUnit[] Results
        {
            get => model.Results;
            set
            {
                if (value != results)
                {
                    results = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// A list of possible units based on user input in unit_Input.
        /// 
        /// </summary>
        public List<Unit> PossibleUnits
        {
            get => possibleUnits;
            set
            {
                possibleUnits = value;
                NotifyPropertyChanged();
            }
        }


        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            model = new Conversion();
            possibleUnits = model.All_Units;

            // For testing only
            //Measure temperature = new Measure(0, 0, 0, 0, 1, 0, 0, "Temperature");
            //model.AddNewUnit(new Unit(temperature, 1, 0, "Kelvin", "K"));
            //model.AddNewUnit(new Unit(temperature, 1, 273, "Degrees Celcius", "C"));
            // End of testing
        }
        #endregion

        #region private fields

        private Conversion model;
        private List<Unit> possibleUnits = new List<Unit>();
        private string inputUnitStr;
        private VariableWithUnit[] results;
        #endregion

        #region private methods

        /// <summary>
        /// Update the PossibleUnits list when the input unit string changes
        /// </summary>
        private void SearchPossibleUnits(string inputStr)
        {
            if (inputStr == "")
            {
                possibleUnits = model.All_Units;
            }
            else
            {
                possibleUnits = new List<Unit>();
                foreach (Unit unit in model.All_Units)
                {
                    if (unit.ToString().ToLower().Contains(inputStr.ToLower()))
                        possibleUnits.Add(unit);
                }
            }
            NotifyPropertyChanged("PossibleUnits");
            //foreach (Unit unit in possibleUnits)
            //    Console.WriteLine(unit.ToString());
        }

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
