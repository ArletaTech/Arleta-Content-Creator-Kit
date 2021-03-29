using Bolt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Arleta.CCK
{
    public class AssetManager : MonoBehaviour
    {
        public static async Task<GameObject> GetAddressable(string key, GameObject referenceBolt = null, string eventName = "", params object[] arguments)
        {
            Debug.Log("Loading.." + key);
            GameObject _template = await Addressables.LoadAssetAsync<GameObject>(key).Task;
            var _locations = await Addressables.LoadResourceLocationsAsync("Enemies").Task;
            foreach (UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation _loc in _locations)
            {
                Debug.Log(_loc.PrimaryKey);
                Debug.Log(_loc.InternalId);
            }

            if (referenceBolt && !string.IsNullOrEmpty(eventName))
            {
                object[] newArguments = new object[(arguments == null ? 0 : arguments.Length) + 1];
                newArguments[0] = _template;
                Array.Copy(arguments, 0, newArguments, 1, arguments.Length);
                CustomEvent.Trigger(referenceBolt, eventName, newArguments);
            }

            return _template;
        }

        public static void GetAddressableWithProgress(string key, GameObject referenceBolt, string progressEventName, string finishEventName = "", params object[] arguments)
        {
            referenceBolt.GetComponent<FlowMachine>().StartCoroutine(GetAddressableWithProgressCoroutine(key, referenceBolt, progressEventName, finishEventName, arguments));
        }

        static IEnumerator GetAddressableWithProgressCoroutine(string key, GameObject referenceBolt, string progressEventName, string finishEventName = "", params object[] arguments)
        {
            AsyncOperationHandle _handle = Addressables.LoadAssetAsync<GameObject>(key);

            // AsyncOperationHandle adalah parameter wajib..diletakkan ditempat pertame..
            object[] newArguments = new object[(arguments == null ? 0 : arguments.Length) + 1];
            newArguments[0] = _handle;
            Array.Copy(arguments, 0, newArguments, 1, arguments.Length);

            while (!_handle.IsDone)
            {
                CustomEvent.Trigger(referenceBolt, progressEventName, newArguments);
                yield return null;
            }

            if (referenceBolt && !string.IsNullOrEmpty(finishEventName))
            {
                object[] finishArguments = new object[(arguments == null ? 0 : arguments.Length) + 2];
                finishArguments[0] = _handle.Result as GameObject;
                finishArguments[1] = _handle.Result;
                CustomEvent.Trigger(referenceBolt, finishEventName, finishArguments);
            }
        }

        public static async void InstantiateAddressables(string key, GameObject referenceBolt = null, string eventName = "", params object[] arguments)
        {
            GameObject _temp = await Addressables.InstantiateAsync(key).Task;

            if (referenceBolt && !string.IsNullOrEmpty(eventName))
            {
                object[] newArguments = new object[(arguments == null ? 0 : arguments.Length) + 1];
                newArguments[0] = _temp;                                // set the prepended value
                Array.Copy(arguments, 0, newArguments, 1, arguments.Length);
                CustomEvent.Trigger(referenceBolt, eventName, newArguments);
            }
        }

        public static void InstantiateAddressablesWithProgress(string key, GameObject referenceBolt, string progressEventName, string finishEventName = "", params object[] arguments)
        {
            referenceBolt.GetComponent<FlowMachine>().StartCoroutine(InstantiateAddressablesWithProgressCoroutine(key, referenceBolt, progressEventName, finishEventName, arguments));
        }

        static IEnumerator InstantiateAddressablesWithProgressCoroutine(string key, GameObject referenceBolt, string progressEventName, string finishEventName = "", params object[] arguments)
        {
            AsyncOperationHandle _handle = Addressables.InstantiateAsync(key);

            // AsyncOperationHandle adalah parameter wajib..diletakkan ditempat pertame..
            object[] newArguments = new object[(arguments == null ? 0 : arguments.Length) + 1];
            newArguments[0] = _handle;
            Array.Copy(arguments, 0, newArguments, 1, arguments.Length);

            while (!_handle.IsDone)
            {
                CustomEvent.Trigger(referenceBolt, progressEventName, newArguments);
                yield return null;
            }

            if (referenceBolt && !string.IsNullOrEmpty(finishEventName))
            {
                object[] finishArguments = new object[(arguments == null ? 0 : arguments.Length) + 2];
                finishArguments[0] = _handle.Result as GameObject;
                finishArguments[1] = _handle.Result;
                CustomEvent.Trigger(referenceBolt, finishEventName, finishArguments);
            }
        }

        public static async void DownloadAddressablesDependencies(string key, GameObject referenceBolt = null, string eventName = "", params object[] arguments)
        {
            Debug.Log("Download dependencies..");
            //if(!Application.isEditor)
            //{
            try
            {
                await Addressables.DownloadDependenciesAsync(key, true).Task;
            }
            catch { }
            //}

            Debug.Log("Siap download dependencies..");
            if (referenceBolt && !string.IsNullOrEmpty(eventName))
            {
                //object[] newArguments = new object[(arguments == null ? 0 : arguments.Length) + 1];
                //newArguments[0] = _temp;                                // set the prepended value
                //Array.Copy(arguments, 0, newArguments, 1, arguments.Length);
                CustomEvent.Trigger(referenceBolt, eventName, arguments);
            }
            Debug.Log("Setel download pe sume..");
        }

        public static void DownloadAddressablesDependenciesWithProgress(string key, GameObject referenceBolt, string progressEventName, string finishEventName = "", params object[] arguments)
        {
            referenceBolt.GetComponent<FlowMachine>().StartCoroutine(DownloadAddressablesDependenciesWithProgressCoroutine(key, referenceBolt, progressEventName, finishEventName, arguments));
        }

        static IEnumerator DownloadAddressablesDependenciesWithProgressCoroutine(string key, GameObject referenceBolt, string progressEventName, string finishEventName = "", params object[] arguments)
        {
            Debug.Log("Download dependencies coroutine..");
            AsyncOperationHandle _handle = Addressables.DownloadDependenciesAsync(key, true);

            // AsyncOperationHandle adalah parameter wajib..diletakkan ditempat pertame..
            object[] newArguments = new object[(arguments == null ? 0 : arguments.Length) + 1];
            newArguments[0] = _handle;
            Array.Copy(arguments, 0, newArguments, 1, arguments.Length);

            while (!_handle.IsDone)
            {
                CustomEvent.Trigger(referenceBolt, progressEventName, newArguments);
                yield return null;
            }

            Debug.Log("Siap download dependencies coroutine..");
            if (!string.IsNullOrEmpty(finishEventName))
            {
                CustomEvent.Trigger(referenceBolt, finishEventName, newArguments);
            }

            Debug.Log("Setel download pe sume coroutine..");
        }
    }
}