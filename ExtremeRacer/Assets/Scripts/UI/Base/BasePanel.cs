using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasePanel : MonoBehaviour
{
    protected UICtrl uiCtrl;

    public Button BackBtn;

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
