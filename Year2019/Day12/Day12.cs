using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2019.Day12 {
	public class Day12 : Solver {
		public Day12() : base(2019, 12) {
		}
		class Moon {
			public int Id { get; set; }
			public LVec3D Pos { get; set; }
			public LVec3D Velocity { get; set; }

			//values are always between -10 / 10
			const long a = 100L;
			const long b = 100 * 100L;
			const long c = 100 * 100 * 100L;
			const long d = 100 * 100 * 100 * 100L; 
			const long e = 100 * 100 * 100 * 100 * 100L;
			public string Hash => Id.ToString() + "m" + (Pos.x + Pos.y * a + Pos.z * b + Velocity.x * c + Velocity.y * d + Velocity.z * e);
			public string HashX => Id.ToString() + "m" + (Pos.x + Velocity.x * a);
			public string HashY => Id.ToString() + "m" + (Pos.y + Velocity.y * a);
			public string HashZ => Id.ToString() + "m" + (Pos.z + Velocity.z * a);
		}
		List<Moon> moons;
		protected override void ReadInputPart1(string fileName) {
			base.ReadInputPart1(fileName);
			moons = new();
			int id = 0;
			foreach (var l in File.ReadAllLines(fileName)) {
				var s = l.Split(',');
				var x = s[0].Split('=')[1];
				var y = s[1].Split('=')[1];
				var z = s[2].Split('=')[1].Split('>')[0];
				moons.Add(new Moon() { Id = id++, Pos = new LVec3D(long.Parse(x), long.Parse(y), long.Parse(z)), Velocity = new LVec3D(0, 0, 0) });
			}
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			result = Simulate(1000);
			return result;
		}

		private long Simulate(long steps) {
			long result = 0;
			long max = 0;

			for (long i = 1; i <= steps; i++) {
				for (int m1 = 0; m1 < moons.Count; m1++) {
					for (int m2 = m1 + 1; m2 < moons.Count; m2++) {
						var moon = moons[m1];
						var other = moons[m2];
						int mX = -moon.Pos.x.CompareTo(other.Pos.x); //??? why - ???
						int mY = -moon.Pos.y.CompareTo(other.Pos.y);
						int mZ = -moon.Pos.z.CompareTo(other.Pos.z);
						moon.Velocity = new LVec3D(moon.Velocity.x + mX, moon.Velocity.y + mY, moon.Velocity.z + mZ);
						other.Velocity = new LVec3D(other.Velocity.x - mX, other.Velocity.y - mY, other.Velocity.z - mZ);
					}
				}
				foreach (var moon in moons) moon.Pos += moon.Velocity;
				//Debug(i);
			}
			foreach (var moon in moons) {
				result += (Math.Abs(moon.Pos.x) + Math.Abs(moon.Pos.y) + Math.Abs(moon.Pos.z)) * (Math.Abs(moon.Velocity.x) + Math.Abs(moon.Velocity.y) + Math.Abs(moon.Velocity.z));
			}
			return result;
		}

		private void Debug(int i) {
			Console.WriteLine($"After {i} steps:");
			foreach (var moon in moons) { Console.WriteLine($"pos={moon.Pos} vel={moon.Velocity}"); }
			Console.WriteLine();
		}

		protected override long SolvePart2() {
			//var t1 = Helpers.Helpers.LCM(Helpers.Helpers.LCM(x, y), z);

			if (!IsShort) return -1;
			long result = 0;
			HashSet<string> states = new HashSet<string>();
			HashSet<string> statesx = new HashSet<string>();
			HashSet<string> statesy = new HashSet<string>();
			HashSet<string> statesz = new HashSet<string>();
			long? cycleX = null, cycleY = null, cycleZ = null;
			//long actState = 0;
			while (true) { 
				for (int m1 = 0; m1 < moons.Count; m1++) {
					for (int m2 = m1 + 1; m2 < moons.Count; m2++) {
						var moon = moons[m1];
						var other = moons[m2];
						int mX = -moon.Pos.x.CompareTo(other.Pos.x); //??? why - ???
						int mY = -moon.Pos.y.CompareTo(other.Pos.y);
						int mZ = -moon.Pos.z.CompareTo(other.Pos.z);
						moon.Velocity = new LVec3D(moon.Velocity.x + mX, moon.Velocity.y + mY, moon.Velocity.z + mZ);
						other.Velocity = new LVec3D(other.Velocity.x - mX, other.Velocity.y - mY, other.Velocity.z - mZ);
					}
				}
				foreach (var moon in moons) moon.Pos += moon.Velocity;
				string actState = "", actStatex = "", actStatey = "", actStatez = "";
				foreach (var moon in moons) {
					actState += moon.Hash;
					actStatex += moon.HashX;
					actStatey += moon.HashY;
					actStatez += moon.HashZ;
				}
				result++;
				//if (cycleX == null && statesx.Contains(actStatex)) cycleX = result - 1;
				//	statesx.Clear();
				//	Console.WriteLine($"Cycle detected on X at {result - 1}");
				//	//break;
				//}
				//if (statesy.Contains(actStatey)) {
				//	statesy.Clear();
				//	Console.WriteLine($"Cycle detected on Y at {result - 1}");
				//	//break;
				//}
				//if (statesz.Contains(actStatez)) {
				//	statesz.Clear();
				//	Console.WriteLine($"Cycle detected on Z at {result - 1}");
				//	//break;
				//}
				if (states.Contains(actState)) break;
				states.Add(actState);
				statesx.Add(actStatex);
				statesy.Add(actStatey); 
				statesz.Add(actStatez);
			} 
			return result - 1;
		}
	}
}
