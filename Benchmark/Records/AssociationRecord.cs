using InvertedTomato.IO.Feather.ClassicCodec;
using InvertedTomato.IO.Feather.CSVCodec;
using System;
using System.Net;

namespace Benchmark.Records {
    public sealed class AssociationRecord {
        public const byte OpCode = 0x01;

        public IPAddress IPAddress { get; set; }
        public byte[] MACAddress { get; set; }
        public string Label { get; set; }

        public static AssociationRecord FromClassic(ClassicDecoder payload) {
            var record = new AssociationRecord();
            record.IPAddress = payload.ReadIPAddress();
            record.MACAddress = payload.Read(6);
            record.Label = payload.ReadString();

            return record;
        }



        public ClassicEncoder ToClassic() {
            var payload = new ClassicEncoder();
            payload.WriteUInt8(OpCode);
            payload.WriteIPAddress(IPAddress);
            payload.Write(MACAddress.Length == 8 ? MACAddress : new byte[6]);
            payload.WriteString(Label);

            return payload;
        }

        public CSVEncoder ToCSV() {
            var payload = new CSVEncoder();
            payload.WriteInteger(OpCode);
            payload.WriteString(IPAddress.ToString());
            payload.WriteString(BitConverter.ToString(MACAddress));
            payload.WriteString(Label);

            return payload;
        }
    }
}
