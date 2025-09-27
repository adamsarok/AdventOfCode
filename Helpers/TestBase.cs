using Xunit.Abstractions;

namespace Helpers;
public class TestBase(ITestOutputHelper output) {
	protected record SolverParams(int Year, int Day, int Part, bool IsTest, IAocSolver Solver);
	protected void Solve(SolverParams solverParams) {
		string daystr = solverParams.Day.ToString("00");
		string inputFileName = solverParams.IsTest ? $"Day{daystr}\\{solverParams.Year}shortinput{daystr}.txt" : $"Day{daystr}\\{solverParams.Year}input{daystr}.txt";
		string[] inputLines = File.ReadAllLines(inputFileName);
		output.WriteLine($"=== Day {solverParams.Day}, Year {solverParams.Year} ===");
		var r = solverParams.Part == 1 ? solverParams.Solver.SolvePart1(inputLines) : solverParams.Solver.SolvePart2(inputLines);
		output.WriteLine($"Part{solverParams.Day} {(solverParams.IsTest ? "Test" : "Live")}: {r}");
	}
}
