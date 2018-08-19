## HTML转为UGUI用的文本(仅供测试)  
```C#
 public  string Html2Text(string htmlStr)
        {
            if (String.IsNullOrEmpty(htmlStr))
            {
                return "";
            }
            string regEx_style = "<style[^>]*?>[\\s\\S]*?<\\/style>"; //定义style的正则表达式 
            string regEx_script = "<script[^>]*?>[\\s\\S]*?<\\/script>"; //定义script的正则表达式 
            string regEx_html = "<[^>]+>"; //定义HTML标签的正则表达式 
            string nsp = "&nbsp;";
            htmlStr = Regex.Replace(htmlStr, "\\s*|\t|\r|\n", "");//去除tab、空格、空行
            htmlStr = Regex.Replace(htmlStr, nsp, "\n");
            htmlStr = Regex.Replace(htmlStr, regEx_style, "\n");//删除css
            htmlStr = Regex.Replace(htmlStr, regEx_script, "\n");//删除js
            htmlStr = Regex.Replace(htmlStr, regEx_html, "");//删除html标记
           
            htmlStr = htmlStr.Replace(" ", "");
            htmlStr = htmlStr.Replace("\"", "");//去除异常的引号" " "
            htmlStr = htmlStr.Replace("\"", "");
            return htmlStr.Trim();
        }
```
