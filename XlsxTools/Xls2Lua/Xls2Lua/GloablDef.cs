using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xls2Lua
{
    /// <summary>
    /// 表格字段类型的枚举
    /// </summary>
    public enum FieldType : byte
    {
        c_unknown,
        c_int32,
        c_int64,
        c_bool,
        c_float,
        c_double,
        c_string,
        c_uint32,
        c_uint64,
        c_fixed32,
        c_fixed64,
        c_enum,
        c_struct
    }

    /// <summary>
    /// 表头字段描述
    /// </summary>
    public class ColoumnDesc
    {
        public int index = -1;
        public string comment = "";
        public string typeStr = "";
        public string name = "";
        public FieldType type;
        public bool isArray = false;
    }
}
