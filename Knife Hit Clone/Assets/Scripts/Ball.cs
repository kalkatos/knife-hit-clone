using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public void Explode ()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            StartCoroutine(MoveRandomly(child));
        }
    }
    IEnumerator MoveRandomly (Transform obj)
    {
        float randomAngle = Random.Range(220f, 320f);
        Vector3 randomDirection = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
        for (int i = 0; i < 100; i++)
        {
            obj.position += randomDirection * 60 * Time.deltaTime;
            yield return null;
        }
        Destroy(obj.gameObject);
    }
}
