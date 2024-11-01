
using System.Collections;
using System.Xml.Serialization;

namespace AdventOfCode;

public class Day10 {
    // | is a vertical pipe connecting north and south.
    // - is a horizontal pipe connecting east and west.
    // L is a 90-degree bend connecting north and east.
    // J is a 90-degree bend connecting north and west.
    // 7 is a 90-degree bend connecting south and west.
    // F is a 90-degree bend connecting south and east.
    // . is ground; there is no pipe in this tile.
    // S is the starting position of the animal; there is a pipe on this tile, but your sketch doesn't show what shape the pipe has.

    static Dictionary<int, List<int>> visitedCoords = new Dictionary<int, List<int>>();
    enum Directions { North, East, West, South };
    static int result = 0;
    static char[][] input;
    public static void SolvePart1() {
        input = ReadInput();
        Queue<Step> q = new Queue<Step>();
        for (int y = 0; y < input.Length; y++) {
            var row = input[y];
            for (int x = 0; x < row.Length; x++) {
                if (row[x] == 'S') {
                    visitedCoords.Add(x, new List<int>() { y });
                    q.Enqueue(new Step(x + 1, y, Directions.West, 0));
                    q.Enqueue(new Step(x - 1, y, Directions.East, 0));
                    q.Enqueue(new Step(x, y + 1, Directions.North, 0));
                    q.Enqueue(new Step(x, y - 1, Directions.South, 0));
                    while (q.Any()) {
                        var next = q.Dequeue();
                        Visit(next, q);
                    }
                }
            }
        }
        Console.WriteLine(result);
    }
    public static void SolvePart2() {
        //start flood fill from all edge tiles
        //how do I check if a tile is floodable with the squeeze rule?
        //it would be easy without the squeeze
        Console.WriteLine(result);
    }
    private static void Queue(int xFrom, int yFrom, int x, int y, Directions direction, int length, Queue<Step> q) {
        Console.WriteLine($"[{xFrom},{yFrom}]={input[yFrom][xFrom]} thisLength={length}");
        result = Math.Max(result, length);
        AddVisited(xFrom, yFrom);
        q.Enqueue(new Step(x, y, direction, length));
    }
    private static void Visit(Step s, Queue<Step> q) {
        if (!CanVisit(s.x, s.y)) return;
        var next = input[s.y][s.x];
        switch (next) {
            case '.':
            case 'S':
                return;
            case '-':
                switch (s.from) {
                    case Directions.East:
                        Queue(s.x, s.y, s.x - 1, s.y, Directions.East, s.length + 1, q);
                        break;
                    case Directions.West:
                        Queue(s.x, s.y, s.x + 1, s.y, Directions.West, s.length + 1, q);
                        break;
                }
                break;
            case '|':
                switch (s.from) {
                    case Directions.North:
                        Queue(s.x, s.y, s.x, s.y + 1, Directions.North, s.length + 1, q);
                        break;
                    case Directions.South:
                        Queue(s.x, s.y, s.x, s.y - 1, Directions.South, s.length + 1, q);
                        break;
                }
                break;
            case 'J':
                switch (s.from) {
                    case Directions.North:
                        Queue(s.x, s.y, s.x - 1, s.y, Directions.East, s.length + 1, q);
                        break;
                    case Directions.West:
                        Queue(s.x, s.y, s.x, s.y - 1, Directions.South, s.length + 1, q);
                        break;
                }
                break;
            case 'L':
                switch (s.from) {
                    case Directions.North:
                        Queue(s.x, s.y, s.x + 1, s.y, Directions.West, s.length + 1, q);
                        break;
                    case Directions.East:
                        Queue(s.x, s.y, s.x, s.y - 1, Directions.South, s.length + 1, q);
                        break;
                }
                break;
            case '7':
                switch (s.from) {
                    case Directions.South:
                        Queue(s.x, s.y, s.x - 1, s.y, Directions.East, s.length + 1, q);
                        break;
                    case Directions.West:
                        Queue(s.x, s.y, s.x, s.y + 1, Directions.North, s.length + 1, q);
                        break;
                }
                break;
            case 'F':
                switch (s.from) {
                    case Directions.South:
                        Queue(s.x, s.y, s.x + 1, s.y, Directions.West, s.length + 1, q);
                        break;
                    case Directions.East:
                        Queue(s.x, s.y, s.x, s.y + 1, Directions.North, s.length + 1, q);
                        break;
                }
                break;
            default:
                throw new NotImplementedException();
        }
    }


