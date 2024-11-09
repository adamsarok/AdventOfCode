using System;
using System.Security.Cryptography.X509Certificates;

namespace AdventOfCode;

public class Day16 {
    //empty space (.)
    //mirrors (/ and \), 
    //and splitters (| and -).
    //static bool[,] energizedTiles;
    record struct Coord(int X, int Y);
    enum Directions { Up, Down, Left, Right, Stop };
    public static void SolvePart1() {
        var input = File.ReadAllLines("testinput.txt");
        var b = new BeamMap(input, new Coord(-1, 0), Directions.Right);
        Console.WriteLine(b.GetResult());
    }
    public static void SolvePart2() {
        var input = File.ReadAllLines("testinput.txt");
        int result = 0;
        for (int y = 0; y < input.Length; y++) {
            var b = new BeamMap(input, new Coord(-1, y), Directions.Right);
            result = Math.Max(result, b.GetResult());
            b = new BeamMap(input, new Coord(input[0].Length, y), Directions.Left);
            result = Math.Max(result, b.GetResult());
        }
        for (int x = 0; x < input.Length; x++) {
            var b = new BeamMap(input, new Coord(x, -1), Directions.Down);
            result = Math.Max(result, b.GetResult());
            b = new BeamMap(input, new Coord(x, input.Length), Directions.Up);
            result = Math.Max(result, b.GetResult());
        }
        Console.WriteLine(result);
    }

    class BeamMap {
        string[] input;
        List<Directions>[,] energizedTiles;
        public BeamMap(string[] input, Coord start, Directions heading) {
            this.input = input;
            this.energizedTiles = new List<Directions>[input[0].Length, input.Length];
            StartBeam(start, heading);
        }
        public int GetResult() {
            int result = 0;
            foreach (var val in energizedTiles) {
                if (val != null && val.Any()) result++;
            }
            return result;
        }
        bool StopIfCycle(Coord coord, Directions heading) {
            var list = energizedTiles[coord.X, coord.Y];
            if (list == null) {
                energizedTiles[coord.X, coord.Y] = new List<Directions>() { heading };
                return false;
            }
            if (list.Contains(heading)) return true;
            list.Add(heading);
            return false;
        }
        public void StartBeam(Coord start, Directions heading) {
            var nextCoord = start;
            while (true) {
                nextCoord = GetNextCoord(nextCoord, heading);
                if (nextCoord.X < 0 || nextCoord.X >= input[0].Length
                    || nextCoord.Y < 0 || nextCoord.Y >= input.Length) {
                    return;
                }
                if (StopIfCycle(nextCoord, heading)) return;
                var next = input[nextCoord.Y][nextCoord.X];

                //Console.WriteLine($"{nextCoord.X}:{nextCoord.Y}+{heading}={next}");
                //PrintState();
                switch (next) {
                    case '.':
                        break;
                    case '/':
                        heading = Slash(heading);
                        break;
                    case '\'':
                    case '\\': // \\ -> is parsed as one char instead of two
                        heading = BackSlash(heading);
                        break;
                    case '|':
                        heading = VerticalSplitter(nextCoord, heading);
                        break;
                    case '-':
                        heading = HorizontalSplitter(nextCoord, heading);
                        break;
                    default:
                        throw new WtfException();
                }
            }
        }

        Directions VerticalSplitter(Coord coord, Directions heading) {
            switch (heading) {
                case Directions.Up:
                case Directions.Down:
                    return heading;
                case Directions.Left:
                case Directions.Right:
                    StartBeam(coord, Directions.Up);
                    return Directions.Down;
                default: throw new WtfException();
            }
        }
        Directions HorizontalSplitter(Coord coord, Directions heading) {
            switch (heading) {
                case Directions.Up:
                case Directions.Down:
                    StartBeam(coord, Directions.Left);
                    return Directions.Right;
                case Directions.Left:
                case Directions.Right:
                    return heading;
                default: throw new WtfException();
            }
        }
        class WtfException : Exception {
            public WtfException() : base("Shouldn't happen lol") { }
        }

        Directions BackSlash(Directions heading) {
            switch (heading) {
                case Directions.Left: return Directions.Up;
                case Directions.Right: return Directions.Down;
                case Directions.Up: return Directions.Left;
                case Directions.Down: return Directions.Right;
                default: throw new WtfException();
            }
        }

        Directions Slash(Directions heading) {
            switch (heading) {
                case Directions.Left: return Directions.Down;
                case Directions.Right: return Directions.Up;
                case Directions.Up: return Directions.Right;
                case Directions.Down: return Directions.Left;
                default: throw new WtfException();
            }
        }

        Coord GetNextCoord(Coord coord, Directions heading) {
            switch (heading) {
                case Directions.Up: return new Coord(coord.X, coord.Y - 1);
                case Directions.Down: return new Coord(coord.X, coord.Y + 1);
                case Directions.Left: return new Coord(coord.X - 1, coord.Y);
                case Directions.Right: return new Coord(coord.X + 1, coord.Y);
                default: throw new WtfException();
            }
        }

    }


}
