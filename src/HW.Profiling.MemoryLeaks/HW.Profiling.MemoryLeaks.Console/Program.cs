using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HW.Profiling.MemoryLeaks.Console.Managed;
using HW.Profiling.MemoryLeaks.Console.Unmanaged;

namespace HW.Profiling.MemoryLeaks.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            LeakManaged leakManaged = new LeakManaged();
            leakManaged.EventLeak();
            leakManaged.EventLeakStatic();
            leakManaged.WpfDataBinding();
            leakManaged.StreamsLeaks();



            LeakUnmanaged leakUnmanaged = new LeakUnmanaged();
            leakUnmanaged.NotClearingUnmanagedMemory();
            leakUnmanaged.DllImportPtrs();
            leakUnmanaged.UnsafeCode();

            System.Console.ReadKey();
        }
    }
}
