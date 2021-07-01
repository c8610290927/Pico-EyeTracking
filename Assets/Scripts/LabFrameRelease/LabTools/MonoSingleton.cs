using System;
using UnityEngine;

namespace LabData
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T m_Instance;

        public static T Instance
        {
            get
            {
                if (object.ReferenceEquals(MonoSingleton<T>.m_Instance, null))
                {
                    MonoSingleton<T>.m_Instance = (UnityEngine.Object.FindObjectOfType(typeof(T)) as T);
                    if (object.ReferenceEquals(MonoSingleton<T>.m_Instance, null))
                    {
                        Debug.LogWarning("cant find a gameobject of instance " + typeof(T) + "!");
                    }
                    else
                    {
                        MonoSingleton<T>.m_Instance.OnAwake();
                    }
                }
                return MonoSingleton<T>.m_Instance;
            }
        }

        public static bool IsInstanceValid
        {
            get
            {
                return !object.ReferenceEquals(MonoSingleton<T>.m_Instance, null);
            }
        }

        private void Awake()
        {
            if (object.ReferenceEquals(MonoSingleton<T>.m_Instance, null))
            {
                MonoSingleton<T>.m_Instance = (this as T);
                MonoSingleton<T>.m_Instance.OnAwake();
            }
        }

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnApplicationQuit()
        {
            MonoSingleton<T>.m_Instance = (T)((object)null);
        }

        protected virtual void DoOnDestroy()
        {
        }

        private void OnDestroy()
        {
            this.DoOnDestroy();
            if (object.ReferenceEquals(MonoSingleton<T>.m_Instance, this))
            {
                MonoSingleton<T>.m_Instance = (T)((object)null);
            }
        }
    }

}
