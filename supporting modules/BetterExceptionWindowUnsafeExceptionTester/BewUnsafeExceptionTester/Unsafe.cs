using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BewUnsafeExceptionTester
{
    public static class Unsafe
    {
        public static unsafe void TriggerAccessViolation()
        {
            byte* p = null;
            *p = 0;
        }
    }
}
