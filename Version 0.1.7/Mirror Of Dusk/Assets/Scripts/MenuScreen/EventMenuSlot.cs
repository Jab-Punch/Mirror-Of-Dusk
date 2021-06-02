using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventMenuSlot : MenuSlot
{
    [System.Serializable]
    public class EventMenuActivate : UnityEvent
    {

    }

    [System.Serializable]
    public class ScreenHighlightSetA
    {
        public bool active = false;
        private bool _summonCoroutineActive = false;
        private bool _destroyCoroutineActive = false;
        public GameObject _prefab;
        private SpriteRenderer[] _screenHighlightSpr = new SpriteRenderer[2];

        public bool SummonCoroutineActive
        {
            get { return _summonCoroutineActive; }
            set { _summonCoroutineActive = value; }
        }

        public bool DestroyCoroutineActive
        {
            get { return _destroyCoroutineActive; }
            set { _destroyCoroutineActive = value; }
        }

        public SpriteRenderer[] ScreenHighlightSpr
        {
            get { return _screenHighlightSpr; }
            set { _screenHighlightSpr = value; }
        }
    }

    [System.Serializable]
    public class BoxHighlightSetA
    {
        public bool active = false;
        private GameObject _boxHighlight;
        private SpriteRenderer _boxHighlightSpr;

        public GameObject BoxHighlight
        {
            get { return _boxHighlight; }
            set { _boxHighlight = value; }
        }

        public SpriteRenderer BoxHighlightSpr
        {
            get { return _boxHighlightSpr; }
            set { _boxHighlightSpr = value; }
        }
    }

    public GameObject rootMenuInstance;
    private MenuEventManager menuEventManagerInstance;
    private RectTransform slotRect;
    private SpriteRenderer spr;
    private NewMenuScreenRoot _rootMenu;
    private GameObject boxHighlight;
    private SpriteRenderer boxHighlightSpr;
    private bool _instantly = false;
    public bool Instantly
    {
        get { return _instantly; }
        set { _instantly = value; }
    }

    [Header("Manager Prefabs")]
    public ScreenHighlightSetA screenHighlightSetA;
    public BoxHighlightSetA boxHighlightSetA;

    [Header("Sprite Prefabs")]
    public Sprite _deselected;
    public Sprite _selected;

    [Header("Event")]
    public EventMenuActivate m_EventMenuActivate = new EventMenuActivate();

    private void Awake()
    {
        if (rootMenuInstance != null)
        {
            rootMenu = rootMenuInstance;
        }
        _rootMenu = rootMenu.GetComponent<NewMenuScreenRoot>();
        menuEventManagerInstance = rootMenu.GetComponent<MenuEventManager>();
        slotRect = gameObject.GetComponent<RectTransform>();
        spr = gameObject.transform.GetComponentInChildren<SpriteRenderer>();
        //boxHighlight = _rootMenu._boxHighlight;
        //boxHighlightSpr = boxHighlight.GetComponent<SpriteRenderer>();
        if (screenHighlightSetA.active)
        {
            screenHighlightSetA.ScreenHighlightSpr = screenHighlightSetA._prefab.GetComponentsInChildren<SpriteRenderer>();
        }
        if (boxHighlightSetA.active)
        {
            if (_rootMenu._boxHighlight == null)
            {
                boxHighlightSetA.active = false;
            } else
            {
                boxHighlightSetA.BoxHighlight = _rootMenu._boxHighlight;
                boxHighlightSetA.BoxHighlightSpr = boxHighlightSetA.BoxHighlight.GetComponent<SpriteRenderer>();
            }
        }
    }

    void Start()
    {

    }

    private void OnEnable()
    {
        if (m_EventMenuActivate.GetPersistentEventCount() > 0)
        {
            menuEventManagerInstance.StartListening("ActivateSlot", ActivateSlot);
        }
        if (screenHighlightSetA.active)
        {
            if (screenHighlightSetA.DestroyCoroutineActive)
            {
                StopCoroutine("DestroyHighlighter");
                screenHighlightSetA.DestroyCoroutineActive = false;
            }
            screenHighlightSetA.ScreenHighlightSpr[0].color = new Color(screenHighlightSetA.ScreenHighlightSpr[0].color.r, screenHighlightSetA.ScreenHighlightSpr[0].color.g, screenHighlightSetA.ScreenHighlightSpr[0].color.b, 0);
            screenHighlightSetA.ScreenHighlightSpr[1].color = new Color(screenHighlightSetA.ScreenHighlightSpr[1].color.r, screenHighlightSetA.ScreenHighlightSpr[1].color.g, screenHighlightSetA.ScreenHighlightSpr[1].color.b, 0);
            if (!Instantly)
            {
                StartCoroutine("SummonHighlighter");
            } else
            {
                InstantSummonHighlighter();
            }
        }
        if (boxHighlightSetA.active)
        {
            boxHighlightSetA.BoxHighlight.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, boxHighlightSetA.BoxHighlight.transform.position.z);
            boxHighlightSetA.BoxHighlightSpr.size = new Vector2(slotRect.rect.width, slotRect.rect.height);
        }
        Instantly = false;
        //boxHighlight.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, boxHighlight.transform.position.z);
        //boxHighlightSpr.size = new Vector2(slotRect.rect.width, slotRect.rect.height);
    }

    private void OnDisable()
    {
        if (m_EventMenuActivate.GetPersistentEventCount() > 0)
        {
            menuEventManagerInstance.StopListening("ActivateSlot", ActivateSlot);
        }
        if (screenHighlightSetA.active)
        {
            if (screenHighlightSetA.SummonCoroutineActive)
            {
                StopCoroutine("SummonHighlighter");
                screenHighlightSetA.SummonCoroutineActive = false;
            }
            if (!Instantly)
            {
                if (this.gameObject.activeInHierarchy)
                {
                    StartCoroutine("DestroyHighlighter");
                }
            } else
            {
                InstantDestroyHighlighter();
            }
            
        }
        Instantly = false;
    }

    private void ActivateSlot(System.Object arg0)
    {
        m_EventMenuActivate.Invoke();
    }

    public void selectUpdate()
    {
        spr.sprite = _selected;
    }

    public void deselectUpdate()
    {
        spr.sprite = _deselected;
    }

    private IEnumerator SummonHighlighter()
    {
        screenHighlightSetA.SummonCoroutineActive = true;
        float curAlpha = screenHighlightSetA.ScreenHighlightSpr[0].color.a;
        while (curAlpha < 0.5f)
        {
            curAlpha += 0.05f;
            if (curAlpha > 0.5f)
            {
                curAlpha = 0.5f;
            }
            screenHighlightSetA.ScreenHighlightSpr[0].color = new Color(screenHighlightSetA.ScreenHighlightSpr[0].color.r, screenHighlightSetA.ScreenHighlightSpr[0].color.g, screenHighlightSetA.ScreenHighlightSpr[0].color.b, curAlpha);
            screenHighlightSetA.ScreenHighlightSpr[1].color = new Color(screenHighlightSetA.ScreenHighlightSpr[1].color.r, screenHighlightSetA.ScreenHighlightSpr[1].color.g, screenHighlightSetA.ScreenHighlightSpr[1].color.b, curAlpha);
            yield return null;
        }
        screenHighlightSetA.SummonCoroutineActive = false;
        yield return null;
    }

    private IEnumerator DestroyHighlighter()
    {
        screenHighlightSetA.DestroyCoroutineActive = true;
        float curAlpha = screenHighlightSetA.ScreenHighlightSpr[0].color.a;
        while (curAlpha > 0f)
        {
            curAlpha -= 0.05f;
            if (curAlpha <= 0f)
            {
                curAlpha = 0f;
            }
            screenHighlightSetA.ScreenHighlightSpr[0].color = new Color(screenHighlightSetA.ScreenHighlightSpr[0].color.r, screenHighlightSetA.ScreenHighlightSpr[0].color.g, screenHighlightSetA.ScreenHighlightSpr[0].color.b, curAlpha);
            screenHighlightSetA.ScreenHighlightSpr[1].color = new Color(screenHighlightSetA.ScreenHighlightSpr[1].color.r, screenHighlightSetA.ScreenHighlightSpr[1].color.g, screenHighlightSetA.ScreenHighlightSpr[1].color.b, curAlpha);
            yield return null;
        }
        screenHighlightSetA.DestroyCoroutineActive = false;
        yield return null;
    }

    public void InstantSummonHighlighter()
    {
        screenHighlightSetA.ScreenHighlightSpr[0].color = new Color(screenHighlightSetA.ScreenHighlightSpr[0].color.r, screenHighlightSetA.ScreenHighlightSpr[0].color.g, screenHighlightSetA.ScreenHighlightSpr[0].color.b, 0.5f);
        screenHighlightSetA.ScreenHighlightSpr[1].color = new Color(screenHighlightSetA.ScreenHighlightSpr[1].color.r, screenHighlightSetA.ScreenHighlightSpr[1].color.g, screenHighlightSetA.ScreenHighlightSpr[1].color.b, 0.5f);
    }

    public void InstantDestroyHighlighter()
    {
        screenHighlightSetA.ScreenHighlightSpr[0].color = new Color(screenHighlightSetA.ScreenHighlightSpr[0].color.r, screenHighlightSetA.ScreenHighlightSpr[0].color.g, screenHighlightSetA.ScreenHighlightSpr[0].color.b, 0f);
        screenHighlightSetA.ScreenHighlightSpr[1].color = new Color(screenHighlightSetA.ScreenHighlightSpr[1].color.r, screenHighlightSetA.ScreenHighlightSpr[1].color.g, screenHighlightSetA.ScreenHighlightSpr[1].color.b, 0f);
    }
}
