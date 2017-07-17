using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdapterPattern
{
    /// <summary>
    /// 适配器接口
    /// </summary>
    public interface PowerPortAdapter
    {
        void PowerSupply();
    }

    /// <summary>
    /// 110V电源适配器类
    /// </summary>
    class Adapter110V : PowerPort110V, PowerPortAdapter
    {
        NoteBook noteBook = new NoteBook();
        
        public void PowerSupply()
        {
            base.PowerSupply();
            Console.WriteLine("适配器将电源转换成了笔记本需要的！");
            noteBook.Work();
        }
    }

    class Adapter220V: PowerPort220V, PowerPortAdapter
    {
        NoteBook noteBook = new NoteBook();

        public void PowerSupply()
        {
            base.PowerSupply();
            Console.WriteLine("适配器将电源转换成了笔记本需要的！");
            noteBook.Work();
        }
    }
}
