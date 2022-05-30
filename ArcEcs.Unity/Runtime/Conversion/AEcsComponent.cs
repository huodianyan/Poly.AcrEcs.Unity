using UnityEngine;

namespace Poly.ArcEcs.Unity
{
    public interface IEcsComponent
    {
        void ApplyTo(World world, Entity entity);
    }
    public abstract class AEcsComponent<T> : MonoBehaviour, IEcsComponent where T : struct
    {
        [SerializeField]
        private T comp;

        public virtual void ApplyTo(World world, Entity entity)
        {
            world.AddComponent<T>(entity, comp);
            Destroy(this);
        }
    }
}