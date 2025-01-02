using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Helpers {
	public class Helpers {
		public static int GCF(int a, int b) {
			while (b != 0) {
				int temp = b;
				b = a % b;
				a = temp;
			}
			return a;
		}

		public static int LCM(int a, int b) {
			return (a / GCF(a, b)) * b;
		}
	}
}
