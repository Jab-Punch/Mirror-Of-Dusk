using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMenuSelectArrowUp {

    List<GameObject> menuSelectArrowUp { get; set; }
    void removeMenuSelectArrowUp();
    void updateArrowUp(bool on);
}
