using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UnitConverter
{
    /// <summary>
    /// Contains Prefixes used in SI units.
    /// </summary>
    class Prefixes: IEnumerable<string>
    {
        /// <summary>
        /// Prefixes for SI unit. Call VariableWithUnit.Prefixes["prefix"] will return the corresponding value (Double)
        /// For example, VariableWithUnit.Prefixes["k"] is 1.0e3.
        /// </summary>
        private static readonly Dictionary<String, Double> Dict_Prefixes = new Dictionary<string, double>
        {
            {"Y", 1.0e24 },
            {"Z", 1.0e21 },
            {"E", 1.0e18 },
            {"P", 1.0e15 },
            {"T", 1.0e12 },
            {"G", 1.0e9 },
            {"M", 1.0e6 },
            {"k", 1.0e3 },
            {"h", 1.0e2 },
            {"da", 1.0e1 },
            {"", 1.0 },
            {"d", 1.0e-1 },
            {"c", 1.0e-2 },
            {"m", 1.0e-3 },
            {"mu", 1.0e-6 },
            {"n", 1.0e-9 },
            {"p", 1.0e-12 },
            {"f", 1.0e-15 },
            {"a", 1.0e-18 },
            {"z", 1.0e-21 },
            {"y", 1.0e-24 },
        };

        /// <summary>
        /// Convert a SI prefix to the corresponding value in Double.
        /// </summary>
        /// <param name="prefix">A string containing the SI prefix. It is case sensitive.
        /// When the prefix is not found, the method returns 1.0.</param>
        /// <returns>The corresponding value of the prefix in Double.</returns>
        public static double GetPrefixValue(string prefix) =>
            Dict_Prefixes.TryGetValue(prefix, out double result) ? result : 1.0;

        public Prefixes()
        {

        }



        public IEnumerable<string> PrefixNames
        {
            get => Dict_Prefixes.Keys;
        }

        #region Implementing IEnumerable<string>
        public IEnumerator<string> GetEnumerator() =>
            Dict_Prefixes.Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            Dict_Prefixes.Keys.GetEnumerator();
        #endregion

    }
}
