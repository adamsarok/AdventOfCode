using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2024.Day17 {
	public class Day17 : Solver {
		public Day17() : base(2024, 17) {
		}
		List<int> commands;
		protected override void ReadInputPart1(string fileName) {
			//for (int i = 0; i < 10; i++) {
			//	var t = To3bits(i);
			//}
			commands = new List<int>();
			var f = File.ReadAllLines(fileName);
			A = GetRegister(f[0]);
			B = GetRegister(f[1]);
			C = GetRegister(f[2]);
			foreach (var c in f[4].Split(':')[1].Split(",")) {
				commands.Add(int.Parse(c));
			}
		}
		private int GetRegister(string r) {
			return int.Parse(r.Split(':')[1]);
		}
		int A, B, C;
		int instructionPointer = 0;
		private int GetComboOperandValue(int operand) {
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
			while (instructionPointer < commands.Count - 1) {
				ProcessCommand(commands[instructionPointer], commands[instructionPointer + 1]);
			}
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
			//for (int i = 0; i < input.Count; i++) {
			//	result += i * Math.Pow(3, i + 1);
			//}
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
		//private int XOR(List<int> op1, List<int> op2) {
		//	List<int> result;
		//	for (int i = 0; i < op1.Count; i++) {
		//	}
		//}
		private void ProcessCommand(int opcode, int operand) {
			switch ((Opcodes)opcode) {
				case Opcodes.adv:
					A = (int)(A / Math.Pow(2, B));
					break;
				case Opcodes.bxl:
					var op1 = To3bits(B);
					var op2 = To3bits(operand);
					break;
				case Opcodes.bst:
					break;
				case Opcodes.jnz:
					break;
				case Opcodes.bxc:
					break;
				case Opcodes.out_:
					break;
				case Opcodes.bdv:
					B = (int)(A / Math.Pow(2, B));
					break;
				case Opcodes.cdv:
					C = (int)(A / Math.Pow(2, B));
					break;
				default:
					throw new Exception("Unknown opcode {instruction}");
			}
		}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}
	}
}
