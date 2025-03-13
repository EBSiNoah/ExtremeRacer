using System;
using System.Collections.Generic;

public class GameEvent
{
    public GameEventType eventType;
    public bool hasParams;
    private Dictionary<Type, object> paramsDict = new Dictionary<Type, object>();

    public GameEvent(GameEventType type, bool hasParams)
    {
        eventType = type;
        this.hasParams = hasParams;
    }

    public void Write<T>(T value)
    {
        paramsDict[typeof(T)] = value;
    }

    public T Read<T>()
    {
        if (paramsDict.TryGetValue(typeof(T), out object value))
            return (T)value;

        throw new KeyNotFoundException();
    }

    public void Reset()
    {
        paramsDict.Clear();
    }

    public void Close()
    {
        // 이벤트 종료 시 필요한 처리
    }
}

