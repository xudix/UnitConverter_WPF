using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
        private static Unit number_Unit = new Unit();
        private VariableWithUnit input;

        #region Public Properties

        public VariableWithUnit Input 
        {
            get => input;
            set
            {
                input = value;
                //CalculateOutput();
            }
        }

        public VariableWithUnit[] Results { get; set; }

        /// <summary>
        /// All units that matches the measure of the input, except the unit of input.
        /// </summary>
        public IList<Unit> New_Units
        {
            get => new_Units;
            set
            {
                new_Units = value;
                CalculateOutput();
            }
        }

        public IList<string> New_Prefixes
        {
            get => new_Prefixes;
            set
            {
                new_Prefixes = value;
                CalculateOutput();
            }
        }

        /// <summary>
        /// List of all available units. This list will be serialized when the Save() method is called.
        /// The list will be reloaded from file when the program starts.
        /// </summary>
        public List<Unit> All_Units { get; set; }

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
            }
            catch
            {
                All_Units = new List<Unit>();
            }
            // For testing only
            Input = new VariableWithUnit(5, All_Units[0]);

            //

        }

        #endregion


        #region private methods
        /// <summary>
        /// Calculate the Output based on Input, New Prefixes, and New Units.
        /// </summary>
        //public void CalculateOutput(VariableWithUnit input, ILi   st<Unit> new_units, IList<string> new_prefixes)
        public void CalculateOutput()
        {
            //if (new_units.Count != new_prefixes.Count)
            //    throw new ArgumentException("The number of new units and that of new prefixes do not match.");
            //Input = input;
            //New_Units = new_units;
            //New_Prefixes = new_prefixes;
            if(New_Units != null && New_Prefixes != null)
            {
                Results = new VariableWithUnit[New_Units.Count];
                for (int index = 0; index < New_Units.Count; index++)
                    Results[index] = Input.Convert(New_Units[index], New_Prefixes[index]);
                foreach (VariableWithUnit var in Results)
                    Console.WriteLine(var);
            }
            
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
                Input.Unit = number_Unit;
                return;
            }   
            // This method will first check if the input string is a symbol of a unit.
            // If it is, then this unit will be chosen as the InputUnit.
            // Otherwise, the method will search for this string in all Unit.ToString to find the unit.
            void updateInputUnit(Unit unit)
            {
                Input.Unit = unit;
                // Found the correct unit. Now generate the list of new units based on measure
                new_Units = new List<Unit>(); // Set the private field here so that we don't invoke CalculateOutput() before New_Prefixes are set.
                foreach (Unit item in All_Units)
                {
                    if (item.IsSameMeasure(unit) && item != unit)
                    {
                        new_Units.Add(item);
                    }
                }
                // Generate a string array and make them all empty
                new_Prefixes = new string[New_Units.Count];
                for (int index = 0; index < new_Prefixes.Count; index++)
                    new_Prefixes[index] = "";
                CalculateOutput();
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
            Input.Unit = number_Unit;

        }

        /// <summary>
        /// Add a new unit to the list of all units.
        /// </summary>
        /// <param name="newUnit">New unit being added</param>
        public void AddNewUnit(Unit newUnit)
        {
            All_Units.Add(newUnit);
            Save();

        }


        /// <summary>
        /// Update one prefix in result. This will update the Output
        /// </summary>
        /// <param name="index">The index of the updated prefix</param>
        /// <param name="new_prefix">The string for the new prefix</param>
        public void UpdatePrefix(int index, string new_prefix)
        {
            if (new_Prefixes[index] != new_prefix)
            {
                new_Prefixes[index] = new_prefix;
                CalculateOutput();
            }
        }

        public void Save()
        {
            System.Xml.Serialization.XmlSerializer xmlWriter = new System.Xml.Serialization.XmlSerializer(typeof(List<Unit>));
            System.IO.FileStream fileStream = System.IO.File.Create(unitsFile);
            xmlWriter.Serialize(fileStream, All_Units);
            fileStream.Close();
        }


        #endregion
    }
}
