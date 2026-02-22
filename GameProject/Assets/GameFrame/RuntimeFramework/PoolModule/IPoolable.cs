namespace Duskvern
{
    public interface IPoolable
    {
        bool IsInPool { get; set; }

        /// <summary>Called when this poolable object is spawned.</summary>
        void OnSpawn();

        /// <summary>Called when this poolable object is despawned.</summary>
        void OnDespawn();
    }
}