using UnityEngine;

public class Sawblades : MonoBehaviour
{
    
    public Sprite[] sawblades;
    public int currentSawblade = 0;

    private void Start()
    {
        if (gameObject.tag != "Decoration Sawblade")
        {
            ChangeSawblade();
        }
    }

    private void FixedUpdate()
    {
        if (gameObject.tag == "Decoration Sawblade")
        {
            gameObject.transform.Rotate(new Vector3(0, 0, 5));
        } else {
            gameObject.transform.Rotate(new Vector3(0, 0, -5));
        }

    }

    public void ChangeSawblade(bool next = false)
    {
        if (next && currentSawblade < sawblades.Length - 1)
        {
            currentSawblade++;
        }
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sawblades[currentSawblade];
    }

}
