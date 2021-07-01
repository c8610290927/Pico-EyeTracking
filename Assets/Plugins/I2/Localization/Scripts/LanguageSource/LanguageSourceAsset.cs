using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace I2.Loc
{
    [CreateAssetMenu(fileName = "I2Languages", menuName = "I2 Localization/LanguageSource", order = 1)]
    public class LanguageSourceAsset : ScriptableObject
    {
        public LanguageSourceData mSource = new LanguageSourceData();
    }
}