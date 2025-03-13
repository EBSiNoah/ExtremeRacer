using UnityEngine;

// PathAttribute 클래스 정의
[System.AttributeUsage(System.AttributeTargets.Field)]
public class PathAttribute : System.Attribute
{
    public string Path { get; private set; }

    public PathAttribute(string path)
    {
        Path = path;
    }
}


public enum PathEnum
{
    [Path("UI/UI_TestPanel1")]
    TestPanel1,

    [Path("UI/UI_TestPanel2")]
    TestPanel2,
}