namespace Vortex.Core.Abstractions.Repositories.Data
{
    public interface IPartitionDataRepository
    {
        void Put(byte[] entryId, byte[] entity);
        byte[] Get(byte[] entryId);
        void Delete(byte[] entryId);

        void CloseConnection();
    }
}
