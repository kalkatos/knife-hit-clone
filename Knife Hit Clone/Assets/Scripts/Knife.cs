using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    bool attached;

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (attached)
            return;

        if (other.CompareTag("Knife"))
        {
            Debug.Log("Hit a knife");
            PlayManager.instance.HitAKnife();
        }
        else if (other.CompareTag("Ball"))
        {
            attached = true;
            transform.SetParent(other.transform);
            PlayManager.instance.HitBall();
        }
    }

    public void PlayAnimation ()
    {
        GetComponent<Animator>().SetTrigger("Get");
    }
}
