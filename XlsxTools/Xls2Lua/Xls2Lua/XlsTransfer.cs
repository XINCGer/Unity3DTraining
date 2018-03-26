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

        /// <summary>
        /// 获取表格行数，行开头是空白直接中断
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public static int GetSheetRows(Sheet sheet)
        {
            int rows = sheet.getRows();
            for (int i = 0; i < sheet.getRows(); i++)
            {
                if (i >= 5)
                {
                    if (string.IsNullOrEmpty(sheet.getCell(0, i).getContents()))
                    {
                        return i;
                    }
                }
            }
            return rows;
        }

        /// <summary>
        /// 获取当前Sheet切页的表头信息
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public static List<ColoumnDesc> GetColoumnDesc(Sheet sheet)
        {
            int coloumnCount = GetSheetColoumns(sheet);
            List<ColoumnDesc> coloumnDescList = new List<ColoumnDesc>();
            for (int i = 0; i < coloumnCount; i++)
            {
                string comment = sheet.getCell(i, 0).getContents().Trim();
                comment = string.IsNullOrWhiteSpace(comment) ? comment : comment.Split('\n')[0];
                string typeStr = sheet.getCell(i, 1).getContents().Trim();
                string nameStr = sheet.getCell(i, 2).getContents().Trim();

                bool isArray = typeStr.Contains("[]");
                typeStr = typeStr.Replace("[]", "");
                FieldType fieldType;
                if (typeStr.ToLower().StartsWith("struct-"))
                {
                    typeStr = typeStr.Remove(0, 7);
                    fieldType = FieldType.c_struct;
                }
                else if (typeStr.ToLower().StartsWith("enum-"))
                {
                    typeStr.Remove(0, 5);
                    fieldType = FieldType.c_enum;
                }
                else
                {
                    fieldType = StringToFieldType(typeStr);
                }
                ColoumnDesc coloumnDesc = new ColoumnDesc();
                coloumnDesc.index = i;
                coloumnDesc.comment = comment;
                coloumnDesc.typeStr = typeStr;
                coloumnDesc.name = nameStr;
                coloumnDesc.type = fieldType;
                coloumnDescList.Add(coloumnDesc);
            }
            return coloumnDescList;
        }

        /// <summary>
        /// 生成最后的lua文件
        /// </summary>
        /// <param name="coloumnDesc"></param>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private static string GenLuaFile(List<ColoumnDesc> coloumnDesc, Sheet sheet)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("--[[Notice:This lua config file is auto generate by Xls2Lua Tools，don't modify it manually! --]]");
            if (null == coloumnDesc || coloumnDesc.Count <= 0)
            {
                return stringBuilder.ToString();
            }
            //创建索引
            Dictionary<string, int> fieldIndexMap = new Dictionary<string, int>();

        }
    }
}
