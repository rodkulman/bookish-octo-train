using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiceTest
{
    public class Program
    {        enum Options
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

            var rolls = new Dictionary<int, List<int[]>>();

            for (int i = minValue; i <= maxValue; i++)
            {
                rolls[i] = new List<int[]>();
            }

            CalculateRolls(rolls, dice, 0, amount, null);

            var chances = 0;

            switch (option)
            {
                case Options.Higher:
                    chances = rolls.Where(x => x.Key >= target).Sum(x => x.Value.Count);
                    break;
                case Options.Lower:
                    chances = rolls.Where(x => x.Key <= target).Sum(x => x.Value.Count);
                    break;
                case Options.Exact:
                default:
                    chances = rolls[target].Count;
                    break;
            }

            Console.WriteLine((chances / possibleRollsCount).ToString("P"));
        }

        private static void CalculateRolls(Dictionary<int, List<int[]>> rolls, int dice, int currentDepth, int maxDepth, int[] values)
        {
            if (currentDepth == 0) { values = new int[maxDepth]; }

            if (currentDepth + 1 > maxDepth)
            {
                var total = values.Sum();
                rolls[total].Add(values);
                return;
            }

            for (int i = 1; i <= dice; i++)
            {
                values[currentDepth] = i;

                CalculateRolls(rolls, dice, currentDepth + 1, maxDepth, values);
            }
        }
    }
}
