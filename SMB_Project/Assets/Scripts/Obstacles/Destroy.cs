using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour
{
    void OnCollisionEnter2D()
    {
        StartCoroutine(WaitForBreak());
    }

    IEnumerator WaitForBreak()
    {
        yield return new WaitForSeconds(1);
        Break();
    }

    void Break()
    {
        gameObject.SetActive(false);
    }

    public void Reset()
    {
        gameObject.SetActive(true);
        StopCoroutine(WaitForBreak());
    }
}
