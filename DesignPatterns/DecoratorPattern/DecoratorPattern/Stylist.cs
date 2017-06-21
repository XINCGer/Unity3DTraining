using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPattern
{
    /// <summary>
    /// 造型师父类
    /// </summary>
    class Stylist : Actor
    {
        protected Actor actor;

        public void MakeUp(Actor actor)
        {
            this.actor = actor;
        }

        /// <summary>
        /// 造型师造型
        /// </summary>
        public override void Act()
        {
            if (null != actor)
            {
                actor.Act();
            }
        }
    }

    /// <summary>
    /// 古装造型师
    /// </summary>
    class AncientStylist : Stylist
    {
        private void ShowRole()
        {
            Console.WriteLine("演员将要拍摄古装剧！");
        }

        private void MakeUpForActor()
        {
            Console.WriteLine("古装剧化妆！");
        }

        public override void Act()
        {
            ShowRole();
            MakeUpForActor();
            base.Act();
        }
    }

    /// <summary>
    /// 现代造型师类
    /// </summary>
    class ModernStyList : Stylist
    {
        private void ShowRule()
        {
            Console.WriteLine("演员将要拍摄现代剧！");
        }

        private void MakeUpForActor()
        {
            Console.WriteLine("现代剧化妆！");
        }

        public override void Act()
        {
            ShowRule();
            MakeUpForActor();
            base.Act();
        }
    }
}
