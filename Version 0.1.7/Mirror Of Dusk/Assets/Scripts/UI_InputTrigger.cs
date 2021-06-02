using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DataEvent : UnityEvent
{

}

public class UI_InputTrigger : MonoBehaviour {

    //InputReader inputReader;
    public DataEvent m_MyEvent = new DataEvent();
    private EventManager eventManager;
    //private UnityAction<GameObject> someListener;
    

    public void InvokeUpdateData()
    {
        m_MyEvent.Invoke();
    }


    // Use this for initialization
    void Awake () {
        
        //inputReader = GameObject.Find("EventSystem").GetComponent<InputReader>();
        /*if (m_MyEvent == null)
            m_MyEvent = new DataEvent();*/

        //someDataListener = new UnityAction<GameObject>();
        //someListener = new UnityAction<GameObject>(ChangeGameData);
        //m_MyEvent.AddListener(assignDataField);
    }
	
	// Update is called once per frame
	void Update () {
        /*if (Input.anyKeyDown && m_MyEvent != null)
        {
            m_MyEvent.Invoke();
        }*/
    }



    void FixedUpdate()
    {

    }

    void OnEnable()
    {
        //EventManager.StartListening("test", someListener);
        //EventManager.StartListening("ChangeGameData", someListener);
    }

    void OnDisable()
    {
        //EventManager.StopListening("test", someListener);
        //EventManager.StopListening("ChangeGameData", someListener);
    }

    public void SomeFunction(GameObject data)
    {
        //Debug.Log("SomeFunction was called with argument ");
        //Debug.Log("SomeFunction was called with argument " + data + " of type " + data.GetType());
        //Debug.Log(data.GetComponent<GameData>().modeStyle);
    }

    public void ChangeGameData(GameObject data)
    {
        //Debug.Log("SomeFunction was called with argument ");
        //Debug.Log("SomeFunction was called with argument " + data + " of type " + data.GetType());
        //Debug.Log(data.GetComponent<GameData>().modeStyle);
    }
}
