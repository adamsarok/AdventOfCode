using Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2024 {
	public class Day2 : IAocSolver {
		List<List<int>> reports = new List<List<int>>();

		public long SolvePart1(string[] input) {
			reports = new List<List<int>>();
			foreach (var l in input) {
				var report = new List<int>();
				foreach (var ll in l.Split(' ')) {
					report.Add(int.Parse(ll));
				}
				reports.Add(report);
			}
			long result = 0;
			foreach (var report in reports) {
				if (CheckReport(report)) result++;
			}
			return result;
		}

		public long SolvePart2(string[] input) {
			reports = new List<List<int>>();
			foreach (var l in input) {
				var report = new List<int>();
				foreach (var ll in l.Split(' ')) {
					report.Add(int.Parse(ll));
				}
				reports.Add(report);
			}
			long result = 0;
			foreach (var report in reports) {
				bool good = false;
				if (!CheckReport(report)) {
					for (int i = 0; i < report.Count; i++) {
						var c = new List<int>(report);
						c.RemoveAt(i);
						if (CheckReport(c)) {
							good = true;
							continue;
						}
					}
				} else good = true;
				if (good) {
					result++;
				}
			}
			return result;
		}

		private bool CheckReport(List<int> report) {
			int lastDiff = 0;
			for (int i = 0; i < report.Count - 1; i++) {
				var diff = report[i] - report[i + 1];
				if (diff > 3 || diff == 0 || diff < -3 ||
					(lastDiff > 0 && diff < 0) ||
					(lastDiff < 0 && diff > 0)) {
					return false;
				}
				lastDiff = diff;
			}
			return true;
		}
	}
}
