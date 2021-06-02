using System;

namespace GCFreeUtils
{
    public class GCFreePredicateList<T>
    {
        private const string ERR_BUFFER_TOO_SMALL = "[GCFreeActionList] Current buffer too small. Consider increasing the initial size or set as auto resizeable.";
        private const string LOG_RESIZING = "[GCFreeActionList] Resizing buffer. Maybe you want to increase the initial size.";
        private Predicate<T>[] actionList;
        private bool autoResizeable;

        public GCFreePredicateList(int size) : this(size, true)
        {

        }

        public GCFreePredicateList(int size, bool autoResizeable)
        {
            this.actionList = new Predicate<T>[size];
            this.autoResizeable = autoResizeable;
            this.Count = 0;
        }

        public int Count { get; private set; }

        public void Add(Predicate<T> action)
        {
            if (this.Count == this.actionList.Length)
            {
                if (!this.autoResizeable)
                {
                    UnityEngine.Debug.LogError("[GCFreeActionList] Current buffer too small. Consider increasing the initial size or set as auto resizeable.", null);
                    return;
                }
                Predicate<T>[] destinationArray = new Predicate<T>[this.actionList.Length * 2];
                Array.Copy(this.actionList, destinationArray, this.actionList.Length);
                this.actionList = destinationArray;
            }
            this.actionList[this.Count] = action;
            this.Count++;
        }

        public void Remove(Predicate<T> action)
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

        public bool CallAnyTrue(T parameter)
        {
            for(int i = 0; i <= this.Count; i++)
            {
                try
                {
                    if(this.actionList[i] != null)
                    {
                        bool flag = this.actionList[i](parameter);
                        if (flag)
                        {
                            return true;
                        }
                    }
                } catch (Exception message)
                {
                    UnityEngine.Debug.LogError(message, null);
                }
            }
            return false;
        }
    }
}

