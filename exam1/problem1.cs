//==========================================================
// Solution to problem 1.
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
            var regex = new Regex(@"^[Cc*].*\n?", RegexOptions.Multiline);
            Console.Write(regex.Replace(fileContent, ""));
        }
    }
}
