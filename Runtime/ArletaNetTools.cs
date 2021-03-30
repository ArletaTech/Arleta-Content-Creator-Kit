using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Arleta.CCK
{
    public class ArletaNetTools
    {
        static MonoBehaviour _mono = null;
        static MonoBehaviour mono
        {
            get
            {
                if(!_mono) _mono = GameObject.FindObjectOfType<MonoBehaviour>();

                return _mono;
            }
        }

        public static void DownloadContent(string url, GameObject referenceBolt, string eventName = "", params object[] arguments)
        {
            if (mono)
            {
                mono.StartCoroutine(DownloadContentAsync(url, referenceBolt, eventName, arguments));
            }
            else
                Debug.Log("No Monobehaviour Found!!");
        }

        static IEnumerator DownloadContentAsync(string url, GameObject referenceBolt, string eventName = "", params object[] arguments)
        {
            UnityWebRequest _www = UnityWebRequest.Get(url);
            _www.certificateHandler = new SSLFix();

            Debug.Log(url);

            yield return _www.SendWebRequest();
            while (!_www.isDone)
                yield return null;

            if (!_www.isNetworkError && !_www.isHttpError)
            {
                Debug.Log(_www.downloadHandler.text);
                if (!string.IsNullOrEmpty(eventName))
                {
                    object[] newArguments = new object[(arguments == null ? 0 : arguments.Length) + 1];
                    newArguments[0] = _www.downloadHandler.text;
                    Array.Copy(arguments, 0, newArguments, 1, arguments.Length);

                    Bolt.CustomEvent.Trigger(referenceBolt, eventName, newArguments);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(eventName))
                {
                    object[] newArguments = new object[(arguments == null ? 0 : arguments.Length) + 1];
                    newArguments[0] = _www.error;
                    Array.Copy(arguments, 0, newArguments, 1, arguments.Length);

                    Bolt.CustomEvent.Trigger(referenceBolt, eventName + "_error", newArguments);
                }
            }
        }

        public static void DownloadImage(string url, GameObject referenceBolt, string eventName = "", params object[] arguments)
        {
            if (mono)
            {
                mono.StartCoroutine(DownloadImageAsync(url, referenceBolt, "", eventName, arguments));
            }
            else
            {
                object[] newArguments = new object[(arguments == null ? 0 : arguments.Length) + 1];
                newArguments[0] = "No Monobehaviour Found!!";
                Array.Copy(arguments, 0, newArguments, 1, arguments.Length);

                Bolt.CustomEvent.Trigger(referenceBolt, eventName + "_error", newArguments);
                Debug.Log("No Monobehaviour Found!!");
            }
        }

        /// <summary>
        /// Force download walau dah ade cache.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="referenceBolt"></param>
        /// <param name="_cacheName"></param>
        /// <param name="eventName"></param>
        /// <param name="arguments"></param>
        public static void DownloadImageAndSaveCache(string url, GameObject referenceBolt, string cacheName = "", string eventName = "", params object[] arguments)
        {
            if (mono)
            {
                mono.StartCoroutine(DownloadImageAsync(url, referenceBolt, cacheName, eventName, arguments));
            }
            else
            {
                object[] newArguments = new object[(arguments == null ? 0 : arguments.Length) + 1];
                newArguments[0] = "No Monobehaviour Found!!";
                Array.Copy(arguments, 0, newArguments, 1, arguments.Length);

                Bolt.CustomEvent.Trigger(referenceBolt, eventName + "_error", newArguments);
                Debug.Log("No Monobehaviour Found!!");
            }
        }

        public static bool IsImageCached(string cacheName)
        {
            if(!string.IsNullOrEmpty(cacheName))
            {
                return System.IO.File.Exists(FullCachePath(cacheName));
            }

            return false;
        }

        public static void DownloadImageIfNotCached(string url, GameObject referenceBolt, string cacheName = "", string eventName = "", params object[] arguments)
        {
            if (IsImageCached(cacheName))
            {
                Texture2D _txtr = new Texture2D(1, 1);
                byte[] _imageBytes = System.IO.File.ReadAllBytes(FullCachePath(cacheName));
                _txtr.LoadImage(_imageBytes);
                _txtr.Apply();

                Debug.Log("Dari byte");

                if (!string.IsNullOrEmpty(eventName))
                {
                    object[] newArguments = new object[(arguments == null ? 0 : arguments.Length) + 1];

                    newArguments[0] = _txtr;
                    Array.Copy(arguments, 0, newArguments, 1, arguments.Length);

                    Bolt.CustomEvent.Trigger(referenceBolt, eventName, newArguments);
                }
            }
            else
            {
                if (mono)
                {
                    mono.StartCoroutine(DownloadImageAsync(url, referenceBolt, cacheName, eventName, arguments));
                }
                else
                {
                    object[] newArguments = new object[(arguments == null ? 0 : arguments.Length) + 1];
                    newArguments[0] = "No Monobehaviour Found!!";
                    Array.Copy(arguments, 0, newArguments, 1, arguments.Length);

                    Bolt.CustomEvent.Trigger(referenceBolt, eventName + "_error", newArguments);
                    Debug.Log("No Monobehaviour Found!!");
                }
            }
        }

        static string NormCacheFileName(string cacheName)
        {
            if (!string.IsNullOrEmpty(cacheName))
            {
                return cacheName.Replace(",", "_").
                    Replace("?", "_").
                    Replace(":", "_").
                    Replace("/", "_").
                    Replace("\\", "_").
                    Replace("|", "_").
                    Replace("*", "_").
                    Replace("<", "_").
                    Replace("?", "_");
            }

            return cacheName;
        }

        static string FullCachePath(string cacheName)
        {
            if(!System.IO.Directory.Exists(System.IO.Path.Combine(Application.persistentDataPath, "ImageCaches")))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Application.persistentDataPath, "ImageCaches"));
            }
            return System.IO.Path.Combine(Application.persistentDataPath, "ImageCaches", NormCacheFileName(cacheName));
        }

        static IEnumerator DownloadImageAsync(string url, GameObject referenceBolt, string cacheName = "", string eventName = "", params object[] arguments)
        {
            UnityWebRequest _www = UnityWebRequestTexture.GetTexture(url);
            _www.certificateHandler = new SSLFix();

            Debug.Log(url);

            yield return _www.SendWebRequest();
            while (!_www.isDone)
                yield return null;

            if (!_www.isNetworkError && !_www.isHttpError)
            {
                Texture2D _txtr = DownloadHandlerTexture.GetContent(_www);
                try
                {
                    if (!string.IsNullOrEmpty(cacheName))
                    {
                        Debug.Log("simpaning.." + FullCachePath(cacheName));
                        System.IO.File.WriteAllBytes(FullCachePath(cacheName), _txtr.EncodeToJPG(80));
                        Debug.Log("simpaned.." + FullCachePath(cacheName));
                    }
                }
                catch(Exception e) { Debug.Log(e.ToString()); }

                Debug.Log(_www.downloadHandler.text);
                if (!string.IsNullOrEmpty(eventName))
                {
                    object[] newArguments = new object[(arguments == null ? 0 : arguments.Length) + 1];

                    newArguments[0] = _txtr;// DownloadHandlerTexture.GetContent(_www);
                    Array.Copy(arguments, 0, newArguments, 1, arguments.Length);

                    Bolt.CustomEvent.Trigger(referenceBolt, eventName, newArguments);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(eventName))
                {
                    object[] newArguments = new object[(arguments == null ? 0 : arguments.Length) + 1];
                    newArguments[0] = _www.error;
                    Array.Copy(arguments, 0, newArguments, 1, arguments.Length);

                    Bolt.CustomEvent.Trigger(referenceBolt, eventName + "_error", newArguments);
                }
            }
        }
    }
    public class SSLFix : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}