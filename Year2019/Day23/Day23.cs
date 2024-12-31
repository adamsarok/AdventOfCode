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
			private readonly bool isFirstPart;

			public IntCodeComputer(long[] startCode, long networkaddress, Network network, bool isFirstPart) {
				intCode = new long[10000];
				this.startCode = startCode;
				this.networkaddress = networkaddress;
				this.network = network;
				this.isFirstPart = isFirstPart;
				Reset();
				inputs = new Queue<long>();
				inputs.Enqueue(networkaddress);
			}

			public void Reset() {
				isStopping = false;
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
			public bool receiving = false;

			public long RunCode() {
				while (!isStopping && instructionPointer < intCode.Length) {
					long cmdNum = intCode[instructionPointer];
					var opcode = (Opcode)(cmdNum % 100);
					if (opcode == 0) throw new Oopsie();
					long modes = cmdNum / 100;
					var aMode = modes % 10;
					modes /= 10;
					var bMode = modes % 10;
					modes /= 10;
					var cMode = modes % 10;
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
							if (inputs.TryDequeue(out input)) {
								SetVariable(instructionPointer + 1, input, aMode);
								if (!receiving) receiving = true;
							} else {
								Thread.Sleep(1);
								SetVariable(instructionPointer + 1, -1, aMode);
							}
							instructionPointer += 2;
							break;
						case Opcode.OUT:
							var c = param1(aMode);
							if (outIP == null) {
								outIP = c;
							} else if (outX == null) {
								outX = c;
							} else {
								if (isFirstPart && outIP.Value == 255) {
									return c;
								}
								network.Send(outIP.Value, new LVec(outX.Value, c));
								outIP = null;
								outX = null;
							}
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
			bool isStopping = false;
			internal void Kill() {
				isStopping = true;
			}
		}

		class Network {
			Dictionary<long, IntCodeComputer> computers = new();
			Dictionary<long, DateTime> lastSendAt = new();
			object locky = new object();
			LVec? natPacket;
			LVec? lastNatPacket;
			public Network(bool isFirstPart) {
				if (!isFirstPart) {
					Timer wakeOnLan = new Timer((o) => {
						lock (locky) {
							if (lastSendAt.All(x => x.Value < DateTime.Now.AddMilliseconds(-100)) && natPacket != null) {
								Console.WriteLine($"All computers idle, sending {natPacket} to compy [0]");
								if (lastNatPacket != null && lastNatPacket.y == natPacket.y) {
									Console.Write($"Part2 final: ");
									Console.ForegroundColor = ConsoleColor.Green;
									Console.Write(natPacket.y);
									Console.ResetColor();
									Environment.Exit(0);
								}
								var compy = computers[0];
								compy.inputs.Enqueue(natPacket.x);
								compy.inputs.Enqueue(natPacket.y);
								lastNatPacket = natPacket;
								natPacket = null;
							}
						}
					}, null, 200, 200);
				}
			}
			bool isStartup = true;
			public void Send(long networkAddress, LVec message) {
				lock (locky) {
					if (networkAddress == 255) natPacket = message;
					else {
						var c = computers[networkAddress];
						while (!c.receiving) {
							Thread.Sleep(100);
						}
						//Console.WriteLine($"{networkAddress} received {message}");
						lastSendAt[networkAddress] = DateTime.Now;
						c.inputs.Enqueue(message.x);
						c.inputs.Enqueue(message.y);
					}
				}
			}
			public void AddComputer(long networkAddress, IntCodeComputer compy) {
				computers.Add(networkAddress, compy);
				lastSendAt.Add(networkAddress, DateTime.Now);
			}
		}

		protected override long SolvePart1() {
			base.SolvePart1();
			if (IsShort) return -1;
			return RunNetwork(true);
		}

		private long RunNetwork(bool isFirstPart) {
			var tasks = new List<Task<long>>();
			int compyCount = 50;
			var network = new Network(isFirstPart);
			List<IntCodeComputer> computers = new();
			for (long l = 0; l < compyCount; l++) {
				var networkAddress = l;
				var compy = new IntCodeComputer(startCode, networkAddress, network, isFirstPart);
				network.AddComputer(networkAddress, compy);
				computers.Add(compy);
			}
			ThreadPool.SetMinThreads(compyCount, compyCount);
			for (int l = 0; l < compyCount; l++) {
				int address = l;
				tasks.Add(Task.Run(() => {
					var compy = computers[address];
					long r = compy.RunCode();
					return r;
				}));
			}
			int index = Task.WaitAny(tasks.ToArray());
			var r = tasks[index].Result;
			foreach (var compy in computers) {
				compy.Kill();
			}
			Task.WaitAll(tasks);
			return r;
		}

		protected override long SolvePart2() {
			base.SolvePart2();
			if (IsShort) return -1;
			RunNetwork(false);
			return -1;
		}
	}
}
