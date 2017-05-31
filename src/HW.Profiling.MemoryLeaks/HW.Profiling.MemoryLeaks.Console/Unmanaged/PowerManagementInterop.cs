using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HW.Profiling.MemoryLeaks.Console.Unmanaged
{
    internal class PowerManagementInterop
    {

    [DllImport("PowrProf.dll", SetLastError = true)]
    public static extern uint CallNtPowerInformation(
        int informaitonLevel,
        IntPtr inputBuffer,
        int inputBufSize,
        IntPtr outputBuffer,
        int outputBufferSize);
    }
}
