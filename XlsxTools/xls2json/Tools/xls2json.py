# -*- coding: utf-8 -*-  
import os,sys,importlib
import xml.etree.ElementTree as ET
import xdrlib,xlrd

# 防止中文乱码
importlib.reload(sys)

#配置文件名
CONFIG_NAME = "config.ini"
#保存文件类型
SAVE_FILE_TYPE = ".json"
#保存映射类类型
SAVE_MAPPING_TYPE = ".cs"
#分隔符
SPLIT_CAHR = "："
#表格路径
XLS_PATH = ""
#解析路径
XML_PATH = ""
#导出路径
OUT_PATH = ""
#映射路径
MAP_PATH = ""
#映射总数据类分表内容
MAPPING_CONTENT = ""

#读取配置
def read_config():
    print("开始读取配置文件")
    config_file = open(CONFIG_NAME, "r", encoding = "utf-8")

    #表格路径
    cur_line = config_file.readline().rstrip("\r\n").split(SPLIT_CAHR)
    global XLS_PATH
    XLS_PATH = os.path.abspath(cur_line[1])
    print("表格路径：", XLS_PATH)

    #解析路径
    cur_line = config_file.readline().rstrip("\r\n").split(SPLIT_CAHR)
    global XML_PATH
    XML_PATH = os.path.abspath(cur_line[1])
    print("解析路径", XML_PATH)

    #导出路径
    cur_line = config_file.readline().rstrip("\r\n").split(SPLIT_CAHR)
    global OUT_PATH
    OUT_PATH = os.path.abspath(cur_line[1])
    print("导出路径", OUT_PATH)
    
    #映射路径
    cur_line = config_file.readline().rstrip("\r\n").split(SPLIT_CAHR)
    global MAP_PATH
    MAP_PATH = os.path.abspath(cur_line[1])
    print("映射路径", MAP_PATH)
    config_file.close()

#删除导出目录原文件
def delect_old_file():
    print("删除导出目录原文件")
    file_list = os.listdir(OUT_PATH)
    for file in file_list:
        #只删除JSON文件
        if file.endswith(SAVE_FILE_TYPE):
            os.remove(OUT_PATH + "\\" + file)
    print("删除映射目录原文件")
    file_list = os.listdir(MAP_PATH)
    for file in file_list:
        #只删除C#文件
        if file.endswith(SAVE_MAPPING_TYPE):
            os.remove(MAP_PATH + "\\" + file)

#转换文件
def change_file():
    print("开始转换文件")
    file_list = os.listdir(XML_PATH)
    for file in file_list:
        if file.endswith(".xml"):
            #拼接XML路径
            xml_file_path = XML_PATH + "\\" + file
            isSucc = parse_file_by_xml(xml_file_path)
            if (False == isSucc):
                print("出错了！！！！！！！！！！！！！！！！！！")
                return


def parse_file_by_xml(xml_file_path):
    #解析XML
    try:
        tree = ET.parse(xml_file_path)
        #获得根节点
        root = tree.getroot()
    except Exception as e:
        print("解析{0}失败！！！！！！！！！！！！".format(xml_file_path))
        sys.exit()
        return False

    #解析内容
    if root.tag == "config":
        xls_file_list = []
        save_file_name = ""
        element_list = []
        for child in root:
            if child.tag == "input":
                #要转换的表格
                for input_child in child:
                    xls_file_list.append(input_child.get("file"))
            elif child.tag == "output":
                #输出文件名称
                save_file_name = child.get("name")
            elif child.tag == "elements":
                #列表转换
                element_list = child
        #转换数据
        return change_file_by_xml_data(xls_file_list, element_list, save_file_name)
    else:
        print("找不到config节点 {0}".format(xml_file_path))
        return False

