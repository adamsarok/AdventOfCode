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
        // var input2 = ReadFile("testinput.txt"); 
        // var s2 = new Solver(input2, true);
        // s2.Solve();
        var input = ReadFile("shortinput.txt");
        var s = new SolverShoeLace(input);
        s.Solve();
        var input2 = ReadFile("testinput.txt");
        var s2 = new SolverShoeLace(input2);
        s2.Solve();
    }
	public static void SolvePart2() {
		var input = ReadFilePart2("shortinput.txt");
		var s = new SolverShoeLace(input);
		s.Solve();
		var input2 = ReadFilePart2("testinput.txt"); 
		var s2 = new SolverShoeLace(input2);
		s2.Solve();
	}

	class SolverShoeLace(List<Input> inputs) {
		//fill is too slow for this giant matrix - apply shoelace method
		//1. convert the matrix into a clockwise collection of vertices in coordinate system
		//current [y,x] is different than matrix [x,y]
		//apply shoelace method to calc
		List<Vertex> vertices = new List<Vertex>();
		private int circumference = 0;
		public void Solve() {
			Stopwatch sw = Stopwatch.StartNew();
			vertices = new List<Vertex> { new Vertex(0, 0) };
			foreach (var input in inputs) {
				var last = vertices.Last();
				circumference += input.length;
				switch (input.dir) { 
					case Directions.U: 
						vertices.Add(new Vertex(last.x, last.y + input.length));
						break;
					case Directions.D: 
						vertices.Add(new Vertex(last.x, last.y - input.length));
						break;
					case Directions.L: 
						vertices.Add(new Vertex(last.x - input.length, last.y));
						break;
					case Directions.R: 
						vertices.Add(new Vertex(last.x + input.length, last.y));
						break;
				}
			}
			// foreach (var vertex in vertices) {
			// 	Console.WriteLine($"[{vertex.x},{vertex.y}]");
			// }
			Calc();
			sw.Stop();
			Console.WriteLine($"Solved in {sw.ElapsedMilliseconds} ms");
		}

		private void Calc() {
			long a = 0;
			//this calc works for Part1 but not Part2... how is this possible?
			//the input reading for Part2 seems correct?
			for (int i = 0; i < vertices.Count; i++)
			{
				Vertex current = vertices[i];
				Vertex next = vertices[(i + 1) % vertices.Count];
				a += (current.x * next.y) - (current.y * next.x);
			}
			var area = Math.Abs(a) / 2.0;
			Console.WriteLine((int)(Math.Abs(area) - 0.5 * circumference + 1) + circumference);
		}
	}
	
    public enum Directions { R, D, L, U }
    record struct Input(Directions dir, int length) { }

    private static List<Input> ReadFile(string fileName) {
        var file = File.ReadAllLines(fileName);
        List<Input> result = new List<Input>();
        foreach (var line in file) {
            var s = line.Split(' ');
            var input = new Input(dir: Enum.Parse<Directions>(s[0]), length: int.Parse(s[1]));
            result.Add(input);
        }
        return result;
    }
    
    private static List<Input> ReadFilePart2(string fileName) {
	    var file = File.ReadAllLines(fileName);
	    List<Input> result = new List<Input>();
	    foreach (var line in file) {
		    var s = line.Split(' ');
		    var hex = s[2].Substring(2, 5);
		    var dir = s[2].Substring(7, 1);
		    var input = new Input(
			    dir: Enum.Parse<Directions>(dir), 
			    length: int.Parse(hex, System.Globalization.NumberStyles.HexNumber));
		    result.Add(input);
	    }
	    return result;
    }


    record struct Vertex(int x, int y) {}
    
    class Solver(List<Input> inputs, bool doPrint) {
        char[,] dig;
        int height, width;
        private int xInside, yInside;
        private int circumference = 0;
        private int minX = Int32.MaxValue, minY = Int32.MaxValue, maxX = 0, maxY = 0;

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

        private void Shrink() {
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
        private void Fill() {
	        //just a simple recursive fill - fill all outside than whats left is what we want
	        //since we grew the matrix +1 in all direction, we can nicely flood fill the outside

	        var q = new Queue<Vertex>();
	        dig[0, 0] = 'X';
	        q.Enqueue(new Vertex(0, 0));
	        Vertex v;
	        while (q.TryDequeue(out v)) {
		        TryQueue(q, v.x, v.y + 1);
		        TryQueue(q, v.x, v.y - 1);
		        TryQueue(q, v.x + 1, v.y);
		        TryQueue(q, v.x - 1, v.y);
	        }
        }

        private void TryQueue(Queue<Vertex> q, int x, int y) {
	        if (x >= 0 && x < width && y >= 0 && y < height && dig[y, x] == '.') {
		        dig[y, x] = 'X';
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
		        circumference += input.length;
		        switch (input.dir) {
			        case Directions.U: Mark(ref x, ref y, dirX: 0, dirY: -1, length: input.length); break;
			        case Directions.D: Mark(ref x, ref y, dirX: 0, dirY: 1, length: input.length); break;
			        case Directions.L: Mark(ref x, ref y, dirX: -1, dirY: 0, length: input.length); break;
			        case Directions.R: Mark(ref x, ref y, dirX: 1, dirY: 0, length: input.length); break;
		        }
	        }
        }
		
        private void PrintOut() { 
	        using (var sw = new StreamWriter(new FileStream("output.txt", FileMode.OpenOrCreate))) {
		        for (int y = 0; y < height; y++) {
			        for (int x = 0; x < width; x++) {
				        sw.Write(dig[y, x]);
			        }
			        sw.WriteLine();
		        }
	        }
        }
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
