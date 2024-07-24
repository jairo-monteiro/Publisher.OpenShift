namespace Publisher.OpenShift.Common.Configurations
{
    public class PackagesNugetSettings
    {
        public bool CopiarPackagesNuget { get; set; }
        
        public string PastaOrigem { get; set; }

        public string PastaDestino { get; set; }

        public string UrlServidor { get; set; }
        
        public string UrlLimparCache { get; set; }
        
        public string UrlRestaurarPacotes { get; set; }
        
        public List<string> PackagesCopiar { get; set; }
    }
}