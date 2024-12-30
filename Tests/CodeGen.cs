using System.Xml.Linq;

namespace Tests {
	public class CodeGen {
		[Fact]
		public void GenerateYear() {
			int year = 2019;
			for (int i = 1; i <= 25; i++) {
				string baseDir = AppContext.BaseDirectory;
				string sourceDir = Path.Combine(baseDir, "..", "..", "..", "..");
				var dir = Path.Combine(sourceDir, $"Year{year}", $"Day{i.ToString("00")}");
				if (!Path.Exists(dir)) Directory.CreateDirectory(dir);
				string csfilePath = Path.Combine(dir, $"Day{i.ToString("00")}.cs");
				if (!File.Exists(csfilePath)) {
					var code = GetTemplate(year, i);
					File.WriteAllText(Path.Combine(dir, $"Day{i.ToString("00")}.cs"), code);
				}
				AddTxts(dir, year, i);
			}
		}

		private void AddTxts(string dir, int year, int day) {
			string daystr = day.ToString("00");
			var file1 = Path.Combine(dir, $"{year}shortinput{daystr}.txt");
			var file2 = Path.Combine(dir, $"{year}input{daystr}.txt");
			if (File.Exists(file1) || File.Exists(file2)) return;
			File.Create(file1).Close();
			File.Create(file2).Close();
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


		private string GetTemplate(int year, int day) {
			var dayStr = day.ToString("00");
			return $@"using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year{year}.Day{dayStr} {{
	public class Day{dayStr} : Solver {{
		public Day{dayStr}() : base({year}, {day}) {{
		}}
		protected override void ReadInputPart1(string fileName) {{
			base.ReadInputPart1(fileName);
			//input = new();
			foreach (var l in File.ReadAllLines(fileName)) {{

			}}
		}}

		protected override void ReadInputPart2(string fileName) {{
			base.ReadInputPart2(fileName);
			//input = new();
			foreach (var l in File.ReadAllLines(fileName)) {{

			}}
		}}

		protected override long SolvePart1() {{
			base.SolvePart1(fileName);
			long result = 0;

			return result;
		}}

		protected override long SolvePart2() {{
			base.SolvePart2(fileName);
			long result = 0;

			return result;
		}}
	}}
}}
";
		}
	}
}
