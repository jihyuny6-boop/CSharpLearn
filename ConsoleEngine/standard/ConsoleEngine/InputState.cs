using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine;

/// <summary>이번 프레임에 눌린 키 목록. 엔진이 프레임마다 비우고 채운다. (v0.4.0)</summary>
public class InputState
{
    private readonly List<ConsoleKey> _pressed = new List<ConsoleKey>();

    /// <summary>이번 프레임에 이 키가 한 번이라도 눌렸는가.</summary>
    public bool WasPressed(ConsoleKey key)
    {
        return _pressed.Contains(key);
    }

    /// <summary>이번 프레임에 이 키가 눌린 횟수. 길게 누르면 키 반복으로 여러 번 온다.</summary>
    public int CountPressed(ConsoleKey key)
    {
        int count = 0;
        foreach (ConsoleKey k in _pressed)
            if (k == key) count++;
        return count;
    }
    internal void BeginFrame()
    {
        _pressed.Clear();
    }

    internal void Add(ConsoleKey key)
    {
        _pressed.Add(key);
    }
}