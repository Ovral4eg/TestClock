using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

public class EntryPoint
{
    private static EntryPoint _instance;
    private Coroutines _coroutines;
    private UiRootView _uiRoot;
    private ITimeDataProvider _dataProvider;
    private ILocalTimer _timer;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void AutoStartGame()
    {
        //����� ����������
        //Application.targetFrameRate = 60;
        Application.runInBackground = true;

        _instance = new EntryPoint();
        _instance.Run();
    }    

    private EntryPoint()
    {
        _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
        Object.DontDestroyOnLoad(_coroutines.gameObject);

        var prefabUiRoot = Resources.Load<UiRootView>("UiRoot");
        _uiRoot = Object.Instantiate(prefabUiRoot);
        Object.DontDestroyOnLoad(_uiRoot.gameObject);

        //���������� CORS API
#if UNITY_EDITOR
        _dataProvider = new YandexDataProvider();
#else
        _dataProvider = new WorldTimeAPIDataProvider();
#endif
        //������ CORS API, ������� ����������� � API �����
        //GoogleTimeZoneAPIDataProvider;
        //TimeZoneDBDataProvider;
        //AbstractAPIDataProvider;  

        //������� ��������� ������
        _timer = new GameObject("Timer").AddComponent<UpdateLocalTimer>();       
    }

    private ITimeView _uiClockView;
    private void Run()
    {
        var prefabUiClock = Resources.Load<UiClockView>("UiClockView");
        _uiClockView = Object.Instantiate(prefabUiClock);
        _uiRoot.AttachSceneUi(_uiClockView.GameObject);
        _uiClockView.Init(_dataProvider,_timer);
    }   
}