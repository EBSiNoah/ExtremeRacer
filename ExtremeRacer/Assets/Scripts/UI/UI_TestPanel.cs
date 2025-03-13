using UnityEngine;
using UnityEngine.UI;

public class UI_TestPanel : BasePanel
{
    public Button testBtn;

    public Transform testTransform;


    public void OnClickTextBtn()
    {
        testTransform = GameObject.Find("Canvas").transform;
        uiCtrl.ShowPanel(PathEnum.TestPanel2, testTransform);
    }
}
