using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skill_Rating_Estimator
{
    static class Calculus
    {
        public delegate double Function(double x);

        public static double Average(Function f, double from, double to, int steps)
        {
            return Integral((x) => x * f(x), from, to, steps) / Integral(f, from, to, steps);
        }

        public static double Integral(Function f, double from, double to, int steps)
        {
            double step = (to - from) / steps;
            double sum = 0;

            for (int i = 0; i < steps; i++)
                sum += f(from + step * i - step / 2) * step;

            return sum;
        }

        public static double MinimizeCost(Function density, Function cost, double from, double to, int steps)
        {
            double greatestCost = double.PositiveInfinity;
            double greatestEstimator = double.NaN;
            for (int i = 0; i < steps; i++)
            {
                double estimator = from + 1.0 * i / steps * (to - from);
                double estimatorCost(double actual) => cost(estimator - actual) * density(actual);
                double expectedCost = Integral(estimatorCost, from, to, steps);
                if (expectedCost < greatestCost)
                {
                    greatestCost = expectedCost;
                    greatestEstimator = estimator;
                }
            }
            return greatestEstimator;
        }
    }
}
