using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTableData : MonoBehaviour {

    private CharacterSelectManager characterSelectManager;

    public int playerId;
    private string currentSelect = "???";
    private Dictionary<string, Dictionary<int, SpriteRenderer>> boxes;  //Lists of three sets of color boxes for display
    private int boxCount = 0;

    private CSPlayerData csPlayerData;
    private FighterDataCollection fighterDataCollection;
    private TempFDC[] fighterSelectData;

    private class TempFDC   //Instance of FighterDataCollection (Data of character colors and statuses)
    {
        public string name;
        public int id;
        public bool active = true;
        public GameObject gm;
        private FighterData _fighterData;

        public TempFDC(string name, int id, bool active, GameObject gm)
        {
            this.name = name;
            this.id = id;
            this.active = active;
            this.gm = gm;
            _fighterData = gm.GetComponent<FighterData>();
        }

        public FighterData fighterData
        {
            get { return _fighterData; }
        }
    }

    private void Awake()
    {
        characterSelectManager = CharacterSelectManager.characterSelectManager;

        csPlayerData = characterSelectManager.players[playerId].GetComponent<CSPlayerData>();
        fighterDataCollection = characterSelectManager.fighterDataCollection.GetComponent<FighterDataCollection>();
        fighterSelectData = new TempFDC[fighterDataCollection.fighterSelectData.Length];
        for (int i = 0; i < fighterSelectData.Length; i++)
        {
            fighterSelectData[i] = new TempFDC(fighterDataCollection.fighterSelectData[i].name, fighterDataCollection.fighterSelectData[i].id, fighterDataCollection.fighterSelectData[i].active, fighterDataCollection.fighterSelectData[i].fighterData);
        }
        boxes = new Dictionary<string, Dictionary<int, SpriteRenderer>>();
        boxes.Add("A", new Dictionary<int, SpriteRenderer>());
        boxes.Add("B", new Dictionary<int, SpriteRenderer>());
        boxes.Add("C", new Dictionary<int, SpriteRenderer>());
        SpriteRenderer[] sprCount = gameObject.transform.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sprCount.Length; i++)
        {
            if (sprCount[i].gameObject.name.Contains("ColorSquare") && !sprCount[i].gameObject.name.Contains("Back"))
            {
                string numberName = sprCount[i].gameObject.transform.parent.gameObject.name;
                int number = System.Convert.ToInt32(numberName.Substring(numberName.Length - 1, 1));
                string letter = sprCount[i].gameObject.name.Substring(sprCount[i].gameObject.name.Length - 1, 1);
                boxes[letter].Add(number, sprCount[i]);
                boxCount++;
            }
        }
    }

    public void updateTable()
    {
        if (currentSelect != csPlayerData.selectedCharacter)
        {
            currentSelect = csPlayerData.selectedCharacter;
            bool erase = true;
            for (int i = 0; i < fighterSelectData.Length; i++)
            {
                if (fighterSelectData[i].name == currentSelect && fighterSelectData[i].active)
                {
                    //FighterData fCData = fighterDataCollection.fighterSelectData[i].fighterData.GetComponent<FighterData>();
                    for (int j = 0; j < boxes["A"].Count; j++)
                    {
                        for (int k = 0; k < fighterSelectData[i].fighterData.fighterColorData.Length; k++)
                        {
                            boxes["A"][fighterSelectData[i].fighterData.fighterColorData[k].colorCode].color = fighterSelectData[i].fighterData.fighterColorData[k].selectColors.colorA;
                        }
                    }
                    for (int j = 0; j < boxes["B"].Count; j++)
                    {
                        for (int k = 0; k < fighterSelectData[i].fighterData.fighterColorData.Length; k++)
                        {
                            boxes["B"][fighterSelectData[i].fighterData.fighterColorData[k].colorCode].color = fighterSelectData[i].fighterData.fighterColorData[k].selectColors.colorB;
                        }
                    }
                    for (int j = 0; j < boxes["C"].Count; j++)
                    {
                        for (int k = 0; k < fighterSelectData[i].fighterData.fighterColorData.Length; k++)
                        {
                            boxes["C"][fighterSelectData[i].fighterData.fighterColorData[k].colorCode].color = fighterSelectData[i].fighterData.fighterColorData[k].selectColors.colorC;
                        }
                    }
                    erase = false;
                    break;
                }
            }
            if (erase)
            {
                for (int j = 0; j < boxes["A"].Count; j++)
                {
                    boxes["A"][j + 1].color = new Color(1, 1, 1, 1);
                }
                for (int j = 0; j < boxes["B"].Count; j++)
                {
                    boxes["B"][j + 1].color = new Color(1, 1, 1, 1);
                }
                for (int j = 0; j < boxes["C"].Count; j++)
                {
                    boxes["C"][j + 1].color = new Color(1, 1, 1, 1);
                }
            }
        }
    }
}
