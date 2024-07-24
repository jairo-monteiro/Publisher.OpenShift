namespace Publisher.OpenShift.Common.Configurations
{
    public class OpenShiftSettings
    {
        public AutenticacaoSettings Autenticacao { get; set; }

        public string Url { get; set; }
        
        public string PastaOpenShiftCLI { get; set; }

        public string Namespace { get; set; }

        public string Projeto { get; set; }
    }
}