using System;
using System.Collections;
using System.Data;
using System.Diagnostics;

namespace AdventOfCode;

public class Day18 {
	public static void SolvePart1() {
        // var input = ReadFile("shortinput.txt");
        // var s = new Solver(input, true);
        // s.Solve();
        var input2 = ReadFile("testinput.txt"); 
        var s2 = new Solver(input2, true);
        s2.Solve();
    }
    public enum Directions { R, D, L, U }
    record struct Input(Directions dir, int length, string color) { }

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

    class Solver(List<Input> inputs, bool doPrint) {
        char[,] dig;
        int height, width;
        private int xInside, yInside;
        private int circumference = 0;
        public void Solve() {
	        Stopwatch sw = Stopwatch.StartNew();
            InitMatrix();
            Dig();
            Shrink();
            if (doPrint) PrintOut();
            Fill();
            if (doPrint) PrintOut();
            Calc();
            sw.Stop();
            Console.WriteLine($"Solved in {sw.ElapsedMilliseconds} ms");
        }

        private void Shrink() { //I could grow a list or array during digging instead?
	        height = maxY - minY + 3;
	        width = maxX - minX + 3;
	        var copy = new char[height, width];
	        int copyY = 0;
	        for (int y = minY - 1; y <= maxY + 1; y++) {
		        int copyX = 0;
		        for (int x = minX - 1; x <= maxX + 1; x++) {
			        copy[copyY, copyX] = dig[y, x];
			        copyX++;
		        }
		        copyY++;
	        }
	        dig = copy;
        }

        private void Calc() {
	        int cnt = 0;
	        for (int x = 0; x < width; x++) {
		        for (int y = 0; y < height; y++) {
			        if (dig[y, x] != 'X') cnt++;
		        }
	        }
	        Console.WriteLine(cnt);
        }
		record struct Vertex(int x, int y) {}
        private void Fill() {
	        //1. find a pixel suspected to be inside
	        //2. raycast, if inside start fill - fail
	        
	        //just a simple recursive fill - fill all outside than whats left is what we want
	        //since we grew the matrix +1 in all direction, we can nicely flood fill the outside

	        var q = new Queue<Vertex>();
	        q.Enqueue(new Vertex(0, 0));
	        Vertex v;
	        while (q.TryDequeue(out v)) {
		        dig[v.y, v.x] = 'X';
		        TryQueue(q, v.x, v.y + 1);
		        TryQueue(q, v.x, v.y - 1);
		        TryQueue(q, v.x + 1, v.y);
		        TryQueue(q, v.x - 1, v.y);
	        }
	        
	        // bool startRaycast = false;
	        // for (int x = 0; x < width; x++) {
	        //  for (int y = 0; y < height; y++) {
	        //   if (dig[y, x] != '#' && IsInside(x, y)) {
	        //    dig[y, x] = 'F';
	        //   }
	        //  }
	        // }
        }

        private void TryQueue(Queue<Vertex> q, int x, int y) {
	        if (x >= 0 && x < width && y >= 0 && y < height && dig[y, x] == '.') {
		        q.Enqueue(new Vertex(x,y));
	        }
        }

        private void InitMatrix() {
	        //sizing the matrix - horizontal = cnt(L + R), vertical(D + U) bigger but ok
	        height = inputs.Where(x => x.dir == Directions.U || x.dir == Directions.D).Sum(x => x.length) * 2;
	        width = inputs.Where(x => x.dir == Directions.L || x.dir == Directions.R).Sum(x => x.length) * 2;
	        dig = new char[height, width];
	        for (int y = 0; y < height; y++) {
		        for (int x = 0; x < width; x++) {
			        dig[y, x] = '.';
		        }
	        }
        }
		private void Dig() {
			int y = (int)height / 2;
	        int x = (int)width / 2;
	        dig[y, x] = '#';
	        foreach (var input in inputs) {
		        switch (input.dir) {
			        case Directions.U: Mark(ref x, ref y, dirX: 0, dirY: -1, length: input.length); break;
			        case Directions.D: Mark(ref x, ref y, dirX: 0, dirY: 1, length: input.length); break;
			        case Directions.L: Mark(ref x, ref y, dirX: -1, dirY: 0, length: input.length); break;
			        case Directions.R: Mark(ref x, ref y, dirX: 1, dirY: 0, length: input.length); break;
		        }
	        }
        }

		/*
		private bool IsInside(int x, int y) {
            //raycast - even=outside, odd=inside
            //TODO: not correct this case fails to detect as inside:
			// .....###
			// ###..#F#
            return RayCast(x, y, 0, 1)
                    && RayCast(x, y, 0, -1)
                    && RayCast(x, y, 1, 0)
                    && RayCast(x, y, -1, 0);
        }

        private bool RayCast(int x, int y, int xDir, int yDir) {
	        int intersects = 0;
	        bool isIntersection = false;
	        int actX = x;
	        int actY = y;
	        while (actX >= 0 && actX < width && actY >= 0 && actY <= height) {
		        if (isIntersection && dig[actY, actX] != '#') {
			        intersects++;
			        isIntersection = false;
		        }
		        else if (dig[actY, actX] == '#') isIntersection = true;

		        actX += xDir;
		        actX += yDir;
	        }
	        return intersects % 2 == 1;
        }
		*/
        private void PrintOut() { 
	        using (var sw = new StreamWriter(new FileStream("output.txt", FileMode.OpenOrCreate))) {
		        for (int y = 0; y < height; y++) {
			        for (int x = 0; x < width; x++) {
				        sw.Write(dig[y, x]);
				        //Console.Write(dig[y, x]);
			        }
			        sw.WriteLine();
			        //Console.WriteLine("");
		        }
	        }
        }

        private int minX = Int32.MaxValue, minY = Int32.MaxValue, maxX = 0, maxY = 0;
        private void Mark(ref int x, ref int y, int dirX, int dirY, int length) {
            for (int i = 0; i < length; i++) {
                y += dirY;
                x += dirX;
                dig[y, x] = '#';
                minX = Math.Min(x, minX);
                minY = Math.Min(y, minY);
                maxX = Math.Max(x, maxX);
                maxY = Math.Max(y, maxY);
            }
        }
    }

}
