using CytoNET.Data.Protein;

namespace CytoNET.Models
{
    public class ProteinModelViewModel
    {
        public Protein protein { get; set; }
        public string alias { get; set; }
        public Product product { get; set; }
        public ProteinLevel proteinLevel { get; set; }
    }
}
