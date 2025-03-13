using UnityEngine;
using UnityEngine.UI;

public class UI_TestPanel : BasePanel
{
    public Button testBtn;

    public Transform testTransform;

    public override void InitChild(params object[] data)
    {
        var test = data[0];
        Debug.Log($"test  {test}");
    }

    public void OnClickTextBtn()
    {
        testTransform = GameObject.Find("Canvas").transform;
        uiCtrl.ShowPanel(PathEnum.TestPanel2, testTransform);
    }
}
