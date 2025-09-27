using System.Xml.Linq;

namespace Codegen {
	public class CodeGen {

		[Theory]
		[InlineData(2025)]
		public void GenerateYear(int year) {
			for (int i = 1; i <= 25; i++) {
				string baseDir = AppContext.BaseDirectory;
				string sourceDir = Path.Combine(baseDir, "..", "..", "..", "..");
				var dir = Path.Combine(sourceDir, $"Year{year}", $"Day{i.ToString("00")}");
				if (!Path.Exists(dir)) Directory.CreateDirectory(dir);
				string csfilePath = Path.Combine(dir, $"Day{i.ToString("00")}.cs");
				if (!File.Exists(csfilePath)) {
					var code = GetSolverTemplate(year, i);
					File.WriteAllText(Path.Combine(dir, $"Day{i.ToString("00")}.cs"), code);
				}
				AddTxts(dir, year, i);
				var testDir = Path.Combine(sourceDir, $"Year{year}Tests");
				string testFilePath = Path.Combine(testDir, $"Day{i.ToString("00")}Tests.cs");
				if (!Path.Exists(testDir)) Directory.CreateDirectory(testDir);
				if (!File.Exists(testFilePath)) {
					var code = GetTestTemplate(year, i);
					File.WriteAllText(Path.Combine(testDir, $"Day{i.ToString("00")}Tests.cs"), code);
				}
			}
		}

		private void AddTxts(string dir, int year, int day) {
			string daystr = day.ToString("00");
			var file1 = Path.Combine(dir, $"{year}shortinput{daystr}.txt");
			var file2 = Path.Combine(dir, $"{year}input{daystr}.txt");
			if (File.Exists(file1) || File.Exists(file2)) return;
			File.Create(file1).Close();
			File.Create(file2).Close();
			WriteCSProj(dir, year, daystr, file1, file2);
			WriteTestCSProj(dir, year, daystr, file1, file2);
		}

		private static void WriteTestCSProj(string dir, int year, string daystr, string file1, string file2) {
			string csprojPath = Path.Combine(dir, "..", $"Year{year}.csproj");

			XDocument csproj = XDocument.Load(csprojPath);
			XElement itemGroup = csproj.Root.Elements("ItemGroup").FirstOrDefault();

			if (itemGroup == null) {
				itemGroup = new XElement("ItemGroup");
				csproj.Root.Add(itemGroup);
			}

			foreach (var file in new[] { file1, file2 }) {
				var path = Path.Combine($"Day{daystr}\\{Path.GetFileName(file)}");
				XElement noneElement = new XElement("None",
					new XAttribute("Update", path),
					new XElement("CopyToOutputDirectory", "Always")
				);
				itemGroup.Add(noneElement);
			}

			csproj.Save(csprojPath);
		}

		private static void WriteCSProj(string dir, int year, string daystr, string file1, string file2) {
			string csprojPath = Path.Combine(dir, "..", $"Year{year}.csproj");

			XDocument csproj = XDocument.Load(csprojPath);
			XElement itemGroup = csproj.Root.Elements("ItemGroup").FirstOrDefault();

			if (itemGroup == null) {
				itemGroup = new XElement("ItemGroup");
				csproj.Root.Add(itemGroup);
			}

			foreach (var file in new[] { file1, file2 }) {
				var path = Path.Combine($"Day{daystr}\\{Path.GetFileName(file)}");
				XElement noneElement = new XElement("None",
					new XAttribute("Update", path),
					new XElement("CopyToOutputDirectory", "Always")
				);
				itemGroup.Add(noneElement);
			}

			csproj.Save(csprojPath);
		}

		private string GetSolverTemplate(int year, int day) {
			var dayStr = day.ToString("00");
			return $@"using Helpers;
namespace Year{year};
public class Day{dayStr} : IAocSolver {{
	public long SolvePart1(string[] input) {{
		return 0;
	}}
	public long SolvePart2(string[] input) {{
		return 0;
	}}
}}
";
		}

		private string GetTestTemplate(int year, int day) {
			var dayStr = day.ToString("00");
			return $@"
using Helpers;
using Xunit.Abstractions;

namespace Year{year}Tests;

public class Day{dayStr}Tests(ITestOutputHelper output) : TestBase(output) {{
	[Fact]
	public void TestPart1() {{
		Solve(new SolverParams({year}, {day}, 1, true, new Year{year}.Day{dayStr}()));
	}}
	[Fact]
	public void SolvePart1() {{
		Solve(new SolverParams({year}, {day}, 1, false, new Year{year}.Day{dayStr}()));
	}}
	[Fact]
	public void TestPart2() {{
		Solve(new SolverParams({year}, {day}, 2, true, new Year{year}.Day{dayStr}()));
	}}
	[Fact]
	public void SolvePart2() {{
		Solve(new SolverParams({year}, {day}, 2, false, new Year{year}.Day{dayStr}()));
	}}
}}
";
		}
	}
}
