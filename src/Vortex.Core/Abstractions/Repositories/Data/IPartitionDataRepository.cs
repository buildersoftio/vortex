namespace Vortex.Core.Abstractions.Repositories.Data
{
    public interface IPartitionDataRepository
    {
        void Put(byte[] entryId, byte[] message);

        void Put(ReadOnlySpan<byte> entryId, ReadOnlySpan<byte> message);
        void PutTemporary(byte[] entryId, byte[] message);
        void PutTemporary(ReadOnlySpan<byte> entryId, ReadOnlySpan<byte> message);

        byte[] Get(byte[] entryId);
        void Delete(byte[] entryId);

        void CloseConnection();
    }
}
