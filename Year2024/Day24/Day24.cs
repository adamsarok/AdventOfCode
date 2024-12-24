using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2024.Day24 {
	public class Day24 : Solver {
		public Day24() : base(2024, 24) {
		}
		class Gate(Dictionary<string, Gate> circuit, 
			string inputGate1, string inputGate2, string name) {
			public string Name => name;
			protected Gate? InputGate1 { get => circuit[inputGate1]; }
			protected Gate? InputGate2 { get => circuit[inputGate2]; }
			public virtual bool Output { get; set; }
		}
		Dictionary<string, Gate> Circuit { get; set; }
		class StarterWire : Gate {
			public StarterWire(Dictionary<string, Gate> circuit,
				string inputGate1, string inputGate2, string name, bool init) 
				: base(circuit, inputGate1, inputGate2, name) {
				Output = init;
			}
		}
		class AND(Dictionary<string, Gate> circuit,
			string inputGate1, string inputGate2, string name) : Gate(circuit, inputGate1, inputGate2, name) {
			public override bool Output {
				get => InputGate1.Output && InputGate2.Output;
				set => throw new Oopsie("cant set gate out directly");
			}
		}
		class OR(Dictionary<string, Gate> circuit,
			string inputGate1, string inputGate2, string name) : Gate(circuit, inputGate1, inputGate2, name) { 
			public override bool Output {
				get => InputGate1.Output || InputGate2.Output;
				set => throw new Oopsie("cant set gate out directly");
			}
		}
		class XOR(Dictionary<string, Gate> circuit,
			string inputGate1, string inputGate2, string name) : Gate(circuit, inputGate1, inputGate2, name) { 
			public override bool Output {
				get => InputGate1.Output ^ InputGate2.Output;
				set => throw new Oopsie("cant set gate out directly");
			}
		}

		protected override void ReadInputPart1(string fileName) {
			//input = new();
			Circuit = new Dictionary<string, Gate>();
			bool starterWires = true;
			foreach (var l in File.ReadAllLines(fileName)) {
				if (starterWires) {
					if (string.IsNullOrWhiteSpace(l)) {
						starterWires = false;
					} else {
						var s = l.Split(':');
						var g = new StarterWire(Circuit, "", "", s[0], s[1].Trim() == "1" ? true : false);
						Circuit.Add(g.Name, g);
					}
				} else {
					var s = l.Split(' ');
					Gate g;
					switch (s[1]) {
						case "AND":
							g = new AND(Circuit, s[0], s[2], s[4]);
							break;
						case "XOR":
							g = new XOR(Circuit, s[0], s[2], s[4]);
							break;
						case "OR":
							g = new OR(Circuit, s[0], s[2], s[4]);
							break;
						default: throw new Oopsie("Unknown gate");
					}
					Circuit.Add(g.Name, g);
				}
			} 
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			int pow = 0;
			var zGates = Circuit
				.Values
				.Where(x => x.Name.StartsWith("z"))
				.OrderBy(x => x.Name);
			foreach (var g in zGates) {
				//Console.WriteLine($"{g.Name}:{g.Output}");
				if (g.Output) result += (long)Math.Pow(2, pow);
				pow++;
			}
			return result;
		}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}
	}
}
