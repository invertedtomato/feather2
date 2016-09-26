using Benchmark.Records;
using InvertedTomato.IO.Feather.ClassicCodec;
using ManagedXZ;
using System;
using System.Diagnostics;
using System.IO;
using ThreePlay.IO.Feather;

namespace Benchmark {
    class Program {
        static void Main(string[] args) {
            Stopwatch stopwatch;
            ClassicDecoder readerPayload;

            // Decompress test data
            if (!File.Exists("test-data.mtd")) {
                XZUtils.DecompressFile("test-data.xz", "test-data.mtd");
            }

            // Open input file
            using (var input = File.OpenRead("test-data.mtd")) {

                // BASELINE
                using (var reader = new FeatherReader(input)) {
                    // Iterate through all records
                    stopwatch = Stopwatch.StartNew();
                    while ((readerPayload = reader.Read<ClassicDecoder>()) != null) {
                        switch (readerPayload.ReadUInt8()) {
                            case AssociationRecord.OpCode: // Association
                                var association = AssociationRecord.FromClassic(readerPayload);
                                break;
                            case ConnectionRecord.OpCode: // Connection
                                var connection = ConnectionRecord.FromClassic(readerPayload);
                                break;
                        }
                    }
                    Console.WriteLine("Baseline: " + stopwatch.ElapsedMilliseconds + "ms " + Math.Round((double)input.Length / 1024 / 1024, 2) + "MB");
                    input.Position = 0;

                    // CLASSIC

                    using (var output = new MemoryStream()) {
                        using (var writer = new FeatherWriter(output)) {

                            // Iterate through all records
                            stopwatch = Stopwatch.StartNew();
                            while ((readerPayload = reader.Read<ClassicDecoder>()) != null) {
                                switch (readerPayload.ReadUInt8()) {
                                    case AssociationRecord.OpCode: // Association
                                        var association = AssociationRecord.FromClassic(readerPayload);
                                        writer.Write(association.ToClassic());
                                        break;
                                    case ConnectionRecord.OpCode: // Connection
                                        var connection = ConnectionRecord.FromClassic(readerPayload);
                                        writer.Write(connection.ToClassic());
                                        break;
                                }
                            }
                            Console.WriteLine("Classic:  " + stopwatch.ElapsedMilliseconds + "ms " + Math.Round((double)output.Length / 1024 / 1024, 2) + "MB");
                            input.Position = 0;
                        }
                    }

                    // CSV

                    using (var output = new MemoryStream()) {
                        using (var writer = new FeatherWriter(output)) {

                            // Iterate through all records
                            stopwatch = Stopwatch.StartNew();
                            while ((readerPayload = reader.Read<ClassicDecoder>()) != null) {
                                switch (readerPayload.ReadUInt8()) {
                                    case AssociationRecord.OpCode: // Association
                                        var association = AssociationRecord.FromClassic(readerPayload);
                                        writer.Write(association.ToCSV());
                                        break;
                                    case ConnectionRecord.OpCode: // Connection
                                        var connection = ConnectionRecord.FromClassic(readerPayload);
                                        writer.Write(connection.ToCSV());
                                        break;
                                }

                            }
                            Console.WriteLine("CSV:      " + stopwatch.ElapsedMilliseconds + "ms " + Math.Round((double)output.Length / 1024 / 1024, 2) + "MB");
                            input.Position = 0;
                        }
                    }
                }

                Console.ReadKey(true);
            }
        }
    }
}
