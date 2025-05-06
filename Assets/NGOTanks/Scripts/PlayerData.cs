using Unity.Collections;
using Unity.Netcode;

namespace NGOTanks
{
    public enum Team
    {
        Red = 0,
        Green = 1,
        Blue = 2,
        Yellow = 3,
    }
    public enum pClass
    {
        Tank = 0,
        Dps = 1,
    }
    public struct PlayerData : INetworkSerializable
    {
        public FixedString64Bytes PlayerName;
        public Team teamId;
        public pClass p_classId;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref teamId);
            serializer.SerializeValue(ref p_classId);
        }

        public override string ToString()
        {
            return $"Player Name : {PlayerName.ToString()}, Team Id : {teamId.ToString()}, Class : {p_classId.ToString()}";
        }
    }
}
