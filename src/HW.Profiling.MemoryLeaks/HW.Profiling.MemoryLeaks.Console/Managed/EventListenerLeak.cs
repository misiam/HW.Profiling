using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW.Profiling.MemoryLeaks.Console.Managed
{
    public class EventListenerLeak
    {
        private readonly string _name;

        public EventListenerLeak(string name)
        {
            _name = name;
        }

        public void Subscribe(LeakManaged leakManaged)
        {
            leakManaged.EventLeakProp += LeakManagedOnEventLeakProp;
        }
        public void Unsubscribe(LeakManaged leakManaged)
        {
            leakManaged.EventLeakProp -= LeakManagedOnEventLeakProp;
        }

        public void SubscribeStatic()
        {
            LeakManaged.EventLeakStaticProp += LeakManagedOnEventLeakProp;
        }
        public void UnsubscribeStatic()
        {
            LeakManaged.EventLeakStaticProp -= LeakManagedOnEventLeakProp;
        }


        private void LeakManagedOnEventLeakProp(object sender, EventArgs eventArgs)
        {
            //some event handler

            System.Console.WriteLine("Leak: " + _name);
        }


        public void DoWork()
        {
            //some work
        }
    }
}
