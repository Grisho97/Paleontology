using UnityEngine;
using UnityEngine.UI;

namespace Paleontology.Student
{
    public class StudentDigHUD : MonoBehaviour
    {
        [SerializeField] StudentSoilGrid grid;
        [SerializeField] StudentDigInput input;
        [SerializeField] Text status;
        [SerializeField] Button reset;
        public void Configure(StudentSoilGrid target, StudentDigInput controls, Text label, Button button)
        { grid = target; input = controls; status = label; reset = button; }
        void Start() { reset.onClick.AddListener(() => { grid.ResetGrid(); }); }
        void Update()
        {
            status.text = $"Убрано: {grid.Removed} / {grid.Total}\nОчищено: {grid.Progress:P0}";
            if (grid.Completed) status.text += "\n\nСлой очищен!";
        }
    }
}
