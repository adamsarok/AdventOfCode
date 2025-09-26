using Helpers;

namespace Year2024Tests;

public class Day02 {
	[Fact]
	public void TestPart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2024, 2, 1, true, new Year2024.Day2()));
	}
	[Fact]
	public void SolvePart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2024, 2, 1, false, new Year2024.Day2()));
	}
	[Fact]
	public void TestPart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2024, 2, 2, true, new Year2024.Day2()));
	}
	[Fact]
	public void SolvePart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2024, 2, 2, false, new Year2024.Day2()));
	}
}
