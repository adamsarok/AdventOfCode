using Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using static Helpers.Graph;
using static Year2023.Day20;

namespace Year2023 {
	public class Day20 : IAocSolver {
		public long SolvePart1(string[] input) {
			var circuit = new Circuit(input);
			for (int i = 0; i < 1000; i++) {
				circuit.PressButton();
			}
			return circuit.HighPulsesSent * circuit.LowPulsesSent;
		}

		public long SolvePart2(string[] input) {
			var circuit = new Circuit(input);
			var nodes = circuit.Modules.Select(x => new Node(x.ModuleName, x.ModuleName));
			var edges = circuit.Modules.SelectMany(x => x.OutputModules.Select(m =>
				new Edge($"{x.ModuleName}-{m.ModuleName}",
					x.ModuleName,
					m.ModuleName,
					$"{x.ModuleName}->{m.ModuleName}"
				)));
			var graph = new Graph(nodes, edges);
			graph.WriteToHtml();
			//this is a bit manual - by visualizing the graph, you can check what is connected to rx
			//find a level which has a realistic
			//then log when the modules feeding into rx are first set to low, then calculate LCM
			circuit.LogModules(["qd", "dh", "bb", "dp"]);
			for (int i = 0; i < 100000000; i++) {
				circuit.PressButton();
				if (!circuit.ModuleFirstSetToLow.Any(x => x.Value == 0)) {
					foreach (var kvp in circuit.ModuleFirstSetToLow) Console.WriteLine($"{kvp.Key}:{kvp.Value}");
					return Helpers.Helpers.LCM(circuit.ModuleFirstSetToLow.Select(x => x.Value).ToArray());
				}
			}
			return 0;
		}

		public class Circuit {
			private Queue<(Module from, Module to, bool pulse)> messageQueue = new();
			public List<Module> Modules = new();
			public long HighPulsesSent = 0;
			public long LowPulsesSent = 0;
			public long ButtonPresses = 0;
			private Module button;
			public Dictionary<string, long> ModuleFirstSetToLow;

			public Circuit(string[] input) {
				button = AddDummy("button");
				foreach (var l in input) {
					var s = l.Split("->");
					var m = s[0].Trim();
					Module module;
					switch (m[0]) {
						case 'b':
							module = new Broadcaster(m, this);
							break;
						case '%':
							module = new FlipFlop(m.Substring(1), this);
							break;
						case '&':
							module = new Conjunction(m.Substring(1), this);
							break;
						default: throw new Exception("module parse failed");
					}
					Modules.Add(module);
				}
				foreach (var l in input) {
					var s = l.Split("->");
					var m = s[0].Trim().Replace("%", "").Replace("&", "");
					var connected = s[1].Split(",");
					var inputModule = Modules.Where(x => x.ModuleName == m).FirstOrDefault();
					foreach (var c in connected) {
						var output = Modules.Where(x => x.ModuleName == c.Trim()).FirstOrDefault();
						if (output == null) output = AddDummy(c.Trim());
						inputModule.AddOutputModule(output);
						output.AddInputModule(inputModule);
					}
				}
			}
			public void PressButton() {
				ButtonPresses++;
				var b = Modules.FirstOrDefault(x => x.ModuleName == "broadcaster");
				Enqueue(button, b, false);
				ProcessQueue();
			}
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

			internal void LogModules(IEnumerable<string> moduleNames) {
				ModuleFirstSetToLow = moduleNames.ToDictionary(k => k, e => (long)0);
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
					foreach (var module in OutputModules) circuit.Enqueue(this, module, State);
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
				foreach (var module in OutputModules) circuit.Enqueue(this, module, State);
				if (!pulse && circuit.ModuleFirstSetToLow != null && circuit.ModuleFirstSetToLow.TryGetValue(ModuleName, out var val)) {
					if (val == 0) {
						circuit.ModuleFirstSetToLow[ModuleName] = circuit.ButtonPresses;
					}
				}
			}
		}
		public class Broadcaster : Module {
			public Broadcaster(string moduleName, Circuit circuit) : base(moduleName, circuit) { }
			public override void ReceivePulse(Module from, Module to, bool pulse) {
				foreach (var module in OutputModules) circuit.Enqueue(this, module, State);
			}
		}
		public class Dummy : Module { //output, button, module with no definition -> does nothing w/ input
			public Dummy(string moduleName, Circuit circuit) : base(moduleName, circuit) { }
			public override void ReceivePulse(Module from, Module to, bool pulse) {	}
		}
	}
}
