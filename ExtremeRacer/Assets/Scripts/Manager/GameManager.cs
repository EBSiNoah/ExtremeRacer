using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        // 등록은 딱 한번만 다른데서 사용하려면 
        // SingletonManager.instance.GetSingleton<GameManger>();
        SingletonManager.Instance.RegisterSingleton(this);
    }


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
