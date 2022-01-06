using System;
using System.Diagnostics;

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

        static DockerPostgresEnv()
        {
            ContainerName = "swe1db";
            // Environment Variables of docker container
            const string usernameEnv = "POSTGRES_USER";
            const string passwordEnv = "POSTGRES_PASSWORD";
            // ReSharper disable once StringLiteralTypo
            PostgresUser = ExecuteCommand($"docker exec {ContainerName} printenv {usernameEnv}");
            // ReSharper disable once StringLiteralTypo
            PostgresPassword = ExecuteCommand($"docker exec {ContainerName} printenv {passwordEnv}");
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