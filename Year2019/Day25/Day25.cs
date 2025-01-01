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
using static System.Net.Mime.MediaTypeNames;

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
							if (CheckOutputFinished()) return new string(output.ToArray());
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

			private bool CheckOutputFinished() { //there must be a better way than this
				var l = output.Count;
				//Command?
				if (output[l - 1] == '?' && output[l - 2] == 'd' && output[l-8] == 'C') return true;
				return false;
			}

		}


		private class Room : IEquatable<Room> {
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

			bool IEquatable<Room>.Equals(Room? other) {
				if (other != null && other.Name == Name) return true;
				return false;
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

		List<string> dontTake = new List<string>() { "photons", "infinite loop", "giant electromagnet", "escape pod", "molten lava" };
		List<string> inventory = new List<string>();
		Room sensorRoom;

		private long Traverse(bool isFirstPart) {
			//Console.Clear();
			var compy = new IntCodeComputer(startCode);
			rooms = new List<Room>();

			string output = compy.RunCode("\n", isFirstPart);
			var room = new Room(output);
			rooms.Add(room);
			//Console.WriteLine(output);
			while (true) {
				var next = FindFirstUnvisited(room);
				if (next == null) {
					//Console.Clear();
					var r = compy.RunCode($"inv\n", isFirstPart);
					//Console.WriteLine(r);
					return DoWeightPuzzle(compy, room, isFirstPart);
				}
				if (next.Name == room.Name) {
					var dir = next.Neighbors.FirstOrDefault(x => x.Value == null).Key;
					room = NavigateToDirection(compy, dir, isFirstPart, room);
				} else {
					room = NavigateToRoom(isFirstPart, compy, room, next);
				}
			}
			return -1;
		}

		private Room NavigateToRoom(bool isFirstPart, IntCodeComputer compy, Room from, Room to) {
			var room = from;
			var path = MapPath(from, to, new List<Room>());
			if (path.Count == 0) throw new Oopsie($"No path found from {from.Name} to {to.Name}");
			foreach (var p in path) {
				var d = room.Neighbors.Where(x => x.Value.Equals(p)).First().Key;
				room = NavigateToDirection(compy, d, isFirstPart, room);
			}
			//Console.WriteLine($"Success navigating {from.Name} to {to.Name}");
			return room;
		}

		

		private long DoWeightPuzzle(IntCodeComputer compy, Room from, bool isFirstPart) {
			var precheck = rooms.Where(x => x.Name == "== Security Checkpoint ==").First();
			var room = NavigateToRoom(isFirstPart, compy, from, precheck);
			var dir = room.Neighbors.Where(x => x.Value.Name == "== Pressure-Sensitive Floor ==").First().Key;
			bool success = false;
			string dirstr = GetDirStr(dir);
			var combinations = inventory.SetCombinations();
			foreach (var c in combinations) {
				foreach (var all in inventory) compy.RunCode($"take {all}\n", isFirstPart);
				foreach (var drop in inventory.Except(c)) compy.RunCode($"drop {drop}\n", isFirstPart);

				//Console.Clear();
				//Console.WriteLine(compy.RunCode($"inv\n", isFirstPart));
				var r = compy.RunCode($"{dirstr}\n", isFirstPart);
				//Console.WriteLine(r);
				if (!r.Contains("lighter") && !r.Contains("heavier")) {
					var key = "get in by typing ";
					var result = r.Substring(r.IndexOf(key) + key.Length, 10);
					return long.Parse(result);
				}
			}
			return -1;
		}

		List<Room> MapPath(Room from, Room to, List<Room> visited) {
			foreach (var neighbor in from.Neighbors.Values.Where(x => x != null)) {
				if (visited.Any(x => x.Equals(neighbor))) {
					continue;
				}
				var pathToNeighbor = new List<Room>(visited);
				pathToNeighbor.Add(neighbor);
				if (neighbor == to) return pathToNeighbor;
				var n = MapPath(neighbor, to, pathToNeighbor);
				if (n.Count > 0) return n;
			}
			return new List<Room>();
		}
		private string GetDirStr(Direction direction) {
			string dirstr = direction switch {
				Direction.North => "north",
				Direction.South => "south",
				Direction.West => "west",
				Direction.East => "east",
				_ => throw new Oopsie("unknown direction")
			};
			return dirstr;
		}
		private Room NavigateToDirection(IntCodeComputer compy, Direction direction, bool isFirstPart, Room from) {
			//Console.Clear();
			string dirstr = GetDirStr(direction);
			//Console.ForegroundColor = ConsoleColor.DarkMagenta;
			//Console.WriteLine($"{dirstr}");
			var r = compy.RunCode($"{dirstr}\n", isFirstPart);
			if (!r.Contains("can't")) {
				if (sensorRoom == null && r.Contains("Pressure-Sensitive Floor")) return WeightPuzzleFound(compy, from, isFirstPart, r, direction);
				//Console.WriteLine($"Success!");
				Room room;
				if (!from.Neighbors.TryGetValue(direction, out room) || room == null) {
					room = new Room(r);
					from.Neighbors[direction] = room;
					room.Neighbors[Opposite(direction)] = from;
					rooms.Add(room);
				}

				foreach (var item in room.Items.Except(dontTake).Except(inventory)) {
					//Console.ForegroundColor = ConsoleColor.Red;
					//Console.WriteLine($"Taking: {item}");
					//Console.ResetColor();
					var r2 = compy.RunCode($"take {item}\n", isFirstPart);
					//Console.WriteLine(r2);
					inventory.Add(item);
				}

				//Console.ResetColor();
				//Console.WriteLine(r);
				return room;
			} else {
				throw new Oopsie($"can't navigate {direction} from {from.Name}");
			}
			//Console.ResetColor();
			//Console.WriteLine(r);
			//return from;
		}

		private Room WeightPuzzleFound(IntCodeComputer compy, Room from, bool isFirstPart, string initStr, Direction direction) {
			if (from.Neighbors[direction] == null) {
				var room = new Room(initStr.Substring(0, initStr.IndexOf("== Security Checkpoint ==")));
				from.Neighbors[direction] = room;
				sensorRoom = room;
				if (initStr.Contains("ejected")) return from; //thrown back where we came from
			}
			throw new Oopsie("We expected to be ejected from the pressure sensor room but we did not :(");
		}

		protected override long SolvePart2() {
			return -1;
		}
	}
}
