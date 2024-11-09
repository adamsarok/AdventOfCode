using System;

namespace AdventOfCode;

public class Day17
{
    
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
    }

    public class Dijkstra(List<string> input) {
        int[,] distances;
        // public int Solve() {
        //     distances = new int[input[0].Length, input.Count];
        //     for (int x = 0; x < input[0].Length) {
        //         for (int y = 0; y < input.Count; y++) {
        //             distances[x, y] = int.MaxValue;
        //         }
        //     }
        // }
    }
}
