using Helpers;

namespace Year2023Tests;

public class Day10Tests {
	[Fact]
	public void TestPart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 10, 1, true, new Year2023.Day10()));
	}
	[Fact]
	public void SolvePart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 10, 1, false, new Year2023.Day10()));
	}
	[Fact]
	public void TestPart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 10, 2, true, new Year2023.Day10()));
	}
	[Fact]
	public void SolvePart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 10, 2, false, new Year2023.Day10()));
	}
}
