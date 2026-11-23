using System;
using UnityEngine;

namespace Teacher
{
    public class LayersSequence : MonoBehaviour
    {
        [SerializeField] private LayerGridController[] layers;

        public event Action Finished;

        private void OnEnable()
        {
            foreach (LayerGridController layer in layers)
            {
                layer.Finished += OnLayerFinished;
            }
        }

        private void OnDisable()
        {
            foreach (LayerGridController layer in layers)
            {
                layer.Finished -= OnLayerFinished;
            }
        }

        public LayerGridController GetCurrentLayer()
        {
            foreach (LayerGridController layer in layers)
            {
                if (!layer.IsFinished)
                    return layer;
            }

            return null;
        }

        private void OnLayerFinished()
        {
            if (GetCurrentLayer() == null)
            {
                Finished?.Invoke();
            }
        }
    }
}
