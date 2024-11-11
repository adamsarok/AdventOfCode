using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace AdventOfCode;

public class Day17 {

    //1. starting step = no heat loss
    //2. can't go more than 3 steps in straight line
    //3. options are go straight, left, right
    //4. find minimal heat loss going from top left to bottom right corner
    public static void SolvePart1() {
        var input = File.ReadAllLines("testinput.txt");
        //1. starting step = no heat loss
        //2. can't go more than 3 steps in straight line
        //3. options are go straight, left, right
        //4. find minimal heat loss going from top left to bottom right corner

        //this is a graph we can use Dijkstra
        //first try without the 3 step rule?
        //implement with priority queue
        var d = new Dijkstra(input);
        d.Solve();
    }
    public class Dijkstra(string[] input) {
        int[,] distances;
        bool[,] visited;
        PriorityQueue<Vertex, int> priorityQueue;
        enum Directions { Left, Right, Up, Down, None }
        record struct Vertex(int x, int y, int cost, Directions lastDir, int dirCount) { }
        public void Solve() {
            distances = new int[input.Length, input[0].Length];
            visited = new bool[input.Length, input[0].Length];
            for (int x = 0; x < input[0].Length; x++) {
                for (int y = 0; y < input.Length; y++) {
                    distances[y, x] = int.MaxValue;
                }
            }
            priorityQueue = new PriorityQueue<Vertex, int>();
            priorityQueue.Enqueue(new Vertex(0, 0, 0, Directions.None, 0), 0);
            long cnt = 0;
            while (priorityQueue.Count > 0) {
                cnt++;
                var next = priorityQueue.Dequeue();
                visited[next.y, next.x] = true;
                //Console.WriteLine($"Visiting {next.x}:{next.y}");
                //if (IsDone(ref cnt)) break;
                if (next.lastDir != Directions.Down) TryQueue(next, Directions.Up);
                if (next.lastDir != Directions.Up) TryQueue(next, Directions.Down);
                if (next.lastDir != Directions.Right) TryQueue(next, Directions.Left);
                if (next.lastDir != Directions.Left) TryQueue(next, Directions.Right);
            }
            for (int y = 0; y < input.Length; ++y) {
                for (int x = 0; x < input[0].Length; x++) {
                    Console.Write(distances[y, x] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine(distances[input.Length - 1, input[0].Length - 1]);
            Console.WriteLine($"in steps: {cnt}");
        }

        private bool IsDone(ref int cnt) {
            //works on small input
            //something is not right because we go extremely slow on the big input
            //queue becomes gigantic. up until 40 rows we are okay, if the input is larger than this we slow down
            //I think we can still turn back, that is the problem?
            //when I try to optimize, I don't get the corret result... I'm missing something
            cnt++;
            for (int y = 0; y < input.Length; y++) {
                for (int x = 0; x < input[0].Length; x++) {
                    if (!visited[y, x]) {
                        if (cnt % 1000000 == 0) {
                            Console.WriteLine($"{x},{y}");
                            cnt = 0;
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        private void TryQueue(Vertex prev, Directions dir) {
            int x = prev.x, y = prev.y;
            int dirCount = prev.lastDir == dir ? prev.dirCount + 1 : 1;
            if (dirCount > 3) {
                return;
            }
            switch (dir) {
                case Directions.Left:
                    x--;
                    break;
                case Directions.Right:
                    x++;
                    break;
                case Directions.Up:
                    y--;
                    break;
                case Directions.Down:
                    y++;
                    break;
            }
            if (x < 0 || y < 0 ||
                x >= input[0].Length || y >= input.Length) return;
            var costTo = int.Parse((input[y][x]).ToString()) + prev.cost;
            if (costTo < distances[y, x]) distances[y, x] = costTo;
            if (visited[y, x]) return;
            priorityQueue.Enqueue(new Vertex(x, y, costTo, dir, dirCount), costTo);
        }
    }
}
