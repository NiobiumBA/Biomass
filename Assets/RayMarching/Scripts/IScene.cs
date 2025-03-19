using System.Collections.Generic;

namespace RayMarching
{
    public interface IScene<T>
    {
        IReadOnlyCollection<T> Objects { get; }

        void AddObject(T obj);
        bool RemoveObject(T obj);
    }
}