using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<MyData> dataList = new List<MyData>();
            MyData myData = new MyData();
            myData.name = "A";
            myData.index = 1;
            myData.state=MyState.Euqip;
            dataList.Add(myData);

            myData = new MyData();
            myData.name = "B";
            myData.index = 2;
            myData.state=MyState.Own;
            dataList.Add(myData);

            myData= new MyData();
            myData.name = "C";
            myData.index = 3;
            myData.state=MyState.NotOwn;
            dataList.Add(myData);

            dataList.Sort((x, y) =>
            {
                //return -x.index.CompareTo(y.index);
                return y.state.CompareTo(x.state);
            });

            foreach (var tmp in dataList)
            {
                Console.WriteLine(tmp.name);
            }
        }
    }

    class MyData
    {
        public string name;
        public int index;
        public MyState state;
    }

    enum MyState
    {
        Euqip,
        Own,
        NotOwn,
    }
}
