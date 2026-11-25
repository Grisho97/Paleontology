using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenContntroller : MonoBehaviour
{
    [SerializeField]
    Button startGameButton;
    [SerializeField]
    bool anableWaitMode;
    [SerializeField]
    GameObject startScreen;
    public event Action StartScreenStageEnded;
    IEnumerator DelayedDisapereance()
    {
        yield return new WaitForSeconds(0.75f);
        startScreen.SetActive(false);
    }
    IEnumerator AnableStartScreen()
    {
        yield return new WaitForSeconds(10f);
        startScreen.SetActive(true);
    }
    public void DisableStartScreen()
    {
        if(anableWaitMode)
        StartCoroutine(AnableStartScreen());
    }
    public void Begin()
    {
        startScreen.SetActive(true);
    }
    void End()
    {
        startScreen.GetComponent<Animator>().SetTrigger("Trigger");
        StartCoroutine(DelayedDisapereance());
        StartScreenStageEnded.Invoke();
    }
    void Awake()
    {
        startGameButton.onClick.AddListener(() => End());
    }

    void Update()
    {
        
    }
}
