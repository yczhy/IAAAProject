using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duskvern
{
    public abstract class AdUnit : IAdSourceUnit
    {
        public abstract E_AdUnitSource Source { get; }

        protected Dictionary<E_Adtype, IPlayUnit> _ads = new();

        public abstract bool IsEmpty { get; }

        public abstract void Init();
        public abstract void LoadAd(E_Adtype adtype);

        public abstract void PlayAd(E_Adtype adtype);

        public abstract bool CanPlay(E_Adtype adtype);
    }
}

