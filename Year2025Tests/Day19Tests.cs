using Helpers;

namespace Year2025Tests;

public class Day19Tests {
	[Fact]
	public void TestPart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2025, 19, 1, true, new Year2025.Day19()));
	}
	[Fact]
	public void SolvePart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2025, 19, 1, false, new Year2025.Day19()));
	}
	[Fact]
	public void TestPart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2025, 19, 2, true, new Year2025.Day19()));
	}
	[Fact]
	public void SolvePart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2025, 19, 2, false, new Year2025.Day19()));
	}
}
