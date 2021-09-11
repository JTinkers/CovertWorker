namespace CovertWorker
{
    public class ServiceTargetOption
    {
        public string Path { get; set; }

        public string In { get; set; }

        public string Out { get; set; }

        public string Backup { get; set; }

        public ServiceTargetConverterOption Converter { get; set; }
    }
}