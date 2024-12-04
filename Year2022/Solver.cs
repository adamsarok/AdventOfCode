namespace Year2022 {
    public abstract class Solver {
		protected int DayNum = 0;
		public Solver(int dayNum) {
			DayNum = dayNum;
		}
        const int YEAR = 2022;
        public void Solve() {
            string shortInput = $"Day{DayNum}\\{YEAR}shortinput{DayNum}.txt";
			string input = $"Day{DayNum}\\{YEAR}input{DayNum}.txt";
            ReadInputPart1(shortInput);
            var r = SolvePart1();
            Console.WriteLine($"Part1 short: {r}");
			ReadInputPart1(input);
			r = SolvePart1();
			Console.WriteLine($"Part1 final: {r}");
			ReadInputPart2(shortInput);
			r = SolvePart2();
			Console.WriteLine($"Part2 short: {r}");
			ReadInputPart2(input);
			r = SolvePart2();
			Console.WriteLine($"Part2 final: {r}");
		}
        protected abstract long SolvePart1();
        protected abstract long SolvePart2();
        protected abstract void ReadInputPart1(string fileName);
        protected abstract void ReadInputPart2(string fileName);
    }
}
