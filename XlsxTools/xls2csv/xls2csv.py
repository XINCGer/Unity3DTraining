# -*- coding: utf-8 -*-  

import os,sys,inspect,re
import xdrlib,xlrd

# 防止中文乱码
reload(sys)
sys.setdefaultencoding("utf-8")

# 分割符
C_SPACE = ","
# 结束符
C_END = "\n"
# 输入路径(存放xls文件的路径）
IN_PATH = ""
# 输出路径(导出csv文件的路径)
OUT_PATH = ""


# 读取配置文件
def read_config():
    config_file = open("config.ini","r")
    cur_line = config_file.readline().rstrip("\r\n").split(',')
    global IN_PATH 
    IN_PATH = cur_line[1]
    cur_line = config_file.readline().rstrip("\r\n").split(',')
    global OUT_PATH 
    OUT_PATH = cur_line[1]
    config_file.close()

# 过滤路径
def cur_file_dir(path):
    if os.path.isfile(path):
        path = os.path.dirname(path)
    print path
    return os.path.abspath(path)

# 搜索指定文件夹下面的文件
def find_file_by_pattern(pattern='.*', base=".", circle=True):  
    # 查找给定文件夹下面所有xls文件
    re_file = re.compile(pattern)
    # 第一次搜索的时候过滤下路径，递归之后直接搜索base路径即可
    if base == ".":
        base = cur_file_dir(IN_PATH)
    print u"开始搜索文件夹：",base

    # 存储xls文件的列表
    final_file_list = []
    # 遍历指定路径下的文件
    cur_list = os.listdir(base)  
    for item in cur_list:
        # 忽略一些干扰的文件，如果你还有其他需要忽略的文件，直接在后面继续添加即可
        if item == ".svn":
            continue
        # 拼接路径
        full_path = os.path.join(base, item)
		# 忽略临时文件
        if full_path.startswith("~"):
            continue
		# 筛选出xlsx\xls文件
        if full_path.endswith(".xlsx") or full_path.endswith(".xls"):
            print u"输入文件:" + full_path
            bfile = os.path.isfile(item)
            if os.path.isfile(full_path):
                if re_file.search(full_path):
                    final_file_list.append(full_path)  
            else:
                final_file_list += find_file_by_pattern(pattern, full_path)

    # 返回文件列表
    return final_file_list


# 打开excel
def open_excel(file= 'file.xls'):
    try:
        data = xlrd.open_workbook(file)
        return data
    except Exception,e:
        print str(e)


#根据索引获取Excel表格中的数据 参数:file：Excel文件路径, colnameindex：表头列名所在行的索引, by_index：表的索引
def excel_table_byindex(file='file.xls', colnameindex=0, by_index=0):
    data = open_excel(file)
    table = data.sheets()[by_index]
    nrows = table.nrows #行数
    ncols = table.ncols #列数
    rowlist = []
	
    '''开始读取数据'''
    for rownum in range(colnameindex, nrows):
        rowdata = table.row_values(rownum)
        if rowdata:
            collist = []
            for i in range(ncols):
                collist.append(rowdata[i])
            rowlist.append(collist)
    return rowlist

#保存csv文件
def savaToCSV(_file, _list, _path):
    filename = ""
    content = ""
    #生成文件内容
    for collist in _list:
        for i in range(len(collist)):
            v = collist[i]
            vstr = ""
            # print k,v
            if isinstance(v, float) or isinstance(v, int):
                vstr = str(int(v))
            else:
                vstr = v
            if i > 0:
                content = content + C_SPACE
            content = content + vstr
        content = content + C_END

    #生成文件后缀 
    fname = os.path.basename(_file).split('.')
    filename = fname[0] + ".csv"

    #写文件
    if len(filename)>0 and len(content)>0:
        filename = OUT_PATH + "/" + filename
        print u"输出文件:" + filename
        file_object = open(filename, 'w')
        file_object.write(content)
        file_object.close()


def main():

    read_config()
    filelist = find_file_by_pattern()
    if len(filelist) > 0:
        path = ""
        #遍历文件生成csv
        for file in filelist:
            datalist = excel_table_byindex(file, 0)
            if len(datalist)>0:
                savaToCSV(file, datalist, path)
    else:
        print u"没有找到任何excel文件！"

if __name__=="__main__":
    main()
