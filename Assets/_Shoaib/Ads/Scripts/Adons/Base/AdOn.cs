using System.Collections;
using UnityEngine;

namespace SH.Ads.Adons
{
    public abstract class AdOn : ScriptableObject
    {
        internal abstract IEnumerator Initialize(AdSettings setting);
    }
}