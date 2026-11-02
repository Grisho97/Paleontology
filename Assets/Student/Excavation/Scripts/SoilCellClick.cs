using UnityEngine;
using UnityEngine.EventSystems;

namespace Paleontology.Student
{
    // Добавьте на UI-картинку (Image или RawImage).
    public class SoilCellClick : MonoBehaviour, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            gameObject.SetActive(false);
        }
    }
}
