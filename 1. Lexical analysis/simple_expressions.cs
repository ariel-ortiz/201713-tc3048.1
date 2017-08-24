using System;
using System.Text.RegularExpressions;

public class SimpleExpressions {

    public static void Main() {
        var regex = new Regex(@"(\d+)|([+])|([*])|(\s)|(.)");
        Console.Write("Input expression: ");
        var str = Console.ReadLine();
        foreach (Match m in regex.Matches(str)) {
            if (m.Groups[1].Success) {
                Console.WriteLine("found integer: " + m.Value);
            } else if (m.Groups[2].Success) {
                Console.WriteLine("found plus symbol");
            } else if (m.Groups[3].Success) {
                Console.WriteLine("found times symbol");
            } else if (m.Groups[4].Success) {
                // found whitespace, just skip it
                continue;
            } else {
                Console.WriteLine("found unrecognized character: " + m.Value);
            }
        }
    }
}