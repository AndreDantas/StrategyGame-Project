using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAnimation : MonoBehaviour
{

    public Animator hitAnim;

    private void Start()
    {
        if (hitAnim == null)
            hitAnim = GetComponent<Animator>();
    }
    public virtual void Play()
    {
        if (hitAnim)
            hitAnim.SetTrigger("Hit");
    }

    public float AnimLength()
    {
        if (hitAnim)
        {
            return hitAnim.GetCurrentAnimatorStateInfo(0).length;

        }
        else return 0;
    }

}
