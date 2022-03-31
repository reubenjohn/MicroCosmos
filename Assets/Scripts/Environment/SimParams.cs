using UnityEngine;

namespace Environment
{
    public class SimParams : MonoBehaviour
    {
        public string hunterBaseAGuid = "7f146de4-9367-4bf8-bef0-82b08b5c10c6";
        public string hunterBaseBGuid = "7f146de4-9367-4bf8-bef0-82b08b5c10c7";

        public int flagellaIndex = 1;
        public int birthCanalIndex = 2;

        public static SimParams Singleton { get; private set; }

        private void Start()
        {
            Singleton = GameObject.Find("Environment").GetComponent<SimParams>();
        }
    }
}