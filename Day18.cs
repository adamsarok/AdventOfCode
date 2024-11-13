using System;

namespace AdventOfCode;

public class Day18
{
    // R 6 (#70c710)
    // D 5 (#0dc571)
    // L 2 (#5713f0)
    // D 2 (#d2c081)
    
    public static void SolvePart1() {
		var input = ReadFile("shortinput.txt"); 
		var d = new Solver(input);
		d.Solve();
		// var input2 = ReadFile("testinput.txt"); 
		// var d2 = new Dijkstra(input);
		// d2.Solve();
	}
    public enum Directions { R, D, L, U }
    record struct Input(Directions dir, int length, string color) {}

	private static List<Input> ReadFile(string fileName) {
		var file = File.ReadAllLines(fileName);
        List<Input> result = new List<Input>();
        foreach (var line in file) {
            var s = line.Split(' ');
            var input = new Input(dir: Enum.Parse<Directions>(s[0]), length: int.Parse(s[1]), color: s[2]);
            result.Add(input);
        }
        return result;
	}

    class Solver(List<Input> inputs) {
        bool[,] dig;
        int height, width;
        public void Solve() {
            //sizing the matrix - horizontal = cnt(L + R), vertical(D + U) bigger but ok
            height = inputs.Where(x => x.dir == Directions.U || x.dir == Directions.D).Sum(x => x.length) * 2;
            width = inputs.Where(x => x.dir == Directions.L || x.dir == Directions.R).Sum(x => x.length) * 2;
            dig = new bool[height, width];
            int y = (int)height / 2;
            int x = (int)width / 2;
            dig[y, x] = true;
            foreach (var input in inputs) {
                switch (input.dir) {
                    case Directions.U: Mark(ref x, ref y, dirX: 0, dirY: -1, length: input.length); break;
                    case Directions.D: Mark(ref x, ref y, dirX: 0, dirY: 1, length: input.length); break;
                    case Directions.L: Mark(ref x, ref y, dirX: -1, dirY: 0, length: input.length); break;
                    case Directions.R: Mark(ref x, ref y, dirX: 1, dirY: 0, length: input.length); break;
                }
            }
            CalcArea();
            PrintOut();
        }

		private void CalcArea() {
			int result = 0;
            for (int y = 0; y < height; y++) {
                bool inside = false;
                for (int x = 0; x < width; x++) {
                    if (dig[y, x]) { 
                        //this is not correct for horizontal lines :)
                        //we need a real fill algorithm instead of this 
                        inside = !inside;
                        result++;
                    } else if (inside) {
                        result++;
                    }
                }
            }
            Console.WriteLine(result);
		}

		private void PrintOut() {
			for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    Console.Write(dig[y, x] ? "#" : ".");
                }
                Console.WriteLine("");
            }
		}

		private void Mark(ref int x, ref int y, int dirX, int dirY, int length) {
            for (int i = 0; i < length; i++) {
                y += dirY;
                x += dirX;
                dig[y, x] = true;
            }
        }
    }
    
}
