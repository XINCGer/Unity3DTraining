using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverPattern
{
    class Spy
    {
        private List<Country> listener= new List<Country>();
        private string intelligence;

        /// <summary>
        /// 添加监听，进入国家
        /// </summary>
        public void Attach(Country country)
        {
            listener.Add(country);
        }

        /// <summary>
        /// 取消监听，离开国家
        /// </summary>
        /// <param name="country"></param>
        public void Detach(Country country)
        {
            listener.Remove(country);
        }

        /// <summary>
        /// 通知消息
        /// </summary>
        public void Notify()
        {
            foreach (var enumator in listener)
            {
                enumator.Update();
            }    
        }

        public void SetIntelligence(string intelligence)
        {
            this.intelligence = intelligence;
        }

        public string GetIntelligence()
        {
            return this.intelligence;
        }
    }
}
