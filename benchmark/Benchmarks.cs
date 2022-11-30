using System;
using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;

namespace benchmark
{
    public class Benchmarks
    {
        readonly protected string path;
        //[GlobalSetup()]
        public Benchmarks() {
            path = Path.GetTempFileName();
        }

        //[GlobalCleanup()]
        ~Benchmarks()
        {
            try { 
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        [Params(0,3,5)]
        public int millisecondDelay { get; set; }
        [Params(true,false)]
        public bool flush { get; set; }

        

        //[Benchmark]
        public long Scenario1()
        {
            // Implement your benchmark here
            // would like to not measure how long these take but rather the number of executions done in the window of time
            // not sure that we can do this.
            // see: https://benchmarkdotnet.org/articles/guides/how-it-works.html
            // looking at some of the standard benchmarks:
            // https://github.com/dotnet/performance/blob/main/src/benchmarks/micro/libraries/System.IO.FileSystem/Perf.FileStream.cs
            // it seems that they only measure duration
            var obj = new CloudTesting.TestFileWriter(outPath: path, durationSeconds: millisecondDelay, bytesPerWrite: 79, flushBetweenWrites: flush);
            return obj.Run();
        }

        

    }
    public record BenchArgs
    {
        public int DataLength { get; set; }
        public char[] Data { get; set; }
        public StreamWriter FS { get; set; }

        public override string ToString()
        {
            return $"size = {Data.Length}";
        }
    }
    public class SimpleWriteBenchmark
    {
        public IEnumerable<BenchArgs> ArgSource()
        {
            var sizes = new[] {
                1, 30, 60, 90, 120, 180, 220, 440 
            };
            //var fs = File.CreateText(@"\\opensolaris\rpool_export_home_kstrauss\filewritetest\somefile.xtt");
            
                foreach (var size in sizes)
                {
                    yield return new BenchArgs() {
                        DataLength = size, Data = new String('x', size).ToCharArray(),
                        FS = File.CreateText(String.Format(@"\\opensolaris\rpool_export_home_kstrauss\filewritetest\{0}.xtt",Guid.NewGuid().ToString("N")))
                    };
                }
            
        }

        [Benchmark]
        [ArgumentsSource(nameof(ArgSource))]
        public void XX(BenchArgs args)
        {
            args.FS.Write(args.Data);
            args.FS.Flush();
        }
    }
}
