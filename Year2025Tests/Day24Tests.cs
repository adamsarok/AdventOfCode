using Helpers;

namespace Year2025Tests;

public class Day24Tests {
	[Fact]
	public void TestPart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2025, 24, 1, true, new Year2025.Day24()));
	}
	[Fact]
	public void SolvePart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2025, 24, 1, false, new Year2025.Day24()));
	}
	[Fact]
	public void TestPart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2025, 24, 2, true, new Year2025.Day24()));
	}
	[Fact]
	public void SolvePart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2025, 24, 2, false, new Year2025.Day24()));
	}
}
