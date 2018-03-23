#define UseEvent
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDelegateException
{

    class Program
    {
        private static event Action multiEvent;
        //定义一个委托
        private static Action multiDelegate;

        private static GetResult getResultDelegate;

        static void Main(string[] args)
        {
#if UseDelegate

            //创建多播委托
            multiDelegate = MultiDelegate.Func1;
            multiDelegate += MultiDelegate.Func2;

            //调用委托，观察结果
            try
            {
                multiDelegate();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("---------------------------分割线------------------------------");
            //手动迭代委托方法列表，可以处理抛出异常后委托链终止执行的问题
            //定义方法列表数组，使用GetInvocationList()  
            //注意使用的是Delegate类，不是delegate关键字  
            Delegate[] myDelegates = multiDelegate.GetInvocationList();
            foreach (var @delegate in myDelegates)
            {
                var delegateItem = (Action) @delegate;
                //分别调用委托
                try
                {
                    delegateItem();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
#elif UseEvent
            //依次注册事件
            multiEvent += MultiDelegate.Func1;
            multiEvent += MultiDelegate.Func2;

            //调用事件，观察结果
            try
            {
                multiEvent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("---------------------------分割线------------------------------");
            //手动迭代委托方法列表，可以处理抛出异常后委托链终止执行的问题
            //定义方法列表数组，使用GetInvocationList()  
            //注意使用的是Delegate类，不是delegate关键字  

            Delegate[] myDelegates = multiEvent.GetInvocationList();
            foreach (var @delegate in myDelegates)
            {
                var delegateItem = @delegate as Action;
                try
                {
                    delegateItem();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
#endif
            Console.WriteLine("---------------------------分割线------------------------------");
            getResultDelegate = MultiDelegate.GetOne;
            getResultDelegate += MultiDelegate.GetTwo;

            Console.WriteLine("直接调用委托返回的一般是最后一个方法的返回值：" + getResultDelegate());

            //手动迭代委托方法列表，可以获取并处理每个委托的返回值
            //定义方法列表数组，使用GetInvocationList()  
            //注意使用的是Delegate类，不是delegate关键字 
            int sum = 0;
            Delegate[] resultDelegates = getResultDelegate.GetInvocationList();
            foreach (var @delegate in resultDelegates)
            {
                var delegateItem = @delegate as GetResult;
                sum += delegateItem();
                Console.WriteLine("获取单个委托的返回值:" + delegateItem());
            }
            Console.WriteLine("多个委托的返回值总和：" + sum);
        }
    }

    /// <summary>
    /// 带有返回值的委托
    /// </summary>
    /// <returns></returns>
    public delegate int GetResult();

    class MultiDelegate
    {
        /// <summary>
        /// 会抛出异常的方法1
        /// </summary>
        public static void Func1()
        {
            Console.WriteLine("方法1，会抛出异常！");
            throw new Exception("抛出异常！");
        }

        /// <summary>
        /// 正常方法2
        /// </summary>
        public static void Func2()
        {
            Console.WriteLine("方法2");
        }

        /// <summary>
        /// 带有返回值的函数
        /// </summary>
        /// <returns></returns>
        public static int GetOne()
        {
            return 1;
        }

        /// <summary>
        /// 带有返回值的函数
        /// </summary>
        /// <returns></returns>
        public static int GetTwo()
        {
            return 2;
        }
    }

}
