using Cytonet.Data.ProteinInteraction;
using CytoNET.Models;

namespace CytoNET.Repository.Interfaces
{
    public interface IProteinInteractionRepository
    {
        Task<
            Tuple<
                List<ProteinInteraction>,
                List<Data.Protein.Protein>,
                Data.SmallMolecule.SmallMolecule,
                bool
            >
        > GetProteinInteractionByProteinId(string uniprotId);
        Task<ProteinInteraction> GetProteinInteractionByShortName(string shortName);
        Task<IEnumerable<ProteinSearchResult>> SearchProteins(string term);
    }
}
