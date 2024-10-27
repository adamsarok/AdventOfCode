using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    public class Day2
    {
        public static void Solve()
        {
            int part1 = 0;
            int part2 = 0;
            var lines = File.ReadAllLines("day2input.txt");
            foreach (var line in lines)
            {
                var spl = line.Split(':');
                int gameID = int.Parse(spl[0].Substring(5, spl[0].Length - 5));
                var pulls = spl[1].Split(';');
                int redMax = 0;
                int greenMax = 0;
                int blueMax = 0;
                foreach (var pull in pulls)
                {
                    var colors = pull.Split(',');
                    foreach (var color in colors)
                    {
                        var colnum = color.Trim().Split(" ");
                        int num = int.Parse(colnum[0]);
                        switch (colnum[1])
                        {
                            case "red":
                                redMax = Math.Max(num, redMax);
                                break;
                            case "green":
                                greenMax = Math.Max(num, greenMax);
                                break;
                            case "blue":
                                blueMax = Math.Max(num, blueMax);
                                break;
                        }
                    }
                }
                var pow = redMax * greenMax * blueMax;
                Console.WriteLine($"{line}:R{redMax} G{greenMax} B{blueMax} Pow:{pow}");
                part2 += pow;
                if (redMax <= 12 && greenMax <= 13 && blueMax <= 14) part1 += gameID;
            }
            Console.WriteLine($"PArt1: {part1}");
            Console.WriteLine($"PArt2: {part2}");
        }
    }
}
