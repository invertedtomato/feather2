using InvertedTomato.IO.Feather.ClassicCodec;
using InvertedTomato.IO.Feather.CSVCodec;
using System;
using System.Net;

namespace Benchmark.Records {
    public sealed class ConnectionRecord {
        public const byte OpCode = 0x02;

        public DateTime OpenAt { get; set; }
        public DateTime CloseAt { get; set; }

        public IPAddress SourceIPAddress { get; set; }
        public IPAddress IntermediateIPAddress { get; set; }
        public IPAddress DestinationIPAddress { get; set; }

        public ushort SourcePort { get; set; }
        public ushort DestinationPort { get; set; }

        public uint PacketCount { get; set; }
        public uint Layer3Length { get; set; }
        public byte IPProtocol { get; set; }
        
        public ClassicEncoder ToClassic() {
            var payload = new ClassicEncoder();
            payload.WriteUInt8(OpCode);
            payload.WriteDateTimeSimple(OpenAt);
            payload.WriteDateTimeSimple(CloseAt);
            payload.WriteIPAddress(SourceIPAddress);
            payload.WriteIPAddress(IntermediateIPAddress);
            payload.WriteIPAddress(DestinationIPAddress);
            payload.WriteUInt16(SourcePort);
            payload.WriteUInt16(DestinationPort);
            payload.WriteUInt32(PacketCount);
            payload.WriteUInt32(Layer3Length);
            payload.WriteUInt8(IPProtocol);

            return payload;
        }

        public CSVEncoder ToCSV() {
            var payload = new CSVEncoder();
            payload.WriteInteger(OpCode);
            payload.WriteDateTime(OpenAt);
            payload.WriteDateTime(CloseAt);
            payload.WriteString(SourceIPAddress.ToString());
            payload.WriteString(IntermediateIPAddress.ToString());
            payload.WriteString(DestinationIPAddress.ToString());
            payload.WriteInteger(SourcePort);
            payload.WriteInteger(DestinationPort);
            payload.WriteInteger(PacketCount);
            payload.WriteInteger(Layer3Length);
            payload.WriteInteger(IPProtocol);

            return payload;
        }

        public static ConnectionRecord FromClassic(ClassicDecoder payload) {
            var record = new ConnectionRecord();
            record.OpenAt = payload.ReadDateTimeSimple();
            record.CloseAt = payload.ReadDateTimeSimple();
            record.SourceIPAddress = payload.ReadIPAddress();
            record.IntermediateIPAddress = payload.ReadIPAddress();
            record.DestinationIPAddress = payload.ReadIPAddress();
            record.SourcePort = payload.ReadUInt16();
            record.DestinationPort = payload.ReadUInt16();
            record.PacketCount = payload.ReadUInt32();
            record.Layer3Length = payload.ReadUInt32();
            record.IPProtocol = payload.ReadUInt8();

            return record;
        }
    }
}
