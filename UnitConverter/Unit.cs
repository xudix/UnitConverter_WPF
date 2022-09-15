using System;
using System.Collections.Generic;
using System.Text;

namespace UnitConverter
{
    public class Unit : Measure
    {
        #region Public Properties

        /// <summary>
        /// Actual name of the unit.
        /// E.g. meter, pound, mega watts, etc.
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// Symbol of the unit.
        /// E.g. m, lb, MW, etc.
        /// </summary>
        public string UnitSymbol { get; set; }

        /// <summary>
        /// The Multiplier converts the variable under this specific unit to SI value.
        /// The value of the variable multiply by the Multiplier plus the Offset equals the value under SI unit.
        /// value (SI) = value (Current Unit) * Multiplier + Offset 
        /// E.g. the unit "kilo meter" has a multiplier of 1000; the unit "feet" has a multiplier of 0.3048.
        /// </summary>
        public double Multiplier { get; set; }

        /// <summary>
        /// The Offset is result of converting zero to the corresponding SI unit.
        /// The value of the variable multiply by the Multiplier plus the Offset equals the value under SI unit.
        /// value (SI) = value (Current Unit) * Multiplier + Offset 
        /// E.g. the unit "degrees Celcius" have an Offset of 273.15; the unit "psig" has an offset of 101325.
        /// </summary>
        public double Offset { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a Unit object.
        /// </summary>
        /// <param name="multiplier">The Multiplier converts the variable under this specific unit to SI value.
        /// The value of the variable multiply by the Multiplier equals the value under SI unit.
        /// E.g. the unit "kilo meter" has a multiplier of 1000; the unit "feet" has a multiplier of 0.3048.
        /// Multiplier should NOT be zero.</param>
        /// <param name="measure">The measure on which this unit will be based on</param>
        /// <param name="unitName">Actual name of the unit.
        /// E.g. meter, pound, mega watts, etc.</param>
        /// <param name="unitSymbol">Symbol of the unit.
        /// E.g. m, lb, MW, etc.</param>
        public Unit( Measure measure, double multiplier = 1.0, double offset = 0, string unitName = "", string unitSymbol = "")
        {
            Multiplier = multiplier;
            Offset = offset;
            PowerOfLength = measure.PowerOfLength;
            PowerOfCurrent = measure.PowerOfCurrent;
            PowerOfTime = measure.PowerOfTime;
            PowerOfTemperature = measure.PowerOfTemperature;
            PowerOfSubstanceAmount = measure.PowerOfSubstanceAmount;
            PowerOfMass = measure.PowerOfMass;
            PowerOfLuminousIntensity = measure.PowerOfLuminousIntensity;
            MeasureName = measure.MeasureName;
            UnitName = unitName;
            UnitSymbol = unitSymbol;
        }
        public Unit( int powerOfLength, int powerOfTime, int powerOfMass, int powerOfSubstanceAmount,
            int powerOfTemperature, int powerOfCurrent, int powerofLuminousIntensity,double multiplier = 1.0, double offset = 0, string measureName = "", string unitName = "", string unitSymbol = "")
        {
            PowerOfLength = powerOfLength;
            PowerOfCurrent = powerOfCurrent;
            PowerOfTime = powerOfTime;
            PowerOfTemperature = powerOfTemperature;
            PowerOfSubstanceAmount = powerOfSubstanceAmount;
            PowerOfMass = powerOfMass;
            PowerOfLuminousIntensity = powerofLuminousIntensity;
            Multiplier = multiplier;
            Offset = offset;
            MeasureName = measureName;
            UnitName = unitName;
            UnitSymbol = unitSymbol;
        }

        /// <summary>
        /// The parameterless constructor will result in an empty unit. The measure is NUMBER and the Multiplier is 1.0.
        /// </summary>
        public Unit(): base()
        {
            Multiplier = 1.0;
            Offset = 0.0;
        }

        /// <summary>
        /// Construct a new Unit as a duplicate of the input.
        /// </summary>
        /// <param name="unit"></param>
        public Unit(Unit unit)
        {
            PowerOfCurrent = unit.PowerOfCurrent;
            PowerOfTime = unit.PowerOfTime;
            PowerOfTemperature = unit.PowerOfTemperature;
            PowerOfSubstanceAmount = unit.PowerOfSubstanceAmount;
            PowerOfLength = unit.PowerOfLength;
            PowerOfMass = unit.PowerOfMass;
            PowerOfLuminousIntensity = unit.PowerOfLuminousIntensity;
            Multiplier = unit.Multiplier;
            Offset = unit.Offset;
            MeasureName = unit.MeasureName;
            UnitName = unit.UnitName;
            UnitSymbol = unit.UnitSymbol;

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Find the unit that corresponds to the product of two units.
        /// </summary>
        /// <param name="unit1"></param>
        /// <param name="unit2"></param>
        /// <returns></returns>
        public static Unit Multiply(Unit unit1, Unit unit2) =>
            new Unit(unit1.PowerOfLength + unit2.PowerOfLength, unit1.PowerOfTime + unit2.PowerOfTime,
                unit1.PowerOfMass + unit2.PowerOfMass, unit1.PowerOfSubstanceAmount + unit2.PowerOfSubstanceAmount,
                unit1.PowerOfTemperature + unit2.PowerOfTemperature, unit1.PowerOfCurrent + unit2.PowerOfCurrent,
                unit1.PowerOfLuminousIntensity + unit2.PowerOfLuminousIntensity, unit1.Multiplier * unit2.Multiplier);

        public static Unit operator *(Unit left, Unit right) =>
            Multiply(left, right);

        public static Unit Divide(Unit unit1, Unit unit2) =>
            new Unit(unit1.PowerOfLength - unit2.PowerOfLength, unit1.PowerOfTime - unit2.PowerOfTime,
                unit1.PowerOfMass - unit2.PowerOfMass, unit1.PowerOfSubstanceAmount - unit2.PowerOfSubstanceAmount,
                unit1.PowerOfTemperature - unit2.PowerOfTemperature, unit1.PowerOfCurrent - unit2.PowerOfCurrent,
                unit1.PowerOfLuminousIntensity - unit2.PowerOfLuminousIntensity, unit1.Multiplier / unit2.Multiplier);

        public static Unit operator /(Unit left, Unit right) =>
            Divide(left, right);

        public static Unit Pow(Unit unit, int pwr) => 
            new Unit(unit.PowerOfLength * pwr,
                unit.PowerOfTime * pwr,
                unit.PowerOfMass * pwr,
                unit.PowerOfSubstanceAmount * pwr,
                unit.PowerOfTemperature * pwr,
                unit.PowerOfCurrent * pwr,
                unit.PowerOfLuminousIntensity * pwr,
                Math.Pow(unit.Multiplier, pwr),
                0);

        public static Unit Pow(Unit unit, Unit pwr) =>
            Pow(unit, (int)pwr.Multiplier);

        public bool Equals(Unit unit) =>
            (unit as object) != null  &&
            PowerOfCurrent == unit.PowerOfCurrent &&
            PowerOfLength == unit.PowerOfLength &&
            PowerOfLuminousIntensity == unit.PowerOfLuminousIntensity &&
            PowerOfMass == unit.PowerOfMass &&
            PowerOfSubstanceAmount == unit.PowerOfSubstanceAmount &&
            PowerOfTemperature == unit.PowerOfTemperature &&
            PowerOfTime == unit.PowerOfTime &&
            Multiplier != 0 &&
            Math.Abs(Multiplier - unit.Multiplier) / Multiplier < 1e-9 &&
            (Offset == unit.Offset ||
            (Offset != 0 && Math.Abs(Offset - unit.Offset) / Offset < 1e-9) ||
            (unit.Offset != 0 && Math.Abs(Offset - unit.Offset) / unit.Offset < 1e-9));

        public static bool operator ==(Unit left, Unit right) =>
            (left as object) == null ? (right as object) == null : left.Equals(right);

        public static bool operator !=(Unit left, Unit right) =>
            (left as object) == null ? (right as object) != null:!left.Equals(right);

        public static bool IsSameUnit(Unit unit1, Unit unit2) =>
            unit1.Equals(unit2);

        public override bool Equals(object o) =>
            o is Unit unit && Equals(unit); //Is this OK if o is Measure?



        public override string ToString() =>
            string.Format("{0} ({1})", UnitSymbol, UnitName);

        #endregion
    }
}
