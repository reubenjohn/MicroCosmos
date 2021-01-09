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

        public void Enqueue(Transform trans, Color color, float scaleMultiplier = 1f) =>
            Enqueue(new PopOrder
                {color = color, position = trans.position, scale = trans.lossyScale * scaleMultiplier});

        protected override void WorkOn(PopOrder item)
        {
            var obj = Instantiate(
                Resources.Load<GameObject>("Objects/HaloPop"), item.position, Quaternion.identity, effectsTransform);
            obj.GetComponent<Light>().color = item.color;
            obj.transform.localScale = item.scale;
        }
    }

    public class PopOrder
    {
        public Color color;
        public Vector3 position;
        public Vector3 scale { get; set; }
    }
}