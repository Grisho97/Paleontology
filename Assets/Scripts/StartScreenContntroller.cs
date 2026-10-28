using System.Collections;
using UnityEngine;

public class StartScreenContntroller : MonoBehaviour
{
    [SerializeField]
    GameObject startScreen;
    IEnumerator DelayedDisapereance()
    {
        yield return new WaitForSeconds(0.75f);
        startScreen.SetActive(false);
    }
    public void DisableStartScreen()
    {
        startScreen.GetComponent<Animator>().SetTrigger("Trigger");
        StartCoroutine(DelayedDisapereance());
    }
    void Start()
    {
        startScreen.SetActive(true);
    }

    void Update()
    {
        
    }
}
