using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        SingletonManager.Instance.RegisterSingleton(this);
    }


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
