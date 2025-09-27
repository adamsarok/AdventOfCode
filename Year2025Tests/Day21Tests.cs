
using Helpers;
using Xunit.Abstractions;

namespace Year2025Tests;

public class Day21Tests(ITestOutputHelper output) : TestBase(output) {
	[Fact]
	public void TestPart1() {
		Solve(new SolverParams(2025, 21, 1, true, new Year2025.Day21()));
	}
	[Fact]
	public void SolvePart1() {
		Solve(new SolverParams(2025, 21, 1, false, new Year2025.Day21()));
	}
	[Fact]
	public void TestPart2() {
		Solve(new SolverParams(2025, 21, 2, true, new Year2025.Day21()));
	}
	[Fact]
	public void SolvePart2() {
		Solve(new SolverParams(2025, 21, 2, false, new Year2025.Day21()));
	}
}
