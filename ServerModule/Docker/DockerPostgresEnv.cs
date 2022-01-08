using System;
using System.Diagnostics;
using DebugAndTrace;
using ServerModule.Utility;
using Char = System.Char;

namespace ServerModule.Docker
{
    /// <summary>
    /// Get Docker Container Environment Variables
    /// </summary>
    public static class DockerPostgresEnv
    {
        /// <summary>
        /// Container Username of the Postgres DB-Image
        /// </summary>
        public static readonly string ContainerName;
        public static readonly string PostgresUser;
        public static readonly string PostgresPassword;
        public static readonly string PostgresPort;

        static DockerPostgresEnv()
        {
            // Change this, if using another container name
            ContainerName = "swe1db";
            // Change this, if using other Environment Variables of docker container
            const string usernameEnv = "POSTGRES_USER";
            const string passwordEnv = "POSTGRES_PASSWORD";
            // ReSharper disable once StringLiteralTypo
            PostgresUser = ExecuteCommand($"docker exec {ContainerName} printenv {usernameEnv}");
            // ReSharper disable once StringLiteralTypo
            PostgresPassword = ExecuteCommand($"docker exec {ContainerName} printenv {passwordEnv}");
            string rawPort = ExecuteCommand($"docker ps --filter \"name={ContainerName}\" --format \"{{{{.Ports}}}}\"");
            // Example: 0.0.0.0:5432->5432/tcp 
            int startIndex = rawPort.IndexOf(Utils.GetChar(Utility.Char.Colon)) + 1;
            int endIndex = rawPort.IndexOf(Utils.GetChar(Utility.Char.Minus));
            if (startIndex >= 0 && rawPort.Length > startIndex) PostgresPort = rawPort[startIndex..endIndex];
            else
            {
                Printer.Instance.WriteLine("Port is invalid. Check if port is exposed. Using default port 5432 ...");
                PostgresPort = "5432";
            }
        }

        private static string ExecuteCommand(string command)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                // *** Redirect the output ***
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            using Process process = Process.Start(processInfo);
            Debug.Assert(process != null, nameof(process) + " != null");
            string envData = null;
            string errorMsg = null;
            process.OutputDataReceived += (_, e) => envData += e.Data;
            process.BeginOutputReadLine();

            process.ErrorDataReceived += (_, e) => errorMsg += e.Data;
            process.BeginErrorReadLine();

            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                Console.WriteLine(errorMsg);
                process.Close();
                return null;
            }
            process.Close();
            return envData;
        }
    }
}