using System.Collections.Generic;
using System.Linq;
using CytoNET.Data.Protein;
using CytoNET.Data.ProteinModification;

namespace CytoNET.Models
{
    // public class ProteinModificationViewModel
    // {
    //     public Data.ProteinModification.ProteinModification ProteinModification { get; set; } =
    //         new Data.ProteinModification.ProteinModification();

    //     public IEnumerable<Data.ProteinModification.ProteinModification> RelatedProductModifications { get; set; } =
    //         new List<Data.ProteinModification.ProteinModification>();

    //     public IEnumerable<Data.ProteinModification.ProteinModification> RelatedModificationModifications { get; set; } =
    //         new List<Data.ProteinModification.ProteinModification>();

    //     public IEnumerable<Data.ProteinModification.ProteinModification> RelatedProteinLevelModifications { get; set; } =
    //         new List<Data.ProteinModification.ProteinModification>();

    //     public int TotalRelatedModifications =>
    //         (RelatedProductModifications?.Count() ?? 0)
    //         + (RelatedModificationModifications?.Count() ?? 0)
    //         + (RelatedProteinLevelModifications?.Count() ?? 0);

    //     public bool HasRelatedData => TotalRelatedModifications > 0;

    //     public bool HasProduct => ProteinModification?.Product != null;

    //     public bool HasModification => ProteinModification?.Modification != null;

    //     public bool HasProteinLevel => ProteinModification?.ProteinLevel != null;

    //     public int GetRelatedProductCount => RelatedProductModifications?.Count() ?? 0;

    //     public int GetRelatedModificationCount => RelatedModificationModifications?.Count() ?? 0;

    //     public int GetRelatedProteinLevelCount => RelatedProteinLevelModifications?.Count() ?? 0;
    // }

    public class ProteinModificationViewModel
    {
        public Protein Protein { get; set; } = new Protein();

        public IEnumerable<ProteinModification> Sites { get; set; } =
            new List<ProteinModification>();
        public bool ShowStimulatory { get; set; } = true;
        public bool ShowInhibitory { get; set; } = true;
        public bool ShowUndefined { get; set; } = true;
        public bool ShowUniProtId { get; set; } = true;
        public bool ShowBlackBackground { get; set; } = true;
    }

    public class ProteinModificationSearchResult
    {
        // public string Id { get; set; }
        public string UniprotId { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Alias { get; set; }
        // public string Site { get; set; }
        // public string ProductCode { get; set; }
        // public string ModificationType { get; set; }
        // public int NumReports { get; set; }
    }
}
