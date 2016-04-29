namespace Moonfire.Core.Collections
{
    using System;
    using System.Collections.Concurrent;

    public class ObjectPool<TEntity>
    {
        public ObjectPool(Func<TEntity> objectGenerator)
        {
            this.Objects = new ConcurrentBag<TEntity>();
            this.ObjectGenerator = objectGenerator;
        }

        private ConcurrentBag<TEntity> Objects { get; set; }

        private Func<TEntity> ObjectGenerator { get; set; }

        public TEntity GetObject()
        {
            TEntity item;

            if (this.Objects.TryTake(out item))
            {
                return item;
            }

            return this.ObjectGenerator();
        }

        public void PutObject(TEntity item)
        {
            this.Objects.Add(item);
        }
    }
}
