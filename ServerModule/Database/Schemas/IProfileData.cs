namespace ServerModule.Database.Schemas
{
    public interface IProfileData
    {
        public string Username { get; }
        public string Name { get; }
        public string Bio { get; }
        public string Image { get; }
    }
}