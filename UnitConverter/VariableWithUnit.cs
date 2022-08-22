using System;
using System.Collections.Generic;

namespace UnitConverter
{
    /// <summary>
    /// 
    /// </summary>
    public class VariableWithUnit
    {
        private string prefix;

        public double Value { get; set; }

        public Unit Unit { get; set; }

        public string Prefix
        {
            get => prefix;
            set 
            {
                Value *= Prefixes.GetPrefixValue(prefix) / Prefixes.GetPrefixValue(value);
                prefix = value;
            }
        }



        #region Constructors
        public VariableWithUnit(double value, Unit unit, String prefix = "")
        {
            Value = value;
            Unit = unit;
            Prefix = prefix;
        }



        #endregion

        #region Public Methods



        public VariableWithUnit Convert(Unit new_unit, String new_prefix = "")
        {
            if (Unit.IsSameMeasure(new_unit))
            {
                double new_value = (Value * Unit.Multiplier * Prefixes.GetPrefixValue(Prefix) + Unit.Offset - new_unit.Offset) / new_unit.Multiplier / Prefixes.GetPrefixValue(new_prefix);
                return new VariableWithUnit(new_value, new_unit, new_prefix);
            }
            else
                return null;
        }

        public override String ToString() =>
            string.Format("{0:G} {1}{2}", Value, Prefix, Unit.UnitSymbol);

        #endregion


    }


}
