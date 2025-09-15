using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CytoNET.Data.Protein;
using CytoNET.Data.TissueDistribution;

namespace CytoNET.Models
{
    public class TissueDistributionViewModel
    {
        public TissueDistribution TissueDistribution { get; set; }

        public int? OrganOrTissuesCount => TissueDistribution?.TissueOrgans?.Count;
        public bool HasOrganOrTissues => TissueDistribution?.TissueOrgans != null;

        public TissueDistributionViewModel(TissueDistribution tissueDistribution)
        {
            TissueDistribution = tissueDistribution;
        }
    }

    public class TissueDistributionSearchResult
    {
        public string UniprotIdorCas { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Alias { get; set; }
        public string MediatorCompoundShortName { get; set; }
        public string MediatorCompoundUniprotId { get; set; }
    }
}
