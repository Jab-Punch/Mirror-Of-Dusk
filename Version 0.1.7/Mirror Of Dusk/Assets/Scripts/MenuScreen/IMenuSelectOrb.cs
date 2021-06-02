using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMenuSelectOrb {

    List<GameObject> menuSelectOrb { get; set; }
    void removeMenuSelectOrb();
    void resetMenuSelectOrb();

}
