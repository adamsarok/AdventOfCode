using Helpers;

namespace Year2023Tests;

public class Day12Tests {
	[Fact]
	public void TestPart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 12, 1, true, new Year2023.Day12()));
	}
	[Fact]
	public void SolvePart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 12, 1, false, new Year2023.Day12()));
	}
	[Fact]
	public void TestPart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 12, 2, true, new Year2023.Day12()));
	}
	[Fact]
	public void SolvePart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 12, 2, false, new Year2023.Day12()));
	}
}
