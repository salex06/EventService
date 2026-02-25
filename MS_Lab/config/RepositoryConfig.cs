namespace MS_Lab.config
{
    public record RepositoryConfig
    {
        public static readonly string SectionName = "RepositorySettings";
        public int ObjectPerRequestLimit { get; set; }
    }
}
