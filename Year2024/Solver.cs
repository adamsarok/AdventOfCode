namespace Year2024 {
    public abstract class Solver {
        protected virtual int DayNum { get; }
        const int YEAR = 2024;
        public enum PartsToRun { Day1, Day2, Both }
        public enum InputType { Short, Final }
        public void Solve(InputType inputType = InputType.Final, PartsToRun partsToRun = PartsToRun.Both) {
            string inputFile = inputType == InputType.Short ? $"inputs\\{YEAR}shortinput{DayNum}.txt" : $"inputs\\{YEAR}input{DayNum}.txt";
            if (partsToRun == PartsToRun.Day1 || partsToRun == PartsToRun.Both) {
                ReadInputPart1(inputFile);
                var r1 = SolvePart1();
                Console.WriteLine($"Part1: {r1}");
            }
            if (partsToRun == PartsToRun.Day2 || partsToRun == PartsToRun.Both) {
                ReadInputPart2(inputFile);
                var r2 = SolvePart2();
                Console.WriteLine($"Part2: {r2}");
            }
        }
        protected abstract long SolvePart1();
        protected abstract long SolvePart2();
        protected abstract void ReadInputPart1(string fileName);
        protected abstract void ReadInputPart2(string fileName);
    }
}
