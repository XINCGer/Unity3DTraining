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
        /// 分割字符串的依据
        /// </summary>
        private static readonly char[] splitSymbol = { '|' };

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
                string temp1 = sheet.getCell(i, 1).getContents();
                string temp2 = sheet.getCell(i, 2).getContents();
                if (string.IsNullOrWhiteSpace(temp1) || string.IsNullOrWhiteSpace(temp2))
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
                coloumnDesc.isArray = isArray;
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
        public static string GenLuaFile(Sheet sheet)
        {
            List<ColoumnDesc> coloumnDesc = GetColoumnDesc(sheet);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("--[[Notice:This lua config file is auto generate by Xls2Lua Tools，don't modify it manually! --]]\n");
            if (null == coloumnDesc || coloumnDesc.Count <= 0)
            {
                return stringBuilder.ToString();
            }
            //创建索引
            Dictionary<string, int> fieldIndexMap = new Dictionary<string, int>();
            for (int i = 0; i < coloumnDesc.Count; i++)
            {
                fieldIndexMap[coloumnDesc[i].name] = i + 1;
            }
            //创建数据块的索引表
            stringBuilder.Append("local fieldIdx = {}\n");
            foreach (var cur in fieldIndexMap)
            {
                stringBuilder.Append(string.Format("fieldIdx.{0} = {1}\n", cur.Key, cur.Value));
            }

            //创建数据块
            stringBuilder.Append("local data = {");
            int rows = GetSheetRows(sheet);
            int validRowIdx = 4;
            //逐行读取并处理
            for (int i = validRowIdx; i < rows; i++)
            {
                StringBuilder oneRowBuilder = new StringBuilder();
                oneRowBuilder.Append("{");
                //对应处理每一列
                for (int j = 0; j < coloumnDesc.Count; j++)
                {
                    ColoumnDesc curColoumn = coloumnDesc[j];
                    var curCell = sheet.getCell(curColoumn.index, i);
                    string content = curCell.getContents();

                    if (FieldType.c_struct != curColoumn.type)
                    {
                        FieldType fieldType = curColoumn.type;
                        //如果不是数组类型的话
                        if (!curColoumn.isArray)
                        {
                            content = GetLuaValue(fieldType, content);
                            oneRowBuilder.Append(content);
                        }
                        else
                        {
                            StringBuilder tmpBuilder = new StringBuilder("{");
                            var tmpStringList = content.Split(splitSymbol, StringSplitOptions.RemoveEmptyEntries);
                            for (int k = 0; k < tmpStringList.Length; k++)
                            {
                                tmpStringList[k] = GetLuaValue(fieldType, tmpStringList[k]);
                                tmpBuilder.Append(tmpStringList[k]);
                                if (k != tmpStringList.Length - 1)
                                {
                                    tmpBuilder.Append(",");
                                }
                            }

                            oneRowBuilder.Append(tmpBuilder);
                            oneRowBuilder.Append("}");
                        }
                    }
                    else
                    {
                        //todo:可以处理结构体类型的字段
                        throw new Exception("暂不支持结构体类型的字段！");
                    }

                    if (j != coloumnDesc.Count - 1)
                    {
                        oneRowBuilder.Append(",");
                    }
                }

                oneRowBuilder.Append("},");
                stringBuilder.Append(string.Format("\n{0}", oneRowBuilder));
            }
            //当所有的行都处理完成之后
            stringBuilder.Append("}\n");
            //设置元表
            string str =
                "local mt = {}\n" +
                "mt.__index = function(a,b)\n" +
                "\tif fieldIdx[b] then\n" +
                "\t\treturn a[fieldIdx[b]]\n" +
                "\tend\n" +
                "\treturn nil\n" +
                "end\n" +
                "mt.__newindex = function(t,k,v)\n" +
                "\terror('do not edit config')\n" +
                "end\n" +
                "mt.__metatable = false\n" +
                "for _,v in ipairs(data) do\n\t" +
                "setmetatable(v,mt)\n" +
                "end\n" +
                "return data";
            stringBuilder.Append(str);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 处理字符串，输出标准的lua格式
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string GetLuaValue(FieldType fieldType, string value)
        {
            if (FieldType.c_string == fieldType)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return "\"\"";
                }

                return string.Format("[[{0}]]", value);
            }
            else if (FieldType.c_enum == fieldType)
            {
                //todo:可以具体地相应去处理枚举型变量
                string enumKey = value.Trim();
                return enumKey;
            }
            else if (FieldType.c_bool == fieldType)
            {
                bool isOk = StringToBoolean(value);
                return isOk ? "true" : "false";
            }
            else
            {
                return string.IsNullOrEmpty(value.Trim()) ? "0" : value.Trim();
            }
        }

        /// <summary>
        /// 字符串转为bool型，非0和false即为真
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool StringToBoolean(string value)
        {
            value = value.ToLower().Trim();
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            if ("false" == value)
            {
                return false;
            }

            int num = -1;
            if (int.TryParse(value, out num))
            {
                if (0 == num)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 获取当前sheet的合法名称
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public static string GetSheetName(Sheet sheet)
        {
            var sheetName = sheet.getName();
            return ParseSheetName(sheetName);
        }

        /// <summary>
        /// 检测Sheet的名称是否合法,并返回合法的sheet名称
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        private static string ParseSheetName(string sheetName)
        {
            sheetName = sheetName.Trim();
            if (string.IsNullOrEmpty(sheetName))
            {
                return null;
            }
            //只有以#为起始的sheet才会被转表
            if (!sheetName.StartsWith("#"))
            {
                return null;
            }

            return sheetName;
        }
    }
}
