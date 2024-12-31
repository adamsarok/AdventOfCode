using Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Helpers.Directions;

namespace Year2019.Day25 {
	public class Day25 : Solver {
		public Day25() : base(2019, 25) {
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

			List<char> output;
			char[] input;

			public string RunCode(string command, bool isFirstPart) {
				input = command.ToString().ToCharArray();
				output = new List<char>();
				int actInput = 0;
				while (instructionPointer < intCode.Length) {
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
							SetVariable(instructionPointer + 1, input[actInput], aMode);
							actInput++;
							instructionPointer += 2;
							break;
						case Opcode.OUT:
							var c = param1(aMode);
							output.Add((char)c);
							instructionPointer += 2;
							if (c == '?') return new string(output.ToArray());
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
							return new string(output.ToArray());
					}
				}
				return "-1"; //shouldn't happen
			}
		}

		private class Room {
			public LVec Pos { get; set; }
			public string InitString { get; }
			public string Name { get; }
			public string Description { get; }
			public List<string> Items { get; }

			public Dictionary<Direction, Room?> Neighbors = new Dictionary<Direction, Room?>();
			public Room(string initString) { 
				Items = new List<string>();
				InitString = initString;
				Description = "";
				bool descriptionNext = false;
				bool parsingItems = false;
				foreach (var l in initString.Split('\n')) {
					if (parsingItems) {
						if (l.StartsWith("-")) {
							Items.Add(l.Substring(2));
						} else parsingItems = false;
					} else if (l.StartsWith("-")) {
						descriptionNext = false;
						if (l.Contains("north")) Neighbors.Add(Direction.North, null);
						if (l.Contains("south")) Neighbors.Add(Direction.South, null);
						if (l.Contains("west")) Neighbors.Add(Direction.West, null);
						if (l.Contains("east")) Neighbors.Add(Direction.East, null);
					} else if (descriptionNext) {
						Description += l;
					} else if (l.StartsWith("=")) {
						Name = l;
						descriptionNext = true;
					} else if (l.StartsWith("Items here")) {
						parsingItems = true;
					}
				}
			}
		}
			
		private List<Room> rooms;
		//LVec pos = new LVec(0, 0); 
		
		//will not work, we are going around in circles when going only north.
		//however when going a second time into hull breach, we can no longer go north???

		protected override long SolvePart1() {
			base.SolvePart1();
			if (IsShort) return -1;
			//SetAllCommands();
			return Traverse(true);
		}



		private Room? FindFirstUnvisited(Room from) {
			if (from.Neighbors.Any(x => x.Value == null)) return from;
			foreach (var room in rooms) {
				if (room.Neighbors.Any(x => x.Value == null)) return room;
			}
			return null;
		}

		List<string> dontTake = new List<string>() { "photons" };

		private long Traverse(bool isFirstPart) {
			Console.Clear();
			var compy = new IntCodeComputer(startCode);
			rooms = new List<Room>();

			string output = compy.RunCode("\n", isFirstPart);
			var room = new Room(output);
			rooms.Add(room);
			Console.WriteLine(output);
			while (true) {
				var next = FindFirstUnvisited(room);
				if (next == null) return -1; //all rooms visited, no solution
				var dir = next.Neighbors.FirstOrDefault(x => x.Value == null).Key;
				if (next.Name == room.Name) {
					room = Navigate(compy, dir, isFirstPart, room);
					foreach (var item in room.Items.Except(dontTake)) {
						//Console.WriteLine($"Taking {item}");
						Console.WriteLine(compy.RunCode($"take {item}\n", isFirstPart));
						//Console.WriteLine(compy.RunCode($"inv\n", isFirstPart));
					}
				} else {
					return -1; //todo: navigate to unvisited room
				}
			}
			return -1;
		}

		private Room Navigate(IntCodeComputer compy, Direction direction, bool isFirstPart, Room from) {
			string dirstr = direction switch {
				Direction.North => "north",
				Direction.South => "south",
				Direction.West => "west",
				Direction.East => "east",
				_ => throw new Oopsie("unknown direction")
			};
			Console.ForegroundColor = ConsoleColor.DarkMagenta;
			Console.WriteLine($"{dirstr}");
			var r = compy.RunCode($"{dirstr}\n", isFirstPart);
			if (!r.Contains("can't")) {
				Console.WriteLine($"Success!");
				Room room;
				if (!from.Neighbors.TryGetValue(direction, out room) || room == null) {
					room = new Room(r);
					from.Neighbors[direction] = room;
					room.Neighbors[Opposite(direction)] = from;
					rooms.Add(room);
				}
				Console.ResetColor();
				Console.WriteLine(r);
				return room;
			}
			Console.ResetColor();
			Console.WriteLine(r);
			return from;
		}

		protected override long SolvePart2() {
			base.SolvePart2();
			if (IsShort) return -1;
			//SetAllCommands();
			return Traverse(false);
		}
	}
}
