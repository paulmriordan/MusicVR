using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableChildrenWhenTargetActive : MonoBehaviour {

    public List<GameObject> Targets;
    
    private void Update()
    {
        bool anyActive = AnyActive();
        foreach (Transform t in transform)
            t.gameObject.SetActive(!anyActive);
    }

    bool AnyActive()
    {
        foreach (var t in Targets)
            if (t.gameObject.activeSelf)
                return true;

        return false;
    }
}
