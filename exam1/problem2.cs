//==========================================================
// Solution de problem 2.
//==========================================================

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Exam1 {
    public class Problem1 {
        public static void Main(String[] args) {
            if (args.Length != 1) {
                Environment.Exit(1);
            }
            var fileName = args[0];
            var fileContent = File.ReadAllText(fileName);
            var regex = new Regex(@"(&#x([0-9a-fA-F]+);)|(.|\n)");
            foreach (Match m in regex.Matches(fileContent)) {
                if (m.Groups[1].Success) {
                    int num = Convert.ToInt32(m.Groups[2].Value, 16);
                    Console.Write("&#" + num + ";");
                } else {
                    Console.Write(m.Value);
                }
            }
        }
    }
}
