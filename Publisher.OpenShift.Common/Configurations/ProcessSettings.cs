namespace Publisher.OpenShift.Common.Configurations
{
    public class ProcessSettings
    {
        public string PastaOrigem { get; set; }

        public string PastaDestino { get; set; }
        
        public List<string> PastasNaoApagar { get; set; }
        
        public List<string> ArquivosNaoApagar { get; set; }
        
        public List<string> PastasNaoCopiar { get; set; }
        
        public List<string> ArquivosNaoCopiar { get; set; }
    }
}