using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Arleta.CCK
{
    public class CustomTags : MonoBehaviour
    {
        public string customTags = "";

        public List<string> tags
        {
            get
            {
                return customTags.Split(';').ToList();
            }
        }

        public void AddTag(string newTag)
        {
            if (!tags.Contains("newTag"))
            {
                customTags += (customTags.Trim().EndsWith(";") ? "" : ";") + newTag;
            }
        }

        public void RemoveTag(string unUsedTag)
        {
            if (tags.Contains(unUsedTag))
            {
                List<string> _newTags = tags;
                _newTags.Remove(unUsedTag);
                customTags = string.Join(";", _newTags);
            }
        }

        public bool HasTag(string tag)
        {
            return tags.Contains(tag);
        }

        public bool HasTags(string tags)
        {
            return HasTags(tags.Split(';').ToList());
        }

        public bool HasTags(List<string> tags)
        {
            bool _has = false;
            foreach (string _tag in tags)
            {
                if (tags.Contains(_tag))
                {
                    _has = true;
                    break;
                }
            }

            return _has;
        }

        public static bool HasTag(GameObject target, string tag)
        {
            if (!target)
                return false;

            CustomTags _targetTags = null;
            if (_targetTags = target.GetComponent<CustomTags>())
            {
                return _targetTags.HasTag(tag);
            }
            else
                return false;
        }

        public static bool HasTags(GameObject target, string tags)
        {
            if (!target)
                return false;

            CustomTags _targetTags = null;
            if (_targetTags = target.GetComponent<CustomTags>())
            {
                return _targetTags.HasTags(tags);
            }
            else
                return false;
        }

        public static bool HasTags(GameObject target, List<string> tags)
        {
            if (!target)
                return false;

            CustomTags _targetTags = null;
            if (_targetTags = target.GetComponent<CustomTags>())
            {
                return _targetTags.HasTags(tags);
            }
            else
                return false;
        }
    }
}