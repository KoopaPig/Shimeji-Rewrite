using RewriteWrapper;
using System;
using System.Diagnostics;
using System.IO;

namespace RewriteWrapper
{
    public class JavaCommunicator
    {
        // communicate.txt should be within the Shimeji folder in the same directory as the executable
        private const string FilePath = "Shimeji/communicate.txt";

        /// PUBLIC METHODS ///


        public static void CheckForCommand()
        {
            Debug.Print("Checking for command...");
            if (!File.Exists(FilePath))
                return;

            try
            {
                string content;

                using (var stream = new FileStream(
                    FilePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite)) // 👈 KEY FIX
                using (var reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd().Trim();
                }

                if (content.StartsWith("COMMAND="))
                {
                    string command = content.Substring("COMMAND=".Length);
                    HandleCommand(command);

                    // Reset safely
                    using (var stream = new FileStream(
                        FilePath,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.ReadWrite))
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write("COMMAND=NONE");
                    }
                }
            }
            catch (IOException)
            {
                // File was temporarily locked — just ignore this cycle
            }
        }

        private static void StopJavaProcess()
        {
            
        }

        /// PRIVATE METHODS ///


        /// <summary>
        /// Handle what happens when command is read.
        /// <br>Should have parity with Java.</br>
        /// </summary>
        private static void HandleCommand(string command)
        {
            switch (command)
            {
                case "STOP":
                    StopJavaProcess();
                    break;
                case "TEST":
                    Debug.Print("Test Command");
                    break;

            }
        }
    }
}
