using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Text.Encodings.Web;

namespace App.HttpApi
{
    /// <summary>
    /// �ַ�������������
    /// </summary>
    internal static class StringHelper
    {
        /// <summary>�������ű��ʽ</summary>
        public static int[] ToIntArray(this string commaText)
        {
            if (String.IsNullOrEmpty(commaText))
                return new int[0];
            else
                return commaText.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
        }

        /// <summary>ȥ���հ��ַ�</summary>
        public static string ClearSpace(this string text)
        {
            return Regex.Replace(text, "\\s", "");
        }

        /// <summary>ȥ��HTML��ǩ</summary>
        public static string ClearTag(this string html)
        {
            return Regex.Replace(html, "<[^>]*>", "");
        }

        /// <summary>ȥ������HTML�ۼ��������ű�����ǩ��ע�͡�ת����ȣ�</summary>
        public static string ClearHtml(this string html)
        {
            if (html.IsEmpty()) return "";

            //ɾ���ű�
            html = Regex.Replace(html, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //ɾ��HTML 
            html = Regex.Replace(html, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"�C>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<!�C.*", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            html.Replace("<", "��");
            html.Replace(">", "��");
            html.Replace("\r\n", "");
            //html = HttpContext.Current.Server.HtmlEncode(html).Trim();
            html = HtmlEncoder.Default.Encode(html).Trim();
            return html;
        }

        /// <summary>�ظ��ַ���</summary>
        public static string Repeat(this string c, int n)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < n; i++)
                sb.Append(c);
            return sb.ToString();
        }

        /// <summary>��ȡ�����ı�</summary>
        public static string GetMask(this string txt, int n, string mask=".")
        {
            if (txt.IsEmpty() || txt.Length < n)
                return txt;
            else
            {
                int len = txt.Length;
                string masks = mask.Repeat(4);
                return txt.Substring(0, len - 8) + masks + txt.Substring(n - 4, 4);
            }
        }

        /// <summary>��ȡժҪ</summary>
        public static string GetSummary(this string txt, int n)
        {
            if (txt.IsEmpty() || txt.Length < n)
                return txt;
            else
                return txt.Substring(0, n) + "....";
        }

        /// <summary>ת��Ϊ����ĸСд�ַ���</summary>
        public static string ToLowCamel(this string word)
        {
            if (string.IsNullOrEmpty(word))
                return string.Empty;
            return word.Substring(0, 1).ToLower() + word.Substring(1);
        }

        /// <summary>ת��Ϊ����ĸ��д�ַ���</summary>
        public static string ToHighCamel(this string word)
        {
            if (string.IsNullOrEmpty(word))
                return string.Empty;
            return word.Substring(0, 1).ToUpper() + word.Substring(1);
        }

        /// <summary>�õ�β����ƥ���ַ�������������ַ���������"a.asp".TrimEndFrom(".") => "a"</summary>
        /// <param name="keepKey">�Ƿ���������"/Pages/test.aspx".TiemEndFrom("/",true) => "/Pages/"</param>
        public static string TrimEnd(this string name, string key, bool keepKey = false)
        {
            if (name.IsEmpty())
                return "";
            var n = name.LastIndexOf(key);
            if (n != -1)
            {
                if (keepKey)
                    return name.Substring(0, n + key.Length);
                else
                    return name.Substring(0, n);
            }
            return name;
        }

        /// <summary>��ȡ�ļ���չ������չ������Сд����;��</summary>
        public static string GetFileExtension(this string fileName)
        {
            if (fileName.IsEmpty())
                return "";
            fileName = fileName.TrimQuery();
            int n = fileName.LastIndexOf('.');
            if (n != -1)
            {
                var ext = fileName.Substring(n).ToLower();
                if (!ext.Contains(@"/") && !ext.Contains(@"\"))  // ������·��б��
                    return ext;
            }
            return "";
        }

        /// <summary>ȥ��β���Ĳ�ѯ�ַ���</summary>
        public static string TrimQuery(this string url)
        {
            if (url.IsEmpty())
                return "";
            int n = url.LastIndexOf('?');
            if (n != -1)
                return url.Substring(0, n);
            return url;
        }


        /// <summary>Ϊ Url ���Ӻϲ���ѯ�ַ������������򸲸ǣ�</summary>
        /// <param name="queryString">Ҫ��ӵĲ�ѯ�ַ�������a=x&b=x</param>
        public static string AddQueryString(this string url, string queryString)
        {
            if (queryString.IsEmpty())
                return url;
            var u = new Url(url);
            var dict = queryString.ParseDict();
            foreach (var key in dict.Keys)
                u[key] = dict[key];
            return u.ToString();
        }

    }
}
