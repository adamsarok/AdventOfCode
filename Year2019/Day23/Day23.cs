using Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Year2019.Day23 {
	public class Day23 : Solver {
		public Day23() : base(2019, 23) {
		}
		private long[] startCode;
		protected override void ReadInputPart1(string fileName) {
			base.ReadInputPart1(fileName);
			IsShort = fileName.Contains("short");
			var cmds = File.ReadAllLines(fileName)[0].Split(',');
			startCode = new long[cmds.Length];
			for (int i = 0; i < cmds.Length; i++) {
				startCode[i] = long.Parse(cmds[i]);
			}
		}

		protected override void ReadInputPart2(string fileName) {
			base.ReadInputPart2(fileName);
			ReadInputPart1(fileName);
		}

		class IntCodeComputer {
			long[] intCode;
			long relativeBase;
			long instructionPointer;
			private long[] startCode;
			private readonly long networkaddress;
			private readonly Network network;

			public IntCodeComputer(long[] startCode, long networkaddress, Network network) {
				intCode = new long[10000];
				this.startCode = startCode;
				this.networkaddress = networkaddress;
				this.network = network;
				Reset();
				inputs = new Queue<long>();
				inputs.Enqueue(networkaddress);
				//inputs.Enqueue(-1);
			}

			public void Reset() {
				instructionPointer = 0;
				relativeBase = 0;
				for (int i = 0; i < startCode.Length; i++) intCode[i] = startCode[i];
			}

			long GetVariable(long pos, long mode) {
				if (mode == 0) return intCode[intCode[pos]];
				if (mode == 1) return intCode[pos];
				if (mode == 2) return intCode[intCode[pos] + relativeBase];
				throw new Oopsie("unknown ParamMode");
			}
			void SetVariable(long pos, long val, long mode) {
				if (mode == 0) intCode[intCode[pos]] = val;
				else if (mode == 1) intCode[intCode[pos]] = val;
				else if (mode == 2) intCode[intCode[pos] + relativeBase] = val;
				else throw new Oopsie("unknown ParamMode");
			}
			enum Opcode {
				ADD = 1,
				MUL = 2,
				IN = 3,
				OUT = 4,
				JIT = 5,
				JIF = 6,
				LT = 7,
				EQ = 8,
				RB = 9,
				HALT = 99
			}
			enum ParamMode {
				Position = 0,
				Immediate = 1,
				Relative = 2
			}

			long param1(long mode) => GetVariable(instructionPointer + 1, mode);
			long param2(long mode) => GetVariable(instructionPointer + 2, mode);

			Queue<long> outputs;
			public Queue<long> inputs;
			LVec? lastMessage;
			long? outIP, outX;

			public long RunCode() {

				Thread.Sleep(1);
				while (instructionPointer < intCode.Length) {
					//if (instructionPointer < 0) instructionPointer = 0;
					long cmdNum = intCode[instructionPointer];
					var opcode = (Opcode)(cmdNum % 100);
					if (opcode == 0) throw new Oopsie();
					long modes = cmdNum / 100;
					var aMode = modes % 10;
					modes /= 10;
					var bMode = modes % 10;
					modes /= 10;
					var cMode = modes % 10;
					//if (network.TryReceive(networkaddress, out var message)) {
					//	inputs.Enqueue(message.x);
					//	inputs.Enqueue(message.y);
					//}
					switch (opcode) {
						case Opcode.ADD:
							SetVariable(instructionPointer + 3, param1(aMode) + param2(bMode), cMode);
							instructionPointer += 4;
							break;
						case Opcode.MUL:
							SetVariable(instructionPointer + 3,
								param1(aMode) * param2(bMode), cMode);
							instructionPointer += 4;
							break;
						case Opcode.IN:
							long input;
							Thread.Sleep(1);
							if (inputs.TryDequeue(out input)) {
								Console.WriteLine($"{networkaddress} received {input}");
								SetVariable(instructionPointer + 1, input, aMode);
							} else SetVariable(instructionPointer + 1, -1, aMode);
							instructionPointer += 2;
							break;
						case Opcode.OUT:
							var c = param1(aMode);
							if (outIP == null) {
								outIP = c;
							} else if (outX == null) {
								outX = c;
							} else {
								if (outIP.Value == 255) {
									return c;
								}
								network.Send(outIP.Value, new LVec(outX.Value, c));
								outIP = null;
								outX = null;
							}
							//output.Add(c);
							instructionPointer += 2;
							break;
						case Opcode.JIT:
							if (param1(aMode) != 0) instructionPointer = param2(bMode);
							else instructionPointer += 3;
							break;
						case Opcode.JIF:
							if (param1(aMode) == 0) instructionPointer = param2(bMode);
							else instructionPointer += 3;
							break;
						case Opcode.LT:
							SetVariable(instructionPointer + 3, param1(aMode) < param2(bMode) ? 1 : 0, cMode);
							instructionPointer += 4;
							break;
						case Opcode.EQ:
							SetVariable(instructionPointer + 3, param1(aMode) == param2(bMode) ? 1 : 0, cMode);
							instructionPointer += 4;
							break;
						case Opcode.RB:
							relativeBase += param1(aMode);
							instructionPointer += 2;
							break;
						case Opcode.HALT:
							return -1;
					}
				}
				return -1;
			}
		}

		class Network {
			//Dictionary<long, ConcurrentQueue<LVec>> messageQueues = new();
			Dictionary<long, IntCodeComputer> computers = new();
			Dictionary<long, DateTime> lastPing = new();
			object locky = new object();
			public Network() {
				//Timer wakeOnLan = new Timer((o) => {
				//	lock (locky) {
				//		foreach (var addr in lastPing.Where(x => x.Value < DateTime.Now.AddSeconds(-2))) {
				//			Console.WriteLine($"Waking: {addr.Key}");
				//			computers[addr.Key].Reset();
				//			computers[addr.Key].RunCode(addr.Key, this);
				//		}
				//	}
				//}, null, 5000, 5000);
			}
			bool isStartup = true;
			public void Send(long networkAddress, LVec message) {
				lock (locky) {
					//if (isStartup) {
					//	Thread.Sleep(30000);
					//	isStartup = false;
					//}
					var c = computers[networkAddress];
					//if (c.inputs == null) Thread.Sleep(1000);
					c.inputs.Enqueue(message.x);
					c.inputs.Enqueue(message.y);
				}
			}
			//public bool TryReceive(long networkAddress, out LVec message) {
			//	//lock (locky) {
			//		lastPing[networkAddress] = DateTime.Now;
			//		if (messageQueues[networkAddress].TryDequeue(out message)) {
			//			Console.WriteLine($"{networkAddress} received {message}");
			//			return true;
			//		}
			//	//}
			//	return false;
			//}
			public void AddComputer(long networkAddress, IntCodeComputer compy) {
				computers.Add(networkAddress, compy);
				//messageQueues.Add(networkAddress, new ConcurrentQueue<LVec>());
				lastPing.Add(networkAddress, DateTime.Now);
			}
		}

		//still flaky - adding computers slows down considerably after a few compys, but why?
		protected override long SolvePart1() {
			base.SolvePart1();
			if (IsShort) return -1;
			var tasks = new List<Task<long>>();
			int compyCount = 50;
			var network = new Network();
			List<IntCodeComputer> computers = new();
			for (long l = 0; l < compyCount; l++) {
				var networkAddress = l;
				var compy = new IntCodeComputer(startCode, networkAddress, network);
				network.AddComputer(networkAddress, compy);
				computers.Add(compy);
			}
			for (int l = 0; l < compyCount; l++) {
				int address = l;
				tasks.Add(Task.Factory.StartNew(() => {
					var compy = computers[address];
					long r = -1;
					while (r < 0) {
						Console.WriteLine($"Starting compy {address}");
						r = compy.RunCode();
						compy.Reset();
					}
					return r;
				}));
			}

			Task.WaitAll(tasks.ToArray());
			long result = tasks.Max(t => t.Result);
			return result;
		}

		

		protected override long SolvePart2() {
			base.SolvePart2();
			if (IsShort) return -1;
			return -1;
		}
	}
}
