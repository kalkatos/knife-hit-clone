using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Inicializa e controla os contadores de facas disponíveis em cada fase.
/// </summary>
public class KnifeCounters : MonoBehaviour
{
    public GameObject knifeCounterPrefab;
    public Sprite unusedKnifeSprite;
    public Sprite usedKnifeSprite;

    List<Image> counters = new List<Image>();
    int lastUnused;

    /// <summary>
    /// Inicializa um novo contador de facas para uma nova fase
    /// </summary>
    /// <param name="quantity">Quantidade de facas a serem inicializadas</param>
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

    /// <summary>
    /// Chamada quando uma faca é usada.
    /// </summary>
    public void Expire ()
    {
        counters[lastUnused].sprite = usedKnifeSprite;
        lastUnused--;
    }

    /// <summary>
    /// Reseta o contador de facas
    /// </summary>
    public void ResetCounters ()
    {
        for (int i = 0; i < counters.Count; i++)
        {
            counters[i].sprite = unusedKnifeSprite;
            counters[i].gameObject.SetActive(false);
        }
    }
}
