using Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Year2024.Day17 {
	public class Day17 : Solver {
		public Day17() : base(2024, 17) {
		}
		List<long> commands;
		List<long> output;
		long Binit, Cinit;
		int instructionPointer = 0;

		long[] registers = new long[7];
		protected override void ReadInputPart1(string fileName) {
			var ccc = To3bits(9);
			commands = new List<long>();
			var f = File.ReadAllLines(fileName);
			registers[0] = 0; 
			registers[1] = 1;
			registers[2] = 2; 
			registers[3] = 3;
			registers[4] = GetRegister(f[0]);
			registers[5] = GetRegister(f[1]);
			registers[6] = GetRegister(f[2]);
			Binit = registers[5]; 
			Cinit = registers[6];
			foreach (var c in f[4].Split(':')[1].Split(",")) {
				commands.Add(int.Parse(c));
			}
		}
		private int GetRegister(string r) {
			return int.Parse(r.Split(':')[1]);
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
			Console.WriteLine($"Output: {string.Join(",", output)}");
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

		//Program: 2,4	1,3	 7,5  0,3  1,5  4,4  5,5  3,0
		//2,4: B = A % 8
		//1,3: B = B xor 3
		//7,5: C = A >> B
		//0,3: A = A >> 3
		//1,5: B = B xor 5
		//4,4: B = B xor C
		//5,5: out -> B % 8
		//3,0: while A != 0 jump to 0

		private List<long> HandCoded4(long a, long b, long c) {
			List<long> output = new List<long>();
			while (a != 0) {
				b = ((a % 8) ^ 3); //2,4 1,3
				c = a >> (int)b; //7,5 
				a = a >> 3; //0,3
				b = b ^ 5; //1,5
				b = b ^ c; //4,4
				output.Add(b % 8); //5,5
			}
			return output;
		}

		protected override long SolvePart2() {
			long current = 0;
			long[] target = new long[] { 2, 4, 1, 3, 7, 5, 0, 3, 1, 5, 4, 4, 5, 5, 3, 0 };

			for (int digit = target.Length - 1; digit >= 0; digit -= 1) {
				for (int i = 0; i < int.MaxValue; i++) {
					var candidate = current + (1L << (digit * 3)) * i;
					var output = HandCoded4(candidate, 0, 0);
					if (output.Skip(digit).SequenceEqual(target.Skip(digit))) {
						current = candidate;
						Console.WriteLine($"Current is now {Convert.ToString(current, 8)} (octal)");
						break;
					}
				}
			}

			Console.WriteLine($"Part 2: {current} / {Convert.ToString(current, 8)} (decimal / octal)");
			return 0;
		}
	}
}
