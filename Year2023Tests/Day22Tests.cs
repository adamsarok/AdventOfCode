using Helpers;
using Xunit;
using Year2023;
namespace Year2023Tests;
public class Day22Tests {
	[Fact]
	public void TestPart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 22, 1, true, new Day22()));
	}
	[Fact]
	public void SolvePart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 22, 1, false, new Day22()));
	}
	[Fact]
	public void TestPart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 22, 2, true, new Day22()));
	}
	[Fact]
	public void SolvePart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 22, 2, false, new Day22()));
	}
}
