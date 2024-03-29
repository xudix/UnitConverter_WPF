﻿using System;
using System.Collections.Generic;
using System.Text;

namespace UnitConverter
{
    public class Measure
    {
        #region Public Properties
        /// <summary>
        /// Common name for the quantity with this measure, e.g. length, temperature, force, concentration, etc.
        /// Upper case only.
        /// </summary>
        public string MeasureName
        {
            get => name;
            set {
                name = value.ToUpper();
            }
        }
        private string name;

        /// <summary>
        /// Power of Length. The SI unit for length is meter (m).
        /// </summary>
        public int PowerOfLength { get; set; }

        /// <summary>
        /// Power of Time. The SI unit for time is second (s).
        /// </summary>
        public int PowerOfTime { get; set; }

        /// <summary>
        /// Power of Mass. The SI unit for mass is kilogram (kg).
        /// </summary>
        public int PowerOfMass { get; set; }

        /// <summary>
        /// Power of Substance Amount The SI unit for amount of substance is mole (mol).
        /// </summary>
        public int PowerOfSubstanceAmount { get; set; }

        /// <summary>
        /// Power of Temperature. The SI unit for temperature is kelvin (K).
        /// </summary>
        public int PowerOfTemperature { get; set; }

        /// <summary>
        /// Power of Electric Current. The SI unit for current is amp (A).
        /// </summary>
        public int PowerOfCurrent { get; set; }

