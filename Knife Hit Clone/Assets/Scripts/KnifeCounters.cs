using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnifeCounters : MonoBehaviour
{
    public GameObject knifeCounterPrefab;
    public Sprite unusedKnifeSprite;
    public Sprite usedKnifeSprite;

    List<Image> counters = new List<Image>();
    int lastUnused;

    public void Set (int quantity)
    {
        if (counters.Count == 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                counters.Add(transform.GetChild(i).GetComponent<Image>());
            }
        }

        ResetCounters();
        for (int i = 0; i < quantity; i++)
        {
            counters[i].gameObject.SetActive(true);
        }
        lastUnused = quantity - 1;
    }

    public void Expire ()
    {
        counters[lastUnused].sprite = usedKnifeSprite;
        lastUnused--;
    }

    public void ResetCounters ()
    {
        for (int i = 0; i < counters.Count; i++)
        {
            counters[i].sprite = unusedKnifeSprite;
            counters[i].gameObject.SetActive(false);
        }
    }
}
