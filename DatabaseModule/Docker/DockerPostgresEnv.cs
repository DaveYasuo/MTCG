using System;
using System.Diagnostics;

namespace DatabaseModule.Docker
{
    public static class DockerPostgresEnv
    {
        // Container Name of the Postgres DB-Image
        private static readonly string ContainerName;
        private static readonly string UsernameEnv;
        private static readonly string PasswordEnv;
        public static string PostgresUser { get; private set; }
        public static string PostgresPassword { get; private set; }

        static DockerPostgresEnv()
        {
            ContainerName = "swe1db";
            UsernameEnv = "POSTGRES_USER";
            PasswordEnv = "POSTGRES_PASSWORD";
            GetEnvironmentVariables();
        }

        private static void GetEnvironmentVariables()
        {
            // ReSharper disable once StringLiteralTypo
            PostgresUser = ExecuteCommand($"docker exec {ContainerName} printenv {UsernameEnv}");
            // ReSharper disable once StringLiteralTypo
            PostgresPassword = ExecuteCommand($"docker exec {ContainerName} printenv {PasswordEnv}");
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