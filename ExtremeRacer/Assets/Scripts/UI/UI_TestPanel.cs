using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TestPanel : BasePanel
{
    public Button testBtn;

    public Transform testTransform;

    protected override List<GameEventType> EventTypeList => new List<GameEventType>()
    {
        GameEventType.TEST_EVENT,
    };

    protected override void ChildHandleGameEvent(GameEvent ge)
    {
        switch (ge.eventType)
        {
            case GameEventType.TEST_EVENT:
                Debug.Log($"이벤트 수신 완료 {ge.Read<string>()} ");
                break;
        }
    }

    public override void InitChild(params object[] data)
    {
        var test = data[0] ?? "null";
        Debug.Log($"test  {test}");
    }

    public void OnClickTextBtn()
    {
        testTransform = GameObject.Find("Canvas").transform;
        uiCtrl.ShowPanel(PathEnum.TestPanel2, testTransform);
    }
}