#开始转换表格
def change_file_by_xml_data(xls_file_list, element_list, save_file_name):
    #主键检查
    primary_key = None
    primary_type = None
    for element in element_list:
        if "true" == element.get("primary"):
            if None == primary_key:
                primary_key = element.get("name")
                primary_type = element.get("type")
            else:
                print("存在多个主键")
                return False
    if None == primary_key:
        print("没有主键")
        return False

    all_value_list = {}
    for xls_file in xls_file_list:
        xls_file_path = XLS_PATH + "\\" + xls_file
        print("转换文件{0}".format(xls_file_path))

        #打开表格
        xls_data = None
        try:
            xls_data = xlrd.open_workbook(xls_file_path)
        except Exception as e:
            print(str(e))
            return False

        #读取sheet1的数据
        xls_table = xls_data.sheets()[0]
        nrows = xls_table.nrows #行数
        ncols = xls_table.ncols #列数

        #转换为XML中的数据
        key_list = xls_table.row_values(0)
        for row_index in range(1, nrows):
            row_values = xls_table.row_values(row_index)
            #将数据转存为字典
            value_dic = {}
            for col_index in range(0, ncols):
                for element in element_list:
                    if key_list[col_index] == element.get("key"):
                        if "int" == element.get("type"):
                            value_dic[element.get("name")] = int(row_values[col_index])
                        elif "string" == element.get("type"):
                            value_dic[element.get("name")] = str(row_values[col_index])
                        else:
                            value_dic[element.get("name")] = str(row_values[col_index])
                        break
            #设置主键
            primary_value = str(value_dic[primary_key])
            if primary_value in all_value_list:
                print("存在重复的主键")
                return False
            all_value_list[primary_value] = value_dic
            #释放内存
            xls_data.release_resources()

    #拼接为JSON字符串
    JSON_STR = str(all_value_list).replace("\'", "\"")
    #拼接类名
    file_name = "Table" + save_file_name[0].upper() + save_file_name[1:]
    #存储为JSON文件
    save_to_json(JSON_STR, file_name)
    
    #生成C#映射类
    save_to_mapping(file_name, element_list, primary_type)
    return True

#存储为JSON文件
def save_to_json(str, file_name):
    save_file_path = OUT_PATH + "\\" + file_name + SAVE_FILE_TYPE
    print("输出文件：" + save_file_path)
    file_object = open(save_file_path, 'w', encoding = "utf-8")
    file_object.write(str)
    file_object.close()
    
#生成C#映射类
def save_to_mapping(file_name, element_list, primary_type):
    table_content_frame = "public class " + file_name + " {{\n{0}{1}\n}}"
    table_content_field = ""
    constructor_content = ""
    constructor_params = None
    constructor_assign = None
    mapping_single_content = create_single_table_mapping_content(file_name)
    mapping_json_value = None
    #映射类成员
    for element in element_list:
        field_name = element.get("name")
        type_str = element.get("type")
        field_str = "\n\t//列名[{0}] Type[{1}]\n\tpublic {2} " + field_name + " = {3};\n"
        define_value_str = None
        if "int" == type_str:
            define_value_str = 0
        elif "string" == type_str:
            define_value_str = "\"\""
        if None != type_str:
            #填充
            key_name_str = element.get("key")
            table_content_field = table_content_field + field_str.format(key_name_str, type_str, type_str, define_value_str)
            if None != constructor_params:
                constructor_params = constructor_params + ", " + type_str + " " + field_name
                constructor_assign = constructor_assign + "\n\t\tthis.{0} = {1};".format(field_name, field_name)
                mapping_json_value = mapping_json_value + (", ({0})json.Value[\"{1}\"]").format(type_str, field_name)
            else:
                constructor_params = type_str + " " + field_name
                constructor_assign = "\t\tthis.{0} = {1};".format(field_name, field_name)
                mapping_json_value = "({0})json.Value[\"{1}\"]".format(type_str, field_name)
    #可以创建构造函数
    if None != constructor_params:
        #构造函数
        constructor_content = ("\n\t//构造函数\n\tpublic " + file_name + "({0})\n\t{{\n{1}\n\t}}").format(constructor_params, constructor_assign)
        #映射总数据
        global MAPPING_CONTENT
        prime_key_trans = "null"
        if "int" == primary_type:
            prime_key_trans = "int.Parse(json.Key)"
        elif "string" == primary_type:
            prime_key_trans = "json.Key"
        MAPPING_CONTENT = MAPPING_CONTENT + mapping_single_content.format(prim_key_type = primary_type, prime_key_trans = prime_key_trans, json_value = mapping_json_value)
    save_file_path = MAP_PATH + "\\" + file_name + SAVE_MAPPING_TYPE
    print("输出映射类：" + save_file_path)
    file_object = open(save_file_path, 'w', encoding = "utf-8")
    file_object.write(table_content_frame.format(table_content_field, constructor_content))
    file_object.close()
    
