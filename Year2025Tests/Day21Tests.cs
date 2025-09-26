using Helpers;

namespace Year2025Tests;

public class Day21Tests {
	[Fact]
	public void TestPart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2025, 21, 1, true, new Year2025.Day21()));
	}
	[Fact]
	public void SolvePart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2025, 21, 1, false, new Year2025.Day21()));
	}
	[Fact]
	public void TestPart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2025, 21, 2, true, new Year2025.Day21()));
	}
	[Fact]
	public void SolvePart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2025, 21, 2, false, new Year2025.Day21()));
	}
}