        /// <summary>
        /// Power ofLuminous Intensity. The SI unit for luminous intensity is candela (cd)
        /// </summary>
        public int PowerOfLuminousIntensity { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Create a Measure object.
        /// </summary>
        /// <param name="powerOfLength"></param>
        /// <param name="powerOfTime"></param>
        /// <param name="powerOfMass"></param>
        /// <param name="powerOfSubstanceAmount"></param>
        /// <param name="powerOfTemperature"></param>
        /// <param name="powerOfCurrent"></param>
        /// <param name="powerofLightIntensity"></param>
        /// <param name="Name">Common name for the quantity with this measure, e.g. length, temperature, force, concentration, etc.</param>
        public Measure(int powerOfLength, int powerOfTime, int powerOfMass, int powerOfSubstanceAmount,
            int powerOfTemperature, int powerOfCurrent, int powerofLightIntensity, string measureName)
        {
            PowerOfLength = powerOfLength;
            PowerOfCurrent = powerOfCurrent;
            PowerOfTime = powerOfTime;
            PowerOfTemperature = powerOfTemperature;
            PowerOfSubstanceAmount = powerOfSubstanceAmount;
            PowerOfMass = powerOfMass;
            PowerOfLuminousIntensity = powerofLightIntensity;
            MeasureName = measureName;
        }

        /// <summary>
        /// Create a Measure object.
        /// </summary>
        /// <param name="powerOfLength"></param>
        /// <param name="powerOfTime"></param>
        /// <param name="powerOfMass"></param>
        /// <param name="powerOfSubstanceAmount"></param>
        /// <param name="powerOfTemperature"></param>
        /// <param name="powerOfCurrent"></param>
        /// <param name="powerofLightIntensity"></param>
        public Measure(int powerOfLength, int powerOfTime, int powerOfMass, int powerOfSubstanceAmount,
            int powerOfTemperature, int powerOfCurrent, int powerofLightIntensity)
        {
            PowerOfLength = powerOfLength;
            PowerOfCurrent = powerOfCurrent;
            PowerOfTime = powerOfTime;
            PowerOfTemperature = powerOfTemperature;
            PowerOfSubstanceAmount = powerOfSubstanceAmount;
            PowerOfMass = powerOfMass;
            PowerOfLuminousIntensity = powerofLightIntensity;
            MeasureName = ""; // Default name to be added
        }

        public Measure()
        {
            PowerOfCurrent = 0;
            PowerOfLength = 0;
            PowerOfLuminousIntensity = 0;
            PowerOfMass = 0;
            PowerOfSubstanceAmount = 0;
            PowerOfTemperature = 0;
            PowerOfTime = 0;
            MeasureName = "NUMBER";
        }

        public Measure(Measure measure)
        {
            PowerOfLength = measure.PowerOfLength;
            PowerOfCurrent = measure.PowerOfCurrent;
            PowerOfTime = measure.PowerOfTime;
            PowerOfTemperature = measure.PowerOfTemperature;
            PowerOfSubstanceAmount = measure.PowerOfSubstanceAmount;
            PowerOfMass = measure.PowerOfMass;
            PowerOfLuminousIntensity = measure.PowerOfLuminousIntensity;
            MeasureName = measure.MeasureName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetSISymbol()
        {
            List<string> symbols = new List<string>();
            if (PowerOfLength != 0)
                symbols.Add(PowerOfLength == 1 ? "m" : "m^" + PowerOfLength);
            if (PowerOfTime != 0)
                symbols.Add(PowerOfTime == 1 ? "s" : "s^" + PowerOfTime);
            if (PowerOfMass != 0)
                symbols.Add(PowerOfMass == 1 ? "kg" : "kg^" + PowerOfMass);
            if (PowerOfSubstanceAmount != 0)
                symbols.Add(PowerOfSubstanceAmount == 1 ? "mol" : "mol^" + PowerOfSubstanceAmount);
            if (PowerOfTemperature != 0)
                symbols.Add(PowerOfTemperature == 1 ? "K" : "K^" + PowerOfTemperature);
            if (PowerOfCurrent != 0)
                symbols.Add(PowerOfCurrent == 1 ? "A" : "A^" + PowerOfCurrent);
            if (PowerOfLuminousIntensity != 0)
                symbols.Add(PowerOfLuminousIntensity == 1 ? "cd" : "cd^" + PowerOfLuminousIntensity);
            if (symbols.Count > 0)
                return String.Join("*", symbols);
            else return String.Empty;
        }

        public string GetDimension()
        {
            List<string> symbols = new List<string>();
            if (PowerOfLength != 0)
                symbols.Add(PowerOfLength == 1 ? "L" : "L^" + PowerOfLength);
            if (PowerOfTime != 0)
                symbols.Add(PowerOfTime == 1 ? "T" : "T^" + PowerOfLength);
            if (PowerOfMass != 0)
                symbols.Add(PowerOfMass == 1 ? "M" : "M^" + PowerOfLength);
            if (PowerOfSubstanceAmount != 0)
                symbols.Add(PowerOfSubstanceAmount == 1 ? "N" : "N^" + PowerOfLength);
            if (PowerOfTemperature != 0)
                symbols.Add(PowerOfTemperature == 1 ? "U+0398" : "U+0398^" + PowerOfLength);
            if (PowerOfCurrent != 0)
                symbols.Add(PowerOfCurrent == 1 ? "I" : "I^" + PowerOfLength);
            if (PowerOfLuminousIntensity != 0)
                symbols.Add(PowerOfLuminousIntensity == 1 ? "J" : "J^" + PowerOfLength);
            if (symbols.Count > 0)
                return String.Join("*", symbols);
            else return String.Empty;
        }

        /// <summary>
        /// Compares whether two measures are the same. They are the same if the power of all quantities are the same.
        /// </summary>
        /// <param name="measure">Another measure to be compared</param>
        /// <returns></returns>
        public bool Equals(Measure measure) =>
            (measure as object) != null &&
            this.PowerOfCurrent == measure.PowerOfCurrent &&
            this.PowerOfLength == measure.PowerOfLength &&
            this.PowerOfLuminousIntensity == measure.PowerOfLuminousIntensity &&
            this.PowerOfMass == measure.PowerOfMass &&
            this.PowerOfSubstanceAmount == measure.PowerOfSubstanceAmount &&
            this.PowerOfTemperature == measure.PowerOfTemperature &&
            this.PowerOfTime == measure.PowerOfTime;

        /// <summary>
        /// Compares whether two measures are the same. They are the same if the power of all quantities are the same.
        /// </summary>
        /// <param name="measure1">1st Measure to be compared</param>
        /// <param name="measure2">2nd Measure to be compared</param>
        /// <returns></returns>
        public static bool IsSameMeasure(Measure measure1, Measure measure2) =>
            measure1.Equals(measure2);

        /// <summary>
        /// Determine if this measure is of the same measure as another unit or measure.
        /// If true, this unit can be converted to the other unit or measure.
        /// </summary>
        /// <param name="measure"></param>
        /// <returns></returns>
        public bool IsSameMeasure(Measure measure) =>
            Equals(measure);

        public static bool operator ==(Measure left, Measure right) =>
            (left as object) == null ? (right as object) == null: left.Equals(right);

        public static bool operator != (Measure left, Measure right) =>
            (left as object) == null ? !((right as object) == null): !left.Equals(right);

        /// <summary>
        /// Compares whether two measures are the same. They are the same if the power of all quantities are the same.
        /// </summary>
        /// <param name="o">o should be an instance of Measure. Otherwise, this method will return False</param>
        /// <returns></returns>
        public override bool Equals(object o)
        {
            return o is Measure measure && Equals(measure); // This is called "type pattern"
        }
        #endregion

        #region standard measures

        public static readonly Measure Number = new Measure();
        public static readonly Measure Length = new Measure(1, 0, 0, 0, 0, 0, 0, "LENGTH");
        public static readonly Measure Time = new Measure(0, 1, 0, 0, 0, 0, 0, "TIME");
        public static readonly Measure Mass = new Measure(0, 0, 1, 0, 0, 0, 0, "MASS");
        public static readonly Measure SubstanceAmount = new Measure(0, 0, 0, 1, 0, 0, 0, "SUBSTANCEAMOUNT");
        public static readonly Measure Temperature = new Measure(0, 0, 0, 0, 1, 0, 0, "TEMPERATURE");
        public static readonly Measure Current = new Measure(0, 0, 0, 0, 0, 1, 0, "CURRENT");
        public static readonly Measure LightIntensity = new Measure(0, 0, 0, 0, 0, 0, 1, "LIGNTINTENSITY");

        #endregion

    }
}
