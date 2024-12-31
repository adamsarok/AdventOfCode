using Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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


			public IntCodeComputer(long[] startCode) {
				intCode = new long[10000];
				this.startCode = startCode;
				Reset();
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
			Queue<long> inputs;
			LVec? lastMessage;
			long? outIP, outX;

			public long RunCode(long networkaddress, Dictionary<long, ConcurrentQueue<LVec>> messageQueues) {
				inputs = new Queue<long>();
				inputs.Enqueue(networkaddress);
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
							if (messageQueues[networkaddress].TryDequeue(out var msg)) {
								Console.WriteLine($"{networkaddress} received {msg}");
								inputs.Enqueue(msg.x);
								inputs.Enqueue(msg.y);
							} 
							if (inputs.TryDequeue(out input)) SetVariable(instructionPointer + 1, input, aMode);
							else SetVariable(instructionPointer + 1, -1, aMode);
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
								messageQueues[outIP.Value].Enqueue(new LVec(outX.Value, c));
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


	
		//something's not right, message flow stops
		protected override long SolvePart1() {
			base.SolvePart1();
			if (IsShort) return -1;
			var messageQueues = new Dictionary<long, ConcurrentQueue<LVec>>();
			var tasks = new List<Task<long>>();
			int compyCount = 50;
			for (long l = 0; l < compyCount; l++) {
				var networkAddress = l;
				messageQueues[networkAddress] = new ConcurrentQueue<LVec>();
				tasks.Add(Task.Factory.StartNew(() => {
					var compy = new IntCodeComputer(startCode);
					long r = -1;
					while (r < 0) {
						r = compy.RunCode(networkAddress, messageQueues);
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
