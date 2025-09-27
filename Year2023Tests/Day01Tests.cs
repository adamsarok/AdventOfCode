using Helpers;

namespace Year2023Tests;

public class Day01Tests {
	[Fact]
	public void TestPart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 1, 1, true, new Year2023.Day01()));
	}
	[Fact]
	public void SolvePart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 1, 1, false, new Year2023.Day01()));
	}
	[Fact]
	public void TestPart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 1, 2, true, new Year2023.Day01()));
	}
	[Fact]
	public void SolvePart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 1, 2, false, new Year2023.Day01()));
	}
}
