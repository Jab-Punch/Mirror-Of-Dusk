using System;

public class OnlineManager
{
    private static OnlineManager instance;

    public static OnlineManager Instance
    {
        get
        {
            if (OnlineManager.instance == null)
                OnlineManager.instance = new OnlineManager();
            return OnlineManager.instance;
        }
    }

    public OnlineInterface Interface { get; private set; }

    public void Init()
    {
        this.Interface = new OnlineInterfaceSteam();
        this.Interface.Init();
    }
}
