using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace HW.Profiling.MemoryLeaks.Console.Unmanaged
{
    public class LeakUnmanaged
    {
        public LeakUnmanaged()
        {
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine(@"
=======================================================================
LeakUnmanaged
=======================================================================
            ");
        }


        #region Marshal.AllocHGlobal leak


        public void NotClearingUnmanagedMemory()
        {
            System.Console.Write(@"
             * ----------------------------------------------------------------------- 
             * NotClearingUnmanagedMemory
             * -----------------------------------------------------------------------
             * In Leaked() method AllocHGlobal allocates memory for some work in DoWork() method.
             * If DoWork() doesn't free memory of that pointer that will cause memory leak.
             * 
             * To avoid that, FreeHGlobal should be used to free allocated memory
             *  
             * -----------------------------------------------------------------------
             *");
            Leaked();
            Fixed();
        }

        private void Leaked()
        {
            for (var i = 0; i < 10000; i++)
            {
                IntPtr ptr =  Marshal.AllocHGlobal(7000);

                DoWork(ptr);

                // 
            }
        }

        private void Fixed()
        {
            for (var i = 0; i < 10000; i++)
            {
                IntPtr ptr = Marshal.AllocHGlobal(7000);

                DoWork(ptr);

                if (ptr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptr);
                    ptr = IntPtr.Zero;
                }
            }
        }


        private void DoWork(IntPtr intPtr)
        {
            // do some work
        }

        #endregion  Marshal.AllocHGlobal leak


        #region CallingConvention

        public void DllImportPtrs()
        {
            System.Console.Write(@" 
             * ----------------------------------------------------------------------- 
             * DllImportPtrs
             * -----------------------------------------------------------------------
             * For DllImport resources could be cleaned up by caller or by callee.
             * The best option is to follow specification of the extern method. 
             * E.g. for PowrProf.dll caller allocates resources and deallocates them.
             * 
             * -----------------------------------------------------------------------
             *");

            long lastSleepTimeTicks = GetStructure_Leak<long>(15 /*last sleep time*/);

            lastSleepTimeTicks = GetStructure_Fixed<long>(15 /*last sleep time*/);


        }


        private T GetStructure_Leak<T>(int informationLevel)
        {
            var informaitonLevel = informationLevel;
            IntPtr lpInBuffer = IntPtr.Zero;
            int inputBufSize = 0;
            int outputPtrSize = Marshal.SizeOf<T>();
            IntPtr outputPtr = Marshal.AllocCoTaskMem(outputPtrSize);

            var retval = PowerManagementInterop.CallNtPowerInformation(
                informaitonLevel,
                lpInBuffer,
                inputBufSize,
                outputPtr,
                outputPtrSize);

            if (retval == 0 /* SUCCESS */)
            {
                var obj = Marshal.PtrToStructure<T>(outputPtr);
                return obj;
            }
            else
            {
                throw new Win32Exception();
            }
        }
        private T GetStructure_Fixed<T>(int informationLevel)
        {
            var informaitonLevel = informationLevel;
            IntPtr lpInBuffer = IntPtr.Zero;
            int inputBufSize = 0;
            int outputPtrSize = Marshal.SizeOf<T>();
            IntPtr outputPtr = Marshal.AllocCoTaskMem(outputPtrSize);

            var retval = PowerManagementInterop.CallNtPowerInformation(
                informaitonLevel,
                lpInBuffer,
                inputBufSize,
                outputPtr,
                outputPtrSize);


            if (retval == 0 /* SUCCESS */)
            {
                var obj = Marshal.PtrToStructure<T>(outputPtr);
                Marshal.FreeCoTaskMem(outputPtr);

                return obj;
            }
            else
            {
                Marshal.FreeCoTaskMem(outputPtr);
                throw new Win32Exception();
            }
        }
        #endregion


        #region UnsafeCode

        public void UnsafeCode()
        {
            System.Console.Write(@"
             * ----------------------------------------------------------------------- 
             * UnsafeCode
             * -----------------------------------------------------------------------
             * It is possible to use 'unsafe' code in c#.
             * However GC is working even here, so e.g. next code would not have memory leak:
             * 
               unsafe
               {
                    while (true) 
                        new int();
               }
             * 
             * After using GC.Collect() all int objects will be collected.
             * Infinity loop exitst here, so that code is not enabled for execution in current sample
             *  
             * -----------------------------------------------------------------------
             *");
            
        }



        #endregion

    }

}
