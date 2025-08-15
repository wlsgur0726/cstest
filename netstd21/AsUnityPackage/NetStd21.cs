using System;
using System.Collections.Generic;
using System.Text;

#if UNITY_2017_1_OR_NEWER
// Suppress CS8632: The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#nullable enable
#endif

namespace netstd21
{
    public static class NetStd21
    {
        public static int Test(params string[] args)
        {
            Console.WriteLine("Hello from netstd21! 한글테스트");
            return 0;
        }
    }
}
