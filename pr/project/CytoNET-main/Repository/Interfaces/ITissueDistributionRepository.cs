using CytoNET.Data.TissueDistribution;
using CytoNET.Models;

namespace CytoNET.Repository.Interfaces
{
    public interface ITissueDistributionRepository
    {
        Task<TissueDistribution> GetTissueDistributionById(string uniprotId);
        Task<TissueDistribution> GetTissueDistributionByCasNumber(string casNumber);
        Task<List<TissueDistribution>> GetTissueDistributionByTissueOrOrganId(string tissueOrganId);
        Task<List<TissueDistribution>> GetTissueDistributionByTissueOrOrganName(
            string tissueOrganName
        );
        Task<List<TissueDistributionSearchResult>> SearchTissueDistributions(string searchTerm);
    }
}
