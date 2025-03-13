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
    public void HandleGameEvent(GameEvent ge) => ChildHandleGameEvent(ge);
    protected virtual void ChildHandleGameEvent(GameEvent ge) { }

    void Start()
    {
        uiCtrl = SingletonManager.Instance.GetSingleton<UICtrl>();

        BackBtn.onClick.AddListener(OnClickBackBtn);
    }

    public void Init(params object[] data)
    {
        // 패널 켜줌
        gameObject.SetActive(true);

        // 이벤트 타입 리스트에 있는거 등록 
        foreach (var eve in EventTypeList)
            GameEventSubject.RegisterHandler(eve, HandleGameEvent);

        // 자식패널에 초기화 명령
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

    private void OnDestroy()
    {
        foreach (var eve in EventTypeList)
            GameEventSubject.UnregisterHandler(eve, HandleGameEvent);
    }
}
