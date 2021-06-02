using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectList : MonoBehaviour
{
    [Serializable]
    public class SceneGroup
    {
        public bool included;
        public Scenes scene;
    }

    public StageSelectList.SceneGroup[] scenes;
    public Button button;
    public RectTransform contentPanel;
    

    private void Awake()
    {
        base.useGUILayout = false;
        this.SetupList();
    }

    private void SetupList()
    {
        List<Scenes> list = new List<Scenes>();
        foreach (Scenes scenes in Enum.GetValues(typeof(Scenes)))
        {
            if (this.GetSceneGroup(scenes).included)
            {
                list.Add(scenes);
            }
        }
        int num = 0;
        foreach (Scenes scenes2 in list)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.button.gameObject);
            Button b = gameObject.GetComponent<Button>();
            string text = scenes2.ToString().Replace("scene_", string.Empty).Replace("stage_", string.Empty);
            b.name = scenes2.ToString();
            gameObject.GetComponentInChildren<Text>().text = text;
            b.onClick.AddListener(delegate ()
            {
                SceneLoader.LoadScene(b.name, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass);
            });
            b.transform.SetParent(this.button.transform.parent);
            b.transform.localPosition = Vector3.zero;
            b.transform.localEulerAngles = Vector3.zero;
            b.transform.localScale = Vector3.one;
        }
        this.button.gameObject.SetActive(false);
        this.contentPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (30f * (float)num));
    }

    public StageSelectList.SceneGroup GetSceneGroup(Scenes s)
    {
        foreach (StageSelectList.SceneGroup sceneGroup in this.scenes)
        {
            if (sceneGroup.scene == s)
            {
                return sceneGroup;
            }
        }
        return new StageSelectList.SceneGroup();
    }

    public bool ContainsScene(Scenes s)
    {
        foreach (StageSelectList.SceneGroup sceneGroup in this.scenes)
        {
            if (sceneGroup.scene == s)
            {
                return true;
            }
        }
        return false;
    }
}
