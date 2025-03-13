using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasePanel : MonoBehaviour
{
    // UICtrl 
    protected UICtrl uiCtrl;

    // 기본 뒤로가기 버튼 - 안써도 됨
    public Button BackBtn;

    // 이벤트 리스트 
    protected virtual List<GameEventType> EventTypeList { get; } = new List<GameEventType>();

    void Start()
    {
        uiCtrl = SingletonManager.Instance.GetSingleton<UICtrl>();

        BackBtn.onClick.AddListener(OnClickBackBtn);
    }

    public void Init(params object[] data)
    {
        gameObject.SetActive(true);
        InitChild(data);
    }

    public virtual void InitChild(params object[] data) { }

    public virtual void EndPanel()
    {
        gameObject.SetActive(false);
        uiCtrl.HidePanel(gameObject);
    }

    public virtual void OnClickBackBtn()
    {
        EndPanel();
    }
}
