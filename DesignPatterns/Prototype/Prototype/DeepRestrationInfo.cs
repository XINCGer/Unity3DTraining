using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype
{
    class DeepRestrationInfo
    {
        private string name;
        private string birthday;
        private string school;
        private string id;
        private Nationality nationality;

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

        //含参构造函数，供普通创建过程使用
        public DeepRestrationInfo(string name)
        {
            this.name = name;
            nationality = new Nationality();
        }

        //供克隆方法使用的私有构造函数
        private DeepRestrationInfo(Nationality nation)
        {
            this.nationality = nation.Clone() as Nationality;
        }

        //展示报名信息
        public void Show()
        {
            Console.WriteLine("姓名：" + name);
            Console.WriteLine("出生年月：" + birthday);
            Console.WriteLine("毕业院校：" + school);
            Console.WriteLine("身份证号：" + id);
            Console.WriteLine("国籍：" + nationality.Nation);
        }

        public void SetNation(string nation)
        {
            nationality.Nation = nation;
        }
        public object Clone()
        {
            return MemberwiseClone();
        }

        public object DeepClone()
        {
            DeepRestrationInfo obj = new DeepRestrationInfo(this.nationality);
            obj.name = this.name;
            obj.birthday = this.birthday;
            obj.id = this.id;
            obj.school = this.school;
            return obj;
        }
    }
}
