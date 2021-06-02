using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MainMenuBackgroundProperties
{
    public static string GetMainMenuSectionScene(Scenes section)
    {
        switch (section)
        {
            case Scenes.scene_title:
                return "scene_title";
            default:
                return "scene_title";
        }
    }

    public static int GetMainMenuBackgroundSection(Scenes section)
    {
        switch (section)
        {
            case Scenes.scene_title:
                return 0;
            default:
                return 0;
        }
    }

    public static string[] sections = new string[]
    {
        "section_default",
        "section_solo"
    };

    public class DefaultGroup : AbstractMainMenuBackgroundProperties<MainMenuBackgroundProperties.DefaultGroup.State, MainMenuBackgroundProperties.DefaultGroup.States>
    {
        public DefaultGroup(int moveTrigger, State[] states) : base((float)moveTrigger, states)
        {

        }

        public static MainMenuBackgroundProperties.DefaultGroup GetMode(MainMenuSections assignedSection)
        {
            int mTrigger = 0;
            List<MainMenuBackgroundProperties.DefaultGroup.State> list = new List<MainMenuBackgroundProperties.DefaultGroup.State>();
            if (assignedSection == MainMenuSections.Default)
            {
                mTrigger = 1;
                list.Add(new MainMenuBackgroundProperties.DefaultGroup.State(mTrigger, MainMenuBackgroundProperties.DefaultGroup.States.Main, new MainMenuBackgroundProperties.DefaultGroup.Tree_1[] {
                    new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Solo, new Tree_1.PositionProperties(-2400f, -1600f, 2f, 2f) },
                    { MainMenuSections.Multiplayer, new Tree_1.PositionProperties(-4000f, -600f, 1.6f, 1.6f) },
                    { MainMenuSections.Online, new Tree_1.PositionProperties(3200f, -1400f, 1.9f, 1.9f) },
                    { MainMenuSections.Data, new Tree_1.PositionProperties(4000f, -600f, 1.6f, 1.6f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Solo, new Tree_1.PositionProperties(-2400f, -1600f, 2f, 2f) },
                    { MainMenuSections.Multiplayer, new Tree_1.PositionProperties(-4000f, -600f, 1.6f, 1.6f) },
                    { MainMenuSections.Online, new Tree_1.PositionProperties(3200f, -1400f, 1.9f, 1.9f) },
                    { MainMenuSections.Data, new Tree_1.PositionProperties(4000f, -600f, 1.6f, 1.6f) }
                }, 1f, -320f, -470f, 0f, 1.3f, 1.3f), new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Solo, new Tree_1.PositionProperties(2000f, -1700f, 1.9f, 1.9f) },
                    { MainMenuSections.Multiplayer, new Tree_1.PositionProperties(-3200f, -500f, 1.5f, 1.5f) },
                    { MainMenuSections.Online, new Tree_1.PositionProperties(3600f, -1500f, 1.8f, 1.8f) },
                    { MainMenuSections.Data, new Tree_1.PositionProperties(4800f, -500f, 1.5f, 1.5f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Solo, new Tree_1.PositionProperties(2000f, -1700f, 1.9f, 1.9f) },
                    { MainMenuSections.Multiplayer, new Tree_1.PositionProperties(-3200f, -500f, 1.5f, 1.5f) },
                    { MainMenuSections.Online, new Tree_1.PositionProperties(3600f, -1500f, 1.8f, 1.8f) },
                    { MainMenuSections.Data, new Tree_1.PositionProperties(4800f, -500f, 1.5f, 1.5f) }
                }, 1f, 480f, -410f, 0f, 1f, 1f), new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Solo, new Tree_1.PositionProperties(-1800f, -1800f, 2.3f, 2.3f) },
                    { MainMenuSections.Multiplayer, new Tree_1.PositionProperties(-4800f, -400f, 1.9f, 1.9f) },
                    { MainMenuSections.Online, new Tree_1.PositionProperties(-2600f, -1700f, 2.2f, 2.2f) },
                    { MainMenuSections.Data, new Tree_1.PositionProperties(3200f, -400f, 1.9f, 1.9f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Solo, new Tree_1.PositionProperties(-1800f, -1800f, 2.3f, 2.3f) },
                    { MainMenuSections.Multiplayer, new Tree_1.PositionProperties(-4800f, -400f, 1.9f, 1.9f) },
                    { MainMenuSections.Online, new Tree_1.PositionProperties(-2600f, -1700f, 2.2f, 2.2f) },
                    { MainMenuSections.Data, new Tree_1.PositionProperties(3200f, -400f, 1.9f, 1.9f) }
                }, 1f, -1120f, -530f, 0f, 1.6f, 1.6f)
                }));
            }
            if (assignedSection == MainMenuSections.Solo)
            {
                mTrigger = 1;
                list.Add(new MainMenuBackgroundProperties.DefaultGroup.State(mTrigger, MainMenuBackgroundProperties.DefaultGroup.States.Main, new MainMenuBackgroundProperties.DefaultGroup.Tree_1[] {
                    new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) },
                    { MainMenuSections.Enter, new Tree_1.PositionProperties(-1200f, -1600f, 1.7f, 1.7f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(250f, 0f, 0f, 0f) },
                    { MainMenuSections.Solo, new Tree_1.PositionProperties(250f, 0f, 0f, 0f) }
                }, 1f, -48f, -291f, 0f, 1f, 1f), new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) },
                    { MainMenuSections.Enter, new Tree_1.PositionProperties(1400f, -1600f, 1.3f, 1.3f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(300f, 0f, 0f, 0f) },
                    { MainMenuSections.Solo, new Tree_1.PositionProperties(300f, 0f, 0f, 0f) }
                }, 1f, 351f, -444f, 0f, 0.8f, 0.8f), new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) },
                    { MainMenuSections.Enter, new Tree_1.PositionProperties(-1800f, -2000f, 1.2f, 1.2f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(200f, 0f, 0f, 0f) },
                    { MainMenuSections.Solo, new Tree_1.PositionProperties(200f, 0f, 0f, 0f) }
                }, 1f, -442f, -302f, 0f, 0.5f, 0.5f), new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) },
                    { MainMenuSections.Enter, new Tree_1.PositionProperties(-2400f, -1600f, 1.9f, 1.7f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(150f, 0f, 0f, 0f) },
                    { MainMenuSections.Solo, new Tree_1.PositionProperties(150f, 0f, 0f, 0f) }
                }, 1f, -749f, -585f, 0f, 1.4f, 1.2f), new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) },
                    { MainMenuSections.Enter, new Tree_1.PositionProperties(2200f, -1700f, 1.2f, 1f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(350f, 0f, 0f, 0f) },
                    { MainMenuSections.Solo, new Tree_1.PositionProperties(350f, 0f, 0f, 0f) }
                }, 1f, 881f, -375f, 0f, 0.7f, 0.5f)
                }));
            }
            if (assignedSection == MainMenuSections.Multiplayer)
            {
                mTrigger = 1;
                list.Add(new MainMenuBackgroundProperties.DefaultGroup.State(mTrigger, MainMenuBackgroundProperties.DefaultGroup.States.Main, new MainMenuBackgroundProperties.DefaultGroup.Tree_1[] {
                    new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(1850f, -100f, 0f, 0f) },
                    { MainMenuSections.Multiplayer, new Tree_1.PositionProperties(1850f, -100f, 0f, 0f) }
                }, 1f, -48f, -291f, 0f, 1f, 1f), new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(1700f, -100f, 0f, 0f) },
                    { MainMenuSections.Multiplayer, new Tree_1.PositionProperties(1700f, -100f, 0f, 0f) }
                }, 1f, 351f, -444f, 0f, 0.8f, 0.8f), new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(1800f, -100f, 0f, 0f) },
                    { MainMenuSections.Multiplayer, new Tree_1.PositionProperties(1800f, -100f, 0f, 0f) }
                }, 1f, -442f, -302f, 0f, 0.5f, 0.5f), new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(1900f, -100f, 0f, 0f) },
                    { MainMenuSections.Multiplayer, new Tree_1.PositionProperties(1900f, -100f, 0f, 0f) }
                }, 1f, -749f, -585f, 0f, 1.4f, 1.2f), new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(1950f, -100f, 0f, 0f) },
                    { MainMenuSections.Multiplayer, new Tree_1.PositionProperties(1950f, -100f, 0f, 0f) }
                }, 1f, 881f, -375f, 0f, 0.7f, 0.5f)
                }));
            }
            if (assignedSection == MainMenuSections.Online)
            {
                mTrigger = 1;
                list.Add(new MainMenuBackgroundProperties.DefaultGroup.State(mTrigger, MainMenuBackgroundProperties.DefaultGroup.States.Main, new MainMenuBackgroundProperties.DefaultGroup.Tree_1[] {
                    new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(-450f, -100f, 0f, 0f) },
                    { MainMenuSections.Online, new Tree_1.PositionProperties(-450f, -100f, 0f, 0f) }
                }, 1f, -408f, -584f, 0f, 1.4f, 1.3f), new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(-300f, -100f, 0f, 0f) },
                    { MainMenuSections.Online, new Tree_1.PositionProperties(-300f, -100f, 0f, 0f) }
                }, 1f, 310f, -362f, 0f, 0.8f, 0.7f), new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(-500f, -100f, 0f, 0f) },
                    { MainMenuSections.Online, new Tree_1.PositionProperties(-500f, -100f, 0f, 0f) }
                }, 1f, -784f, -294f, 0f, 0.7f, 0.5f), new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(-250f, -100f, 0f, 0f) },
                    { MainMenuSections.Online, new Tree_1.PositionProperties(-250f, -100f, 0f, 0f) }
                }, 1f, 1010f, -633f, 0f, 1.5f, 1.2f), new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(-350f, -100f, 0f, 0f) },
                    { MainMenuSections.Online, new Tree_1.PositionProperties(-350f, -100f, 0f, 0f) }
                }, 1f, -23f, -224f, 0f, 0.4f, 0.4f)
                }));
            }
            if (assignedSection == MainMenuSections.Data)
            {
                mTrigger = 1;
                list.Add(new MainMenuBackgroundProperties.DefaultGroup.State(mTrigger, MainMenuBackgroundProperties.DefaultGroup.States.Main, new MainMenuBackgroundProperties.DefaultGroup.Tree_1[] {
                    new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(-2050f, -100f, 0f, 0f) },
                    { MainMenuSections.Data, new Tree_1.PositionProperties(-2050f, -100f, 0f, 0f) }
                }, 1f, -644f, -487f, 0f, 1f, 0.9f), new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(-1975f, -100f, 0f, 0f) },
                    { MainMenuSections.Data, new Tree_1.PositionProperties(-1975f, -100f, 0f, 0f) }
                }, 1f, -233f, -283f, 0f, 0.5f, 0.6f), new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(-1900f, -100f, 0f, 0f) },
                    { MainMenuSections.Data, new Tree_1.PositionProperties(-1900f, -100f, 0f, 0f) }
                }, 1f, 249f, -500f, 0f, 1f, 1f), new MainMenuBackgroundProperties.DefaultGroup.Tree_1(new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(0f, 0f, 0f, 0f) }
                }, new Dictionary<MainMenuSections, Tree_1.PositionProperties>()
                {
                    { MainMenuSections.Default, new Tree_1.PositionProperties(-1825f, -100f, 0f, 0f) },
                    { MainMenuSections.Data, new Tree_1.PositionProperties(-1825f, -100f, 0f, 0f) }
                }, 1f, 766f, -376f, 0f, 0.7f, 0.6f)
                }));
            }
            return new MainMenuBackgroundProperties.DefaultGroup(mTrigger, list.ToArray());
        }

        public class State : AbstractMainMenuBackgroundState<MainMenuBackgroundProperties.DefaultGroup.States>
        {
            public State(float moveTrigger, MainMenuBackgroundProperties.DefaultGroup.States stateName,
                MainMenuBackgroundProperties.DefaultGroup.Tree_1[] treeCollection) : base(moveTrigger, stateName)
            {
                this.treeCollection = treeCollection;
            }

            public readonly MainMenuBackgroundProperties.DefaultGroup.Tree_1[] treeCollection;
        }

        public class Entity : AbstractMainMenuBackgroundGroupProperty
        {
            private MainMenuBackgroundProperties.DefaultGroup properties { get; set; }

            public string SetSortingLayer
            {
                set {
                    this.spriteRenderer.sortingLayerName = value;
                    this.shadowRenderer.sortingLayerName = value;
                }
            }

            public virtual void BackgroundInit(MainMenuBackgroundProperties.DefaultGroup properties, int key)
            {
                this.properties = properties;
            }
            
            /*public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
            {
            }*/
        }

        public enum States
        {
            Main,
            Generic,
            Tree
        }

        public class Tree_1
        {
            public readonly Dictionary<MainMenuSections, PositionProperties> nextPositionProperties;
            public readonly Dictionary<MainMenuSections, PositionProperties> prevPositionProperties;
            public readonly float startDelay;
            public readonly float xPos;
            public readonly float yPos;
            public readonly float zPos;
            public readonly float sizeX;
            public readonly float sizeY;

            public Tree_1(Dictionary<MainMenuSections, PositionProperties> nextPositionProperties, Dictionary<MainMenuSections, PositionProperties> prevPositionProperties, float startDelay, float xPos, float yPos, float zPos, float sizeX, float sizeY)
            {
                this.nextPositionProperties = nextPositionProperties;
                this.prevPositionProperties = prevPositionProperties;
                this.startDelay = startDelay;
                this.xPos = xPos;
                this.yPos = yPos;
                this.zPos = zPos;
                this.sizeX = sizeX;
                this.sizeY = sizeY;
            }

            public class PositionProperties
            {
                public float posX;
                public float posY;
                public float sizeX;
                public float sizeY;

                public PositionProperties(float posX, float posY, float sizeX, float sizeY)
                {
                    this.posX = posX;
                    this.posY = posY;
                    this.sizeX = sizeX;
                    this.sizeY = sizeY;
                }
            }
        }
    }
}
