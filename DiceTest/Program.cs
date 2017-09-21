using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiceTest
{
    public class Program
    {
        enum Options
        {
            Higher,
            Lower,
            Exact
        }

        public static void Main(string[] args)
        {
            var regexp = Regex.Match(string.Join(" ", args), @"-(e|expression|exp) (?<Expression>\d+?d\d+(?<Mod>[+-]\d+)?) -(t|target) (?<Target>\d+)( -(?<Option>higher|lower|exact))?", RegexOptions.IgnoreCase);

            if (!regexp.Success)
            {
                Console.WriteLine("help");
                return;
            }

            var modifier = string.IsNullOrWhiteSpace(regexp.Groups["Mod"].Value) ? 0 : int.Parse(regexp.Groups["Mod"].Value);
            var diceExpression = regexp.Groups["Expression"].Value.Split("d+-".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var target = int.Parse(regexp.Groups["Target"].Value) - modifier;
            var option = (Options)Enum.Parse(typeof(Options), string.IsNullOrWhiteSpace(regexp.Groups["Option"].Value) ? "Exact" : regexp.Groups["Option"].Value, true);

            var amount = int.Parse(diceExpression[0]);
            var dice = int.Parse(diceExpression[1]);

            var possibleRollsCount = Math.Pow(dice, amount);
            var maxValue = dice * amount;
            var minValue = amount;

            if (target > maxValue || target < minValue)
            {
                Console.WriteLine("0%");
                return;
            }
            else if (target == maxValue || target == minValue)
            {
                Console.WriteLine((1.0 / possibleRollsCount).ToString("P"));
                return;
            }

            var chances = 0;

            switch (option)
            {
                case Options.Higher:
                    for (int i = target; i <= maxValue; i++)
                    {
                        chances += CalculateChance(dice, i, amount);
                    }
                    break;
                case Options.Lower:
                    for (int i = target; i >= minValue; i--)
                    {
                        chances += CalculateChance(dice, i, amount);
                    }
                    break;
                case Options.Exact:
                default:
                    chances = CalculateChance(dice, target, amount);
                    break;
            }

            Console.WriteLine((chances / possibleRollsCount).ToString("P"));
        }

        // http://mathforum.org/library/drmath/view/52207.html
        private static int CalculateChance(int dice, int target, int amount)
        {
            var x = (target - amount) / dice;

            var retVal = 0.0;

            for (int k = 0; k <= x; k++)
            {
                retVal += Math.Pow(-1, k) * Permutate(amount, k) * Permutate(target - (dice * k) - 1, amount - 1);
            }

            return (int)retVal;
        }

        private static double Permutate(int i, int j)
        {
            return Factorial(i) / (Factorial(i - j) * Factorial(j));
        }

        private static double Factorial(double d)
        {
            if (d > 1)
            {
                return d * Factorial(d - 1);
            }
            else
            {
                return 1.0;
            }
        }
    }
}
