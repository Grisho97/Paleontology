using System.Collections;
using UnityEngine;

public class StartScreenContntroller : MonoBehaviour
{
    [SerializeField]
    GameObject startScreen;
    [SerializeField]
    GameObject gameInterface;
    IEnumerator DelayedDisapereance()
    {
        yield return new WaitForSeconds(0.75f);
        startScreen.SetActive(false);
    }
//    IEnumerator AnableStartScreen()
//    {
//        yield return new WaitForSeconds(10f);
//        startScreen.SetActive(true);
//    }
    public void DisableStartScreen()
    {
        gameInterface.SetActive(true);
        startScreen.GetComponent<Animator>().SetTrigger("Trigger");
        gameInterface.GetComponent<Animator>().SetTrigger("Trigger2");
        StartCoroutine(DelayedDisapereance());
        //StartCoroutine(AnableStartScreen());
    }
    void Start()
    {
        startScreen.SetActive(true);
    }

    void Update()
    {
        
    }
}
