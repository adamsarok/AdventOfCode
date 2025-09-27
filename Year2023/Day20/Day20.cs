using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2023 {
	public class Day20 : IAocSolver {
		public long SolvePart1(string[] input) {
			long result = 0;

			Circuit circuit = new Circuit();
			foreach (var l in input) {
				var s = l.Split("->");
				var m = s[0].Trim();
				var connected = s[1].Split(",");
				Module module;
				switch (m[0]) {
					case 'b':
						module = new Broadcaster(m, circuit);
						break;
					case '%':
						module = new FlipFlop(m.Substring(1), circuit);
						break;
					case '&':
						module = new Conjunction(m.Substring(1), circuit);
						break;
					default: throw new Exception("module parse failed");
				}
				foreach (var c in connected) {
					module.ConnectedModuleNames.Add(c.Trim());
				}
				circuit.Modules.Add(module);
			}
			foreach (var module in circuit.Modules) {
				foreach (var mn in module.ConnectedModuleNames) {
					module.ConnectedModules.Add(circuit.Modules.FirstOrDefault(x => x.ModuleName == mn));
				}
			}
			var b = circuit.Modules.FirstOrDefault(x => x.ModuleName == "broadcaster");
			circuit.Enqueue(b, false);
			circuit.ProcessQueue();
			//TODO: not correct

			return result;
		}

		public long SolvePart2(string[] input) {
			return 0;
		}

		public class Circuit {
			private Queue<(Module, bool)> messageQueue = new();
			public List<Module> Modules = new();
			public void Enqueue(Module module, bool pulse) {
				messageQueue.Enqueue((module, pulse));
			}

			public void ProcessQueue() {
				(Module, bool) next;
				while (messageQueue.TryDequeue(out next)) {
					next.Item1.ReceivePulse(next.Item2);
				}
			}
		}
		public abstract class Module {
			protected readonly Circuit circuit;
			public Module(string moduleName, Circuit circuit) {
				this.circuit = circuit;
				ModuleName = moduleName;
			}
			public string ModuleName { get; set; }
			public bool State { get; set; } = false;
			public abstract void ReceivePulse(bool pulse);
			public List<Module> ConnectedModules { get; set; } = new List<Module>();
			public List<string> ConnectedModuleNames { get; set; } = new List<string>();
		}
		public class FlipFlop : Module {
			public FlipFlop(string moduleName, Circuit circuit) : base(moduleName, circuit) { }
			public override void ReceivePulse(bool pulse) {
				if (!pulse) {
					State = !State;
					Console.WriteLine($"{ModuleName} -> {State}");
					foreach (var module in ConnectedModules) circuit.Enqueue(module, State);
				} else {
					Console.WriteLine($"{ModuleName} -> {State}");
				}
			}
		}
		public class Conjunction : Module {
			public Conjunction(string moduleName, Circuit circuit) : base(moduleName, circuit) { }
			public override void ReceivePulse(bool pulse) {
				if (ConnectedModules.Any(x => x.State == false)) State = true;
				State = false;
				Console.WriteLine($"{ModuleName} -> {State}");
				foreach (var module in ConnectedModules) circuit.Enqueue(module, State);
			}
		}
		public class Broadcaster : Module {
			public Broadcaster(string moduleName, Circuit circuit) : base(moduleName, circuit) { }
			public override void ReceivePulse(bool pulse) {
				Console.WriteLine($"{ModuleName} -> {pulse}");
				foreach (var module in ConnectedModules) circuit.Enqueue(module, State);
			}
		}
	}
}
