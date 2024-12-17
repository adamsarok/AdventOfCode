using Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			A = GetRegister(f[0]);
			B = GetRegister(f[1]);
			C = GetRegister(f[2]);
			Binit = B; Cinit = C;
			foreach (var c in f[4].Split(':')[1].Split(",")) {
				commands.Add(int.Parse(c));
			}
		}
		private int GetRegister(string r) {
			return int.Parse(r.Split(':')[1]);
		}
		long A, B, C;
		long Binit, Cinit;
		int instructionPointer = 0;
		private long GetComboOperandValue(long operand) {
			if (operand <= 3) return operand;
			switch (operand) {
				case 4: return A;
				case 5: return B;
				case 6: return C;
				default: throw new Exception("Invalid operand");
			}
		}


		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {
			long result = 0;
			output = new List<long>();
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
			adv, //divide A by 2^B, truncate to int and write to A
			bxl, //bitwise XOR of B and literal, stores in B
			bst, //combo operand % 8 (lowest 3 bits), writes it to B
			jnz, //nothing if A 0, else jumps to literal
				 //if jumps, instruction pointer is not increased
			bxc, //bitwise XOR of B and C, stores in B
			out_,//combo % 8 outputs value
			bdv, //same as adv but store in B
			cdv, //same as adv but store in C
		}
		//0, 5, 3 is tested, the error lies somewhere else
		private void ProcessCommand(long opcode, long operand) {
			var operation = (Opcodes)opcode;
			//Console.WriteLine($"Executing {operation} {operand}, A={A} B={B} C={C}");
			switch (operation) {
				case Opcodes.adv: //OK!
					A = (int)(A / Math.Pow(2, GetComboOperandValue(operand)));
					instructionPointer += 2;
					break;
				case Opcodes.bxl: //seems OK?
					//var op1 = To3bits(B);
					//var op2 = To3bits(operand);
					//B = XOR(op1, op2);
					B = B ^ operand;
					instructionPointer += 2;
					break;
				case Opcodes.bst: //seems OK?
					var op = GetComboOperandValue(operand);
					B = op % 8;
					instructionPointer += 2;
					break;
				case Opcodes.jnz: //OK!
					if (A != 0) instructionPointer = (int)operand;
					else instructionPointer += 2;
					break;
				case Opcodes.bxc:
					//var op4 = To3bits(B);
					//var op5 = To3bits(C);
					//B = XOR(op4, op5);
					B = B ^ C;
					instructionPointer += 2;
					break;
				case Opcodes.out_: //OK!
					var opO = GetComboOperandValue(operand);
					output.Add(opO % 8);
					instructionPointer += 2;
					break;
				case Opcodes.bdv:
					B = (int)(A / Math.Pow(2, GetComboOperandValue(operand)));
					instructionPointer += 2;
					break;
				case Opcodes.cdv:
					C = (int)(A / Math.Pow(2, GetComboOperandValue(operand)));
					instructionPointer += 2;
					break;
				default:
					throw new Exception("Unknown opcode {instruction}");
			}
		}
		private void Reset(long a) {
			output = new();
			A = a;
			B = Binit;
			C = Cinit;
			instructionPointer = 0;
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
		protected override long SolvePart2() {
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
			matchedBits = 1;
			long aTry = 0;
			//if (commands.Count > 10) aTry = (long)Math.Pow(8, 10);
			int outputCnts = 0;
			sw.Restart();
			while (true) {
				Reset(aTry);
				SolvePart1();
				if (Enumerable.SequenceEqual(output, commands)) return aTry;
				Log(aTry);
				//if (outputCnts < output.Count) {
				//	outputCnts = output.Count;
				//	Console.WriteLine($"Output length {outputCnts} at {aTry}");
				//}
				aTry++;
				//Console.WriteLine($"Trying A={aTry}  {sw.ElapsedMilliseconds} ms");
				//Console.WriteLine($"Output: {string.Join(",", output)}");
			}
		}
	}
}
