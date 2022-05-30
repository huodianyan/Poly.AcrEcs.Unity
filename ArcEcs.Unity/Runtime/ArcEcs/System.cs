using System;

namespace Poly.ArcEcs
{
    public interface ISystem
    {
        //IWorld World { get; set; }
        void Init(World world);
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

    public class RemoveCompSystem : ISystem
    {
        private World world;
        private Query[] removeCompQuerys;
        public void Init(World world)
        {
            this.world = world;
            removeCompQuerys = new Query[256];
        }
        public void AddComp<T>() where T : struct
        {
            var compId = world.GetComponentId<T>();
            var desc = world.CreateQueryDesc().WithAll(compId).Build();
            var query = world.GetQuery(desc);
            removeCompQuerys[compId] = query;
        }
        public void Update()
        {
            var count = removeCompQuerys.Length;
            for (int i = 0; i < count; i++)
            {
                var query = removeCompQuerys[i];
                if (query == null) continue;
                var compId = (byte)i;
                query.ForEach((Entity entity) =>
                {
                    world.RemoveComponent(entity, compId);
                });
            }
        }
    }
}
