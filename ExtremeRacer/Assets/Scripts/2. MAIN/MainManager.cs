using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    public Canvas canvas;

    private void Start()
    {
        SingletonManager.Instance.DebugSingleton();
        var uictrl = SingletonManager.Instance.GetSingleton<UICtrl>();
        uictrl.ShowPanel("UI/UI_TestPanel1", canvas.transform);
    }

    public void OnClickBtn()
    {
        SceneManager.LoadScene("3.InGame");
    }
}
