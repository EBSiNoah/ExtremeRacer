using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtrl : MonoBehaviour
{
    // 지금까지 생성했던 모든 패널들 보관
    public Dictionary<string, GameObject> panelInstance = new Dictionary<string, GameObject>();

    // 현재 생성되서 켜져있는 패널 순서 보관
    [SerializeField]
    private Stack<GameObject> panelStack = new Stack<GameObject>();

    // 생성된 최신 패널
    [SerializeField]
    private GameObject _createdPanel;

    // 전체 패널 이름 보관
    private Dictionary<PathEnum, string> _panelPaths = new Dictionary<PathEnum, string>();

    private void Awake()
    {
        SingletonManager.Instance.RegisterSingleton(this);

        foreach (PathEnum type in System.Enum.GetValues(typeof(PathEnum)))
        {
            var pathAttribute = (PathAttribute)System.Attribute.GetCustomAttribute(type.GetType().GetField(type.ToString()), typeof(PathAttribute));
            if (pathAttribute != null)
            {
                _panelPaths[type] = pathAttribute.Path;
            }
        }
    }

    public Transform GetCanvas(string sceneName)
    {
        GameObject canvasObject = GameObject.Find("Canvas");
        if (canvasObject != null)
        {
            return canvasObject.transform;
        }
        else
        {
            Debug.LogError("Canvas not found in scene: " + sceneName);
            return null;
        }
    }

    public GameObject ShowPanel(PathEnum pathEnum, Transform parent)
    {
        // 패널은 enum으로 이름을 등록하고 프리팹은 Resources폴더안에 넣어서 로드하기
        string path = _panelPaths[pathEnum];
        GameObject pgo = Resources.Load<GameObject>(path);
        if (pgo == null)
        {
            Debug.Log("Game Obj Load FAIL");
            return null;
        }

        // 패널사용시 
        // 이미 생성한 적이 있던 패널은 panelInstance에서 꺼내서 사용함
        if (panelInstance.ContainsKey(pgo.name))
        {
            var bp = panelInstance[pgo.name].GetComponent<BasePanel>();
            bp.transform.SetParent(parent, false);
            bp.Init();
            panelInstance[pgo.name].SetActive(true); // 패널 활성화
            panelStack.Push(panelInstance[pgo.name]); // 패널 스택에 추가
            return panelInstance[pgo.name];
        }
        // 없다면 새로 생성해서 반환함
        else
        {
            _createdPanel = Instantiate(pgo, parent);
            panelInstance.Add(pgo.name, _createdPanel);
            panelStack.Push(_createdPanel); // 패널 스택에 추가
            return _createdPanel; // 실제 생성된 패널 반환
        }
    }


    // UICtrl에 사용 종료된 패널들을 가지고 있게함
    public void HidePanel(GameObject panel)
    {
        if (panelStack.Count > 0)
        {
            GameObject topPanel = panelStack.Pop(); // 스택에서 제거
            topPanel.transform.SetParent(transform, false);
            topPanel.SetActive(false); // 패널 비활성화
        }
        else
        {
            Debug.Log("No panels to hide.");
        }
    }
}
