using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSequenceTester : MonoBehaviour
{
    public Animator anim;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            anim.SetTrigger("Test");
        }
    }
}
