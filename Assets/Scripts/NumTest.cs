// for testing purposes ONLY

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saigai.Studios;

class Test {
    static void Main() {
        var x = Interop.add_two_nums(1,2);
        Console.WriteLine(x);
        if (x != 4) {
            throw new LibCallFailure("Basic test call printed incorrect value");
        }
    }
}