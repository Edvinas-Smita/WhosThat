using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Backend.Models
{
	public class LazyClass
	{
		int lazy;
		public LazyClass(int l){lazy = l;}
        public void Lazy() { }
	}
}