using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieMiner
{
	interface IMiner
	{
		string Url { get; }

		void Mine();
	}
}