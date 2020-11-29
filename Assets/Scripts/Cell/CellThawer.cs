using Newtonsoft.Json;
using UnityEngine;

namespace Cell
{
    public class CellThawer : MonoBehaviour
    {
        public string cellData;
        
        private void Start() => CellData.Load(JsonConvert.DeserializeObject<CellData>(cellData), transform);
    }
}