
using Helpers;
using Xunit.Abstractions;

namespace Year2025Tests;

public class Day14Tests(ITestOutputHelper output) : TestBase(output) {
	[Fact]
	public void TestPart1() {
		Solve(new SolverParams(2025, 14, 1, true, new Year2025.Day14()));
	}
	[Fact]
	public void SolvePart1() {
		Solve(new SolverParams(2025, 14, 1, false, new Year2025.Day14()));
	}
	[Fact]
	public void TestPart2() {
		Solve(new SolverParams(2025, 14, 2, true, new Year2025.Day14()));
	}
	[Fact]
	public void SolvePart2() {
		Solve(new SolverParams(2025, 14, 2, false, new Year2025.Day14()));
	}
}
