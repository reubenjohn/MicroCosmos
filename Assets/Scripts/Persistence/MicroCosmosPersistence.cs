using System.Linq;
using UnityEngine;

namespace Persistence
{
    public class MicroCosmosPersistence : MonoBehaviour
    {
        private SubsystemsPersistence subsystemsPersistence;

        public string SaveDirectory
        {
            get => subsystemsPersistence.SaveDirectory;
            set => subsystemsPersistence.SaveDirectory = value;
        }

        public ISavableSubsystem[] SavableSubsystems => GetComponentsInChildren<ISavableSubsystem>().ToArray();

        private void Start()
        {
            subsystemsPersistence =
                new SubsystemsPersistence(SavableSubsystems, $"{Application.persistentDataPath}/saves");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F6)) OnSave();
            else if (Input.GetKeyDown(KeyCode.F7)) OnLoad();
        }

        public void OnLoad() => subsystemsPersistence.Load();

        public void OnSave() => subsystemsPersistence.Save();
    }
}