using System.Collections.Generic;
using System.Linq;
using CytoNET.Data.Protein;
using CytoNET.Data.SmallMolecule;
using CytoNET.Data.TissueDistribution;

namespace CytoNET.Models
{
    public class TissueDistributionResultViewModel
    {
        public string UniprotId { get; set; } = string.Empty;
        public TissueDistribution TissueDistribution { get; set; } = new TissueDistribution();
        public SmallMolecule? SmallMolecule { get; set; }

        public string ReceptorName { get; set; } = string.Empty;
        public string ReceptorUniprotId { get; set; } = string.Empty;
        public string ReceptorType { get; set; } = string.Empty;
        public List<OrganViewModel> ReceptorOrgans { get; set; } = new List<OrganViewModel>();

        public bool IsMediatorEnzyme { get; set; }
        public bool IsMediator { get; set; }
        public bool IsReceptor { get; set; }
        public bool IsInteractingProtein { get; set; }

        public string ShortName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string FunctionDescription { get; set; } = string.Empty;
        public string PhosphoSiteId { get; set; } = string.Empty;

        public bool HasOrganOrTissues => TissueDistribution?.TissueOrgans?.Any() == true;

        public List<OrganViewModel> GetTissueOrgans()
        {
            if (!HasOrganOrTissues)
            {
                return new List<OrganViewModel>();
            }

            return TissueDistribution
                .TissueOrgans.Where(to => to.TissueOrgan != null)
                .Select(to => new OrganViewModel
                {
                    OrganId = to.TissueOrganId,
                    OrganName = to.TissueOrgan?.Name ?? "Unknown",
                    IsBold = to.TissueOrgan?.IsBold ?? false,
                })
                .OrderBy(o => o.OrganName)
                .ToList();
        }

        public string GetEntityTypeDescription()
        {
            if (IsMediatorEnzyme)
                return "Mediator Enzyme";
            if (IsMediator)
                return "Mediator";
            if (IsReceptor)
                return "Receptor";
            if (IsInteractingProtein)
                return "Interacting Protein";
            return "Unknown";
        }

        public string GetMoleculeTypeDescription()
        {
            if (SmallMolecule != null)
            {
                return !string.IsNullOrEmpty(SmallMolecule.MediatorType)
                    ? SmallMolecule.MediatorType
                    : "Small Molecule";
            }

            return "Protein";
        }

        public bool IsHormone => SmallMolecule?.HormoneStatus?.ToUpper() == "T";
        public bool IsCytokine => SmallMolecule?.CytokineStatus?.ToUpper() == "T";
        public bool IsNeurotransmitter => SmallMolecule?.NeurotransmitterStatus?.ToUpper() == "T";
        public bool IsGrowthFactor =>
            SmallMolecule != null
            && !string.IsNullOrEmpty(SmallMolecule.MediatorId)
            && SmallMolecule.MediatorId.ToUpper() == "T";
    }

    public class OrganViewModel
    {
        public string OrganId { get; set; } = string.Empty;
        public string OrganName { get; set; } = string.Empty;
        public bool IsBold { get; set; }
    }
}
