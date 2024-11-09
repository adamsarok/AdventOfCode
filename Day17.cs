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

        //this is a graph we can use Dijkstra on?
    }
}
