using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Year2023.Day20;

namespace Year2023 {
	public class Day20 : IAocSolver {
		public long SolvePart1(string[] input) {
			long result = 0;

			Circuit circuit = new Circuit();
			foreach (var l in input) {
				var s = l.Split("->");
				var m = s[0].Trim();
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
				circuit.Modules.Add(module);
			}
			foreach (var l in input) {
				var s = l.Split("->");
				var m = s[0].Trim().Replace("%", "").Replace("&", "");
				var connected = s[1].Split(",");
				var inputModule = circuit.Modules.Where(x => x.ModuleName == m).FirstOrDefault();
				foreach (var c in connected) {
					var output = circuit.Modules.Where(x => x.ModuleName == c.Trim()).FirstOrDefault();
					if (output == null) output = circuit.AddDummy(c.Trim());
					inputModule.AddOutputModule(output);
					output.AddInputModule(inputModule);
				}
			}
			var b = circuit.Modules.FirstOrDefault(x => x.ModuleName == "broadcaster");
			var button = circuit.AddDummy("button");
			for (int i = 0; i < 1000; i++) {
				circuit.Enqueue(button, b, false);
				circuit.ProcessQueue();
			}

			return circuit.HighPulsesSent * circuit.LowPulsesSent;
		}

		public long SolvePart2(string[] input) {
			return 0;
		}

		public class Circuit {
			private Queue<(Module from, Module to, bool pulse)> messageQueue = new();
			public List<Module> Modules = new();
			public long HighPulsesSent = 0;
			public long LowPulsesSent = 0;
			public void Enqueue(Module from, Module to, bool pulse) {
				if (pulse) HighPulsesSent++;
				else LowPulsesSent++;
				messageQueue.Enqueue((from, to, pulse));
			}
			public void ProcessQueue() {
				(Module from, Module to, bool pulse) next;
				while (messageQueue.TryDequeue(out next)) {
					next.Item2.ReceivePulse(next.from, next.to, next.pulse);
				}
			}
			internal Module AddDummy(string moduleName) {
				var dummy = new Dummy(moduleName, this);
				Modules.Add(dummy);
				return dummy;
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
			public abstract void ReceivePulse(Module from, Module to, bool pulse);
			public virtual void AddOutputModule(Module module) {
				OutputModules.Add(module);
			}
			public virtual void AddInputModule(Module module) {
				InputModules.Add(module);
			}
			public List<Module> OutputModules { get; set; } = new List<Module>();
			public List<Module> InputModules { get; set; } = new List<Module>();
		}
		public class FlipFlop : Module {
			public FlipFlop(string moduleName, Circuit circuit) : base(moduleName, circuit) { }
			public override void ReceivePulse(Module from, Module to, bool pulse) {
				if (!pulse) {
					State = !State;
					Console.WriteLine($"{from.ModuleName} -{(pulse ? "high" : "low")}-> {to.ModuleName}");
					foreach (var module in OutputModules) circuit.Enqueue(this, module, State);
				} else {
					Console.WriteLine($"{from.ModuleName}  - {(pulse ? "high" : "low")} ->  {to.ModuleName}");
				}
			}
		}
		public class Conjunction : Module {
			public Conjunction(string moduleName, Circuit circuit) : base(moduleName, circuit) { }
			Dictionary<string, bool> rememberedStates = new();
			public override void AddInputModule(Module module) {
				base.AddInputModule(module);
				rememberedStates.Add(module.ModuleName, false);
			}
			public override void ReceivePulse(Module from, Module to, bool pulse) {
				rememberedStates[from.ModuleName] = pulse;
				State = rememberedStates.Any(x => !x.Value);
				Console.WriteLine($"{from.ModuleName} -{(pulse ? "high" : "low")}-> {ModuleName}");
				foreach (var module in OutputModules) circuit.Enqueue(this, module, State);
			}
		}
		public class Broadcaster : Module {
			public Broadcaster(string moduleName, Circuit circuit) : base(moduleName, circuit) { }
			public override void ReceivePulse(Module from, Module to, bool pulse) {
				Console.WriteLine($"{from.ModuleName} -{(pulse ? "high" : "low")}-> {ModuleName}");
				foreach (var module in OutputModules) circuit.Enqueue(this, module, State);
			}
		}
		public class Dummy : Module { //output, button, module with no definition -> does nothing w/ input
			public Dummy(string moduleName, Circuit circuit) : base(moduleName, circuit) { }
			public override void ReceivePulse(Module from, Module to, bool pulse) { }
		}
	}
}
