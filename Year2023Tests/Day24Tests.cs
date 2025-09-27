using Helpers;
using Xunit;
using Year2023;
namespace Year2023Tests;
public class Day24Tests {
	[Fact]
	public void TestPart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 24, 1, true, new Day24()));
	}
	[Fact]
	public void SolvePart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 24, 1, false, new Day24()));
	}
	[Fact]
	public void TestPart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 24, 2, true, new Day24()));
	}
	[Fact]
	public void SolvePart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 24, 2, false, new Day24()));
	}
}
