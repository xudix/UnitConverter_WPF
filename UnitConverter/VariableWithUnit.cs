using System;
using System.Collections.Generic;

namespace UnitConverter
{
    /// <summary>
    /// 
    /// </summary>
    public class VariableWithUnit
    {
        //private string prefix;

        public double Value { get; set; }

        public Unit Unit { get; set; }

        public string Prefix { get; set; }
        //{
        //    get => prefix;
        //    set 
        //    {
        //        Value *= Prefixes.GetPrefixValue(prefix) / Prefixes.GetPrefixValue(value);
        //        prefix = value;
        //    }
        //}



        #region Constructors
        public VariableWithUnit(double value, Unit unit, String prefix = "")
        {
            Value = value;
            Unit = new Unit(unit);
            Prefix = prefix;
        }

        public VariableWithUnit(VariableWithUnit var) =>
            new VariableWithUnit(var.Value, var.Unit, var.Prefix);

        #endregion

        #region Public Methods



        public VariableWithUnit? Convert(Unit new_unit, String new_prefix = "")
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


        public bool Equals(VariableWithUnit var)
        {
            if((var as object) == null )
                return false;
            VariableWithUnit converted = var.Convert(Unit, Prefix);
            if ((converted as object) == null)
                return false;
            return converted.Value == Value;
        }

        public static bool Equals(VariableWithUnit var1, VariableWithUnit var2) =>
            var1 == var2;
            
        public static bool operator == (VariableWithUnit var1, VariableWithUnit var2) =>
            (var1 as object) == null ? (var2 as object) == null : var1.Equals(var2);

        public static bool operator != (VariableWithUnit var1, VariableWithUnit var2) =>
            (var1 as object) == null ? (var2 as object) != null : !var1.Equals(var2);

        public override bool Equals(object obj) =>
            obj is VariableWithUnit var && Equals(var);

        public VariableWithUnit Multiply(VariableWithUnit other) =>
            Unit.IsSameMeasure(Measure.Number)?
            new VariableWithUnit(Value * Prefixes.GetPrefixValue(Prefix) * other.Value, other.Unit, other.Prefix):
            other.Unit.IsSameMeasure(Measure.Number)?
            new VariableWithUnit(Value * other.Value * Prefixes.GetPrefixValue(other.Prefix), Unit, Prefix):
            new VariableWithUnit(Value * Prefixes.GetPrefixValue(Prefix) * other.Value * Prefixes.GetPrefixValue(other.Prefix), Unit * other.Unit);
        
            

        public static VariableWithUnit Multiply(VariableWithUnit var1, VariableWithUnit var2) =>
            var1.Multiply(var2);

        public static VariableWithUnit operator *(VariableWithUnit var1, VariableWithUnit var2) =>
            var1.Multiply(var2);

        public VariableWithUnit Divide(VariableWithUnit other) =>
            other.Unit.IsSameMeasure(Measure.Number) ?
            new VariableWithUnit(Value / (other.Value * Prefixes.GetPrefixValue(other.Prefix)), Unit, Prefix):
            new VariableWithUnit(Value * (Prefixes.GetPrefixValue(Prefix)) / (other.Value * Prefixes.GetPrefixValue(other.Prefix)), Unit / other.Unit);

        public static VariableWithUnit Divide(VariableWithUnit var1, VariableWithUnit var2) =>
            var1.Divide(var2);

        public static VariableWithUnit operator /(VariableWithUnit var1, VariableWithUnit var2) =>
            var1.Divide(var2);

        public static VariableWithUnit Pow(VariableWithUnit var1, VariableWithUnit var2) =>
            new VariableWithUnit(Math.Pow(var1.Value * Prefixes.GetPrefixValue(var1.Prefix), var2.Value*Prefixes.GetPrefixValue(var2.Prefix)), Unit.Pow(var1.Unit, (int)var2.Value));

        public static VariableWithUnit Pow(VariableWithUnit var1, int power) =>
            new VariableWithUnit(Math.Pow(var1.Value * Prefixes.GetPrefixValue(var1.Prefix), power), Unit.Pow(var1.Unit, power));

        public VariableWithUnit Pow(int power) =>
            Pow(this, power);

        public VariableWithUnit? Add(VariableWithUnit other) =>
            Unit.IsSameMeasure(other.Unit) ?
            new VariableWithUnit(Value + other.Convert(Unit, Prefix).Value, Unit, Prefix) : null;

        public static VariableWithUnit? Add(VariableWithUnit var1, VariableWithUnit var2) =>
            var1.Add(var2);

        public static VariableWithUnit? operator + (VariableWithUnit var1, VariableWithUnit var2) =>
            var1.Add(var2);

        public VariableWithUnit? Subtract(VariableWithUnit other) =>
            Unit.IsSameMeasure(other.Unit) ?
            new VariableWithUnit(Value - other.Convert(Unit, Prefix).Value, Unit, Prefix) : null;

        public static VariableWithUnit? Subtract(VariableWithUnit var1, VariableWithUnit var2) =>
            var1.Subtract(var2);

        public static VariableWithUnit? operator -(VariableWithUnit var1, VariableWithUnit var2) =>
            var1.Subtract(var2);
        #endregion


    }


}
