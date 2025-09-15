using System.Collections.Generic;
using CytoNET.Data.Protein;

namespace CytoNET.Repository.Interfaces
{
    public interface IProteinModelRepository
    {
        Protein GetProteinById(string uniprotId);
        Protein GetProteinByType(string type);
        List<Protein> GetProteinsByType(string type);
        Protein GetProteinByShortName(string shortName);
        Protein GetProteinByLongName(string longName);
        Protein GetProteinByProcessing(string processing);
        Protein GetProteinByPrecursorProteinMass(string precursorProteinMass);
        Protein GetProteinByFunctionDescription(string functionDescription);
        Protein GetProteinCstPhosphositePlusEntryId(string cstPhosphositePlusEntryId);
        Protein GetProteinLevelId(string proteinLevelId);
        Protein GetCompleteProtein(string uniprotId);
    }
}
