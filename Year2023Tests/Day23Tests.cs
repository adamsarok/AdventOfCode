using Helpers;
using Xunit;
using Year2023;
namespace Year2023Tests;
public class Day23Tests {
	[Fact]
	public void TestPart1() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 23, 1, true, new Day23()));
	}
	[Fact]
	public void TestPart1_Release() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 23, 1, false, new Day23()));
	}
	[Fact]
	public void TestPart2() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 23, 2, true, new Day23()));
	}
	[Fact]
	public void TestPart2_Release() {
		AocHelper.Solve(new AocHelper.SolverParams(2023, 23, 2, false, new Day23()));
	}
}
