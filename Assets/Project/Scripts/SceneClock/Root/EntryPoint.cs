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
        //опции приложения
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

        //бесплатный CORS API
#if UNITY_EDITOR
        _dataProvider = new YandexDataProvider();
#else
        _dataProvider = new WorldTimeAPIDataProvider();
#endif
        //другие CORS API, требуют регистрации и API ключа
        //GoogleTimeZoneAPIDataProvider;
        //TimeZoneDBDataProvider;
        //AbstractAPIDataProvider;  

        //создаем локальный таймер
        _timer = new GameObject("Timer").AddComponent<UpdateLocalTimer>();       
    }

    private void Run()
    {
        _coroutines.StartCoroutine(StartClock());
    }

    private ITimeView _uiClockView;
    private IEnumerator StartClock()
    {
        //настраиваем отображение времени
        var prefabUiClock = Resources.Load<UiClockView>("UiClockView");
        _uiClockView = Object.Instantiate(prefabUiClock);
        _uiRoot.AttachSceneUi(_uiClockView.GameObject);
        _uiClockView.Init(_timer);

        //пытаемся синхронизировать время
        yield return TrySyncTime();

        //запускаем локальный таймер
        _timer.StartTimer(_dataProvider.DateTime);

        //отслеживаем, когда на локальном таймере пройдет час
        _timer.OnHourPass += Timer_OnHourPass;
    }

    private void Timer_OnHourPass(object sender, EventArgs e)
    {
        _coroutines. StartCoroutine(TrySyncTime());
    }

    IEnumerator TrySyncTime()
    {
        yield return _dataProvider.SyncTimeData(0);

        _timer.SetTimeServer(_dataProvider);
    }
}