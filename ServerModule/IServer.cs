namespace ServerModule
{
    public interface IServer
    {
        /// <summary>
        ///     Starts the server.
        /// </summary>
        public void Start();

        /// <summary>
        ///     Stops the server and release all unmanaged resources.
        /// </summary>
        public void Stop();
    }
}