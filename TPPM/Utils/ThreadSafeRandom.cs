using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPPM.Utils
{
    public static class ThreadSafeRandom
    {
        [ThreadStatic] private static Random _local;
        public static Random CurrentThreadRandom => _local ??= new Random(Guid.NewGuid().GetHashCode());
    }

}
