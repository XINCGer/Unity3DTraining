using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpJExcel.Jxl;

namespace Xls2Lua
{

    /// <summary>
    /// Xls表格转换处理核心类
    /// </summary>
    public class XlsTransfer
    {

        /// <summary>
        /// 根据字符串返回对应字段类型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static FieldType StringToFieldType(string str)
        {
            str = str.Trim();
            str = str.ToLower();
            if ("int32" == str)
                return FieldType.c_int32;
            else if ("int64" == str)
                return FieldType.c_int64;
            else if ("bool" == str)
                return FieldType.c_bool;
            else if ("float" == str)
                return FieldType.c_float;
            else if ("double" == str)
                return FieldType.c_double;
            else if ("string" == str)
                return FieldType.c_string;
            else if ("uint32" == str)
                return FieldType.c_uint32;
            else if ("uint64" == str)
                return FieldType.c_uint64;
            else if ("fixed32" == str)
                return FieldType.c_fixed32;
            else if ("fixed64" == str)
                return FieldType.c_fixed64;
            return FieldType.c_unknown;
        }

        /// <summary>
        /// 根据字段类型，返回对应的字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string FieldTypeToString(FieldType type)
        {
            if (type == FieldType.c_int32)
            {
                return "int32";
            }
            else if (type == FieldType.c_int64)
            {
                return "int64";
            }
            else if (type == FieldType.c_bool)
            {
                return "bool";
            }
            else if (type == FieldType.c_float)
            {
                return "float";
            }
            else if (type == FieldType.c_double)
            {
                return "double";
            }
            else if (type == FieldType.c_string)
            {
                return "string";
            }
            else if (type == FieldType.c_uint32)
            {
                return "uint32";
            }
            else if (type == FieldType.c_uint64)
            {
                return "uint64";
            }
            else if (type == FieldType.c_fixed32)
            {
                return "fixed32";
            }
            else if (type == FieldType.c_fixed64)
            {
                return "fixed64";
            }
            return "";
        }

        /// <summary>
        /// 获取表格的列数，表头碰到空白列直接中断
        /// </summary>
        public static int GetSheetColoumns(Sheet sheet)
        {
            int coloum = sheet.getColumns();
            for (int i = 0; i < coloum; i++)
            {
                string temp1 = sheet.getCell(i, 2).getContents();
                string temp2 = sheet.getCell(i, 3).getContents();
                string temp3 = sheet.getCell(i, 4).getContents();
                if (string.IsNullOrWhiteSpace(temp1) || string.IsNullOrWhiteSpace(temp2) || string.IsNullOrWhiteSpace(temp3))
                {
                    return i;
                }
            }
            return coloum;
        }
    }
}
