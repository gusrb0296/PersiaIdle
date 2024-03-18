using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils
{
    public class EasyUIPooling
    {
        public static CustomPool<T> MakePool<T>(T prefab, Transform root, Action<T> create, Action<T> get,
            Action<T> release, int poolSize, bool isFlexible = true) where T : MonoBehaviour
        {
            return new CustomPool<T>(prefab, root, create, get, release, poolSize, isFlexible);
        }
    }

    public class CustomPool<T> where T : MonoBehaviour
    {
        private T obj;
        private Transform root;
        private Action<T> create;
        private Action<T> get;
        private Action<T> release;
        private Queue<T> pool;
        private LinkedList<T> usedObjs;
        private int size;
        private bool flexible;

        public int Size => size;
        public int UsedCount => usedObjs.Count;
        public int UnusedCount => pool.Count;
        public LinkedList<T> UsedList => usedObjs;

        public CustomPool(T prefab, Transform root, Action<T> createAction, Action<T> getAction,
            Action<T> releaseAction, int size, bool isFlexible)
        {
            obj = prefab;
            this.root = root;
            create = createAction;
            get = getAction;
            release = releaseAction;
            pool = new Queue<T>();
            this.size = size;
            flexible = isFlexible;
            usedObjs = new LinkedList<T>();

            for (int i = 0; i < size; ++i)
            {
                Create();
            }
        }

        private T Create()
        {
            var item = Object.Instantiate(obj, root);
            pool.Enqueue(item);
            item.gameObject.SetActive(false);
            create?.Invoke(item);
            return item;
        }

        public T Get()
        {
            T item;
            if (pool.Count <= 0)
            {
                if (flexible)
                {
                    item = Object.Instantiate(obj, root);
                    create?.Invoke(item);
                }
                else
                {
                    item = usedObjs.First.Value;
                    usedObjs.RemoveFirst();
                    release?.Invoke(item);
                }
            }
            else
                item = pool.Dequeue();

            item.gameObject.SetActive(true);
            get?.Invoke(item);
            usedObjs.AddLast(item);
            return item;
        }

        public void Clear()
        {
            while (usedObjs.Count > 0)
                Release(usedObjs.First.Value);
        }

        public void Release(T item)
        {
            item.gameObject.SetActive(false);
            usedObjs.Remove(item);
            pool.Enqueue(item);
            release?.Invoke(item);
        }
    }
}