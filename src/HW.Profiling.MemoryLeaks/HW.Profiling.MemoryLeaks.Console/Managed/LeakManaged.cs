using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW.Profiling.MemoryLeaks.Console.Managed
{
    public class LeakManaged
    {
        public event EventHandler EventLeakProp;
        public static event EventHandler EventLeakStaticProp;

        public LeakManaged()
        {
            System.Console.WriteLine(@"
           
=======================================================================
LeakUnmanaged
=======================================================================


             * In managed code memory leaks could be provided by the following reasons:
             * - Holding references to managed objects
             * - Failing to release unmanaged resources ( it is more related to unmanaged resources section)
             * - Bugs in .Net (very rare now. In .NET 3.0 - 3.5 version exists issues with BitmapImage usages, CMilChannel in XP, ShutdownListener in WPF and etc.)


            ");
        }

        #region EventLeak 

        public void EventLeak()
        {
            System.Console.Write(@"
             * ----------------------------------------------------------------------- 
             * EventLeak
             * -----------------------------------------------------------------------
             * If some object is subscribed to event, its reference is alive while host of event is alive.
             * If listener is no need more, it should be unsubscribed from the event in apropriate plase or in Finalizer.
             * Another one solution is to use WeakEventManager.
             *  
             * -----------------------------------------------------------------------
             *");

            System.Console.WriteLine("");
            System.Console.WriteLine(">> Run EventLeak_Leaked");
            EventLeak_Leaked();
            System.Console.WriteLine("");
            System.Console.WriteLine(">> Run EventLeak_Fixed");
            EventLeak_Fixed();
            System.Console.WriteLine("");


        }

        private void EventLeak_Leaked()
        {
            for (int i = 0; i < 10; i++)
            {
                var eventListenerLeak = new EventListenerLeak("EventLeak_Leaked_" + i);
                eventListenerLeak.Subscribe(this);
                // all EventListenerLeak objects are still linked to parent object

            }
            OnEventLeakProp();
        }

        private void EventLeak_Fixed()
        {
            for (int i = 0; i < 10; i++)
            {
                var eventListenerLeak = new EventListenerLeak("EventLeak_Fixed_" + i);
                eventListenerLeak.Subscribe(this);
                eventListenerLeak.Unsubscribe(this);
            }
            OnEventLeakProp();
        }

        protected virtual void OnEventLeakProp()
        {
            EventLeakProp?.Invoke(this, EventArgs.Empty);
        }

        #endregion EventLeak

        #region EventLeakStatic

        public void EventLeakStatic()
        {
            System.Console.Write(@"
             * ----------------------------------------------------------------------- 
             * EventLeakStatic
             * -----------------------------------------------------------------------
             * EventLeakStatic is very similar to event leaks, but GC will not collect these objects until application is closed or objects are unsubscribed.
             * 
             *  
             * -----------------------------------------------------------------------
             *");

            System.Console.WriteLine("");
            System.Console.WriteLine(">> Run EventLeakStatic_Leaked");
            EventLeakStatic_Leaked();
            System.Console.WriteLine("");
            System.Console.WriteLine(">> Run EventLeakStatic_Fixed");
            EventLeakStatic_Fixed();
            System.Console.WriteLine("");


        }

        private void EventLeakStatic_Leaked()
        {
            for (int i = 0; i < 10; i++)
            {
                var eventListenerLeak = new EventListenerLeak("EventLeakStatic_Leaked " + i);
                eventListenerLeak.SubscribeStatic();
                // all EventListenerLeak objects are still linked to static property

            }
            OnEventLeakStaticProp();
        }

        private void EventLeakStatic_Fixed()
        {
            for (int i = 0; i < 10; i++)
            {
                var eventListenerLeak = new EventListenerLeak("EventLeakStatic_Leaked " + i);
                eventListenerLeak.SubscribeStatic();
                eventListenerLeak.UnsubscribeStatic();
            }
            OnEventLeakStaticProp();
        }

        protected virtual void OnEventLeakStaticProp()
        {
            EventLeakStaticProp?.Invoke(this, EventArgs.Empty);
        }

        #endregion EventLeakStatic

        #region WPF DataBinding 

        //A memory leak may occur when you use data binding in Windows Presentation Foundation
        public void WpfDataBinding()
        {
            System.Console.Write(@"
             * ----------------------------------------------------------------------- 
             * WpfDataBinding
             * -----------------------------------------------------------------------
             * This issue occurs if the following conditions are true:
             *      A data-binding path refers to property P of object X.
             *      Object X contains a direct reference or an indirect reference to the target of the data-binding operation.
             *      Property P is accessed through a PropertyDescriptor object instead of a DependencyProperty object or a PropertyInfo object.
             *  To work around this issue, use one of the following methods:
             *      Access property P through a DependencyProperty object.
             *      Expose the INotifyPropertyChanged interface on object X.
             *      Set the mode of the data binding to OneTime.
             *  
             *  And just to clarify current status of that issue, that behaviour is 'by design.'
             *  
             * -----------------------------------------------------------------------
             *");

        }

        #endregion WPF DataBinding 

        #region Streams

        public void StreamsLeaks()
        {
            System.Console.Write(@"
             * ----------------------------------------------------------------------- 
             * StreamsLeaks
             * -----------------------------------------------------------------------
             * Streams in .NET inmpelents IDispose interface.
             * However if somebody forget to call for Dispose method, resources will be stored in the stream until GC will fire Finaliser.
             * So it is not pure memory leak but for some time memory consumption could be increased.
             * Best practise for avoiding that is to use try/finally with calling dispose or use 'using' construction
             * 
             * -----------------------------------------------------------------------
             *");

            StreamsLeaks_Leak();
            StreamsLeaks_Fixed();
        }

        private void StreamsLeaks_Leak()
        {
            var array = new List<byte>();
            for (int i = 0; i < 1000; i++)
            {
                array.Add( (byte) (i % 200));
                MemoryStream memStrm = new MemoryStream();
                memStrm.Write(array.ToArray<byte>(),0,array.Count);
                DoWork(memStrm);
            }
        }

        private void StreamsLeaks_Fixed()
        {
            var array = new List<byte>();
            for (int i = 0; i < 1000; i++)
            {
                array.Add((byte)(i % 200));
                using (var memStrm = new MemoryStream())
                {
                    memStrm.Write(array.ToArray<byte>(), 0, array.Count);
                    DoWork(memStrm);                   
                }
            }
        }

        private void DoWork(Stream stream)
        {
            //do some work
        }


        #endregion Streams
    }
}
