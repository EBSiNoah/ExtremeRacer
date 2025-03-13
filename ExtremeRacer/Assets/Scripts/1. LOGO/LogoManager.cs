using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoManager : MonoBehaviour
{
    public Canvas canvas;

    void Awake()
    {
        
    }

    protected UICtrl uiCtrl;

    void Start()
    {
        SingletonManager.Instance.DebugSingleton();
        uiCtrl = SingletonManager.Instance.GetSingleton<UICtrl>();
    }

    public void OnClickBtn()
    {
        uiCtrl.ShowPanel(PathEnum.TestPanel1, canvas.transform, "hello world");
        //SceneManager.LoadScene("2.MAIN");
    }

    public void OnClickBtn2()
    {
        SceneManager.LoadScene("2.MAIN");
    }
}
