// <copyright file="Program.cs" company="Erratic Motion Ltd">
// Copyright (c) Erratic Motion Ltd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ErraticMotion
{
    using System;
    using System.Collections;
    using System.IO;

    using Test.Tools.Gherkin;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World! In Docker {0}", InDocker);
            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
            {
                Console.WriteLine("  {0} = {1}", de.Key, de.Value);
            }

            var root = Environment.GetEnvironmentVariable("GHERKIN_ROOT");

            var features = Directory.EnumerateFiles(root + "/features/google/", "*.feature");
            foreach (var feature in features)
            {
                var lexer = Lexer.For(feature);
                var result = lexer.Parse();
                Console.WriteLine(result);
                using (var writer = File.CreateText(feature + ".ast"))
                {
                    writer.Write(result); 
                }
            }

            //Console.WriteLine("GHERKIN_ROOT: {0}", root);
            //var folderExists = Directory.Exists(root);
            //Console.WriteLine("Folder exists {0}, {1}", folderExists, root);
            //if (folderExists)
            //{
            //    using (var writer = File.CreateText(root + "/stuff.txt"))
            //    {
            //        writer.WriteLine("log message"); 
            //    }

            //    foreach (var f in Directory.GetFiles(root))
            //    {
            //        Console.WriteLine("In target directory: {0}", f);
            //    }
            //}

            if (!InDocker)
            {
                Console.ReadLine();
            }
        }

        private static bool InDocker => Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    }
}