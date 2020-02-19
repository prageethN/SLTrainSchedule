using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PrototypeBeta.Util
{
    public class AppUtility
    {

        static string[] prepositionsArr = { "to" };

        public static string ToCamelCase(string input)
        {
            try
            {

                string[] words = input.Split(' ');
                StringBuilder sb = new StringBuilder();
                int length = 0;
                foreach (string s in words)
                {
                    if (!s.Equals(""))
                    {
                        foreach (string value in prepositionsArr)
                        {
                            if (value.Equals(s))
                            {
                                continue;
                            }
                        }
                        string firstLetter = s.Substring(0, 1);
                        string rest = s.Substring(1, s.Length - 1);
                        sb.Append(firstLetter.ToUpper() + rest.ToLower());
                        sb.Append(" ");
                    }
                }
                return sb.ToString().Substring(0, sb.ToString().Length - 1);
            }
            catch (Exception ex)
            {
                return input;
            }
        }
        public static string ToLowerCase(string input)
        {
            return input.ToLower();

        }

        public static void shareStatusTask(string status)
        {

            ShareStatusTask shareStatusTask = new ShareStatusTask();
            shareStatusTask.Status = status;
            shareStatusTask.Show();
        }
    }


}
