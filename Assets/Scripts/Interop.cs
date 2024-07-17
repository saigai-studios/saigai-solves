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
        public const string NativeLib = "saigai";

        static Interop()
        {
        }


        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "data_load")]
        public static extern bool data_load();

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "data_save")]
        public static extern bool data_save();

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "data_get_language")]
        public static extern byte data_get_language();

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "data_set_language")]
        public static extern bool data_set_language(int index);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "data_has_earthquake_card")]
        public static extern bool data_has_earthquake_card(int index);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "data_unlock_earthquake_card")]
        public static extern bool data_unlock_earthquake_card(int index);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "update_anim")]
        public static extern Vec2 update_anim();

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "init_marker")]
        public static extern void init_marker(int ind, Vec2 pos);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "update_pos_key")]
        public static extern void update_pos_key(bool opt);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "update_pos_click")]
        public static extern bool update_pos_click(int marker);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "get_anim_state")]
        public static extern PlayerAnim get_anim_state();

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "set_anim_state")]
        public static extern void set_anim_state(PlayerAnim new_anim);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "set_speed")]
        public static extern void set_speed(int new_speed);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "init_bus_game")]
        public static extern void init_bus_game(uint level);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "add_piece")]
        public static extern uint add_piece();

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "add_coordinate")]
        public static extern void add_coordinate(uint piece, Coord loc);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "place_on_board")]
        public static extern bool place_on_board(uint piece, float mouse_x, float mouse_y);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "place_off_board")]
        public static extern bool place_off_board(uint piece, float mouse_x, float mouse_y);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "set_window")]
        public static extern void set_window(float x, float y, float width, float height);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "get_snap_pos")]
        public static extern Vec2 get_snap_pos(uint piece);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "is_bus_game_won")]
        public static extern bool is_bus_game_won();

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "init_catch_game")]
        public static extern void init_catch_game(bool hard_mode);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "is_next_spawn_ready")]
        public static extern bool is_next_spawn_ready();

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "is_catch_game_won")]
        public static extern bool is_catch_game_won();

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "good_catch")]
        public static extern int good_catch();

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "missed_catch")]
        public static extern int missed_catch();

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "bad_catch")]
        public static extern int bad_catch();

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "spawn_new_item")]
        public static extern int spawn_new_item();

    }

    public enum PlayerAnim
    {
        IDLE = 0,
        LEFT = 1,
        RIGHT = 2,
        FORWARD = 3,
        BACK = 4,
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
