using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoManager : MonoBehaviour
{
    public void OnClickBtn()
    {
        SceneManager.LoadScene("MAIN");
    }
}
