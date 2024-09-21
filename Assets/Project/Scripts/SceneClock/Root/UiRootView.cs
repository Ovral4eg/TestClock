using UnityEngine;

public class UiRootView : MonoBehaviour
{
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private Transform Container_SceneUi;

    private void Awake()
    {
        HideLoadingScreen();
    }

    public void ShowLoadingScreen()
    {
        _loadingScreen.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        _loadingScreen.SetActive(false);
    }

    public void AttachSceneUi(GameObject sceneUi)
    {
        ClearSceneUi();

        sceneUi.transform.SetParent(Container_SceneUi, false);

    }

    private void ClearSceneUi()
    {
        var childCount = Container_SceneUi.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Destroy(Container_SceneUi.GetChild(i).gameObject);
        }
    }
}
