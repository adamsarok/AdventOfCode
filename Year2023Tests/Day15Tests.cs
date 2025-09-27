using Helpers;

namespace Year2023Tests;

public class Day15Tests {
	[Fact]
	public void TestPart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 15, 1, true, new Year2023.Day15()));
	}
	[Fact]
	public void SolvePart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 15, 1, false, new Year2023.Day15()));
	}
	[Fact]
	public void TestPart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 15, 2, true, new Year2023.Day15()));
	}
	[Fact]
	public void SolvePart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 15, 2, false, new Year2023.Day15()));
	}
}
