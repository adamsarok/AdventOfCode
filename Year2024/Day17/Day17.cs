using Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Year2024.Day17 {
	public class Day17 : Solver {
		public Day17() : base(2024, 17) {
		}
		List<long> commands;
		List<long> output;
		protected override void ReadInputPart1(string fileName) {
			for (int i = 0; i < 10; i++) {
				var t = To3bits(i);
				var b = To10bits(t);
				//if (b != i) throw new Exception("Conversion error");
			}
			var ccc = To3bits(9);
			commands = new List<long>();
			var f = File.ReadAllLines(fileName);
			registers[0] = 0; registers[1] = 1; registers[2] = 2; registers[3] = 3;

			registers[4] = GetRegister(f[0]);


			registers[5] = GetRegister(f[1]);
			registers[6] = GetRegister(f[2]);
			Binit = registers[5]; Cinit = registers[6];
			foreach (var c in f[4].Split(':')[1].Split(",")) {
				commands.Add(int.Parse(c));
			}
		}
		private int GetRegister(string r) {
			return int.Parse(r.Split(':')[1]);
		}
		//long A, B, C;
		long Binit, Cinit;
		int instructionPointer = 0;

		long[] registers = new long[7];

		//private long GetComboOperandValue(long operand) {
		//	if (operand <= 3) return operand;
		//	switch (operand) {
		//		case 4: return A;
		//		case 5: return B;
		//		case 6: return C;
		//		default: throw new Exception("Invalid operand");
		//	}
		//}


		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			output = new List<long>();

			//test!!!
			//registers[4] = (long)Math.Pow(8, 15) - 1;
			//16

			while (instructionPointer < commands.Count - 1) {
				ProcessCommand(commands[instructionPointer], commands[instructionPointer + 1]);
			}
			//Console.WriteLine($"Output: {string.Join(",", output)}");
			return result;
		}
		private List<int> To3bits(int input) {
			if (input == 0) return new() { 0 };
			List<int> result = new List<int>();
			int remainder = input;
			int acc = input;
			while (acc > 0) {
				remainder = acc % 3;
				acc /= 3;
				result.Add(remainder);
			}
			return result;
		}
		private int To10bits(List<int> input) {
			int result = 0;
			for (int i = 0; i < input.Count; i++) {
				result += (int)(i * Math.Pow(3, i + 1));
			}
			return result;
		}
        enum Opcodes {
            adv = 0, //divide A by 2^B, truncate to int and write to A
            bxl = 1, //bitwise XOR of B and literal, stores in B
            bst = 2, //combo operand % 8 (lowest 3 bits), writes it to B
            jnz = 3, //nothing if A 0, else jumps to literal
                 //if jumps, instruction pointer is not increased
            bxc = 4, //bitwise XOR of B and C, stores in B
            out_ = 5,//combo % 8 outputs value
            bdv = 6, //same as adv but store in B
            cdv = 7, //same as adv but store in C
        }


		private void ProcessCommand(long opcode, long operand) {
			var operation = (Opcodes)opcode;
			//Console.WriteLine($"Executing {operation} {operand}, A={A} B={B} C={C}");
			switch (operation) {
				case Opcodes.adv:
					registers[4] = registers[4] >> (int)registers[operand];
					instructionPointer += 2;
					break;
				case Opcodes.bxl:
					registers[5] = registers[5] ^ operand;
					instructionPointer += 2;
					break;
				case Opcodes.bst:
					registers[5] = registers[operand] % 8;
					instructionPointer += 2;
					break;
				case Opcodes.jnz:
					if (registers[4] != 0) instructionPointer = (int)operand;
					else instructionPointer += 2;
					break;
				case Opcodes.bxc:
					registers[5] = registers[5] ^ registers[6];
					instructionPointer += 2;
					break;
				case Opcodes.out_:
					output.Add(registers[operand] % 8);
					instructionPointer += 2;
					break;
				case Opcodes.bdv:
					registers[5] = registers[4] >> (int)registers[operand];
					instructionPointer += 2;
					break;
				case Opcodes.cdv:
					registers[6] = registers[4] >> (int)registers[operand];
					instructionPointer += 2;
					break;
				default:
					throw new Exception("Unknown opcode {instruction}");
			}
		}

		Stopwatch sw = new();
		int matchedBits = 1;

		//does not help yet...
		//6 Output: 0 vs 2,4,1,3,7,5,0,3,1,5,4,4,5,5,3,0
		//49 Output: 3,0 vs 2,4,1,3,7,5,0,3,1,5,4,4,5,5,3,0
		//393 Output: 5,3,0 vs 2,4,1,3,7,5,0,3,1,5,4,4,5,5,3,0
		//3145 Output: 5,5,3,0 vs 2,4,1,3,7,5,0,3,1,5,4,4,5,5,3,0
		//25420 Output: 4,5,5,3,0 vs 2,4,1,3,7,5,0,3,1,5,4,4,5,5,3,0
		//220294 Output: 4,4,5,5,3,0 vs 2,4,1,3,7,5,0,3,1,5,4,4,5,5,3,0
		//1762354 Output: 5,4,4,5,5,3,0 vs 2,4,1,3,7,5,0,3,1,5,4,4,5,5,3,0
		//14098836 Output: 1,5,4,4,5,5,3,0 vs 2,4,1,3,7,5,0,3,1,5,4,4,5,5,3,0
		void Log(long aTry) {
			while (matchedBits <= Math.Min(commands.Count, output.Count)) {
				if (commands.Skip(commands.Count - matchedBits).SequenceEqual(output.Skip(output.Count - matchedBits))) {
					Console.WriteLine($"{aTry} Output: {string.Join(",", output)} vs {string.Join(",", commands)} ");
					matchedBits++;
				} else {
					break;
				}
			}
		}

		//Program: 2,4	1,3	 7,5  0,3  1,5  4,4  5,5  3,0
		//2,4: B = A % 8
		//1,3: B = B xor 3
		//7,5: C = A >> B
		//0,3: A = A >> 3
		//1,5: B = B xor 5
		//4,4: B = B xor C
		//5,5: out -> B % 8
		//3,0: while A != 0 jump to 0

		long A, B, C;
		private void ResetPart2(long a) {
			output = new();
			A = a;
			B = Binit;
			C = Cinit;
			actOut = target.Length - 1;
			//instructionPointer = 0;
		}

		private void HandCoded() {
			while (registers[4] != 0) {
				registers[5] = registers[4] % 8; //2,4
				registers[5] = registers[5] ^ 3; //1,3
				registers[6] = registers[4] >> (int)registers[5]; //7,5 
				registers[4] = registers[4] >> 3; //0,3
				registers[5] = registers[5] ^ 5; //1,5
				registers[5] = registers[5] ^ registers[6]; //4,4
				output.Add(registers[5] % 8); //5,5
			}
		}

		//Program: 2,4	1,3	 7,5  0,3  1,5  4,4  5,5  3,0

		long[] target;
		int actOut;
		private bool HandCoded2() {
			while (A != 0) {
				B = ((A % 8) ^ 3); //2,4 1,3
				C = A >> (int)B; //7,5 
				A = A >> 3; //0,3
				B = B ^ 5; //1,5
				B = B ^ C; //4,4

				output.Add(B % 8); //5,5

				if (B % 8 != target[actOut--]) {
					return false;
				}
				if (actOut < 0) {
					return true;
				}

				bool wtf = true;
			}
			return false;
		}


		protected override long SolvePart2() {
			//Program: 2,4	1,3	 7,5  0,3  1,5  4,4  5,5  3,0
			actOut = 0;
			target = new long[]{ 2,4,  1,3,  7,5,  0,3,  1,5, 4,4,  5,5 , 3,0 };
			//long result = 0;
			//change register A, so the program produces a copy of itself

			//both input programs get a new output at multiples of 8
			//Output length 1 at 0
			//Output length 2 at 8
			//Output length 3 at 64
			//Output length 4 at 512
			//Output length 5 at 4096
			//Output length 6 at 32768
			//Output length 7 at 262144
			//Output length 8 at 2097152
			//Output length 9 at 16777216
			//Output length 10 at 134217728
			//to get 16 length output 8 ^ 15 =
			//35184372088832
			//5120000000000


			//5,5 = out -> B % 8
			//mod 8 means only the lowest 3 bits are relevant
			//we only care about the LSB of B - still not sure how to generate : (


			matchedBits = 1;
			long aTry = 1;
			//if (commands.Count == 16) aTry = (long)Math.Pow(8, 15);
			aTry = 0;
			int outputCnts = 0;
			sw.Restart();
			target = new long[] { 7, 5, 5, 3, 0 };
			while (true) {
				ResetPart2(aTry);
				//0 0 2 0
				//long[] test = new long[output.Count];
				//output.CopyTo(test);
				//Reset(aTry);
				if (HandCoded2()) {

					//HandCoded2(); //6 3 0
					//6 3 0
					instructionPointer = 0;
					registers[4] = aTry;
					registers[5] = 0;
					registers[6] = 0;
					output = new List<long>();
					SolvePart1();
					return aTry;
				}
				//}
				//if (actOut < 3) {
				//	Console.WriteLine("Gotcha!");
				//}
				//Trying A = 10000000  1324 ms
				//Output: 6,4,1,6,5,3,1,2

				//if (Enumerable.SequenceEqual(output, commands)) return aTry;
				//if (output.Count >= 16 && output[15] == 0) {
				//	Console.WriteLine($"Trying A={aTry}  {sw.ElapsedMilliseconds} ms");
				//	Console.WriteLine($"Output: {string.Join(",", output)}");
				//}
				//if (commands.Count == 16 && output.Skip(output.Count - 1).First() == 0) {
				//	Console.WriteLine($"Trying A={aTry}  {sw.ElapsedMilliseconds} ms");
				//	Console.WriteLine($"Output: {string.Join(",", output)}");
				//}
				if (aTry % 1000000000 == 0) {  //2226 ms per 1m
					Console.WriteLine($"Trying A={aTry}  {sw.ElapsedMilliseconds} ms");
					Console.WriteLine($"Output: {string.Join(",", output)}");
					sw.Restart();
				}
				//Log(aTry);
				//if (outputCnts < output.Count) {
				//	outputCnts = output.Count;
				//	Console.WriteLine($"Output length {outputCnts} at {aTry}");
				//}

				//if (commands.Count == 16) {
				//	Console.WriteLine($"Trying A={aTry}  {sw.ElapsedMilliseconds} ms");
				//	Console.WriteLine($"Output: {string.Join(",", output)}");
				//	if (output.Count == 16) {
				//		Console.WriteLine($"Gotcha 16 length output!");
				//	}
				//	aTry *= 8;
				//} else
				aTry++;

			}
		}
	}
}
