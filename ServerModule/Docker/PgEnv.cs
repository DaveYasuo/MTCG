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
    public static class PgEnv
    {
        /// <summary>
        /// Container Username of the Postgres DB-Image
        /// </summary>
        public static readonly string Username = "POSTGRES_USER";
        public static readonly string Password = "POSTGRES_PASSWORD";
        public static readonly string Database = "POSTGRES_DB";
        public static readonly string Port = "POSTGRES_PORT";

        static PgEnv()
        {
            // change this string, if using other container name
            const string containerName = "swe1db";

            // Required cannot be empty
            SetEnvironmentVariable(Password, GetCommand(containerName, Password));

            // Optional
            SetEnvironmentVariable(Username, GetCommand(containerName, Username), "swe1user");
            SetEnvironmentVariable(Database, GetCommand(containerName, Database), "swe1db");

            // Optional get the first matched exposed Port of the docker container
            string rawPort = ExecuteCommand( $"docker ps --filter \"name={containerName}\" --format \"{{{{.Ports}}}}\"");
            ParsePort(rawPort, out string postgresPort);
            SetEnvironmentVariable(Port, null, postgresPort);
        }

        /// <summary>
        /// Gets the Command string from container with the specific environment variable
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="envVariable"></param>
        /// <returns>Returns the command as a string</returns>
        private static string GetCommand(string containerName, string envVariable)
        {
            return $"docker exec {containerName} printenv {envVariable}";
        }

        /// <summary>
        /// Gets the first matched port of the raw port.
        /// </summary>
        /// <param name="rawPort"></param>
        /// <param name="parsedPort"></param>
        private static void ParsePort(string rawPort, out string parsedPort)
        {
            // Example: 0.0.0.0:5432->5432/tcp 
            int startIndex = rawPort.IndexOf(Utils.GetChar(Utility.Char.Colon)) + 1;
            int endIndex = rawPort.IndexOf(Utils.GetChar(Utility.Char.Minus));
            if (startIndex >= 0 && rawPort.Length > startIndex)
                parsedPort = rawPort[startIndex..endIndex];
            else
            {
                throw new ArgumentException("Port is invalid. Check if port is exposed.");
            }
        }

        /// <summary>
        /// Executes the command, if the output of the command is null of empty, the default value is used to set the Environment Variable.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="command"></param>
        /// <param name="defaultValue"></param>
        private static void SetEnvironmentVariable(string variable, string command, string defaultValue = "")
        {
            if (!string.IsNullOrEmpty(command))
            {
                string value = ExecuteCommand(command);
                if (value != "") defaultValue = value;
            }
            Environment.SetEnvironmentVariable(variable, defaultValue);
        }

        /// <summary>
        /// Executes the command in cmd.exe. Use it only under Windows. When an Exception is thrown, it ends the current process.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Returns the output of the command, can be an empty string ""</returns>
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
            string envData = null;
            string errorMsg = null;
            try
            {
                // no need to call Close explicitly if using using-clause
                // See: https://stackoverflow.com/a/33803135
                using Process process = Process.Start(processInfo);
                Debug.Assert(process != null, nameof(process) + " != null");
                process.OutputDataReceived += (_, e) => envData += e.Data;
                process.BeginOutputReadLine();

                process.ErrorDataReceived += (_, e) => errorMsg += e.Data;
                process.BeginErrorReadLine();

                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    if (errorMsg != "")
                    {
                        throw new Exception(errorMsg);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Cannot read Environment Variables of Docker container.");
                Console.Error.WriteLine(e.Message);
                throw;
            }
            return envData;
        }
    }
}