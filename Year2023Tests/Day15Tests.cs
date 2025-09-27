
using Helpers;
using Xunit.Abstractions;

namespace Year2023Tests;

public class Day15Tests(ITestOutputHelper output) : TestBase(output) {
	[Fact]
	public void TestPart1() {
		Solve(new SolverParams(2023, 15, 1, true, new Year2023.Day15()));
	}
	[Fact]
	public void SolvePart1() {
		Solve(new SolverParams(2023, 15, 1, false, new Year2023.Day15()));
	}
	[Fact]
	public void TestPart2() {
		Solve(new SolverParams(2023, 15, 2, true, new Year2023.Day15()));
	}
	[Fact]
	public void SolvePart2() {
		Solve(new SolverParams(2023, 15, 2, false, new Year2023.Day15()));
	}
}
