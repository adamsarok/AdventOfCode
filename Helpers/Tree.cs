using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers {
	public record Tree<T>(T value) {
		public T Value => value;
		public Tree<T>? Parent { get; set; }
		public List<Tree<T>> Children { get; set; } = new List<Tree<T>>();
	}
}
