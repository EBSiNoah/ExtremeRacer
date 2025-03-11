using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        SingletonManager.Instance.RegisterSingleton(this);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
