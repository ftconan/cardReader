using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardReader
{
    class Temp
    {
        public TagMsg Tagmsg { get; set; }
        public long Count { get; set; }

        public Temp(TagMsg msg)
        {
            this.Tagmsg = msg;
            Count = 1;
        }
    }
}
