namespace Vortex.Core.Abstractions.Services.Data
{
    public interface IPartitionDataService<T>
    {
        void Put(ReadOnlySpan<byte> entryId, ReadOnlySpan<T> entity);

        //T Get(long entryId);
        //T GetNext(long entryId);

        //bool TryGet(long entryId, out T entity);
        //bool TryGetNext(long entryId, out T entity);

        //void Delete(long entryId);
    }
}
