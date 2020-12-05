using Newtonsoft.Json;
using UnityEngine;

namespace Cell
{
    public class CellThawer : MonoBehaviour
    {
        private bool isStarted = false;
        public string cellData;

        private void Start()
        {
            isStarted = true;
            enabled = !string.IsNullOrEmpty(cellData);
            if (enabled) OnEnable();
        }

        private void OnEnable()
        {
            if (isStarted) CellData.Load(JsonConvert.DeserializeObject<CellData>(cellData), transform);
        }
    }
}