using System;

namespace Poly.ArcEcs
{
    #region System
    public interface IEcsSystem : IDisposable
    {
        //IWorld World { get; set; }
        void Init(EcsWorld world);
        void Update();
    }
    //public interface IEcsInitSystem : ISystem
    //{
    //    void Init(EcsWorld world);
    //}

    //public interface IEcsRunSystem : ISystem
    //{
    //    void Run(EcsWorld world);
    //}

    //public interface IEcsDestroySystem : ISystem
    //{
    //    void Destroy(EcsWorld world);
    //}
    #endregion

}