    struct Step {
        public int x;
        public int y;
        public Directions from;
        public int length;
        public Step(int x, int y, Directions from, int length) {
            this.x = x;
            this.y = y;
            this.from = from;
            this.length = length;
        }
    }

    //DFS is not correct as we dont stop at the correct position
    public static void SolveDFS() {
        input = ReadInput();
        for (int y = 0; y < input.Length; y++) {
            var row = input[y];
            for (int x = 0; x < row.Length; x++) {
                if (row[x] == 'S') {
                    visitedCoords.Add(x, new List<int>() { y });
                    VisitDFS(x + 1, y, Directions.West, 1);
                    VisitDFS(x - 1, y, Directions.East, 1);
                    VisitDFS(x, y + 1, Directions.North, 1);
                    VisitDFS(x, y - 1, Directions.South, 1);
                }
            }
        }
        Console.WriteLine(result);
    }
    private static void VisitDFS(int x, int y, Directions comingFrom, int length) {
        if (!CanVisit(x, y)) return;
        var next = input[y][x];
        Console.WriteLine($"[{x},{y}]={next} thisLength={length}");
        switch (next) {
            case '.':
            case 'S':
                return;
            case '-':
                switch (comingFrom) {
                    case Directions.East:
                        VisitDFS(x - 1, y, Directions.East, length + 1);
                        break;
                    case Directions.West:
                        VisitDFS(x + 1, y, Directions.West, length + 1);
                        break;
                }
                break;
            case '|':
                switch (comingFrom) {
                    case Directions.North:
                        VisitDFS(x, y + 1, Directions.North, length + 1);
                        break;
                    case Directions.South:
                        VisitDFS(x, y - 1, Directions.South, length + 1);
                        break;
                }
                break;
            case 'J':
                switch (comingFrom) {
                    case Directions.North:
                        VisitDFS(x - 1, y, Directions.East, length + 1);
                        break;
                    case Directions.West:
                        VisitDFS(x, y - 1, Directions.South, length + 1);
                        break;
                }
                break;
            case 'L':
                switch (comingFrom) {
                    case Directions.North:
                        VisitDFS(x + 1, y, Directions.West, length + 1);
                        break;
                    case Directions.East:
                        VisitDFS(x, y - 1, Directions.South, length + 1);
                        break;
                }
                break;
            case '7':
                switch (comingFrom) {
                    case Directions.South:
                        VisitDFS(x - 1, y, Directions.East, length + 1);
                        break;
                    case Directions.West:
                        VisitDFS(x, y + 1, Directions.North, length + 1);
                        break;
                }
                break;
            case 'F':
                switch (comingFrom) {
                    case Directions.South:
                        VisitDFS(x + 1, y, Directions.West, length + 1);
                        break;
                    case Directions.East:
                        VisitDFS(x, y + 1, Directions.North, length + 1);
                        break;
                }
                break;
            default:
                throw new NotImplementedException();
        }
        result = Math.Max(result, length);
    }

    private static bool CanVisit(int x, int y) {
        if (y < 0 || y >= input.Length
            || x < 0 || x >= input[y].Length) {
            return false;
        }
        List<int> l;
        if (visitedCoords.TryGetValue(x, out l)) {
            if (l.Contains(y)) return false;
        }
        return true;
    }
    private static void AddVisited(int x, int y) {
        List<int> l;
        if (!visitedCoords.TryGetValue(x, out l)) {
            l = new List<int>();
            visitedCoords.Add(x, l);
        }
        l.Add(y);
    }

    private static char[][] ReadInput() {
        var lines = File.ReadAllLines("testinput.txt");
        char[][] result = new char[lines.Length][];
        for (int i = 0; i < lines.Length; i++) {
            var s = lines[i];
            var row = new char[s.Length];
            result[i] = row;
            for (int j = 0; j < s.Length; j++) {
                row[j] = s[j];
            }
        }
        return result;
    }
}
