using System;

public class PlmManager
{
    private static PlmManager instance;

    public static PlmManager Instance
    {
        get
        {
            if (PlmManager.instance == null)
            {
                PlmManager.instance = new PlmManager();
            }
            return PlmManager.instance;
        }
    }

    public PlmInterface Interface { get; private set; }

    public void Init()
    {
        this.Interface = new DummyPlmInterface();
        this.Interface.Init();
        this.Interface.OnSuspend += this.OnSuspend;
        this.Interface.OnResume += this.OnResume;
        this.Interface.OnConstrained += this.OnConstrained;
        this.Interface.OnUnconstrained += this.OnUnconstrained;
    }

    private void OnSuspend() { }
    
    private void OnResume() { }
    
    private void OnConstrained() { }

    private void OnUnconstrained() { }
}
