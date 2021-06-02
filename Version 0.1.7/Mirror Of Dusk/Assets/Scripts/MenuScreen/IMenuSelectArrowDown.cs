using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMenuSelectArrowDown {

    List<GameObject> menuSelectArrowDown { get; set; }
    void removeMenuSelectArrowDown();
    void updateArrowDown(bool on);
}
