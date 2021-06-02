using System;

namespace GCFreeUtils
{
    public class GCFreeActionList
    {
        private const string ERR_BUFFER_TOO_SMALL = "[GCFreeActionList] Current buffer too small. Consider increasing the initial size or set as auto resizeable.";
        private const string LOG_RESIZING = "[GCFreeActionList] Resizing buffer. Maybe you want to increase the initial size.";
        private Action[] actionList;
        private bool autoResizeable;
        
        public GCFreeActionList(int size) : this(size, true)
        {

        }
        
        public GCFreeActionList(int size, bool autoResizeable)
        {
            this.actionList = new Action[size];
            this.autoResizeable = autoResizeable;
            this.Count = 0;
        }

        public int Count { get; private set; }
        
        public void Add(Action action)
        {
            if (this.Count == this.actionList.Length)
            {
                if (!this.autoResizeable)
                {
                    UnityEngine.Debug.LogError("[GCFreeActionList] Current buffer too small. Consider increasing the initial size or set as auto resizeable.", null);
                    return;
                }
                Action[] destinationArray = new Action[this.actionList.Length * 2];
                Array.Copy(this.actionList, destinationArray, this.actionList.Length);
                this.actionList = destinationArray;
            }
            this.actionList[this.Count] = action;
            this.Count++;
        }

        public void Remove(Action action)
        {
            if (this.Count > 0)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (this.actionList[i] == action)
                    {
                        if (this.Count > 1)
                        {
                            this.actionList[i] = this.actionList[this.Count - 1];
                        }
                        else
                        {
                            this.actionList[i] = null;
                        }
                        this.Count--;
                        break;
                    }
                }
            }
        }

        public void Call()
        {
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    if (this.actionList[i] != null)
                    {
                        this.actionList[i]();
                    }
                }
                catch (Exception message)
                {
                    UnityEngine.Debug.LogError(message, null);
                }
            }
        }
    }
}