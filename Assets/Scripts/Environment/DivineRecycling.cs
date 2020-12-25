using UnityEngine;

namespace Environment
{
    public class DivineRecycling : MonoBehaviour
    {
        private static DivineRecycling singleton;
        public float divineRecycleInterval = 5;

        public static DivineRecycling Instance
        {
            get
            {
                if (singleton)
                    return singleton;
                return singleton = GameObject.Find("Environment").GetComponent<DivineRecycling>();
            }
        }
    }
}