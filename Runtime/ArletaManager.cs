using Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Arleta.CCK
{
    public class ArletaManager : MonoBehaviour
    {
        public string prefix;
        [ReadOnly]
        [SerializeField]
        private string m_uuid;
        public string uuid
        {
            get
            {
                return m_uuid;
            }
        }

        Variables _variables;
        public Variables variables
        {
            get
            {
                if (!_variables)
                {
                    _variables = gameObject.GetComponent<Variables>();
                    if (!_variables)
                        _variables = gameObject.AddComponent<Variables>();
                }

                return _variables;
            }
        }

        public static Dictionary<string, ArletaManager> managers;

        public UnityEvent onTargetFound, onTargetLost;

        bool isPlaying = false;
        private void Awake()
        {
            RegisterGameManager(this);
        }


        private void OnDestroy()
        {
            UnregisterGameManager(this);
        }

        public void SetUuid(string newUuid)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                m_uuid = newUuid;
            }
        }

        #region AR Event

        /// <summary>
        /// ArletaKit akan panggil method ini bile AR target utk manager ini muncul.
        /// </summary>
        public void OnTargetFound()
        {
            Debug.Log("Triggering OnFound");
            if (onTargetFound != null)
            {
                Debug.Log("TriggerOnFound");
                onTargetFound.Invoke();
            }
        }

        /// <summary>
        /// ArletaKit akan panggil method ini bile AR target utk manager ini hilang.
        /// </summary>
        public void OnTargetLost()
        {
            Debug.Log("Triggering OnLost");
            if (onTargetLost != null)
            {
                Debug.Log("OnTargetLost");
                onTargetLost.Invoke();
            }
        }

        public void RegisterOnTargetTracked(GameObject referenceBolt, string eventName = "", params object[] arguments)
        {
            if (referenceBolt && !string.IsNullOrEmpty(eventName))
            {
                if (onTargetFound == null)
                    onTargetFound = new UnityEvent();

                Debug.Log("RegisterOnTargetTracked");
                onTargetFound.AddListener(() =>
                {
                    Debug.Log("Trigering event [tracked] .." + eventName);
                    CustomEvent.Trigger(referenceBolt, eventName, arguments);
                });

            }
        }
        public void RegisterOnTargetLost(GameObject referenceBolt, string eventName = "", params object[] arguments)
        {
            if (referenceBolt && !string.IsNullOrEmpty(eventName))
            {
                if (onTargetLost == null)
                    onTargetLost = new UnityEvent();

                Debug.Log("RegisterOnTargetLost");
                onTargetLost.AddListener(() =>
                {
                    Debug.Log("Trigering event [lost] .." + eventName);
                    CustomEvent.Trigger(referenceBolt, eventName, arguments);
                });

            }
        }

        /// <summary>
        /// Hanye berfungsi kat app sebenar.
        /// </summary>
        /// <param name="referenceBolt"></param>
        /// <param name="eventName"></param>
        /// <param name="arguments"></param>
        public static void RegisterOnTargetTrackedEvent(string gameUuid, GameObject referenceBolt, string eventName = "", params object[] arguments)
        {
            ArletaManager _manager = GetManager(gameUuid);
            Debug.Log("Register track to : " + gameUuid);
            if (_manager)
                _manager.RegisterOnTargetTracked(referenceBolt, eventName, arguments);
        }

        /// <summary>
        /// Hanye berfungsi kat app sebenar.
        /// </summary>
        /// <param name="referenceBolt"></param>
        /// <param name="eventName"></param>
        /// <param name="arguments"></param>
        public static void RegisterOnTargetLostEvent(string gameUuid, GameObject referenceBolt, string eventName = "", params object[] arguments)
        {
            ArletaManager _manager = GetManager(gameUuid);
            Debug.Log("Register lost to : " + gameUuid);
            if (_manager)
                _manager.RegisterOnTargetLost(referenceBolt, eventName, arguments);
        }

        #endregion

        #region Variable Set & Get

        public object Get(string variableKey)
        {
            return variables.declarations.Get(variableKey);
        }

        public void Set(string variableKey, object value)
        {
            variables.declarations.Set(variableKey, value);
        }

        public static object GetVariable(string gameUuid, string variableKey)
        {
            if (managers == null || !managers.ContainsKey(gameUuid))
            {
                Debug.Log("Manager with this uuid is not exists");
                return null;
            }

            return managers[gameUuid].Get(variableKey);
        }

        public static void SetVariable(string gameUuid, string variableKey, object value)
        {
            if (managers == null || !managers.ContainsKey(gameUuid))
            {
                Debug.Log("Manager with this uuid is not exists");
                return;
            }

            managers[gameUuid].Set(variableKey, value);
        }

        #endregion

        #region GameManagering

        public static ArletaManager GetManager(string gameUuid)
        {
            if (managers == null || !managers.ContainsKey(gameUuid))
            {
                Debug.Log("Manager with this uuid is not exists");
                return null;
            }

            return managers[gameUuid];
        }

        public static void RegisterGameManager(ArletaManager _manager)
        {
            if (managers == null)
                managers = new Dictionary<string, ArletaManager>();

            if (managers.ContainsKey(_manager.uuid))
            {
                if (managers[_manager.uuid] != _manager)
                    Debug.Log("Manager with same uuid is already exists");
            }
            else
                managers.Add(_manager.uuid, _manager);
        }

        public static void UnregisterGameManager(ArletaManager _manager)
        {
            if (managers == null)
                return;

            if (managers.ContainsKey(_manager.uuid))
            {
                managers.Remove(_manager.uuid);
            }
            else
                Debug.Log("Manager with this uuid is not exists");
        }

        #endregion

    }

    /// <summary>
    /// Read Only attribute.
    /// Attribute is use only to mark ReadOnly properties.
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute { }
}