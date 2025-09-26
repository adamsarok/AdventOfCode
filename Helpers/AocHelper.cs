using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Helpers {
    public static class AocHelper { 
		public record SolverParams(int Year, int Day, int Part, bool IsTest, IAocSolver Solver);
		public static void Solve(SolverParams solverParams) {
			string daystr = solverParams.Day.ToString("00");
			string inputFileName = solverParams.IsTest ? $"Day{daystr}\\{solverParams.Year}shortinput{daystr}.txt" : $"Day{daystr}\\{solverParams.Year}input{daystr}.txt";
			string[] inputLines = File.ReadAllLines(inputFileName);
			Console.WriteLine($"=== Day {solverParams.Day}, Year {solverParams.Year} ===");
			var r = solverParams.Part == 1 ? solverParams.Solver.SolvePart1(inputLines) : solverParams.Solver.SolvePart2(inputLines);
			Console.WriteLine($"Part{solverParams.Day} {(solverParams.IsTest ? "Test" : "Live")}: {r}");
		}
    }
}