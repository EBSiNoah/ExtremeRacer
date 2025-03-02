using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    public void OnClickBtn()
    {
        SceneManager.LoadScene("MAIN");
    }
}
