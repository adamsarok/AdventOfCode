
using Helpers;
using Xunit.Abstractions;

namespace Year2023Tests;

public class Day09Tests(ITestOutputHelper output) : TestBase(output) {
	[Fact]
	public void TestPart1() {
		Solve(new SolverParams(2023, 9, 1, true, new Year2023.Day09()));
	}
	[Fact]
	public void SolvePart1() {
		Solve(new SolverParams(2023, 9, 1, false, new Year2023.Day09()));
	}
	[Fact]
	public void TestPart2() {
		Solve(new SolverParams(2023, 9, 2, true, new Year2023.Day09()));
	}
	[Fact]
	public void SolvePart2() {
		Solve(new SolverParams(2023, 9, 2, false, new Year2023.Day09()));
	}
}
