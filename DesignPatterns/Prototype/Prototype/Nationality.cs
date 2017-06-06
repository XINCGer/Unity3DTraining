using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype
{
    class Nationality : ICloneable
    {
        private String nation;

        public string Nation
        {
            get
            {
                return nation;
            }

            set
            {
                nation = value;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
