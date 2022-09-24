using System;
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

        /// <summary>
        /// Read the expression from index "current" to find the next item, either an operator or an operant.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="current">The beginning of the current item. Will be moved to the next item when this method exits.</param>
        /// <param name="result">When the item is an operator, result is a char. When the item is an operant, result is an object of class VariablewithUnit</param>
        /// <param name="isOperant">True if the item is an operant</param>
        /// <param name="isOperator">True if the item is an operator</param>
        /// <returns>True if an item is found. False if the item cannot be read.</returns>
        private bool GetNextExpressionItem(char[] expression, ref int current, out Object? result, out bool isOperant, out bool isOperator)
        {
            result = null;
            isOperant = false;
            isOperator = false;
            int start = current;
            if (current >= expression.Length)
                return false;
            char c = expression[current];
            if (operatorPrecedence.ContainsKey(c)) // operator
            {
                isOperator = true;
                result = c;
                current++;
            }
            else
            {
                isOperant = true;

                double number;
                if (char.IsDigit(c) || c == '.') // starts with a number
                {
                    while (char.IsDigit(c) || c == '.')
                    {
                        if (++current == expression.Length)
                            break;
                        c = expression[current];
                    }// done with the number part. Potentially go into exponent part

                    if ((c == 'e' || c == 'E') && current < expression.Length - 1 && (char.IsDigit(expression[current + 1]) || expression[current + 1] == '-' || expression[current + 1] == '+'))
                    {
                        // exponent part
                        current++;
                        if (expression[current] == '-' || expression[current] == '+') // move over the +- sign in exponent
                            current++;
                        while (current < expression.Length)
                        {
                            if (!char.IsDigit(expression[current]))
                                break;
                            current++;
                        }
                    }// done with exponent
                    // current should be at the char next to the number
                    if (!double.TryParse(new String(expression, start, current - start), out number))
                        return false;
                }
                else
                    number = 1;

                // After reading the number
                start = current;
                while (current < expression.Length && !operatorPrecedence.ContainsKey(expression[current])) // 
                {
                    current++; // move on until an operator is found
                }
                if (current > start) // Actually have a unit
                {
                    string unitSymbol = new string(expression, start, current - start);
                    int unitIndex = SearchUnit(unitSymbol);
                    if (unitIndex >= 0) // The unit exist
                    {
                        result = new VariableWithUnit(number, new Unit(All_Units[unitIndex]));
                    }
                    else if (Prefixes.IsPrefix(c = expression[start]))// see if it has a prefix
                    {
                        start++;
                        unitSymbol = new string(expression, start, current - start);
                        unitIndex = SearchUnit(unitSymbol);
                        if (unitIndex >= 0) // The unit exist
                        {
                            result = new VariableWithUnit(number, new Unit(All_Units[unitIndex]), c.ToString());
                        }
                        else// Not an existing unit
                            return false;
                    }
                    else // Not an existing unit
                        return false;
                }
                else // does not have a unit. Just a number
                {
                    result = new VariableWithUnit(number, new Unit());
                }
            }
            return true;
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
            int newUnitIndex = SearchUnit(newUnit.UnitSymbol);
            if (newUnitIndex >=0)
                return false;
            else
                All_Units.Insert(~newUnitIndex, newUnit);
            Save();
            return true;
        }

        /// <summary>
        /// Perform a binary search for a specific unit symbol in All_Units
        /// </summary>
        /// <param name="unitSymbol"></param>
        /// <returns>If the unit symbol is found, returns the index of the unit. If the symbol is not found, returns the bitwise complement of the index of the first unit that follows unitSymbol</returns>
        public int SearchUnit(string unitSymbol)
        {
            int lo = 0, hi = All_Units.Count;
            int mid;
            while (lo < hi)
            {
                mid = lo + ((hi - lo) >> 1);
                if (All_Units[mid].UnitSymbol.CompareTo(unitSymbol) > 0)
                    hi = mid;
                else
                    lo = mid + 1;
            }
            lo--;
            if (All_Units[lo].UnitSymbol.CompareTo(unitSymbol) == 0) // found
                return lo;
            else
                return ~hi;
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
            Save();
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


        public VariableWithUnit? EvaluateExpression(string expression)
        {
            char[] expressionChars = expression.Where(c => !Char.IsWhiteSpace(c)).ToArray();
            Stack<VariableWithUnit> operants = new Stack<VariableWithUnit>();
            Stack<char> operators = new Stack<char>();
            operators.Push('\0');
            Unit? result = null;
            
            int current = 0;
            int nOperators = 0; // number of operators, except '(', '[', or '{'
            bool isOperant, isOperator;
            object expItem;
            VariableWithUnit minusOne = new VariableWithUnit(-1, new Unit());

            while (operators.Count > 0)
            {
                if (current == expressionChars.Length)
                {
                    expItem = '\0';
                    isOperator = true;
                    isOperant = false;
                }
                else if (!GetNextExpressionItem(expressionChars, ref current, out expItem, out isOperant, out isOperator))
                    return null;

                if (isOperant)
                {
                    if (operants.Count != nOperators) // invalid expression
                        return null;
                    operants.Push(expItem as VariableWithUnit);
                }
                else if (isOperator)
                {
                    char c = (char)expItem;
                    // handle ([{ and +-
                    switch (c)
                    {
                        case '(': case '[': case '{':
                            if (operants.Count - nOperators == 1) // This is a parenthesis directly following a operant
                            {
                                operators.Push('*');
                                nOperators++;

                            }
                            operators.Push(c);
                            continue;

                        case '+': case '-':
                            if (operants.Count == nOperators) // This sign is following a operator, or at the beginning of the expression. It's a unary operator
                            {
                                if (c == '-')
                                {

                                    operants.Push(minusOne);
                                    operators.Push('*');
                                    nOperators++;
                                }
                                continue;
                            }
                            break;
                    }
                    while (operatorPrecedence[c] <= operatorPrecedence[operators.Peek()]) // Gets a low precedence operator. Evaluate the previous one.
                    {
                        char opr = operators.Pop();
                        if (operatorPrecedence[opr] < 1) // opr is ([{ or \0. c must be ),] or}
                            break;
                        nOperators--;
                        VariableWithUnit opt1, opt2; 
                        opt1 = operants.Pop();
                        opt2 = operants.Pop();
                        switch (opr)
                        {
                            case '^':
                                operants.Push(VariableWithUnit.Pow(opt2, opt1));
                                break;
                            case '*':
                                operants.Push(opt2 * opt1);
                                break;
                            case '/':
                                operants.Push(opt2 / opt1);
                                break;

                            case '+':
                                if ((opt2 += opt1) == null)
                                    return null;
                                else
                                    operants.Push(opt2);
                                break;
                            case '-':
                                if ((opt2 -= opt1) == null)
                                    return null;
                                else
                                    operants.Push(opt2);
                                break;
                        }
                    }
                    if (operatorPrecedence[c] > 1) // An operator that can be pushed
                        if (operants.Count - nOperators == 1)
                        {
                            operators.Push(c);
                            nOperators++;
                        }
                        else
                            return null;
                }
                else
                    return null;
            }
            return operants.Pop();
        }

        private static Dictionary<char, int> operatorPrecedence = new Dictionary<char, int>()
            {
                {'\0', -1 },
                {')', 0 },
                {']', 0 },
                {'}', 0 },
                {'+', 2 },
                {'-', 2 },
                {'*', 4 },
                {'/', 4 },
                {'^', 6 },
                {'(', 0 },
                {'[', 0 },
                {'{', 0 },
            };


        #endregion
    }
}
