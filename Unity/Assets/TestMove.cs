using UnityEngine;
using UnityEngine.UI;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.ECS;

public class TestMove : MonoBehaviour
{
    public static TestMove Instance;

    public Button moveBtn;
    public World world;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //moveBtn.onClick.AddListener(ClickHandler);
    }

    private void ClickHandler()
    {

    }
}
