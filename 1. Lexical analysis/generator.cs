using System;
using System.Collections.Generic;

public class Pow2Generator {
    
    public static IEnumerable<int> Start() {
        var c = 1;
        while (c < 1000000) {
            yield return c;
            c *= 2;
        }
    }

    public static void Main() {
        
        IEnumerable<int> e = Start();
        IEnumerator<int> enu = e.GetEnumerator();

        Console.WriteLine(enu.MoveNext());
        Console.WriteLine(enu.Current);
        Console.WriteLine(enu.MoveNext());
        Console.WriteLine(enu.Current);
        Console.WriteLine(enu.MoveNext());
        Console.WriteLine(enu.Current);
    }
}