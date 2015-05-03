
namespace LunarPluginInternal
{
    interface IBaseCollection<T>
    {
        bool Add(T t);
        bool Remove(T t);
        bool Contains(T t);
        void Clear();
        int Count { get; }
    }
}
