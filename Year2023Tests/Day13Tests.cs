using Helpers;

namespace Year2023Tests;

public class Day13Tests {
	[Fact]
	public void TestPart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 13, 1, true, new Year2023.Day13()));
	}
	[Fact]
	public void SolvePart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 13, 1, false, new Year2023.Day13()));
	}
	[Fact]
	public void TestPart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 13, 2, true, new Year2023.Day13()));
	}
	[Fact]
	public void SolvePart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 13, 2, false, new Year2023.Day13()));
	}
}
