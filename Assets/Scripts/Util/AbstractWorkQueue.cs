using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;

namespace Util
{
    public abstract class AbstractWorkQueue<T> : MonoBehaviour
    {
        private static bool instanceReferenced;
        private static AbstractWorkQueue<T> instance;
        private readonly ConcurrentQueue<T> queue = new ConcurrentQueue<T>();

        protected virtual void Start() => StartCoroutine(PopServer());

        private void OnDestroy() => StopAllCoroutines();

        protected static AbstractWorkQueue<T> GetInstance()
        {
            if (instanceReferenced)
                return instance;
            instanceReferenced = true;
            return instance = GameObject.Find("Environment").GetComponent<AbstractWorkQueue<T>>();
        }


        public void Enqueue(T workItem) => queue.Enqueue(workItem);

        private IEnumerator PopServer()
        {
            while (true)
                if (queue.TryDequeue(out var item))
                    WorkOn(item);
                else
                    yield return null;

            // ReSharper disable once FunctionNeverReturns
        }

        protected abstract void WorkOn(T item);
    }
}