using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Text.RegularExpressions;

namespace Tsms
{
    public class Utility
    {
        public static bool IsDigit(string input)
        {
            return RegexValidate(@"^\d+$", input);
        }
        /// <summary>
        /// 是否为合法的Email
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsEmail(string input)
        {
            return RegexValidate(@"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$", input);
        }
        /// <summary>
        /// 正则验证
        /// </summary>
        /// <param name="pattern">表达式</param>
        /// <param name="input">输入字符串</param>
        /// <returns></returns>
        private static bool RegexValidate(string pattern, string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            else
                return new Regex(pattern).IsMatch(input);
        }
    }
}