using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cytonet.Data.ProteinInteraction;

namespace CytoNET.Models
{
    public class ProteinInteractionViewModel
    {
        public List<ProteinInteraction> ProteinInteraction { get; set; }

        public List<Data.Protein.Protein> ProteinAdditionalInfo { get; set; }
        public Data.SmallMolecule.SmallMolecule smallMoleculeInfo { get; set; }
        public bool IsInitiatingProtein { get; set; }
        public bool ShowStimulatory { get; set; } = true;
        public bool ShowInhibitory { get; set; } = true;
        public bool ShowUndefined { get; set; } = true;
        public bool ShowUniProtId { get; set; } = true;
        public bool ShowBlackBackground { get; set; } = true;
        public string UniprotId { get; set; } = string.Empty;

        public bool InitiatorIsSmallMolecule()
        {
            var uniprotOrCasNumber = UniprotId ?? null;
            if (string.IsNullOrEmpty(uniprotOrCasNumber))
                return false;
            char firstChar = uniprotOrCasNumber.ToCharArray(0, 1)[0];
            return char.IsNumber(firstChar);
        }

        public ProteinInteractionViewModel(
            List<ProteinInteraction> proteinInteraction,
            List<Data.Protein.Protein> additionalInfo,
            Data.SmallMolecule.SmallMolecule smallMolInfo,
            bool isInitiatingProtein
        )
        {
            ProteinInteraction = proteinInteraction;
            ProteinAdditionalInfo = additionalInfo;
            IsInitiatingProtein = isInitiatingProtein;
            smallMoleculeInfo = smallMolInfo;
        }
    }
}
