using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class GameEventSubject
{
    public delegate void EventCaller(GameEvent e);
    static public Dictionary<GameEventType, List<EventCaller>> gameEventHandlerList = null;

    static public void Destroy()
    {
        gameEventHandlerList?.Clear();
        gameEventHandlerList = null;
    }

    /// <summary>
    /// 게임 이벤트 등록
    /// </summary>
    /// <param name="inEventType">게임 이벤트 타입</param>
    /// <param name="inHandler">등록자</param>
    static public void RegisterHandler(GameEventType inEventType, EventCaller inHandler)
    {
        if (null == gameEventHandlerList)
            gameEventHandlerList = new Dictionary<GameEventType, List<EventCaller>>();

        if (!gameEventHandlerList.TryGetValue(inEventType, out var eventList))
        {
            eventList = new List<EventCaller>();
            gameEventHandlerList.Add(inEventType, eventList);
        }

#if UNITY_EDITOR
        if (eventList.Contains(inHandler))
        {
            Debug.LogError($"중복키 발생 - {inEventType}");
            return;
        }
#endif

        eventList.Add(inHandler);
    }

    /// <summary>
    /// 게임 이벤트 등록 해제
    /// </summary>
    /// <param name="inEventType">게임 이벤트 타입</param>
    /// <param name="inHandler">등록자</param>
    static public void UnregisterHandler(GameEventType inEventType, EventCaller inHandler)
    {
        if (null == gameEventHandlerList)
            return;

        if (!gameEventHandlerList.TryGetValue(inEventType, out var eventList) || eventList.Count == 0)
            return;

        eventList.Remove(inHandler);

#if UNITY_EDITOR
        var whereNull = eventList.Where((a) => a == null);
        if (0 < whereNull.Count())
            Debug.LogError($"해제 안된 action 발견됨 - {inEventType}");
#endif
    }

    /// <summary>
    /// 게임 이벤트를 발생시키는 함수
    /// </summary>
    /// <param name="inType">게임 이벤트 타입</param>
    /// <param name="inParams">게임 이벤트와 같이 보낼 수치들</param>
    /// <returns>발생 성공 여부</returns>
    static public bool SendGameEvent(GameEventType inType)
    {
        if (!GameEventCount(inType, out List<EventCaller> value))
            return false;

        return SendGameEvent(new GameEvent(inType, false), value);
    }

    static public void SendGameEvent(GameEventType inType, List<EventCaller> value)
    {
        SendGameEvent(new GameEvent(inType, false), value);
    }

    static public bool SendGameEvent(GameEventType inType, params int[] inParams)
    {
        if (!GameEventCount(inType, out var value))
            return false;

        var newGameEvent = new GameEvent(inType, true);
        foreach (var addint in inParams)
            newGameEvent.Write(addint);
        return SendGameEvent(newGameEvent, value);
    }

    static public bool SendGameEvent(GameEventType inType, bool inParams)
    {
        if (!GameEventCount(inType, out var value))
            return false;

        var newGameEvent = new GameEvent(inType, true);
        newGameEvent.Write(inParams);
        return SendGameEvent(newGameEvent, value);
    }

    static public bool SendGameEvent(GameEventType inType, params float[] inParams)
    {
        if (!GameEventCount(inType, out var value))
            return false;

        var newGameEvent = new GameEvent(inType, true);
        foreach (var addfloat in inParams)
            newGameEvent.Write(addfloat);
        return SendGameEvent(newGameEvent, value);
    }

    static public bool SendGameEvent(GameEventType inType, params double[] inParams)
    {
        if (!GameEventCount(inType, out var value))
            return false;

        var newGameEvent = new GameEvent(inType, true);
        foreach (var addfloat in inParams)
            newGameEvent.Write(addfloat);
        return SendGameEvent(newGameEvent, value);
    }

    static public bool SendGameEvent(GameEventType inType, params ulong[] inParams)
    {
        if (!GameEventCount(inType, out var value))
            return false;

        var newGameEvent = new GameEvent(inType, true);
        foreach (var addfloat in inParams)
            newGameEvent.Write(addfloat);
        return SendGameEvent(newGameEvent, value);
    }

    static public bool SendGameEvent(GameEventType inType, params long[] inParams)
    {
        if (!GameEventCount(inType, out var value))
            return false;

        var newGameEvent = new GameEvent(inType, true);
        foreach (var addfloat in inParams)
            newGameEvent.Write(addfloat);
        return SendGameEvent(newGameEvent, value);
    }

    static public bool SendGameEvent(GameEventType inType, params string[] inParams)
    {
        if (!GameEventCount(inType, out var value))
            return false;

        var newGameEvent = new GameEvent(inType, true);
        foreach (var addString in inParams)
            newGameEvent.Write(addString);
        return SendGameEvent(newGameEvent, value);
    }

    /*static public bool SendGameEvent(GameEventType inType, params BigIntegerExtend[] inParams)
    {
        if (!GameEventCount(inType, out var value))
            return false;

        var newGameEvent = new GameEvent(inType, true);
        foreach (var bigInt in inParams)
            newGameEvent.Write(bigInt);

        return SendGameEvent(newGameEvent, value);
    }*/

    static public bool SendGameEvent(GameEventType inType, bool isSucess, string inParams)
    {
        if (!GameEventCount(inType, out var value))
            return false;

        var newGameEvent = new GameEvent(inType, true);
        newGameEvent.Write(isSucess);
        newGameEvent.Write(inParams);
        return SendGameEvent(newGameEvent, value);
    }

    static public bool GameEventCount(GameEventType inType, out List<EventCaller> value)
    {
        value = default;
        if (gameEventHandlerList == null || !gameEventHandlerList.TryGetValue(inType, out value) || value.Count == 0)
            return false;

        return true;
    }

    static public bool GameEventCount(GameEventType inType) => GameEventCount(inType, out _);

    static public bool SendGameEvent(GameEvent inGameEvent, List<EventCaller> value = null)
    {
        if (null == gameEventHandlerList)
            return false;

        if (value == null && (!gameEventHandlerList.TryGetValue(inGameEvent.eventType, out value) || value.Count == 0))
            return false;

        int maxCount = value.Count;
        for (int i = 0; i < maxCount; ++i)
        {
            inGameEvent.Reset();

            var de = value[i];
            de.Invoke(inGameEvent);

            if (value.Count < maxCount)
            {
                i -= (maxCount - value.Count);
                maxCount = value.Count;
            }
        }

        inGameEvent.Close();
        return true;
    }
}
