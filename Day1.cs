using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    public class Day1
    {
        public static void Solve()
        {
            var lines = File.ReadAllLines("day1input.txt");
            int result = 0;
            Dictionary<string, int> nums = new Dictionary<string, int>() {
                { "one", 1 },
                { "two", 2},
                { "three",3},
                { "four", 4},
                { "five", 5},
                { "six", 6},
                { "seven", 7},
                { "eight", 8},
                { "nine", 9},
            };
            foreach (var line in lines)
            {
                string first = "";
                string last = "";
                int i = 0;
                int j = 1;
                while (i < line.Length)
                { //min length to check should be three!
                    j = 1;
                    char c = line[i];
                    if (char.IsNumber(c))
                    {
                        if (string.IsNullOrEmpty(first)) first = c.ToString();
                        last = c.ToString();
                        i++;
                    }
                    else
                    {
                        while (j <= Math.Min(5, line.Length - i))
                        {
                            var str = line.Substring(i, j);
                            int num;
                            if (nums.TryGetValue(str, out num))
                            {
                                if (string.IsNullOrEmpty(first)) first = num.ToString();
                                last = num.ToString();
                                break;
                            }
                            j++;
                        }
                        i++;
                    }
                }
                var sub = int.Parse(first + last);
                Console.WriteLine($"{line}:{sub}");
                result += sub;
            }
            Console.WriteLine($"Total: {result}");
        }
    }
}
