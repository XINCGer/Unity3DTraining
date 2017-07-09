using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyweightPattern
{
    /// <summary>
    /// 抽象图片类
    /// </summary>
    public abstract class ImageNode
    {
        public abstract void Show();
    }

    /// <summary>
    /// 从抽象图片类派生出来图片类
    /// </summary>
    class CImageNode : ImageNode
    {
        /// <summary>
        /// 代表图片类型的变量
        /// </summary>
        private string imageType;

        public CImageNode(string imageType)
        {
            this.imageType = imageType;
        }

        public override void Show()
        {
            Console.WriteLine("图片是:"+imageType);
        }
    }
}
