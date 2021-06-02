using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenuSlot : MenuSlot {

    public MenuSlot[] subMenuSlots;
    private int _currentMenuSlot;
    public int currentMenuSlot
    {
        get { return _currentMenuSlot; }
        set { _currentMenuSlot = value; }
    }
    private int _slotLimit;
    public int slotLimit
    {
        get { return _slotLimit; }
        set { _slotLimit = value; }
    }

    private void Awake()
    {
        slotLimit = subMenuSlots.Length;
    }
}
