using UnityEngine;

namespace Environment
{
    public class MicroCosmosParameters : MonoBehaviour
    {
        private static MicroCosmosParameters singleton;

        public ChemicalBlobAttraction.GlobalConfiguration coalesceChemicalBlobs;

        public static MicroCosmosParameters Instance
        {
            get
            {
                if (singleton)
                    return singleton;
                return singleton = GameObject.Find("Environment").GetComponent<MicroCosmosParameters>();
            }
        }
    }
}