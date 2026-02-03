using System;
using System.Collections.Generic;
using System.Linq;
using Prova.FsCheck;

namespace Prova.FsCheck.Sample
{
    public class FsCheckTests
    {
        [Property]
        public void ReversingAListTwiceYieldsOriginalList(List<int> list)
        {
            var reversed = Enumerable.Reverse(list).ToList();
            var doubleReversed = Enumerable.Reverse(reversed).ToList();
            
            // Should be equal
            // Using basic assertion or manual check
            bool equal = list.SequenceEqual(doubleReversed);
            if (!equal) throw new Exception("List not equal after double reverse");
        }

        [Property(MaxTest = 1000)]
        public void AdditionIsCommutative(int a, int b)
        {
            if (a + b != b + a) throw new Exception("Math is broken");
        }

        // Falsifiable test (uncomment to test failure)
        /*
        [Property]
        public void ThisShouldFail(int a)
        {
            if (a > 100) throw new Exception("Fail on large numbers");
        }
        */
    }
}
