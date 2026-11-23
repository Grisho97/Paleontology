using System;
using UnityEngine;

namespace Teacher
{
    public class StartStageController : MonoBehaviour
    {
        [SerializeField] private GameObject startScreen;

        public event Action Finished;

        public void Begin()
        {
            startScreen.SetActive(true);
        }

        // Этот метод вызывается кнопкой через On Click в Inspector.
        public void RequestStart()
        {
            Finished?.Invoke();
        }

        public void End()
        {
            startScreen.SetActive(false);
        }
    }
}
