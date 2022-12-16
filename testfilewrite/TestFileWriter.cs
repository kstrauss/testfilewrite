// See https://aka.ms/new-console-template for more information
/*
 * A quick utility for testing the impact of many small writes over potentially a filesystem that has a
 * large latency.
 * For testing environment for MGTriton where we see poor write performance in azure. Using Procmon we see
 * that some files that are written are written in many short writes with a flush b/t each write (i.e. 79 bytes/write)
 * with say 1 million writes. created this to test this scenario with 
 * https://jagt.github.io/clumsy/index.html for testing what this impact is when we have different latency
 * on the network.
 * 
 * On my local laptop to a solaris fileshare seeing a 48x difference if we use flush or not flush with a 79 byte write over a
 * <1 millisecond latency.
 * on the local drive we see a 8.2x difference
 * 
 * Most of this is pretty intuitively obvious if you just read the directions for fileWritestream as it has a default buffer of 4096 bytes
 * after which it will flush for you anyways.
*/

using System.Diagnostics;
using System.IO;

namespace CloudTesting
{
    public class TestFileWriter
    {
        readonly string _path;
        readonly int _durationSeconds;
        readonly int _bytesPerWrite;
        readonly int _delayBetweenWrites;
        readonly bool _flushBetweenWrites;
        readonly char[] _writeData;
        public TestFileWriter(string outPath, int durationSeconds, int bytesPerWrite = 79, int delayBetweenWrites = 0, bool flushBetweenWrites = true)
        {
            _path = outPath;
            _durationSeconds = durationSeconds;
            _bytesPerWrite = bytesPerWrite;
            _delayBetweenWrites = delayBetweenWrites;
            _flushBetweenWrites = flushBetweenWrites;
            _writeData = new string('x', bytesPerWrite).ToCharArray();
        }

        /// <summary>
        /// will write the data in the pattern specified in the constructor, and will return
        /// with the number of writes done during the period
        /// </summary>
        /// <returns>number of writes completed</returns>
        public long Run()
        {
            using (var stream = File.CreateText(_path))
            {
                long writeCalls = 0;
                var finishT = DateTime.Now.AddSeconds(_durationSeconds);                
                while (DateTime.Now < finishT)
                {
                    stream.Write(_writeData);
                    writeCalls++;
                    if (_flushBetweenWrites)
                        stream.Flush();
                    if (_delayBetweenWrites >0)
                        Task.Delay(_delayBetweenWrites).Wait();
                }
                return writeCalls;
            }
        }
    }
}