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
        public const string NativeLib = "saigai.96d42fec";

        static Interop()
        {
        }


        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "add_two_nums")]
        public static extern int add_two_nums(int x, int y);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "update_anim")]
        public static extern Vec2 update_anim();

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "init_marker")]
        public static extern void init_marker(int ind, Vec2 pos);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "update_pos_key")]
        public static extern void update_pos_key(bool opt);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "update_pos_click")]
        public static extern bool update_pos_click(int marker);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "init_game")]
        public static extern void init_game(uint level);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "add_piece")]
        public static extern uint add_piece();

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "add_coordinate")]
        public static extern void add_coordinate(uint piece, Coord loc);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "place_on_board")]
        public static extern bool place_on_board(uint piece, float mouse_x, float mouse_y);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "set_grid_space")]
        public static extern void set_grid_space(float x, float y, float width, float height);

    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Coord
    {
        public byte row;
        public byte col;
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
