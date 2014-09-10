using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycReSoft.Extensions
{
    static public class Numerics
    {
        public static double StirlingApproximationFactorial(this  int n)
        {
            double result = n * Math.Log(n);
            result -= n;
            result += 0.5 * Math.Log(2 * Math.PI * n);

            return result;
        }

        public static double Combinations(int n, int k)
        {
            double result = 0;

            double numerator = n.StirlingApproximationFactorial();
            double denominator = k.StirlingApproximationFactorial() + (n - k).StirlingApproximationFactorial();

            result = numerator - denominator;

            return Math.Round(Math.Exp(result));
        }

    }
}
