using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace UnitConverter.MainWindow
{
    /// <summary>
    /// View model connects the model (Conversion) and the view (MainWindowView).
    /// The view model will be responsible for providing search suggestions for the Unit input box
    /// </summary>
    class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Public properties exposed to view

        public double InputValue
        {
            get => model.Input.Value;
            set
            {
                if (value != model.Input.Value)
                {
                    model.SetInputValue(value);
                    ObservableResults.RaiseCollectionChangedEvent();
                    //NotifyPropertyChanged("ObservableResults");
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
                    model.SetInputPrefix(value);
                    ObservableResults.RaiseCollectionChangedEvent();
                    //NotifyPropertyChanged("ObservableResults");
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
                    SearchPossibleDisplayUnits(inputUnitStr);
                    // Also send it to model (Conversion) to update results
                    model.SetInputUnit(inputUnitStr);
                    ObservableResults.RaiseCollectionChangedEvent();
                    //NotifyPropertyChanged("ObservableResults");

                }
            }
        }
        //public VariableWithUnit[] Results
        //{
        //    get => model.Results;
        //    set
        //    {
        //        if (value != results)
        //        {
        //            results = value;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}

        //public ObservableCollection<VariableWithUnit> ObservableResults
        //{
        //    get;
        //    set;
        //}

        public ObservableWrapper<VariableWithUnit> ObservableResults { get; set; }

        /// <summary>
        /// A list of possible units based on user input in unit_Input.
        /// 
        /// </summary>
        public ObservableWrapper<Unit> PossibleDisplayUnits
        {
            get => possibleDisplayUnits;
            set
            {
                if(possibleDisplayUnits != value)
                {
                    possibleDisplayUnits = value;
                    PossibleDisplayUnits.RaiseCollectionChangedEvent();
                }
                
            }
        }

        public Unit EditTabUnit
        { 
            get => editTabUnit;
            set
            {
                editTabUnit = value;
                NotifyPropertyChanged();
            }
        }

        private Unit editTabUnit;
        public string EditTabExpression 
        { 
            get => editTabExpression;
            set
            {
                if(editTabExpression != value)
                {
                    editTabExpression = value;
                    try
                    {
                        var expResult = model.EvaluateExpression(editTabExpression);
                        if (expResult != null)
                        {
                            editTabUnit = expResult.Unit;
                            editTabUnit.Multiplier *= expResult.Value * Prefixes.GetPrefixValue(expResult.Prefix);
                            editTabUnit.UnitSymbol = "";
                            editTabUnit.UnitName = "";
                            NotifyPropertyChanged("EditTabUnit");
                        }
                    }
                    catch { }
                }
                
            }
        }

        public ObservableWrapper<Unit> All_Units 
        { 
            get => all_Units;
            set 
            {
                if(all_Units != value)
                {
                    all_Units = value;
                    All_Units.RaiseCollectionChangedEvent();
                    //NotifyPropertyChanged();
                }
            }
        }

        private ObservableWrapper<Unit> all_Units;

        private String editTabExpression;


        #endregion

        #region Public methods exposed to view
        public void UpdateResultPrefix(int index, string newPrefix)
        {
            model.UpdateResultPrefix(index, newPrefix);
            ObservableResults.RaiseCollectionChangedEvent();
            //NotifyPropertyChanged("Results");
        }

        internal void UpdateUnitToEdit(Unit unit)
        {
            if(unit != null)
            {
                EditTabUnit = new Unit(unit);
                oldUnit = unit;
            }
        }


        #endregion

        #region Commands exposed to view

        public ICommand AddUnitCommand
        {
            get => new RelayCommand(() =>
                {
                    try
                    {
                        if(EditTabUnit.UnitSymbol == "")
                        {
                            MessageBox.Show("Unit symbol required.");
                        }
                        else if (model.AddNewUnit(EditTabUnit))
                        {
                            MessageBox.Show(string.Format("New unit {0} added!", EditTabUnit));
                            EditTabUnit = new Unit();
                            All_Units.RaiseCollectionChangedEvent();
                        }
                        else
                            MessageBox.Show(string.Format("Fail to add unit {0}. Unit {0} already exist.", EditTabUnit.UnitSymbol));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Error adding unit.\n{0}", ex.Message));
                    }
                    

                }
            );
        }

        public ICommand ModifyExistingUnitCommand
        {
            get => new RelayCommand(() =>
                {
                    model.ModifyExistingUnit(oldUnit, EditTabUnit);
                    MessageBox.Show(string.Format("Unit {0} modified!", EditTabUnit));
                    EditTabUnit = new Unit();
                }
            );
        }

        public ICommand DeleteUnitCommand
        {
            get => new RelayCommand(() => 
                { 
                    model.DeleteUnit(oldUnit);
                    MessageBox.Show(string.Format("Unit {0} Deleted!", oldUnit));
                    All_Units.RaiseCollectionChangedEvent();
                    EditTabUnit = new Unit();
                }
            );
        }

        public ICommand ClearEditUnitCommand
        {
            get => new RelayCommand(() =>
            {
                EditTabUnit = new Unit();
            }
            );
        }



        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            model = new Conversion();
            //ObservableResults = new ObservableCollection<VariableWithUnit>();
            //model.Results = ObservableResults;
            ObservableResults = new ObservableWrapper<VariableWithUnit>(model.Results);
            All_Units = new ObservableWrapper<Unit>(model.All_Units);
            
            PossibleDisplayUnits = new ObservableWrapper<Unit>(model.All_Units);
            editTabUnit = new Unit();

            // For testing only
            //Measure temperature = new Measure(0, 0, 0, 0, 1, 0, 0, "Temperature");
            //model.AddNewUnit(new Unit(temperature, 1, 0, "Kelvin", "K"));
            //model.AddNewUnit(new Unit(temperature, 1, 273, "Degrees Celcius", "C"));
            // End of testing
        }
        #endregion

        #region private fields

        private Conversion model;
        private ObservableWrapper<Unit> possibleDisplayUnits;
        private string inputUnitStr;
        private VariableWithUnit[] results;
        /// <summary>
        /// Saves the old unit to be updated
        /// </summary>
        private Unit oldUnit;
        #endregion

        #region private methods

        /// <summary>
        /// Update the PossibleUnits list when the input unit string changes
        /// </summary>
        private void SearchPossibleDisplayUnits(string inputStr)
        {
            if (inputStr == "")
            {
                PossibleDisplayUnits = new ObservableWrapper<Unit>(model.All_Units);
            }
            else
            {
                List<Unit> searchResults = new List<Unit>();
                foreach (Unit unit in model.All_Units)
                {
                    if (unit.ToString().ToLower().Contains(inputStr.ToLower()))
                        searchResults.Add(unit);
                }
                PossibleDisplayUnits = new ObservableWrapper<Unit>(searchResults);
            }
            NotifyPropertyChanged("PossibleDisplayUnits");
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