#生成单个映射总数据内容
def create_single_table_mapping_content(file_name):
    content = ""
    content = content + "\n\n\t//{xml_name}"
    content = content + "\n\tprivate Dictionary<{{prim_key_type}}, {file_name}> {lower_file_name}Dic = new Dictionary<{{prim_key_type}}, {file_name}>();"
    content = content + "\n\t//初始化{xml_name}字典"
    content = content + "\n\tprivate void Init{file_name}()"
    content = content + "\n\t{{{{"
    content = content + "\n\t\tJObject jsonData = JsonManager.GetTableJson(\"{file_name}\");"
    content = content + "\n\t\tforeach (var json in jsonData)"
    content = content + "\n\t\t{{{{"
    content = content + "\n\t\t\t{{prim_key_type}} key = {{prime_key_trans}};"
    content = content + "\n\t\t\tvar jsonValue = json.Value;"
    content = content + "\n\t\t\t{file_name} value = new {file_name}({{json_value}});"
    content = content + "\n\t\t\t{lower_file_name}Dic.Add(key, value);"
    content = content + "\n\t\t}}}}"
    content = content + "\n\t}}}}"
    content = content + "\n\t//通过主键值获取{xml_name}数据"
    content = content + "\n\tpublic {file_name} Get{file_name}ByPrimKey({{prim_key_type}} primKey)"
    content = content + "\n\t{{{{"
    content = content + "\n\t\tif (0 == {lower_file_name}Dic.Count) Init{file_name}();"
    content = content + "\n\t\t//获取数据"
    content = content + "\n\t\t{file_name} {lower_file_name}Data = null;"
    content = content + "\n\t\t{lower_file_name}Dic.TryGetValue(primKey, out {lower_file_name}Data);"
    content = content + "\n\t\treturn {lower_file_name}Data;"
    content = content + "\n\t}}}}"
    
    return content.format(xml_name = file_name[5:], file_name = file_name, lower_file_name = file_name[0].lower() + file_name[1:])

#创建映射总数据文件
def craete_table_mapping_cs():
    mapping_frame = ""
    mapping_frame = mapping_frame + "using System.Collections.Generic;"
    mapping_frame = mapping_frame + "\nusing Newtonsoft.Json.Linq;"
    mapping_frame = mapping_frame + "\n\npublic class TableMapping"
    mapping_frame = mapping_frame + "\n{{\n{0}{1}\n}}"
    mapping_ins = ""
    mapping_ins = mapping_ins + "//单例"
    mapping_ins = mapping_ins + "\n\tprivate TableMapping() { }"
    mapping_ins = mapping_ins + "\n\tprivate static TableMapping _ins;"
    mapping_ins = mapping_ins + "\n\tpublic static TableMapping Ins { get { if (null == _ins) { _ins = new TableMapping(); } return _ins; } }"
    #保存文件
    save_file_path = MAP_PATH + "\\TableMappnig" + SAVE_MAPPING_TYPE
    file_object = open(save_file_path, 'w', encoding = "utf-8")
    file_object.write(mapping_frame.format(mapping_ins, MAPPING_CONTENT))
    file_object.close()

def main():
    read_config()
    delect_old_file()
    change_file()
    craete_table_mapping_cs()

if __name__ == "__main__":
    main()