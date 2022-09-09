﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UnitConverter
{
    /// <summary>
    /// This will be the Model for the main window of the unit converter.
    /// It contains the input and output for the conversion.
    /// This class will search for a correct list of new units that match the measure of the input, and perform conversion.
    /// </summary>
    class Conversion
    {
        private IList<Unit> new_Units;
        private IList<string> new_Prefixes;
        // When no unit is specified, the Unit is set to the unit of number, i.e. Measure of NUMBER and multiplier of 1.0.
        private static readonly Unit number_Unit = new Unit();
        private VariableWithUnit input;

        #region Public Properties

        public VariableWithUnit Input 
        {
            get => input;
        }

        public IList<VariableWithUnit> Results { get; set; }

        /// <summary>
        /// All units that matches the measure of the input, except the unit of input.
        /// </summary>
        public IList<Unit> New_Units
        {
            get => new_Units;
            set
            {
                new_Units = value;
                CalculateNewOutput();
            }
        }



        /// <summary>
        /// List of all available units. This list will be serialized when the Save() method is called.
        /// The list will be reloaded from file when the program starts.
        /// </summary>
        public IList<Unit> All_Units { get; set; }
        public IList<string> New_Prefixes { get => new_Prefixes; set => new_Prefixes = value; }

        #endregion

        #region private fields

        private static readonly string unitsFile = "Units.xml";

        #endregion

        #region Constructor

        public Conversion()
        {
            try
            {
                //Read the list of all units from xml file.
                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(unitsFile))
                {
                    System.Xml.Serialization.XmlSerializer xmlReader = new System.Xml.Serialization.XmlSerializer(typeof(List<Unit>));
                    All_Units = (List<Unit>)xmlReader.Deserialize(streamReader);
                }
                All_Units.OrderBy((Unit unit) => unit.UnitSymbol);
            }
            catch
            {
                All_Units = new List<Unit>();
                All_Units.Add(new Unit());
            }
            // For testing only
            input = new VariableWithUnit(1.0, All_Units[0]);
            Results = new List<VariableWithUnit>();
            //

        }

        #endregion


        #region private methods
        /// <summary>
        /// Calculate the new Output when a new set of Unit is given.
        /// </summary>
        
        private void CalculateNewOutput()
        {
            //if (new_units.Count != new_prefixes.Count)
            //    throw new ArgumentException("The number of new units and that of new prefixes do not match.");
            //Input = input;
            //New_Units = new_units;
            //New_Prefixes = new_prefixes;
            if(New_Units != null)
            {
                Results.Clear();
                for (int index = 0; index < New_Units.Count; index++)
                    Results.Add(Input.Convert(New_Units[index], New_Prefixes[index]));
                //foreach (VariableWithUnit var in Results)
                //    Console.WriteLine(var);
            }
            
        }


        /// <summary>
        /// Set the given unit as the input unit, then find all possible units that have the same measure.
        /// Then calculate the results for all possible units.
        /// </summary>
        /// <param name="unit"></param>
        private void updateInputUnit(Unit unit)
        {
            Input.Unit = unit;
            // Found the correct unit. Now generate the list of new units based on measure
            new_Units = new List<Unit>(); // Set the private field here so that we don't invoke CalculateOutput() before New_Prefixes are set.
            new_Prefixes = new List<string>();
            foreach (Unit item in All_Units)
            {
                if (item.IsSameMeasure(unit) && item != unit)
                {
                    new_Units.Add(item);
                    new_Prefixes.Add("");
                }
            }
            CalculateNewOutput();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Set the Input Unit based on a string input.
        /// The method will try to match the symbol or name of the unit.
        /// </summary>
        /// <param name="inputUnitStr">The symbol or name of the unit</param>
        public void SetInputUnit(string inputUnitStr)
        {
            if(inputUnitStr == "" || inputUnitStr == null)
            {
                input.Unit = number_Unit;
                return;
            }   
            
            foreach (Unit unit in All_Units)
            {
                if (unit.UnitSymbol.ToLower() == inputUnitStr.ToLower())
                {
                    updateInputUnit(unit);
                    return;
                }
                    
            }
            foreach (Unit unit in All_Units)
            {
                if (unit.ToString().ToLower().Contains(inputUnitStr.ToLower()))
                {
                    updateInputUnit(unit);
                    return;
                }
            }
            input.Unit = number_Unit;

        }

        public void SetInputValue(double newValue)
        {
            if(input.Value != newValue)
            {
                input.Value = newValue;
                for (int i = 0; i < Results.Count; i++)
                {
                    Results[i] = Input.Convert(Results[i].Unit, Results[i].Prefix);
                }
            }       
        }
            

        public void SetInputPrefix(string newPrefix)
        {
            if(input.Prefix != newPrefix)
            {
                input.Prefix = newPrefix;
                //Results = new VariableWithUnit[Results.Length]; ;
                for (int i = 0; i < Results.Count; i++)
                {
                    Results[i] = Input.Convert(Results[i].Unit, Results[i].Prefix);
                }
            }
        }

        /// <summary>
        /// Add a new unit to the list of all units.
        /// </summary>
        /// <param name="newUnit">New unit being added</param>
        /// <returns>True if unit sucessfully added. False if new unit already exist.</returns>
        public bool AddNewUnit(Unit newUnit)
        {
            //Do a binary search to determine the location of the new item
            int lo = 0, hi = All_Units.Count;
            int mid;
            while(lo < hi)
            {
                mid = lo + ((hi - lo)>>1);
                if (All_Units[mid].UnitSymbol.CompareTo(newUnit.UnitSymbol) > 0)
                    hi = mid;
                else
                    lo = mid + 1;
            }
            if (All_Units[lo-1].UnitSymbol.CompareTo(newUnit.UnitSymbol) == 0)
                return false;
            else
                All_Units.Insert(hi, newUnit);
            Save();
            return true;
        }

        /// <summary>
        /// Delete a unit from the collection of units
        /// </summary>
        /// <param name="unitToDelete"></param>
        public void DeleteUnit(Unit unitToDelete)
        {
            All_Units.Remove(unitToDelete);
            Save();
        }

        /// <summary>
        /// Update an existing Unit to newUnit
        /// </summary>
        /// <param name="oldUnit"></param>
        /// <param name="newUnit"></param>
        public void ModifyExistingUnit(Unit oldUnit, Unit newUnit)
        {
            if (oldUnit.UnitSymbol == newUnit.UnitSymbol)
                All_Units[All_Units.IndexOf(oldUnit)] = newUnit;
            else
            {
                All_Units.Remove(oldUnit);
                AddNewUnit(newUnit);
            }
        }


        /// <summary>
        /// Update one prefix in result. This will update the Output
        /// </summary>
        /// <param name="index">The index of the updated prefix</param>
        /// <param name="new_prefix">The string for the new prefix</param>
        public void UpdateResultPrefix(int index, string new_prefix)
        {
            if (index > -1 && index < Results.Count && new_prefix != New_Prefixes[index])
                Results[index] = Input.Convert(Results[index].Unit, New_Prefixes[index] = new_prefix);
        }

        public void Save()
        {
            //Unit[] unitArray = All_Units.ToArray();
            //Array.Sort(unitArray,(Unit x, Unit y)=> x.UnitSymbol.CompareTo(y.UnitSymbol));
            //Type allUnitType = All_Units.GetType();
            //All_Units = Activator.CreateInstance(allUnitType, new object[] { unitArray }) as IList<Unit>;

            System.Xml.Serialization.XmlSerializer xmlWriter = new System.Xml.Serialization.XmlSerializer(All_Units.GetType());
            using(System.IO.FileStream fileStream = System.IO.File.Create(unitsFile))
            {
                xmlWriter.Serialize(fileStream, All_Units);
            }
        }


        #endregion
    }
}
