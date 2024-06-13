// Automatically generated by Interoptopus.

#pragma warning disable 0105
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#if UNITY_2018_1_OR_NEWER
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;
#endif
using Saigai.Studios;
#pragma warning restore 0105

namespace Saigai.Studios
{
    public static partial class Interop
    {
        public const string NativeLib = "saigai.497350388";

        static Interop()
        {
        }


        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "my_function")]
        public static extern void my_function(Vec2 input);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "add_two_nums")]
        public static extern int add_two_nums(int x, int y);

    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Vec2
    {
        public float x;
        public float y;
    }



    public class InteropException<T> : Exception
    {
        public T Error { get; private set; }

        public InteropException(T error): base($"Something went wrong: {error}")
        {
            Error = error;
        }
    }

}
