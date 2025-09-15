using CytoNET.Data.Protein;
using CytoNET.Data.ProteinInteraction;
using CytoNET.Data.ProteinModification;
using CytoNET.Models;

namespace CytoNET.Repository.Interfaces
{
    public interface IProteinModificationRepository
    {
        // ProteinModification GetProteinModificationById(string uniprotId);
        // ProteinModification GetProteinModificationByProduct(string product);
        // ProteinModification GetProteinModificationByModification(string modification);
        // ProteinModification GetProteinModificationByProteinLevel(string proteinLevel);
        // ProteinModification GetProteinModificationByPSiteAbProduct(string pSiteAbProduct);
        // ProteinModification GetProteinModificationWithAllLinks(string uniprotId);
        // ProteinModification GetCompleteProteinModification(string uniprotId);
        (Protein, List<ProteinModification>) GetProteinModificationData(string UniprotId);
        Task<IEnumerable<ProteinModificationSearchResult>> SearchProteinModification(string term);
    }
}
