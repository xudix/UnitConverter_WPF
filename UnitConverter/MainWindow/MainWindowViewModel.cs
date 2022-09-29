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
        
        public ObservableListWrapper<VariableWithUnit> ObservableResults { get; set; }

        /// <summary>
        /// A list of possible units based on user input in unit_Input.
        /// 
        /// </summary>
        public ObservableListWrapper<Unit> PossibleDisplayUnits
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

        /// <summary>
        /// The unit in edit tab. It's binded to various input boxes in edit tab.
        /// </summary>
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
                            var newUnit = expResult.Unit;
                            newUnit.Multiplier *= expResult.Value * Prefixes.GetPrefixValue(expResult.Prefix);
                            model.TryFindUnit(newUnit, out string measureName, out string unitSymbol, out string unitName);
                            newUnit.UnitSymbol = unitSymbol;
                            newUnit.UnitName = unitName;
                            newUnit.MeasureName = measureName;
                            UpdateUnitToEdit(newUnit);
                        }
                        else
                            UpdateUnitToEdit(new Unit());
                    }
                    catch { }
                }
            }
        }
        private String editTabExpression;

        /// <summary>
        /// 
        /// </summary>
        public ObservableListWrapper<Unit> All_Units 
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

        private ObservableListWrapper<Unit> all_Units;

        /// <summary>
        /// 
        /// </summary>
        public string ConversionExpression
        {
            get => model.InputExpression;
            set
            {
                model.InputExpression = value;
                inputUnitStr = model.Input.Unit.ToString();
                NotifyPropertyChanged("InputValue");
                NotifyPropertyChanged("InputPrefix");
                NotifyPropertyChanged("InputUnitStr");
                NotifyPropertyChanged("CustomConversionResult");
                ObservableResults.RaiseCollectionChangedEvent();
            }
        }

        public string CustomConversionUnitExpression
        {
            get => model.CustomConversionUnitExpression;
            set
            {
                model.CustomConversionUnitExpression = value;
                NotifyPropertyChanged("CustomConversionResult");
            }
        }

        public VariableWithUnit CustomConversionResult
        {
            get => model.CustomConversionResult;
        }


        #endregion

        #region Public methods exposed to view
        public void UpdateResultPrefix(VariableWithUnit variable, string newPrefix)
        {
            int index;
            if (variable != null)
                for (index = 0; index < ObservableResults.Count; index++)
                    if (ObservableResults[index].Unit.UnitSymbol == variable.Unit.UnitSymbol)
                    {
                        model.UpdateResultPrefix(index, newPrefix);
                        ObservableResults.RaiseCollectionChangedEvent();
                        return;
                    }
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
                            SearchPossibleDisplayUnits(inputUnitStr);
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
                    All_Units.RaiseCollectionChangedEvent();
                    SearchPossibleDisplayUnits(inputUnitStr);
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
                    SearchPossibleDisplayUnits(inputUnitStr);
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
            ObservableResults = new ObservableListWrapper<VariableWithUnit>(model.Results);
            All_Units = new ObservableListWrapper<Unit>(model.All_Units);
            
            PossibleDisplayUnits = new ObservableListWrapper<Unit>(model.All_Units);
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
        private ObservableListWrapper<Unit> possibleDisplayUnits;
        private string inputUnitStr = "";
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
                PossibleDisplayUnits = new ObservableListWrapper<Unit>(model.All_Units);
            }
            else
            {
                List<Unit> searchResults = new List<Unit>();
                foreach (Unit unit in model.All_Units)
                {
                    if (unit.ToString().ToLower().Contains(inputStr.ToLower()))
                        searchResults.Add(unit);
                }
                PossibleDisplayUnits = new ObservableListWrapper<Unit>(searchResults);
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
