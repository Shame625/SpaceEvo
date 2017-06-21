using UnityEngine;
using System.Collections;

public class DestroyTrail : MonoBehaviour {

    public void DestructionSequence()
    {
        Destroy(gameObject, 1f);
    }
}
