using UnityEngine;

namespace Paleontology.Student
{
    public class StudentLayerSequence : MonoBehaviour
    {
        [SerializeField] StudentSoilGrid[] layers;

        public StudentSoilGrid CurrentLayer
        {
            get
            {
                // В списке сначала верхний слой, затем слои под ним.
                foreach (StudentSoilGrid layer in layers)
                {
                    if (!layer.Completed) return layer;
                }

                return null;
            }
        }

        public void Configure(StudentSoilGrid[] orderedLayers)
        {
            layers = orderedLayers;
        }

        public void ResetLayers()
        {
            foreach (StudentSoilGrid layer in layers) layer.ResetGrid();
        }
    }
}
