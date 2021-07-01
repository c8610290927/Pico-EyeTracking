using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEventCenter
{
    private static Dictionary<string, List<Delegate>> _gameEvents;
    private static bool _isCenterInit;
    public static void EventCenterInit()
    {
        if (_isCenterInit)
        {
            return;
        }
        _gameEvents = new Dictionary<string, List<Delegate>>();
        _isCenterInit = true;
    }

    public static void AddEvent<T>(string eventName, Action<T> callback)
    {
        //eventName已存在
        if (_gameEvents.TryGetValue(eventName, out var actions))
        {
            actions.Add(callback);
        }
        //eventName不存在
        else
        {
            actions = new List<Delegate> {callback};
            _gameEvents.Add(eventName, actions);
        }
    }

    public static void AddEvent(string eventName, Action callback)
    {
        //eventName已存在
        if (_gameEvents.TryGetValue(eventName, out var actions))
        {
            actions.Add(callback);
        }
        //eventName不存在
        else
        {
            actions = new List<Delegate> {callback};
            _gameEvents.Add(eventName, actions);
        }
    }

    public static void RemoveEvent<T>(string eventName, Action<T> callback)
    {
        if (!_gameEvents.TryGetValue(eventName, out var actions)) return;
        actions.Remove(callback);
        if (actions.Count == 0)
        {
            _gameEvents.Remove(eventName);
        }
    }

    public static void RemoveEvent(string eventName, Action callback)
    {
        if (!_gameEvents.TryGetValue(eventName, out var actions)) return;
        actions.Remove(callback);
        if (actions.Count == 0)
        {
            _gameEvents.Remove(eventName);
        }
    }

    public static void DispatchEvent(string eventName)
    {
        if (!_gameEvents.ContainsKey(eventName)) return;
        _gameEvents.TryGetValue(eventName, out var actions);

        if (actions == null) return;
        foreach (var act in actions)
        {
            act.DynamicInvoke();
        }
    }

    public static void DispatchEvent<T>(string eventName, T arg)
    {
        if (!_gameEvents.ContainsKey(eventName)) return;
        _gameEvents.TryGetValue(eventName, out var actions);

        if (actions == null) return;
        foreach (var act in actions)
        {
            act.DynamicInvoke(arg);
        }
    }

    public static void RemoveAllEvents()
    {
        _gameEvents?.Clear();
        _gameEvents = null;
        _isCenterInit = false;
    }
}
