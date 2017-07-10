using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyweightPattern
{
    /// <summary>
    /// 共享图片工厂类
    /// </summary>
    class ImageNodeFactory
    {
        private Dictionary<string, CImageNode> imageNodes = new Dictionary<string, CImageNode>();

        public ImageNodeFactory()
        {
           imageNodes.Add("Lion",new CImageNode("狮子"));
           imageNodes.Add("Tiger",new CImageNode("狮子"));
           imageNodes.Add("Cat",new CImageNode("猫"));
           imageNodes.Add("Dog",new CImageNode("狗"));
        }

        public ImageNode GetImage(string imageType)
        {
            if (imageNodes.ContainsKey(imageType))
            {
                return imageNodes[imageType];
            }
            return null;
        }
    }
}
