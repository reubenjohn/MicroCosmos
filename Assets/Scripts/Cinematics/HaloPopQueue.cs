using UnityEngine;
using Util;

namespace Cinematics
{
    public class HaloPopQueue : AbstractWorkQueue<PopOrder>
    {
        private Transform effectsTransform;

        public static HaloPopQueue Instance => (HaloPopQueue) GetInstance();

        protected override void Start()
        {
            base.Start();
            effectsTransform = transform.Find("Effects");
        }

        public void PlacePopOrder(Transform trans, Color color) =>
            Enqueue(new PopOrder {color = color, position = trans.position});

        protected override void WorkOn(PopOrder item)
        {
            var obj = Instantiate(
                Resources.Load<GameObject>("Objects/HaloPop"), item.position, Quaternion.identity, effectsTransform);
            obj.GetComponent<Light>().color = item.color;
        }
    }

    public class PopOrder
    {
        public Color color;
        public Vector3 position;
    }
}