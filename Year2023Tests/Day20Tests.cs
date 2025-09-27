using Helpers;

namespace Year2023Tests;

public class Day20Tests {
	[Fact]
	public void TestPart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 20, 1, true, new Year2023.Day20()));
	}
	[Fact]
	public void SolvePart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 20, 1, false, new Year2023.Day20()));
	}
	[Fact]
	public void TestPart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 20, 2, true, new Year2023.Day20()));
	}
	[Fact]
	public void SolvePart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 20, 2, false, new Year2023.Day20()));
	}
}
