using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using CommandLine.Text;
using SkriptInsight.JavaMetadataExtractorLib;
using SkriptInsight.JavaMetadataExtractorLib.MetadataRepresentation;

namespace SkriptInsight.JavaMetadataExtractorConsole
{
    class Program
    {
        public class Options
        {
            
            [Option("none", Required = false, HelpText = "Set to disable output.")]
            public bool None { get; set; }
            
            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }
            
            [Option('d', "debug", Required = false, HelpText = "Set output to debug messages.")]
            public bool Debug { get; set; }

            [Option("skip-existing", Required = false,
                HelpText = "Skip existing metadata files and don't rewrite them")]
            public bool SkipExistingFiles { get; set; }

            [Value(0, MetaName = "files", Required = true, HelpText = "The files to be processed")]
            public IEnumerable<string> Files { get; set; }

            [Option("classpath", HelpText = "Classpath for the files")]
            public IEnumerable<string> ClassPath { get; set; }

            [Option('o', "outputDir", Default = ".\\", Required = false,
                HelpText = "The output folder for the metadata files")]
            public string OutputDirectory { get; set; }
        }

        private const string JarExtension = ".jar";
        private const string SkriptInsightMetaExtension = ".simeta";
        private const string JreMetadataFile = "jre-rt.simeta";

        private static void Log(LogLevel eLevel, string msg)
        {
            if (currentLevel.IsSameOrHigher(eLevel))
                Console.WriteLine(msg);
        }

        private static void Log(string msg) => Log(LogLevel.Verbose, msg);
        private static void LogVerbose(string msg) => Log(LogLevel.Verbose, msg);
        
        private static void LogDebug(string msg) => Log(LogLevel.Debug, msg);

        private static LogLevel currentLevel = LogLevel.Normal;

        static void Main(string[] args)
        {
            var parser = new Parser(with => with.HelpWriter = null);
            var result = parser.ParseArguments<Options>(args);

            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.Heading = "SkriptInsight Metadata Extractor - Console Tool";
                h.Copyright = "Copyright (c) 2019 SkriptInsight";
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);

            result
                .WithNotParsed(errs => Console.WriteLine(helpText))
                .WithParsed(o =>
                {
                    if (o.None)
                        currentLevel = LogLevel.None;
                    else if (o.Verbose)
                        currentLevel = LogLevel.Verbose;
                    else if (o.Debug)
                        currentLevel = LogLevel.Debug;
                    
                    
                    MetadataIo.LogMessage += (_, s) => Log(s.level, s.message);

                    if (File.Exists(JreMetadataFile))
                    {
                        //Load the JRE's rt metadata from an existing file.        
                        var metaJre = MetadataIo.ReadArchiveMetadata("jre-rt.simeta");
                        var dataJre = metaJre.ToDataClass();
                        dataJre.LoadDataProperties();
                    }
                    else
                    {
                        //Try to load it from the java install
                        if (!MetadataExtractor.ReadJreStandardFile(out _))
                        {
                            Log(
                                "ERROR: Unable to find the rt.jar file and there's no metadata in the current working directory!");
                            Log("ERROR: Do you have the JAVA_HOME variable setup correctly?");
                            return;
                        }
                    }

                    GC.Collect();

                    foreach (var s in o.ClassPath)
                    {
                        if (!File.Exists(s))
                        {
                            Log($"WARN: File \"{s}\" doesn't exist and was skipped.");
                            continue;
                        }

                        switch (Path.GetExtension(s))
                        {
                            case JarExtension:
                                MetadataExtractor.ReadFile(s);
                                break;
                            case SkriptInsightMetaExtension:
                                MetadataIo.ReadArchiveMetadata(s).ToDataClass();
                                break;
                            default:
                                Log($"WARN: File \"{s}\" has an unknown file extension and got skipped.");
                                break;
                        }
                    }

                    foreach (var s in o.Files)
                    {
                        if (!File.Exists(s))
                        {
                            Log($"WARN: File \"{s}\" doesn't exist and was skipped.");
                            continue;
                        }

                        var finalFileName = Path.Combine(o.OutputDirectory,
                            Path.GetFileName(Path.ChangeExtension(s, SkriptInsightMetaExtension)));

                        if (File.Exists(finalFileName))
                        {
                            if (o.SkipExistingFiles)
                            {
                                Log($"INFO: Skipping writing file \"{s}\" because it already exists and the \"skip-existing\" option is enabled.");
                                continue;
                            }

                            File.Delete(finalFileName);
                        }

                        LogVerbose($"Starting to write archive metadata for \"{finalFileName}\"");
                        MetadataIo.WriteArchiveMetadata(finalFileName, MetadataExtractor.ReadFile(s).ToMetadata());
                        LogVerbose($"Finished writing archive metadata for \"{finalFileName}\"");
                        GC.Collect();
                    }

                    Log("Finished processing all files.");
                });
        }
    }
}