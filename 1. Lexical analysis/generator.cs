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

        foreach (int i in Start()) {
            Console.WriteLine(i);
        }
    }
}