using UnityEngine;

namespace Teacher
{
    public class GameFlowController : MonoBehaviour
    {
        private enum Stage
        {
            Start,
            Excavation,
            Assembly,
            Finished
        }

        [SerializeField] private StartStageController startStage;
        [SerializeField] private ExcavationStageController excavationStage;
        [SerializeField] private AssemblyStageController assemblyStage;

        private Stage currentStage = Stage.Start;

        private void OnEnable()
        {
            startStage.Finished += StartExcavation;
            excavationStage.Finished += StartAssembly;
            assemblyStage.Finished += FinishGame;
        }

        private void OnDisable()
        {
            startStage.Finished -= StartExcavation;
            excavationStage.Finished -= StartAssembly;
            assemblyStage.Finished -= FinishGame;
        }

        private void Start()
        {
            startStage.Begin();
        }

        private void StartExcavation()
        {
            if (currentStage != Stage.Start)
                return;

            currentStage = Stage.Excavation;
            startStage.End();
            excavationStage.Begin();
        }

        private void StartAssembly()
        {
            if (currentStage != Stage.Excavation)
                return;

            currentStage = Stage.Assembly;
            excavationStage.End();
            assemblyStage.Begin();
        }

        private void FinishGame()
        {
            if (currentStage != Stage.Assembly)
                return;

            currentStage = Stage.Finished;
            assemblyStage.End();
        }
    }
}
