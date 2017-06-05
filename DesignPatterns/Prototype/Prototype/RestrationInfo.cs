using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype
{
    class RestrationInfo:ICloneable
    {
        private string name;
        private string birthday;
        private string school;
        private string id;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Birthday
        {
            get
            {
                return birthday;
            }

            set
            {
                birthday = value;
            }
        }

        public string School
        {
            get
            {
                return school;
            }

            set
            {
                school = value;
            }
        }

        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public RestrationInfo(string name)
        {
            this.name = name;
        }

        //展示报名信息
        public void Show()
        {
            Console.WriteLine("姓名："+name);
            Console.WriteLine("出生年月：" + birthday);
            Console.WriteLine("毕业院校：" + school);
            Console.WriteLine("身份证号：" + id);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
