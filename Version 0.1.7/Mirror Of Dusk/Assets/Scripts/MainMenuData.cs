using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class MainMenuData
{
    private static bool _initialized = false;
    public static bool inGame = false;
    private static MainMenuData _mainMenuData;

    [SerializeField]
    private MainMenuData.SectionDataManager mainMenuSectionDataManager = new MainMenuData.SectionDataManager();

    public delegate void MainMenuDataInitHandler(bool success);

    [Serializable]
    public class SectionData
    {
        public Scenes id;
        public bool sessionStarted;
        public MainMenuSections sectionPlacement;
    }

    [Serializable]
    public class SectionDataManager
    {
        public Scenes currentSection = Scenes.scene_title;
        public List<MainMenuData.SectionData> sectionData;
        public int itemPosition = 0;

        public SectionDataManager()
        {
            this.sectionData = new List<SectionData>();
            this.itemPosition = 0;
        }

        public MainMenuData.SectionData GetCurrentSectionData()
        {
            return this.GetSectionData(this.currentSection);
        }

        public MainMenuData.SectionData GetSectionData(Scenes section)
        {
            for (int i = 0; i < this.sectionData.Count; i++)
            {
                if (this.sectionData[i].id == section)
                {
                    return this.sectionData[i];
                }
            }
            MainMenuData.SectionData sectionData = new MainMenuData.SectionData();
            sectionData.id = section;
            this.sectionData.Add(sectionData);
            return sectionData;
        }
    }

    public MainMenuData()
    {

    }

    public static bool Initialized
    {
        get
        {
            return MainMenuData._initialized;
        }
        private set
        {
            MainMenuData._initialized = value;
        }
    }

    public static MainMenuData Data
    {
        get
        {
            return MainMenuData._mainMenuData;
        }
    }

    public static void Init()
    {
        if (MainMenuData.Data == null)
        {
            MainMenuData._mainMenuData = new MainMenuData();
        }
        MainMenuData.Initialized = true;
    }

    public MainMenuData.SectionData CurrentSectionData
    {
        get
        {
            return this.mainMenuSectionDataManager.GetCurrentSectionData();
        }
    }

    public MainMenuData.SectionData GetSectionData(Scenes section)
    {
        return this.mainMenuSectionDataManager.GetSectionData(section);
    }

    public Scenes CurrentSection
    {
        get
        {
            return this.mainMenuSectionDataManager.currentSection;
        }
        set
        {
            this.mainMenuSectionDataManager.currentSection = value;
        }
    }

    public int ItemPosition
    {
        get { return this.mainMenuSectionDataManager.itemPosition; }
        set { this.mainMenuSectionDataManager.itemPosition = value; }
    }
}
