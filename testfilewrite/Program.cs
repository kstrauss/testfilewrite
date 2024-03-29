﻿/*
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
*/

using System.CommandLine;

Console.WriteLine("TestFileWriter");
var pathOption = new Option<string?>(name: "path", "path will file will be written");
var durationOption = new Option<int>(name: "duration", "seconds to execute for");
var bytesLengthOption = new Option<int>(name: "bytesCount", "number of bytes to write on each write");
var delayMSDelay = new Option<double>(name: "MSdelay", description: "number of MS to delay b/t writes", getDefaultValue: () => 0.0);
var flushOption = new Option<bool>(name: "flush", description: "whether to flush after each write", getDefaultValue: ()=>false);

if (args.Length >= 4)
{
    Console.WriteLine("Should write to {0} for {1} seconds, each write will be {2} bytes, with a delay of {3} milliseconds", args[0], args[1], args[2], args[3]);
    var obj = new CloudTesting.TestFileWriter(outPath: args[0],
        durationSeconds: int.Parse(args[1]),
        bytesPerWrite: int.Parse(args[2]),
        delayBetweenWrites: int.Parse(args[3]),
        flushBetweenWrites: args.Length >= 5 ? bool.Parse(args[4]) : false);
    var r = obj.Run();
    var timePerWrite = r / int.Parse(args[1]);
    Console.WriteLine($"Did {r} writes, which equates to {timePerWrite} writes/second");  
    
}
else
{
    Console.Error.WriteLine("no args");
    Console.Error.WriteLine("Args should be: path secondsToRun bytesPerWrite delayinMilliseconds [Flush true|force]");
}
