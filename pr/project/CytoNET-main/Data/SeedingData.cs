using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using CytoNET.Data.Protein;
using Cytonet.Data.ProteinInteraction;
using CytoNET.Data.ProteinModification;
using CytoNET.Data.SmallMolecule;
using CytoNET.Data.TissueDistribution;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CytoNET.Seed
{
    public static class DatabaseSeeder
    {
        /// <summary>
        /// Main entry point to seed all databases
        /// </summary>
        /// <param name="clearExistingData">If true, clears all existing data before seeding</param>
        public static void SeedAllDatabases(
            SmallMoleculeDbContext smallMoleculeContext,
            ProteinDbContext proteinContext,
            ProteinModificationDbContext proteinModificationContext,
            ProteinInteractionDbContext proteinInteractionContext,
            TissueDistributionDbContext tissueDistributionContext,
            bool clearExistingData = true
        )
        {
            Console.WriteLine("Starting database seeding process...");

            if (clearExistingData)
            {
                ClearAllData(
                    smallMoleculeContext,
                    proteinContext,
                    proteinModificationContext,
                    proteinInteractionContext,
                    tissueDistributionContext
                );
            }

            SeedSmallMoleculeData(smallMoleculeContext);
            SeedProteinData(proteinContext);
            SeedProteinModificationData(proteinModificationContext);
            SeedProteinInteractionData(proteinInteractionContext);
            SeedTissueDistributionData(tissueDistributionContext);

            Console.WriteLine("Database seeding completed successfully.");
        }

        /// <summary>
        /// Clears all data from all database contexts
        /// </summary>
        private static void ClearAllData(
            SmallMoleculeDbContext smallMoleculeContext,
            ProteinDbContext proteinContext,
            ProteinModificationDbContext proteinModificationContext,
            ProteinInteractionDbContext proteinInteractionContext,
            TissueDistributionDbContext tissueDistributionContext
        )
        {
            Console.WriteLine("Clearing existing data from all databases...");

            // Clear SmallMolecule DB
            try
            {
                ClearDbSet(smallMoleculeContext.MediatorCompounds);
                ClearDbSet(smallMoleculeContext.SmallMolecules);
                smallMoleculeContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error clearing small molecule db");
                Console.WriteLine(e.Message);
            }

            // Clear Protein DB
            try
            {
                ClearDbSet(proteinContext.Aliases);
                ClearDbSet(proteinContext.Products);
                ClearDbSet(proteinContext.Proteins);
                ClearDbSet(proteinContext.ProteinLevels);
                proteinContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error clearing protein db");
                Console.WriteLine(e.Message);
            }

            // Clear ProteinModification DB
            try
            {
                ClearDbSet(proteinModificationContext.ProteinModifications);
                ClearDbSet(proteinModificationContext.PSiteAbProducts);
                ClearDbSet(proteinModificationContext.Modifications);
                ClearDbSet(proteinModificationContext.ProteinLevels);
                proteinModificationContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error clearing protein modification db");
                Console.WriteLine(e.Message);
            }

            // Clear ProteinInteraction DB
            try
            {
                ClearDbSet(proteinInteractionContext.ProteinInteractions);
                ClearDbSet(proteinInteractionContext.ProteinLevels);
                ClearDbSet(proteinInteractionContext.Proteins);
                proteinInteractionContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error clearing protein interaction db");
                Console.WriteLine(e.Message);
            }

            // Clear TissueDistribution DB
            try
            {
                ClearDbSet(tissueDistributionContext.TissueDistributionTissueOrgans);
                ClearDbSet(tissueDistributionContext.TissueOrgans);
                ClearDbSet(tissueDistributionContext.TissueDistributions);
                tissueDistributionContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error clearing tissue distribution db");
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("All existing data cleared successfully.");
        }

        /// <summary>
        /// Helper method to clear a DbSet
        /// </summary>
        private static void ClearDbSet<T>(DbSet<T> dbSet)
            where T : class
        {
            if (dbSet.Any())
            {
                dbSet.RemoveRange(dbSet);
            }
        }

        /// <summary>
        /// Seed SmallMolecule data from CSV
        /// </summary>
        public static void SeedSmallMoleculeData(SmallMoleculeDbContext context)
        {
            Console.WriteLine("Starting small molecule database seed...");

            // DEBUG: Check database schema for ProteaseInfo property
            try
            {
                var proteaseInfoProperty = context
                    .Model.FindEntityType(typeof(SmallMolecule))
                    .FindProperty(nameof(SmallMolecule.ProteaseInfo));
                Console.WriteLine(
                    $"ProteaseInfo property IsNullable: {proteaseInfoProperty.IsNullable}"
                );
                Console.WriteLine(
                    $"ProteaseInfo property HasDefaultValue: {proteaseInfoProperty.GetDefaultValue() != null}"
                );
                if (proteaseInfoProperty.GetDefaultValue() != null)
                {
                    Console.WriteLine(
                        $"ProteaseInfo default value: '{proteaseInfoProperty.GetDefaultValue()}'"
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking ProteaseInfo property: {ex.Message}");
            }

            string projectDirectory = Path.GetFullPath(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../")
            );
            string filePath = Path.Combine(projectDirectory, "SeedData", "TableA.csv");
            Console.WriteLine($"Attempting to load from: {filePath}");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                Delimiter = ",",
                Quote = '"',
                IgnoreBlankLines = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                BadDataFound = null,
            };

            // Define counters for reporting
            int totalRowsProcessed = 0;
            int smallMoleculesAdded = 0;
            int mediatorCompoundsAdded = 0;
            var problemRows = new Dictionary<string, string>();

            // Get existing records to avoid duplicates
            var existingCasNos = new HashSet<string>(
                context.SmallMolecules.AsNoTracking().Select(sm => sm.CasNo)
            );
            var existingUniprotIds = new HashSet<string>(
                context.MediatorCompounds.AsNoTracking().Select(mc => mc.UniprotId)
            );

            try
            {
                using var reader = new StreamReader(filePath);

                // Read column headers
                string? headerLine = reader.ReadLine();
                string? descriptiveHeaderLine = reader.ReadLine();

                if (headerLine == null || descriptiveHeaderLine == null)
                {
                    Console.WriteLine("File does not contain proper header information");
                    return;
                }

                // Parse column headers
                string[] headers = headerLine.Split(',');
                string[] descriptions = descriptiveHeaderLine.Split(',');

                Console.WriteLine($"Found {headers.Length} columns in header");

                // Add debug output to verify column detection for proteases
                Console.WriteLine("\nVerifying column detection for proteases:");
                for (int i = 0; i < descriptions.Length; i++)
                {
                    if (
                        descriptions[i]
                            .Contains("Proteases implicated", StringComparison.OrdinalIgnoreCase)
                    )
                    {
                        Console.WriteLine(
                            $"Found proteases column at index {i}: '{descriptions[i]}'"
                        );
                    }
                }

                // Map column indices
                int casNoIndex = -1;
                int mediatorTFIndex = -1;
                int mediatorShortNameIndex = -1;
                int mediatorLongNameIndex = -1;
                int mediatorAliasIndex = -1;
                int mediatorTypeIndex = -1;
                int precursorMassIndex = -1;
                int aaPositionsIndex = -1;
                int proteasesIndex = -1;
                int compoundMassIndex = -1;
                int pubchemNumIndex = -1;
                int chemspiderNumIndex = -1;
                int chemblNumIndex = -1;
                int biosynthNumIndex = -1;
                int uniprotIdIndex = -1;
                int phosphositeplusIdIndex = -1;
                int descriptionIndex = -1;
                int hormoneStatusIndex = -1;
                int cytokineStatusIndex = -1;
                int neurotransmitterStatusIndex = -1;

                // Map columns by their descriptive headers
                for (int i = 0; i < Math.Min(headers.Length, descriptions.Length); i++)
                {
                    string description = descriptions[i].Trim();

                    if (description.Contains("CAS No.", StringComparison.OrdinalIgnoreCase))
                        casNoIndex = i;
                    else if (
                        description.Contains("Mediator - T/F", StringComparison.OrdinalIgnoreCase)
                    )
                        mediatorTFIndex = i;
                    else if (
                        description.Contains(
                            "Mediator Short Name",
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                        mediatorShortNameIndex = i;
                    else if (
                        description.Contains(
                            "Mediator Long Name",
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                        mediatorLongNameIndex = i;
                    else if (
                        description.Contains("Mediator Alias", StringComparison.OrdinalIgnoreCase)
                    )
                        mediatorAliasIndex = i;
                    else if (
                        description.Contains("Mediator Type", StringComparison.OrdinalIgnoreCase)
                    )
                        mediatorTypeIndex = i;
                    else if (
                        description.Contains(
                            "Precursor Mediator Protein Mass",
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                        precursorMassIndex = i;
                    else if (
                        description.Contains(
                            "Mature Mediator Protein AA Positions",
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                        aaPositionsIndex = i;
                    // Modified protease detection to be more flexible
                    else if (
                        description.Contains(
                            "Proteases implicated",
                            StringComparison.OrdinalIgnoreCase
                        ) || description.Contains("Proteases", StringComparison.OrdinalIgnoreCase)
                    )
                        proteasesIndex = i;
                    else if (
                        description.Contains(
                            "Mediator Compound Mass",
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                        compoundMassIndex = i;
                    else if (
                        description.Contains("PubChem Number", StringComparison.OrdinalIgnoreCase)
                    )
                        pubchemNumIndex = i;
                    else if (
                        description.Contains(
                            "ChemSpider Number",
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                        chemspiderNumIndex = i;
                    else if (
                        description.Contains("ChEMBL Number", StringComparison.OrdinalIgnoreCase)
                    )
                        chemblNumIndex = i;
                    else if (
                        description.Contains(
                            "Biosynthetic Enzyme",
                            StringComparison.OrdinalIgnoreCase
                        ) && description.Contains("Type", StringComparison.OrdinalIgnoreCase)
                    )
                        biosynthNumIndex = i;
                    else if (description.Contains("UniProt ID", StringComparison.OrdinalIgnoreCase))
                        uniprotIdIndex = i;
                    else if (
                        description.Contains(
                            "PhosphoSitePlus ID",
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                        phosphositeplusIdIndex = i;
                    else if (
                        description.Contains("Description", StringComparison.OrdinalIgnoreCase)
                    )
                        descriptionIndex = i;
                    else if (
                        description.Contains("Hormone - T/F", StringComparison.OrdinalIgnoreCase)
                    )
                        hormoneStatusIndex = i;
                    else if (
                        description.Contains("Cytokine - T/F", StringComparison.OrdinalIgnoreCase)
                    )
                        cytokineStatusIndex = i;
                    else if (
                        description.Contains(
                            "Neurotransmitter - T/F",
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                        neurotransmitterStatusIndex = i;
                }

                Console.WriteLine("Using column indices:");
                Console.WriteLine($"  - CAS No.: {casNoIndex}");
                Console.WriteLine($"  - Mediator T/F: {mediatorTFIndex}");
                Console.WriteLine($"  - Mediator Short Name: {mediatorShortNameIndex}");
                Console.WriteLine($"  - Mediator Long Name: {mediatorLongNameIndex}");
                Console.WriteLine($"  - UniProt ID: {uniprotIdIndex}");
                Console.WriteLine($"  - Proteases: {proteasesIndex}");

                if (casNoIndex < 0 || mediatorShortNameIndex < 0 || mediatorLongNameIndex < 0)
                {
                    Console.WriteLine("WARNING: Could not find all required columns. Aborting.");
                    return;
                }

                // Lists to hold entities we'll add to the database
                var smallMolecules = new List<SmallMolecule>();
                var mediatorCompounds = new List<MediatorCompound>();

                // Improved CSV parsing with CsvHelper
                reader.BaseStream.Position = 0;
                reader.DiscardBufferedData();

                using (var csv = new CsvReader(reader, config))
                {
                    // Skip header rows
                    csv.Read();
                    csv.Read();

                    while (csv.Read())
                    {
                        totalRowsProcessed++;

                        try
                        {
                            // Extract CAS No. and check if valid
                            var casNo = csv.GetField(casNoIndex)?.Trim();
                            if (string.IsNullOrWhiteSpace(casNo))
                            {
                                problemRows[$"Row {totalRowsProcessed + 2}"] = "Missing CAS No.";
                                continue;
                            }

                            Console.WriteLine($"Processing row for CAS No: {casNo}");

                            // Skip if already exists
                            if (existingCasNos.Contains(casNo))
                            {
                                Console.WriteLine($"Skipping existing CAS No: {casNo}");
                                continue;
                            }

                            // Check if this is a mediator compound
                            var isMediatorTF = csv.GetField(mediatorTFIndex)?.Trim();
                            Console.WriteLine($"  Mediator T/F value: '{isMediatorTF}'");

                            // Original mediator check logic (keep this as is)
                            bool isMediator =
                                !string.IsNullOrEmpty(isMediatorTF)
                                && (
                                    isMediatorTF.Equals("T", StringComparison.OrdinalIgnoreCase)
                                    || isMediatorTF.Equals(
                                        "TRUE",
                                        StringComparison.OrdinalIgnoreCase
                                    )
                                );

                            if (!isMediator)
                            {
                                Console.WriteLine(
                                    $"Skipping non-mediator compound with CAS No: {casNo}"
                                );
                                continue;
                            }

                            // Get basic small molecule fields
                            var shortName = csv.GetField(mediatorShortNameIndex)?.Trim() ?? "";
                            var longName = csv.GetField(mediatorLongNameIndex)?.Trim() ?? "";
                            var alias =
                                (mediatorAliasIndex >= 0)
                                    ? csv.GetField(mediatorAliasIndex)?.Trim() ?? ""
                                    : "";
                            var mediatorType =
                                (mediatorTypeIndex >= 0)
                                    ? csv.GetField(mediatorTypeIndex)?.Trim() ?? "Small molecule"
                                    : "Small molecule";

                            // Parse numeric values
                            string precursorMass = csv.GetField(precursorMassIndex)?.Trim() ?? "";

                            var aaPositions =
                                (aaPositionsIndex >= 0)
                                    ? csv.GetField(aaPositionsIndex)?.Trim() ?? ""
                                    : "";

                            // Get status fields with default "F"
                            var hormoneStatus =
                                (hormoneStatusIndex >= 0)
                                    ? csv.GetField(hormoneStatusIndex)?.Trim() ?? "F"
                                    : "F";
                            var cytokineStatus =
                                (cytokineStatusIndex >= 0)
                                    ? csv.GetField(cytokineStatusIndex)?.Trim() ?? "F"
                                    : "F";
                            var neurotransmitterStatus =
                                (neurotransmitterStatusIndex >= 0)
                                    ? csv.GetField(neurotransmitterStatusIndex)?.Trim() ?? "F"
                                    : "F";

                            // Extract proteases information with enhanced error handling and logging
                            string proteaseInfo = "";
                            Console.WriteLine(
                                $"\n  PROTEASE DEBUG - Column index: {proteasesIndex}"
                            );

                            if (proteasesIndex >= 0)
                            {
                                try
                                {
                                    // Try directly reading the raw CSV data first
                                    var currentRecord = csv.Parser.Record;
                                    Console.WriteLine(
                                        $"  PROTEASE DEBUG - Raw record length: {currentRecord?.Length ?? 0}"
                                    );

                                    if (
                                        currentRecord != null
                                        && proteasesIndex < currentRecord.Length
                                    )
                                    {
                                        var rawValue = currentRecord[proteasesIndex];
                                        Console.WriteLine(
                                            $"  PROTEASE DEBUG - Raw value: '{rawValue}'"
                                        );
                                    }
                                    else
                                    {
                                        Console.WriteLine(
                                            "  PROTEASE DEBUG - Index out of range or null record"
                                        );
                                    }

                                    // Try using the GetField method
                                    proteaseInfo = csv.GetField(proteasesIndex)?.Trim() ?? "";
                                    Console.WriteLine(
                                        $"  PROTEASE DEBUG - GetField result: '{proteaseInfo}'"
                                    );

                                    // Let's also try to get the raw field value for column A9 by index 8
                                    try
                                    {
                                        var forcedProteasesValue = csv.GetField(8)?.Trim() ?? "";
                                        Console.WriteLine(
                                            $"  PROTEASE DEBUG - Forced index 8 value: '{forcedProteasesValue}'"
                                        );
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(
                                            $"  PROTEASE DEBUG - Forced index error: {ex.Message}"
                                        );
                                    }

                                    // Try to read the entire row as a string to see what's there
                                    string[] allFields = new string[35]; // Assuming there might be up to 35 fields
                                    for (int i = 0; i < 35 && i < currentRecord?.Length; i++)
                                    {
                                        try
                                        {
                                            allFields[i] = csv.GetField(i)?.Trim() ?? "";
                                        }
                                        catch
                                        {
                                            allFields[i] = "ERROR";
                                        }
                                    }

                                    Console.WriteLine(
                                        $"  PROTEASE DEBUG - Field 7: '{allFields[7]}'"
                                    );
                                    Console.WriteLine(
                                        $"  PROTEASE DEBUG - Field 8: '{allFields[8]}'"
                                    );
                                    Console.WriteLine(
                                        $"  PROTEASE DEBUG - Field 9: '{allFields[9]}'"
                                    );

                                    // Special handling - try to find a column that might have protease data
                                    var possibleProteaseColumns = new List<int>();
                                    for (int i = 0; i < allFields.Length; i++)
                                    {
                                        if (
                                            !string.IsNullOrEmpty(allFields[i])
                                            && allFields[i]
                                                .Contains(
                                                    "protease",
                                                    StringComparison.OrdinalIgnoreCase
                                                )
                                        )
                                        {
                                            possibleProteaseColumns.Add(i);
                                            Console.WriteLine(
                                                $"  PROTEASE DEBUG - Possible protease data at index {i}: '{allFields[i]}'"
                                            );
                                        }
                                    }

                                    // If proteaseInfo is still empty, try to find any field that looks like it contains protease information
                                    if (string.IsNullOrEmpty(proteaseInfo))
                                    {
                                        // Special handling for specific row patterns in the sample data
                                        if (casNo == "5536-17-4" && allFields[8].Contains("diol"))
                                        {
                                            proteaseInfo = allFields[8];
                                            Console.WriteLine(
                                                $"  PROTEASE DEBUG - Found special case for CAS {casNo} - using field 8: '{proteaseInfo}'"
                                            );
                                        }
                                        else if (possibleProteaseColumns.Count > 0)
                                        {
                                            int bestIndex = possibleProteaseColumns[0];
                                            proteaseInfo = allFields[bestIndex];
                                            Console.WriteLine(
                                                $"  PROTEASE DEBUG - Using best candidate from index {bestIndex}: '{proteaseInfo}'"
                                            );
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(
                                        $"  PROTEASE DEBUG - Error extracting proteases data: {ex.Message}"
                                    );
                                    Console.WriteLine(
                                        $"  PROTEASE DEBUG - Stack trace: {ex.StackTrace}"
                                    );
                                    proteaseInfo = ""; // Default to empty string on error
                                }
                            }
                            else
                            {
                                Console.WriteLine(
                                    "  PROTEASE DEBUG - Proteases column index is invalid"
                                );
                            }

                            // Force a non-empty value for testing
                            if (string.IsNullOrEmpty(proteaseInfo))
                            {
                                proteaseInfo = $"[No protease info for {casNo}]";
                                Console.WriteLine(
                                    $"  PROTEASE DEBUG - Using forced non-empty value: '{proteaseInfo}'"
                                );
                            }

                            Console.WriteLine(
                                $"  PROTEASE DEBUG - Final proteaseInfo value: '{proteaseInfo}'"
                            );

                            // Generate an ID for the small molecule
                            string smallMoleculeId = Guid.NewGuid().ToString();

                            // Create the small molecule entity
                            var smallMolecule = new SmallMolecule
                            {
                                Id = smallMoleculeId,
                                CasNo = casNo,
                                MediatorShortName = shortName,
                                MediatorLongName = longName,
                                MediatorAlias = alias,
                                MediatorType = mediatorType,
                                PrecursorMediatorProteinMass = csv.GetField(6)?.Trim() ?? "",
                                MatMedProtAaPositions = aaPositions,
                                HormoneStatus = hormoneStatus,
                                CytokineStatus = cytokineStatus,
                                NeurotransmitterStatus = neurotransmitterStatus,
                                MediatorId = casNo, // Using CAS No. as mediator ID
                                ProteaseInfo = proteaseInfo, // Store proteases info here
                                CasLink = csv.GetField(13)?.Trim() ?? "",
                                PubChemLink = csv.GetField(14)?.Trim() ?? "",
                                ChemSpiderLink = csv.GetField(15)?.Trim() ?? "",
                                ChemBlLink = csv.GetField(16)?.Trim() ?? "",
                            };

                            // Check entity state
                            Console.WriteLine(
                                $"  Entity initial state: {context.Entry(smallMolecule).State}"
                            );

                            // Add small molecule to our list
                            smallMolecules.Add(smallMolecule);
                            existingCasNos.Add(casNo);
                            smallMoleculesAdded++;
                            Console.WriteLine($"  Added small molecule with ID: {smallMoleculeId}");

                            // Process mediator compound info if available
                            if (uniprotIdIndex >= 0)
                            {
                                var uniprotId = csv.GetField(uniprotIdIndex)?.Trim();
                                if (
                                    !string.IsNullOrEmpty(uniprotId)
                                    && !existingUniprotIds.Contains(uniprotId)
                                )
                                {
                                    Console.WriteLine(
                                        $"  Processing mediator compound with UniProt ID: {uniprotId}"
                                    );

                                    // Parse compound details
                                    double mass = 0;
                                    if (compoundMassIndex >= 0)
                                    {
                                        var massStr = csv.GetField(compoundMassIndex)?.Trim();
                                        if (!string.IsNullOrEmpty(massStr))
                                        {
                                            if (!double.TryParse(massStr, out mass))
                                            {
                                                Console.WriteLine(
                                                    $"    Warning: Could not parse compound mass '{massStr}' for {uniprotId}"
                                                );
                                            }
                                        }
                                    }

                                    var pubchemNum =
                                        (pubchemNumIndex >= 0)
                                            ? csv.GetField(pubchemNumIndex)?.Trim() ?? ""
                                            : "";
                                    var chemspiderNum =
                                        (chemspiderNumIndex >= 0)
                                            ? csv.GetField(chemspiderNumIndex)?.Trim() ?? ""
                                            : "";
                                    var chemblNum =
                                        (chemblNumIndex >= 0)
                                            ? csv.GetField(chemblNumIndex)?.Trim() ?? ""
                                            : "";
                                    var biosynthNum =
                                        (biosynthNumIndex >= 0)
                                            ? csv.GetField(biosynthNumIndex)?.Trim() ?? ""
                                            : "";
                                    var phosphositeplusId =
                                        (phosphositeplusIdIndex >= 0)
                                            ? csv.GetField(phosphositeplusIdIndex)?.Trim() ?? ""
                                            : "";
                                    var description =
                                        (descriptionIndex >= 0)
                                            ? csv.GetField(descriptionIndex)?.Trim() ?? ""
                                            : "";

                                    // Create mediator compound with SmallMoleculeId
                                    var mediatorCompound = new MediatorCompound
                                    {
                                        UniprotId = uniprotId,
                                        SmallMoleculeId = smallMoleculeId, // This is the foreign key
                                        Mass = mass,
                                        PubchemNum = pubchemNum,
                                        ChemspiderNum = chemspiderNum,
                                        ChemblNum = chemblNum,
                                        BiosynthNum = biosynthNum,
                                        ShortName = csv.GetField(18)?.Trim() ?? "",
                                        LongName = csv.GetField(19)?.Trim() ?? "",
                                        Alias = csv.GetField(20)?.Trim() ?? "",
                                        PhosphositeplusId = phosphositeplusId,
                                        Description = description,
                                    };

                                    mediatorCompounds.Add(mediatorCompound);
                                    existingUniprotIds.Add(uniprotId);
                                    mediatorCompoundsAdded++;
                                    Console.WriteLine(
                                        $"    Added mediator compound with UniProt ID: {uniprotId}"
                                    );
                                }
                                else if (!string.IsNullOrEmpty(uniprotId))
                                {
                                    Console.WriteLine(
                                        $"  Skipping duplicate UniProt ID: {uniprotId}"
                                    );
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            problemRows[$"Row {totalRowsProcessed + 2}"] = $"Error: {ex.Message}";
                            Console.WriteLine(
                                $"Error processing row {totalRowsProcessed + 2}: {ex.Message}"
                            );
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                }

                // Debug information before saving
                Console.WriteLine("\n========== DATA TO BE SAVED ==========");
                Console.WriteLine($"Small molecules to save: {smallMolecules.Count}");
                Console.WriteLine($"Mediator compounds to save: {mediatorCompounds.Count}");

                // Check entities before saving
                Console.WriteLine("\nPRE-SAVE CHECK - Verifying ProteaseInfo values:");
                foreach (var sm in smallMolecules.Take(5))
                {
                    Console.WriteLine(
                        $"SmallMolecule {sm.CasNo} has ProteaseInfo = '{sm.ProteaseInfo}'"
                    );
                }

                if (smallMolecules.Count == 0)
                {
                    Console.WriteLine(
                        "WARNING: No small molecules to save! Aborting database update."
                    );
                    return;
                }

                // Now save everything within a transaction
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // Save small molecules first
                        Console.WriteLine(
                            $"\nAdding {smallMolecules.Count} small molecules to database"
                        );
                        context.SmallMolecules.AddRange(smallMolecules);

                        // Check entity state again before saving
                        var firstEntity = smallMolecules.FirstOrDefault();
                        if (firstEntity != null)
                        {
                            Console.WriteLine(
                                $"  First entity state before SaveChanges: {context.Entry(firstEntity).State}"
                            );
                            Console.WriteLine(
                                $"  First entity ProteaseInfo: '{firstEntity.ProteaseInfo}'"
                            );
                        }

                        int smallMoleculesResult = context.SaveChanges();
                        Console.WriteLine(
                            $"  Result: {smallMoleculesResult} small molecules saved"
                        );

                        // Double-check saved data
                        if (smallMoleculesResult > 0 && firstEntity != null)
                        {
                            var savedEntity = context
                                .SmallMolecules.AsNoTracking()
                                .FirstOrDefault(s => s.Id == firstEntity.Id);

                            if (savedEntity != null)
                            {
                                Console.WriteLine(
                                    $"  SAVE CHECK - Saved entity ProteaseInfo: '{savedEntity.ProteaseInfo}'"
                                );
                            }
                            else
                            {
                                Console.WriteLine("  SAVE CHECK - Could not retrieve saved entity");
                            }
                        }

                        // Save mediator compounds next if any exist
                        if (mediatorCompounds.Count > 0)
                        {
                            Console.WriteLine(
                                $"Adding {mediatorCompounds.Count} mediator compounds to database"
                            );
                            context.MediatorCompounds.AddRange(mediatorCompounds);
                            int mediatorCompoundsResult = context.SaveChanges();
                            Console.WriteLine(
                                $"  Result: {mediatorCompoundsResult} mediator compounds saved"
                            );
                        }
                        else
                        {
                            Console.WriteLine("No mediator compounds to save");
                        }

                        transaction.Commit();
                        Console.WriteLine("Transaction committed successfully!");
                    }
                    catch (DbUpdateException dbEx)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"ERROR: Database update exception: {dbEx.Message}");

                        if (dbEx.InnerException != null)
                        {
                            Console.WriteLine($"Inner exception: {dbEx.InnerException.Message}");
                        }

                        // If there are entity validation errors, print them
                        var entries = dbEx.Entries;
                        foreach (var entry in entries)
                        {
                            Console.WriteLine(
                                $"Problem with entity: {entry.Entity.GetType().Name}"
                            );
                            Console.WriteLine($"State: {entry.State}");
                            foreach (var prop in entry.Properties)
                            {
                                Console.WriteLine($"  {prop.Metadata.Name}: {prop.CurrentValue}");
                            }
                        }

                        throw;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"ERROR: Transaction rolled back: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                        }
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing CSV: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            // Final summary
            Console.WriteLine("\n========== SMALL MOLECULE SEEDING SUMMARY ==========");
            Console.WriteLine($"Total rows processed: {totalRowsProcessed}");
            Console.WriteLine($"Small molecules added: {smallMoleculesAdded}");
            Console.WriteLine($"Mediator compounds added: {mediatorCompoundsAdded}");

            // Report problems
            if (problemRows.Any())
            {
                Console.WriteLine($"\nEncountered {problemRows.Count} problem rows:");
                foreach (var problem in problemRows.Take(20))
                {
                    Console.WriteLine($"  - {problem.Key}: {problem.Value}");
                }
                if (problemRows.Count > 20)
                {
                    Console.WriteLine($"  - ... and {problemRows.Count - 20} more issues");
                }
            }

            Console.WriteLine("=================================================");
        }

        public static void SeedProteinData(ProteinDbContext context)
        {
            Console.WriteLine("Starting protein database seed...");

            string projectDirectory = Path.GetFullPath(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../")
            );
            string filePath = Path.Combine(projectDirectory, "SeedData", "TableB.csv");
            Console.WriteLine($"Attempting to load from: {filePath}");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                Delimiter = ",",
                Quote = '"',
                IgnoreBlankLines = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                BadDataFound = null,
            };

            try
            {
                using var reader = new StreamReader(filePath);

                string? headerLine = reader.ReadLine();
                string? descriptionHeaderLine = reader.ReadLine();

                if (headerLine == null || descriptionHeaderLine == null)
                {
                    Console.WriteLine("File does not contain proper header information");
                    return;
                }

                string[] headers = headerLine.Split(',');
                string[] descriptions = descriptionHeaderLine.Split(',');

                Console.WriteLine($"Found {headers.Length} columns in header");

                var mediatorEnzymeLevel = new Data.Protein.ProteinLevel
                {
                    ProteinLevelId = "1",
                    Category = "mediatorEnzyme",
                };
                var mediatorLevel = new Data.Protein.ProteinLevel
                {
                    ProteinLevelId = "2",
                    Category = "mediator",
                };
                var receptorLevel = new Data.Protein.ProteinLevel
                {
                    ProteinLevelId = "3",
                    Category = "receptor",
                };
                var interactingProteinLevel = new Data.Protein.ProteinLevel
                {
                    ProteinLevelId = "4",
                    Category = "interactingProtein",
                };
                context.ProteinLevels.Add(mediatorEnzymeLevel);
                context.ProteinLevels.Add(mediatorLevel);
                context.ProteinLevels.Add(receptorLevel);
                context.ProteinLevels.Add(interactingProteinLevel);
                context.SaveChanges();
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Read();
                    csv.Read();

                    Console.WriteLine("----- Seeding Protein Level Table -----");
                    while (csv.Read())
                    {
                        string uniprotId = csv.GetField(0)?.Trim() ?? "";
                        if (string.IsNullOrWhiteSpace(uniprotId))
                        {
                            Console.WriteLine("No uniprot id, skipping");
                            continue;
                        }
                        string? proteinLevelId;
                        var mediatorEnzyme = csv.GetField(1)?.Trim() ?? "";
                        var mediator = csv.GetField(2)?.Trim() ?? "";
                        var receptor = csv.GetField(3)?.Trim() ?? "";
                        var interactingProtein = csv.GetField(4)?.Trim() ?? "";
                        bool isMediatorEnzyme =
                            !string.IsNullOrEmpty(mediatorEnzyme)
                            && (
                                mediatorEnzyme.Equals("T", StringComparison.OrdinalIgnoreCase)
                                || mediatorEnzyme.Equals("TRUE", StringComparison.OrdinalIgnoreCase)
                            );
                        bool isMediator =
                            !string.IsNullOrEmpty(mediator)
                            && (
                                mediator.Equals("T", StringComparison.OrdinalIgnoreCase)
                                || mediator.Equals("TRUE", StringComparison.OrdinalIgnoreCase)
                            );
                        bool isReceptor =
                            !string.IsNullOrEmpty(receptor)
                            && (
                                receptor.Equals("T", StringComparison.OrdinalIgnoreCase)
                                || receptor.Equals("TRUE", StringComparison.OrdinalIgnoreCase)
                            );
                        bool isInteractingProtein =
                            !string.IsNullOrEmpty(interactingProtein)
                            && (
                                interactingProtein.Equals("T", StringComparison.OrdinalIgnoreCase)
                                || interactingProtein.Equals(
                                    "TRUE",
                                    StringComparison.OrdinalIgnoreCase
                                )
                            );

                        if (isMediatorEnzyme)
                            proteinLevelId = "1";
                        else if (isMediator)
                            proteinLevelId = "2";
                        else if (isReceptor)
                            proteinLevelId = "3";
                        else
                            proteinLevelId = "4";

                        try
                        {
                            string aliasesString = csv.GetField(8)?.Trim() ?? "";

                            var protein = new Data.Protein.Protein
                            {
                                UniprotId = uniprotId,
                                Type = csv.GetField(5)?.Trim() ?? "",
                                ShortName = csv.GetField(6)?.Trim() ?? "",
                                LongName = csv.GetField(7)?.Trim() ?? "",
                                Processing = csv.GetField(9)?.Trim() ?? "",
                                Aliases = csv.GetField(8)?.Trim() ?? "",
                                PrecursorProteinMass = csv.GetField(10)?.Trim() ?? "",
                                FunctionDescription = csv.GetField(11)?.Trim() ?? "",
                                CstPhosphositePlusEntryId = csv.GetField(17)?.Trim() ?? "",
                                ProteinLevelId = proteinLevelId,
                                StringDbLink = csv.GetField(20)?.Trim() ?? "",
                                UniprotLink = csv.GetField(16)?.Trim() ?? "",
                                KinaseNetLink = csv.GetField(13)?.Trim() ?? "",
                                PhosphoNetLink = csv.GetField(12)?.Trim() ?? "",
                                TranscriptoNetLink = csv.GetField(19)?.Trim() ?? "",
                                OncoNetLink = csv.GetField(14)?.Trim() ?? "",
                                DrugProNetLink = csv.GetField(15)?.Trim() ?? "",
                                PhosphoSitePlusLink = csv.GetField(18)?.Trim() ?? "",
                                IconType = csv.GetField(46)?.Trim() ?? "Unclassified",
                            };

                            var existingProtein = context.Proteins.FirstOrDefault(p =>
                                p.UniprotId == protein.UniprotId
                            );
                            if (existingProtein == null)
                            {
                                Console.WriteLine("Adding new protein");
                                context.Proteins.Add(protein);
                                context.SaveChanges();

                                if (!string.IsNullOrEmpty(aliasesString))
                                {
                                    var aliasesList = aliasesString
                                        .Split(';', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(a => a.Trim())
                                        .Where(a => !string.IsNullOrEmpty(a))
                                        .ToList();

                                    int aliasCount = 0;
                                    foreach (var aliasName in aliasesList)
                                    {
                                        try
                                        {
                                            var alias = new Data.Protein.Alias
                                            {
                                                AliasId = $"{uniprotId}_{aliasCount++}",
                                                ProteinUniprotId = uniprotId,
                                                Name = aliasName,
                                            };

                                            context.Aliases.Add(alias);
                                            Console.WriteLine(
                                                $"Added alias: {aliasName} for protein {uniprotId}"
                                            );
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine($"Error adding alias: {aliasName}");
                                            Console.WriteLine(e.Message);
                                        }
                                    }

                                    context.SaveChanges();
                                }

                                // Add products
                                try
                                {
                                    var products = new List<Product>();

                                    for (int i = 0; i <= 10; i++)
                                    {
                                        var nameIndex = (i * 2) + 26;
                                        var linkIndex = nameIndex + 1;

                                        var name = csv.GetField(nameIndex);
                                        var link = csv.GetField(linkIndex);

                                        if (
                                            !string.IsNullOrEmpty(name)
                                            && !string.IsNullOrEmpty(link)
                                        )
                                            products.Add(
                                                new Product
                                                {
                                                    ProteinUniprotId = uniprotId,
                                                    Name = name,
                                                    Link = link,
                                                }
                                            );
                                    }

                                    foreach (Product product in products)
                                    {
                                        try
                                        {
                                            Console.WriteLine(product.ProteinUniprotId);
                                            context.Products.Add(product);
                                            context.SaveChanges();
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(
                                                $"Error adding product: {product.Link}"
                                            );
                                            Console.WriteLine(e.Message);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                            else
                            {
                                Console.WriteLine(
                                    $"Protein with UniprotId {protein.UniprotId} already exists - skipping"
                                );
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error creating protein for row");
                            Console.WriteLine(e.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Helper method to seed reference data for ProteinModification
        /// </summary>
        public static void SeedProteinModificationData(ProteinModificationDbContext context)
        {
            Console.WriteLine("Starting protein modification database seed...");

            // Start by seeding the predefined protein levels
            SeedProteinLevels(context);

            string projectDirectory = Path.GetFullPath(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../")
            );
            string filePath = Path.Combine(projectDirectory, "SeedData", "TableD.csv");
            Console.WriteLine($"Attempting to load from: {filePath}");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            try
            {
                using var debugReader = new StreamReader(filePath);
                string? line;
                int lineCount = 0;
                while ((line = debugReader.ReadLine()) != null)
                {
                    lineCount++;
                }
                Console.WriteLine($"CSV file contains {lineCount} lines");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error counting lines in CSV: {ex.Message}");
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                Delimiter = ",",
                Quote = '"',
                IgnoreBlankLines = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                BadDataFound = null,
                AllowComments = true,
                ShouldSkipRecord = record =>
                    record.Row.Parser.Record.All(string.IsNullOrWhiteSpace),
            };

            int totalRowsProcessed = 0;
            int rowsWithValidData = 0;
            int modsAttempted = 0;
            int modsAdded = 0;
            int productsAdded = 0;
            int modificationsAdded = 0;
            var problemRows = new Dictionary<string, string>();

            var existingModIds = new HashSet<string>(
                context.ProteinModifications.AsNoTracking().Select(m => m.Id).ToList()
            );
            var existingProductIds = new HashSet<string>(
                context.PSiteAbProducts.AsNoTracking().Select(p => p.ProductId).ToList()
            );
            var existingModificationIds = new HashSet<string>(
                context.Modifications.AsNoTracking().Select(m => m.Id).ToList()
            );
            var existingLevelIds = new HashSet<string>(
                context.ProteinLevels.AsNoTracking().Select(pl => pl.ProteinLevelId).ToList()
            );

            Console.WriteLine("First pass: Reading and analyzing data structure...");
            var allRecords = new List<Dictionary<string, object>>();
            var allModificationTypes = new HashSet<string>();
            var allSites = new Dictionary<string, List<string>>();
            var allProducts = new HashSet<string>();

            var productFrequency = new Dictionary<string, int>();
            var modificationFrequency = new Dictionary<string, int>();
            var proteinLevelFrequency = new Dictionary<string, int>();

            try
            {
                using var reader = new StreamReader(filePath);

                string? headerLine = reader.ReadLine();
                Dictionary<string, int> columnMeanings = new Dictionary<string, int>();

                if (headerLine != null)
                {
                    string? descriptiveHeaderLine = reader.ReadLine();
                    if (descriptiveHeaderLine != null)
                    {
                        var headerFields = headerLine.Split(',');
                        var descriptiveFields = descriptiveHeaderLine.Split(',');

                        Console.WriteLine("Header structure analysis:");
                        Console.WriteLine(
                            $"First row: {string.Join(", ", headerFields.Take(10))}..."
                        );
                        Console.WriteLine(
                            $"Second row: {string.Join(", ", descriptiveFields.Take(10))}..."
                        );

                        for (
                            int i = 0;
                            i < Math.Min(headerFields.Length, descriptiveFields.Length);
                            i++
                        )
                        {
                            if (!string.IsNullOrWhiteSpace(descriptiveFields[i]))
                            {
                                Console.WriteLine(
                                    $"Column {headerFields[i]} contains: {descriptiveFields[i]}"
                                );
                                columnMeanings[descriptiveFields[i].Trim()] = i;
                            }
                        }
                    }
                }

                reader.Close();
                using var mainReader = new StreamReader(filePath);
                using var csv = new CsvReader(mainReader, config);

                csv.Read();
                csv.ReadHeader();

                var headerNames = csv.HeaderRecord?.ToList() ?? new List<string>();
                Console.WriteLine($"Found {headerNames.Count} columns in header:");
                foreach (var name in headerNames.Take(Math.Min(26, headerNames.Count)))
                {
                    Console.WriteLine($"  - {name}");
                }
                if (headerNames.Count > 10)
                    Console.WriteLine("  - ...");

                int uniprotIdIndex = -1;
                int siteIndex = -1;
                int modTypeIndex = -1;
                int modEffectCodeIndex = -1;
                int modDescIndex = -1;
                int descIndex = -1;
                int numReportsIndex = -1;
                int colorIndex = -1;
                int shapeIndex = -1;
                int mediatorIndex = -1; // Index for "Mediator - T/F" column
                int receptorIndex = -1; // Index for "Receptor - T/F" column
                List<int> productIndices = new List<int>();

                if (headerNames.Any(h => h.StartsWith("D")))
                {
                    Console.WriteLine(
                        "Detected D-format headers. Using second row for column meanings."
                    );

                    uniprotIdIndex = 0;
                    siteIndex = 4;
                    modTypeIndex = 5;
                    modEffectCodeIndex = 6;
                    modDescIndex = 7;
                    numReportsIndex = 8;
                    mediatorIndex = 2; // Based on sample data
                    receptorIndex = 3; // Based on sample data
                    descIndex = 23;

                    // Find the last two column indices for color and shape
                    if (headerNames.Count >= 2)
                    {
                        colorIndex = headerNames.Count - 2;
                        shapeIndex = headerNames.Count - 1;
                    }

                    // Find product columns
                    for (int i = 1; i <= 4; i++)
                    {
                        string productKey = $"Kinexus Psite Ab Product {i}";
                        if (columnMeanings.ContainsKey(productKey))
                        {
                            productIndices.Add(columnMeanings[productKey]);
                        }
                    }
                }
                else
                {
                    uniprotIdIndex = headerNames.FindIndex(h =>
                        h.Contains("Uniprot", StringComparison.OrdinalIgnoreCase)
                    );
                    siteIndex = headerNames.FindIndex(h =>
                        h.Equals("Site", StringComparison.OrdinalIgnoreCase)
                    );
                    modTypeIndex = headerNames.FindIndex(h =>
                        h.Contains("Modification Type", StringComparison.OrdinalIgnoreCase)
                    );
                    modEffectCodeIndex = headerNames.FindIndex(h =>
                        h.Contains("Effect Code", StringComparison.OrdinalIgnoreCase)
                    );
                    modDescIndex = headerNames.FindIndex(h =>
                        h.Contains("Effect Description", StringComparison.OrdinalIgnoreCase)
                    );
                    descIndex = headerNames.FindIndex(h =>
                        h.Contains("Modification Description", StringComparison.OrdinalIgnoreCase)
                    );
                    numReportsIndex = headerNames.FindIndex(h =>
                        h.Contains("Reports", StringComparison.OrdinalIgnoreCase)
                    );
                    mediatorIndex = headerNames.FindIndex(h =>
                        h.Contains("Mediator - T/F", StringComparison.OrdinalIgnoreCase)
                    );
                    receptorIndex = headerNames.FindIndex(h =>
                        h.Contains("Receptor", StringComparison.OrdinalIgnoreCase)
                        && h.Contains("T/F", StringComparison.OrdinalIgnoreCase)
                    );

                    // Look for color and shape in header names
                    colorIndex = headerNames.FindIndex(h =>
                        h.EndsWith("Color", StringComparison.OrdinalIgnoreCase)
                    );
                    shapeIndex = headerNames.FindIndex(h =>
                        h.EndsWith("Shape", StringComparison.OrdinalIgnoreCase)
                        || h.Contains("Icon Shape", StringComparison.OrdinalIgnoreCase)
                    );

                    for (int i = 0; i < headerNames.Count; i++)
                    {
                        if (headerNames[i].Contains("Product", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("found product index");
                            productIndices.Add(i);
                        }
                    }
                }

                Console.WriteLine("Using column indices:");
                Console.WriteLine(
                    $"  - UniprotId: {uniprotIdIndex} {(uniprotIdIndex >= 0 ? headerNames[uniprotIdIndex] : "MISSING")}"
                );
                Console.WriteLine(
                    $"  - Site: {siteIndex} {(siteIndex >= 0 ? headerNames[siteIndex] : "MISSING")}"
                );
                Console.WriteLine(
                    $"  - Modification Type: {modTypeIndex} {(modTypeIndex >= 0 ? headerNames[modTypeIndex] : "MISSING")}"
                );
                Console.WriteLine(
                    $"  - Mediator: {mediatorIndex} {(mediatorIndex >= 0 ? headerNames[mediatorIndex] : "MISSING")}"
                );
                Console.WriteLine(
                    $"  - Receptor: {receptorIndex} {(receptorIndex >= 0 ? headerNames[receptorIndex] : "MISSING")}"
                );
                Console.WriteLine(
                    $"  - Effect Code: {modEffectCodeIndex} {(modEffectCodeIndex >= 0 ? headerNames[modEffectCodeIndex] : "MISSING")}"
                );
                Console.WriteLine(
                    $"  - Color: {colorIndex} {(colorIndex >= 0 ? headerNames[colorIndex] : "MISSING")}"
                );
                Console.WriteLine(
                    $"  - Shape: {shapeIndex} {(shapeIndex >= 0 ? headerNames[shapeIndex] : "MISSING")}"
                );
                Console.WriteLine(
                    $"  -  Modification Description: {descIndex} {(descIndex >= 0 ? headerNames[descIndex] : "MISSING")}"
                );

                if (uniprotIdIndex < 0 || siteIndex < 0 || modTypeIndex < 0)
                {
                    Console.WriteLine(
                        "WARNING: Some critical columns could not be found. Will try to continue with available data."
                    );

                    if (uniprotIdIndex < 0)
                        uniprotIdIndex = 0;
                    if (siteIndex < 0)
                        siteIndex = 4;
                    if (modTypeIndex < 0)
                        modTypeIndex = 5;
                }

                if (headerNames.Any(h => h.StartsWith("D")))
                {
                    csv.Read(); // Skip the second descriptive header row in D-format
                }

                // Generate incrementing IDs for different entities
                int modificationIdCounter = 1;
                if (context.Modifications.Any())
                {
                    var ids = context.Modifications.AsNoTracking().Select(m => m.Id).ToList();
                    var numericIds = ids.Where(id =>
                    {
                        int temp;
                        return int.TryParse(id, out temp);
                    });

                    if (numericIds.Any())
                    {
                        modificationIdCounter = numericIds.Select(id => int.Parse(id)).Max() + 1;
                    }
                }

                int proteinModIdCounter = 1;
                if (context.ProteinModifications.Any())
                {
                    var ids = context
                        .ProteinModifications.AsNoTracking()
                        .Select(m => m.Id)
                        .ToList();
                    var numericIds = ids.Where(id =>
                    {
                        int temp;
                        return int.TryParse(id, out temp);
                    });

                    if (numericIds.Any())
                    {
                        proteinModIdCounter = numericIds.Select(id => int.Parse(id)).Max() + 1;
                    }
                }

                int productIdCounter = 1;
                if (context.PSiteAbProducts.Any())
                {
                    var ids = context
                        .PSiteAbProducts.AsNoTracking()
                        .Select(p => p.ProductId)
                        .ToList();
                    var numericIds = ids.Where(id =>
                    {
                        int temp;
                        return int.TryParse(id, out temp);
                    });

                    if (numericIds.Any())
                    {
                        productIdCounter = numericIds.Select(id => int.Parse(id)).Max() + 1;
                    }
                }

                while (csv.Read())
                {
                    totalRowsProcessed++;

                    if (totalRowsProcessed % 100 == 0)
                    {
                        Console.WriteLine($"Processed {totalRowsProcessed} rows...");
                    }

                    var uniprotId = csv.GetField(uniprotIdIndex)?.Trim();
                    if (string.IsNullOrWhiteSpace(uniprotId))
                    {
                        problemRows[$"Row {totalRowsProcessed + 1}"] = "Missing UniprotId";
                        continue;
                    }

                    var modificationDescription = csv.GetField(descIndex)?.Trim();

                    var site = csv.GetField(siteIndex)?.Trim();
                    if (string.IsNullOrWhiteSpace(site))
                    {
                        problemRows[$"Row {totalRowsProcessed + 1}"] = "Missing site";
                        continue;
                    }

                    var modType = csv.GetField(modTypeIndex)?.Trim();
                    if (string.IsNullOrWhiteSpace(modType))
                    {
                        modType = "Unknown";
                        Console.WriteLine(
                            $"Warning: Row {totalRowsProcessed + 1} missing modification type, using 'Unknown'"
                        );
                    }

                    var record = new Dictionary<string, object>();
                    record["UniprotId"] = uniprotId;
                    record["Site"] = site;
                    record["ModificationType"] = modType;

                    // Store mediator and receptor flags in record if available
                    if (mediatorIndex >= 0 && mediatorIndex < csv.Parser.Record.Length)
                    {
                        var mediatorValue = csv.GetField(mediatorIndex)?.Trim() ?? "F";
                        record["Mediator"] = mediatorValue;
                    }

                    if (receptorIndex >= 0 && receptorIndex < csv.Parser.Record.Length)
                    {
                        var receptorValue = csv.GetField(receptorIndex)?.Trim() ?? "F";
                        record["Receptor"] = receptorValue;
                    }

                    // Get effect code and description
                    var effectCode =
                        modEffectCodeIndex >= 0 && modEffectCodeIndex < csv.Parser.Record.Length
                            ? csv.GetField(modEffectCodeIndex)?.Trim() ?? "0"
                            : "0";
                    record["EffectCode"] = effectCode;

                    var description =
                        modDescIndex >= 0 && modDescIndex < csv.Parser.Record.Length
                            ? csv.GetField(modDescIndex)?.Trim() ?? "Unknown effect"
                            : "Unknown effect";
                    record["Description"] = description;

                    // Get color and shape
                    var color =
                        colorIndex >= 0 && colorIndex < csv.Parser.Record.Length
                            ? csv.GetField(colorIndex)?.Trim() ?? "Grey"
                            : "Grey";
                    record["Color"] = color;

                    var shape =
                        shapeIndex >= 0 && shapeIndex < csv.Parser.Record.Length
                            ? csv.GetField(shapeIndex)?.Trim() ?? "Rectangle"
                            : "Rectangle";
                    record["Shape"] = shape;

                    // Parse number of reports
                    var numReportsStr =
                        numReportsIndex >= 0 && numReportsIndex < csv.Parser.Record.Length
                            ? csv.GetField(numReportsIndex)?.Trim().Replace("?", "0")
                            : "0";
                    int numReports = 0;
                    if (
                        !string.IsNullOrEmpty(numReportsStr)
                        && int.TryParse(numReportsStr, out numReports)
                    )
                    {
                        record["NumReports"] = numReports;
                    }
                    else
                    {
                        record["NumReports"] = 0;
                    }

                    // Track unique modification types
                    if (!allModificationTypes.Contains(modType))
                    {
                        allModificationTypes.Add(modType);
                        modificationFrequency[modType] = 0;
                    }
                    modificationFrequency[modType]++;

                    // Track unique proteins
                    if (!allSites.ContainsKey(uniprotId))
                    {
                        allSites[uniprotId] = new List<string>();
                    }
                    allSites[uniprotId].Add(site);

                    // Process product information
                    bool foundProduct = false;
                    int prodCount = 1;
                    foreach (var productIndex in productIndices)
                    {
                        if (productIndex < csv.Parser?.Record?.Length)
                        {
                            var product = csv.GetField(productIndex)?.Trim();
                            var link = csv.GetField(productIndex + 1)?.Trim();

                            if (!string.IsNullOrWhiteSpace(product))
                            {
                                record[$"Kinexus Psite Ab Product {prodCount}"] = product;
                                record[$"Kinexus Psite Ab Link {prodCount}"] = link;

                                if (!allProducts.Contains(product))
                                {
                                    allProducts.Add(product);
                                }

                                foundProduct = true;

                                if (!productFrequency.ContainsKey(product))
                                {
                                    productFrequency[product] = 0;
                                }
                                productFrequency[product]++;
                            }
                        }
                        prodCount++;
                    }

                    if (!foundProduct)
                    {
                        var defaultProductName = "DEFAULT_PRODUCT";
                        record["Product_default"] = defaultProductName;

                        if (!allProducts.Contains(defaultProductName))
                        {
                            allProducts.Add(defaultProductName);
                        }

                        if (!productFrequency.ContainsKey(defaultProductName))
                        {
                            productFrequency[defaultProductName] = 0;
                        }
                        productFrequency[defaultProductName]++;
                    }

                    // Determine protein level based on mediator/receptor flags
                    string levelId = DetermineProteinLevelIdFromCsv(
                        csv,
                        mediatorIndex,
                        receptorIndex
                    );
                    record["ProteinLevelId"] = levelId;

                    if (!proteinLevelFrequency.ContainsKey(levelId))
                    {
                        proteinLevelFrequency[levelId] = 0;
                    }
                    proteinLevelFrequency[levelId]++;

                    // Assign IDs for database
                    record["ModificationId"] = modType; // Store type as key, we'll replace with actual ID later
                    record["ProteinModificationId"] = $"{proteinModIdCounter++}";
                    record["ModificationDescription"] = modificationDescription ?? "";

                    allRecords.Add(record);
                    rowsWithValidData++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during first pass: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine($"First pass complete. Found {rowsWithValidData} valid records.");
            Console.WriteLine($"Unique values found:");
            Console.WriteLine($"  - Modification types: {allModificationTypes.Count}");
            Console.WriteLine($"  - Unique proteins: {allSites.Count}");
            Console.WriteLine($"  - Products: {allProducts.Count}");

            Console.WriteLine("\nSecond pass: Adding reference data...");

            // Cache for modification IDs by type
            var modificationIds = new Dictionary<string, string>();

            Console.WriteLine("Adding modifications...");
            int modCounter = 1;
            foreach (var modType in allModificationTypes)
            {
                // Use simple numeric IDs
                var modId = modCounter.ToString();
                modCounter++;

                if (!existingModificationIds.Contains(modId))
                {
                    try
                    {
                        var effectCode = GetDefaultEffectCode(modType);
                        var description = GetDefaultDescription(modType);

                        // Find records with this modification type to get color and shape
                        var color = "Grey";
                        var shape = "Rectangle";

                        // Use the first record with this modification type for color and shape
                        var modRecord = allRecords.FirstOrDefault(r =>
                            r["ModificationType"].ToString() == modType
                            && r.ContainsKey("Color")
                            && r.ContainsKey("Shape")
                        );

                        if (modRecord != null)
                        {
                            color = modRecord["Color"].ToString();
                            shape = modRecord["Shape"].ToString();
                        }

                        // For phosphorylation, default to oval shape if shape is missing
                        if (
                            modType.Contains("Phosphorylation", StringComparison.OrdinalIgnoreCase)
                            && shape == "Rectangle"
                        )
                        {
                            shape = "Oval";
                        }

                        var modification = new Modification
                        {
                            Id = modId,
                            Type = modType,
                            EffectCode = effectCode,
                            Description = description,
                            Color = color,
                            Shape = shape,
                        };

                        context.Modifications.Add(modification);
                        modificationsAdded++;
                        existingModificationIds.Add(modId);
                        modificationIds[modType] = modId;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error adding modification {modId}: {ex.Message}");
                        context.ChangeTracker.Clear();
                    }
                }
            }

            if (modificationsAdded > 0)
            {
                context.SaveChanges();
                context.ChangeTracker.Clear();
            }

            Console.WriteLine("Third pass: Adding protein modifications...");

            int pmCounter = 1;
            foreach (var record in allRecords)
            {
                modsAttempted++;

                try
                {
                    // Use incrementing numeric ID
                    var id = pmCounter.ToString();
                    pmCounter++;

                    var uniprotId = record["UniprotId"].ToString();
                    var site = record["Site"].ToString();
                    var description = record["ModificationDescription"].ToString();
                    var modificationDescription = record["Description"].ToString();
                    var modType = record["ModificationType"].ToString();
                    var numReports = Convert.ToInt32(record["NumReports"]);
                    var proteinLevelId = record["ProteinLevelId"].ToString();

                    if (existingModIds.Contains(id))
                    {
                        continue;
                    }

                    // Look up modificationId
                    string modificationId;
                    if (modificationIds.ContainsKey(modType))
                    {
                        modificationId = modificationIds[modType];
                    }
                    else
                    {
                        problemRows[$"Record {id}"] =
                            $"Missing modification ID for type: {modType}";
                        continue;
                    }

                    if (!existingLevelIds.Contains(proteinLevelId))
                    {
                        problemRows[$"Record {id}"] = $"Missing protein level ID: {proteinLevelId}";
                        continue;
                    }

                    var proteinMod = new ProteinModification
                    {
                        Id = id,
                        UniprotId = uniprotId,
                        Site = site,
                        Description = description,
                        NumReports = numReports,
                        ModificationId = modificationId,
                        ProteinLevelId = proteinLevelId,
                        ModificationDescription = modificationDescription,
                    };

                    context.ProteinModifications.Add(proteinMod);
                    context.SaveChanges();

                    modsAdded++;
                    existingModIds.Add(id);

                    if (modsAdded % 25 == 0)
                    {
                        context.SaveChanges();
                        context.ChangeTracker.Clear();
                        Console.WriteLine($"Added {modsAdded} modifications so far...");
                    }

                    // Add Products
                    try
                    {
                        var products = new List<PSiteAbProduct>();

                        for (int i = 1; i <= 4; i++)
                        {
                            string nameKey = $"Kinexus Psite Ab Product {i}";
                            string linkKey = $"Kinexus Psite Ab Link {i}";

                            if (
                                record.TryGetValue(nameKey, out object nameValue)
                                && record.TryGetValue(linkKey, out object linkValue)
                            )
                            {
                                string name = nameValue?.ToString() ?? "";
                                string link = linkValue?.ToString() ?? "";

                                if (!string.IsNullOrWhiteSpace(name))
                                {
                                    products.Add(
                                        new PSiteAbProduct
                                        {
                                            ProductId = Guid.NewGuid().ToString(),
                                            ProteinUniprotId = uniprotId,
                                            ProteinModificationId = proteinMod.Id,
                                            Name = name,
                                            Link = link,
                                        }
                                    );
                                }
                            }
                        }
                        foreach (PSiteAbProduct product in products)
                        {
                            Console.WriteLine(product.ProteinUniprotId);
                            try
                            {
                                context.PSiteAbProducts.Add(product);
                                context.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Error adding product: {product.Link}");
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error adding products");
                        Console.WriteLine(e);
                    }
                }
                catch (Exception ex)
                {
                    var idDisplay = record.ContainsKey("Id") ? record["Id"].ToString() : "Unknown";
                    problemRows[$"Record {idDisplay}"] = $"Error: {ex.Message}";
                    context.ChangeTracker.Clear();
                }
            }

            if (modsAdded % 25 != 0)
            {
                context.SaveChanges();
                context.ChangeTracker.Clear();
            }

            int totalModsInDb = context.ProteinModifications.Count();
            int totalProductsInDb = context.PSiteAbProducts.Count();
            int totalModificationsInDb = context.Modifications.Count();
            int totalLevelsInDb = context.ProteinLevels.Count();

            Console.WriteLine("\n========== PROTEIN MODIFICATION SEEDING SUMMARY ==========");
            Console.WriteLine($"Total CSV rows processed: {totalRowsProcessed}");
            Console.WriteLine($"Rows with valid data: {rowsWithValidData}");
            Console.WriteLine($"Protein modifications attempted: {modsAttempted}");
            Console.WriteLine($"Protein modifications added: {modsAdded}");
            Console.WriteLine($"Reference data - Expected vs. Added:");
            Console.WriteLine(
                $"  - Products: {productFrequency.Count} expected, {productsAdded} added, {totalProductsInDb} total in DB"
            );
            Console.WriteLine(
                $"  - Modifications: {modificationFrequency.Count} expected, {modificationsAdded} added, {totalModificationsInDb} total in DB"
            );
            Console.WriteLine($"Current database counts:");
            Console.WriteLine($"  - Total protein modifications: {totalModsInDb}");
            Console.WriteLine($"  - Total products: {totalProductsInDb}");
            Console.WriteLine($"  - Total modifications: {totalModificationsInDb}");
            Console.WriteLine($"  - Total protein levels: {totalLevelsInDb}");

            if (productFrequency.Count > totalProductsInDb)
            {
                Console.WriteLine(
                    "\nWARNING: Some expected products were not added to the database!"
                );
                Console.WriteLine(
                    "This could be due to normalization issues or database constraints."
                );
            }

            if (modificationFrequency.Count > totalModificationsInDb)
            {
                Console.WriteLine(
                    "\nWARNING: Some expected modifications were not added to the database!"
                );
            }

            if (problemRows.Any())
            {
                Console.WriteLine($"\nEncountered {problemRows.Count} problem rows:");
                foreach (var problem in problemRows.Take(20))
                {
                    Console.WriteLine($"  - {problem.Key}: {problem.Value}");
                }
                if (problemRows.Count > 20)
                {
                    Console.WriteLine($"  - ... and {problemRows.Count - 20} more issues");
                }
            }

            Console.WriteLine("=======================================================");
        }

        // This function will determine which protein level ID to use based on the CSV data
        private static string DetermineProteinLevelIdFromCsv(
            CsvReader csv,
            int mediatorIndex,
            int receptorIndex
        )
        {
            // Default to Interactor if we can't determine
            string levelId = "level4";

            // Extract the T/F values from the CSV
            bool isMediator = false;
            bool isReceptor = false;

            if (mediatorIndex >= 0 && mediatorIndex < csv.Parser.Record.Length)
            {
                var mediatorValue = csv.GetField(mediatorIndex)?.Trim();
                isMediator =
                    !string.IsNullOrEmpty(mediatorValue)
                    && mediatorValue.Equals("T", StringComparison.OrdinalIgnoreCase);
            }

            if (receptorIndex >= 0 && receptorIndex < csv.Parser.Record.Length)
            {
                var receptorValue = csv.GetField(receptorIndex)?.Trim();
                isReceptor =
                    !string.IsNullOrEmpty(receptorValue)
                    && receptorValue.Equals("T", StringComparison.OrdinalIgnoreCase);
            }

            // Determine the protein level based on the flags
            if (isMediator)
            {
                levelId = "level1"; // Mediator
            }
            else if (isReceptor)
            {
                levelId = "level2"; // Receptor
            }

            return levelId;
        }

        private static void SeedProteinLevels(ProteinModificationDbContext context)
        {
            // Check if protein levels already exist
            if (context.ProteinLevels.Any())
            {
                Console.WriteLine("Protein levels already exist, skipping seeding");
                return;
            }

            var levels = new List<CytoNET.Data.ProteinModification.ProteinLevel>
            {
                new CytoNET.Data.ProteinModification.ProteinLevel
                {
                    ProteinLevelId = "level1",
                    Category = "Mediator",
                },
                new CytoNET.Data.ProteinModification.ProteinLevel
                {
                    ProteinLevelId = "level2",
                    Category = "Receptor",
                },
                new CytoNET.Data.ProteinModification.ProteinLevel
                {
                    ProteinLevelId = "level3",
                    Category = "Receptor Subunit",
                },
                new CytoNET.Data.ProteinModification.ProteinLevel
                {
                    ProteinLevelId = "level4",
                    Category = "Interactor",
                },
            };

            try
            {
                context.ProteinLevels.AddRange(levels);
                context.SaveChanges();
                Console.WriteLine("Protein levels seeded successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding protein levels: {ex.Message}");
            }
        }

        private static string GetDefaultEffectCode(string modificationType)
        {
            if (modificationType.Contains("Phosphorylation", StringComparison.OrdinalIgnoreCase))
                return "PHOS";
            else if (modificationType.Contains("Acetylation", StringComparison.OrdinalIgnoreCase))
                return "ACET";
            else if (modificationType.Contains("Methylation", StringComparison.OrdinalIgnoreCase))
                return "METH";
            else if (
                modificationType.Contains("Ubiquitination", StringComparison.OrdinalIgnoreCase)
            )
                return "UBIQ";
            else if (modificationType.Contains("Glycosylation", StringComparison.OrdinalIgnoreCase))
                return "GLYC";
            else if (modificationType.Contains("Sumoylation", StringComparison.OrdinalIgnoreCase))
                return "SUMO";
            else
                return "MOD";
        }

        private static string GetDefaultDescription(string modificationType)
        {
            if (modificationType.Contains("Phosphorylation", StringComparison.OrdinalIgnoreCase))
                return "Addition of a phosphate group";
            else if (modificationType.Contains("Acetylation", StringComparison.OrdinalIgnoreCase))
                return "Addition of an acetyl group";
            else if (modificationType.Contains("Methylation", StringComparison.OrdinalIgnoreCase))
                return "Addition of a methyl group";
            else if (
                modificationType.Contains("Ubiquitination", StringComparison.OrdinalIgnoreCase)
            )
                return "Addition of ubiquitin";
            else if (modificationType.Contains("Glycosylation", StringComparison.OrdinalIgnoreCase))
                return "Addition of a carbohydrate";
            else if (modificationType.Contains("Sumoylation", StringComparison.OrdinalIgnoreCase))
                return "Addition of a SUMO protein";
            else
                return "Protein modification";
        }

        public static void SeedProteinInteractionData(ProteinInteractionDbContext context)
        {
            Console.WriteLine("Starting protein interaction database seed with direct parsing...");

            try
            {
                string projectDirectory = Path.GetFullPath(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../")
                );
                string filePath = Path.Combine(projectDirectory, "SeedData", "TableE.csv");
                Console.WriteLine($"Attempting to load from: {filePath}");

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"File not found: {filePath}");
                    return;
                }

                // First ensure protein levels exist
                SeedProteinLevels(context);

                var problemRows = new Dictionary<string, string>();
                var successfulInteractionIds = new HashSet<string>();

                Console.WriteLine("Processing file to add protein interactions...");

                int totalRowsProcessed = 0;
                int rowsWithValidData = 0;
                int interactionsAttempted = 0;
                int interactionsAdded = 0;
                int smallMoleculeCount = 0;

                try
                {
                    context.DisableForeignKeyConstraints();

                    using (var reader = new StreamReader(filePath))
                    {
                        // Skip header rows (there are two header rows in this file)
                        for (int i = 0; i < 2 && !reader.EndOfStream; i++)
                        {
                            string headerLine = reader.ReadLine();
                            Console.WriteLine(
                                $"Skipped header row {i + 1}: {headerLine.Substring(0, Math.Min(50, headerLine.Length))}..."
                            );
                        }

                        int lineNumber = 2; // Start counting after headers
                        int batchSize = 50;
                        var proteinBatch = new List<Cytonet.Data.ProteinInteraction.Protein>();
                        var interactionBatch = new List<ProteinInteraction>();
                        var existingProteins = new HashSet<string>(
                            context.Proteins.Select(p => p.UniprotIdCasNumber)
                        );

                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            lineNumber++;
                            totalRowsProcessed++;

                            if (totalRowsProcessed % 100 == 0)
                            {
                                Console.WriteLine($"Processed {totalRowsProcessed} rows so far");
                            }

                            if (string.IsNullOrWhiteSpace(line))
                            {
                                continue;
                            }

                            try
                            {
                                // Split the CSV line properly
                                string[] fields = ParseCsvRow(line);

                                // Debug output for first few rows
                                if (lineNumber <= 5)
                                {
                                    Console.WriteLine(
                                        $"Row {lineNumber} has {fields.Length} fields:"
                                    );
                                    for (int i = 0; i < Math.Min(fields.Length, 16); i++)
                                    {
                                        Console.WriteLine($"  Field {i}: '{fields[i]}'");
                                    }
                                }

                                // Extract all required fields
                                string uniprotId = fields.Length > 0 ? fields[0] : "";
                                string casNumber = fields.Length > 1 ? fields[1] : "";

                                // Parse T/F fields for protein levels
                                bool isLevel1 =
                                    fields.Length > 2
                                    && fields[2].Equals("T", StringComparison.OrdinalIgnoreCase);
                                bool isLevel2 =
                                    fields.Length > 3
                                    && fields[3].Equals("T", StringComparison.OrdinalIgnoreCase);
                                bool isLevel3 =
                                    fields.Length > 4
                                    && fields[4].Equals("T", StringComparison.OrdinalIgnoreCase);
                                bool isLevel4 =
                                    fields.Length > 5
                                    && fields[5].Equals("T", StringComparison.OrdinalIgnoreCase);

                                string associatingProteinId = fields.Length > 6 ? fields[6] : "";
                                string associatingPhosphositeId =
                                    fields.Length > 7 ? fields[7] : "";
                                string associatingProteinType = fields.Length > 8 ? fields[8] : "";
                                string effectOfInteraction = fields.Length > 9 ? fields[9] : "";
                                string initiatingProteinIconType =
                                    fields.Length > 11 ? fields[11] : "";
                                string associatingProteinIconType =
                                    fields.Length > 12 ? fields[12] : "";
                                string interactionEdgeType = fields.Length > 13 ? fields[13] : "";
                                string initiatingProteinShortName =
                                    fields.Length > 14 ? fields[14] : "";
                                string associatingProteinShortName =
                                    fields.Length > 15 ? fields[15] : "";

                                // CRITICAL: Determine the initiating protein ID - either Uniprot or CAS number
                                string initiatingProteinId;
                                if (!string.IsNullOrWhiteSpace(uniprotId))
                                {
                                    initiatingProteinId = uniprotId;
                                    if (lineNumber <= 5)
                                        Console.WriteLine(
                                            $"Row {lineNumber}: Using Uniprot ID: {initiatingProteinId}"
                                        );
                                }
                                else if (!string.IsNullOrWhiteSpace(casNumber))
                                {
                                    initiatingProteinId = casNumber;
                                    smallMoleculeCount++;
                                    if (lineNumber <= 5)
                                        Console.WriteLine(
                                            $"Row {lineNumber}: Using CAS number: {initiatingProteinId}"
                                        );
                                }
                                else
                                {
                                    // Skip row if neither is available
                                    problemRows[$"Row {lineNumber}"] =
                                        "No valid initiating protein ID";
                                    continue;
                                }

                                // Validate associating protein ID
                                if (
                                    string.IsNullOrWhiteSpace(associatingProteinId)
                                    || associatingProteinId == "0"
                                    || associatingProteinId == "+"
                                    || associatingProteinId == "-"
                                )
                                {
                                    problemRows[$"Row {lineNumber}"] =
                                        $"Invalid associating protein ID: '{associatingProteinId}'";
                                    continue;
                                }

                                // CRITICAL: Determine protein types
                                string initiatingProteinType;
                                if (!string.IsNullOrWhiteSpace(uniprotId))
                                {
                                    // For proteins with Uniprot IDs
                                    initiatingProteinType = "Protein";
                                }
                                else
                                {
                                    // For small molecules
                                    initiatingProteinType = "Small Molecule Mediator";
                                }

                                // Use defaults for missing values
                                if (string.IsNullOrWhiteSpace(associatingProteinType))
                                {
                                    associatingProteinType = "Protein";
                                }

                                // CRITICAL: Use the actual short name from the CSV
                                if (string.IsNullOrWhiteSpace(initiatingProteinShortName))
                                {
                                    initiatingProteinShortName = initiatingProteinId;
                                }

                                if (string.IsNullOrWhiteSpace(associatingProteinShortName))
                                {
                                    associatingProteinShortName = associatingProteinId;
                                }

                                // Set protein level
                                string initiatingProteinLevelId = DetermineProteinLevel(
                                    isLevel1,
                                    isLevel2,
                                    isLevel3,
                                    isLevel4
                                );

                                // Debug output
                                if (lineNumber <= 5)
                                {
                                    Console.WriteLine($"Row {lineNumber} final values:");
                                    Console.WriteLine(
                                        $"  Initiating: id='{initiatingProteinId}', type='{initiatingProteinType}', shortName='{initiatingProteinShortName}'"
                                    );
                                    Console.WriteLine(
                                        $"  Associating: id='{associatingProteinId}', type='{associatingProteinType}', shortName='{associatingProteinShortName}'"
                                    );
                                }

                                // Create initiating protein
                                if (!existingProteins.Contains(initiatingProteinId))
                                {
                                    var initiatingProtein =
                                        new Cytonet.Data.ProteinInteraction.Protein
                                        {
                                            UniprotIdCasNumber = initiatingProteinId,
                                            ShortName = initiatingProteinShortName,
                                            ProteinType = initiatingProteinType,
                                            ProteinLevelId = initiatingProteinLevelId,
                                        };
                                    proteinBatch.Add(initiatingProtein);
                                    existingProteins.Add(initiatingProteinId);
                                }

                                // Create associating protein
                                if (!existingProteins.Contains(associatingProteinId))
                                {
                                    var associatingProtein =
                                        new Cytonet.Data.ProteinInteraction.Protein
                                        {
                                            UniprotIdCasNumber = associatingProteinId,
                                            ShortName = associatingProteinShortName,
                                            ProteinType = associatingProteinType,
                                            ProteinLevelId = "level4", // Default level for associating proteins
                                        };
                                    proteinBatch.Add(associatingProtein);
                                    existingProteins.Add(associatingProteinId);
                                }

                                // Set defaults for empty icon types
                                if (string.IsNullOrWhiteSpace(initiatingProteinIconType))
                                {
                                    initiatingProteinIconType = initiatingProteinType;
                                }

                                if (string.IsNullOrWhiteSpace(associatingProteinIconType))
                                {
                                    associatingProteinIconType = associatingProteinType;
                                }

                                // Set default for empty interaction edge
                                if (string.IsNullOrWhiteSpace(interactionEdgeType))
                                {
                                    interactionEdgeType = "+";
                                }

                                // Set default for empty effect
                                if (string.IsNullOrWhiteSpace(effectOfInteraction))
                                {
                                    effectOfInteraction = "Activates receptor";
                                }

                                // Create protein interaction
                                string interactionId = Guid.NewGuid()
                                    .ToString()
                                    .Substring(0, 16)
                                    .Replace("-", "");
                                var proteinInteraction = new ProteinInteraction
                                {
                                    ProteinInteractionId = interactionId,
                                    InitiatingProteinId = initiatingProteinId,
                                    AssociatingProteinId = associatingProteinId,
                                    InitiatingProteinIcon = initiatingProteinIconType,
                                    AssociatingProteinIcon = associatingProteinIconType,
                                    EffectOfInteraction = effectOfInteraction,
                                    InteractionEdge = interactionEdgeType,
                                };

                                interactionBatch.Add(proteinInteraction);
                                interactionsAttempted++;
                                rowsWithValidData++;

                                // Save proteins in batches
                                if (proteinBatch.Count >= batchSize)
                                {
                                    try
                                    {
                                        context.Proteins.AddRange(proteinBatch);
                                        context.SaveChanges();
                                        Console.WriteLine(
                                            $"Saved batch of {proteinBatch.Count} proteins"
                                        );
                                        proteinBatch.Clear();
                                        context.ChangeTracker.Clear();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(
                                            $"Error saving protein batch: {ex.Message}"
                                        );
                                        proteinBatch.Clear();
                                        context.ChangeTracker.Clear();
                                    }
                                }

                                // Save interactions in batches
                                if (interactionBatch.Count >= batchSize)
                                {
                                    try
                                    {
                                        context.ProteinInteractions.AddRange(interactionBatch);
                                        context.SaveChanges();
                                        Console.WriteLine(
                                            $"Saved batch of {interactionBatch.Count} interactions"
                                        );
                                        interactionsAdded += interactionBatch.Count;
                                        interactionBatch.Clear();
                                        context.ChangeTracker.Clear();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(
                                            $"Error saving interaction batch: {ex.Message}"
                                        );
                                        interactionBatch.Clear();
                                        context.ChangeTracker.Clear();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(
                                    $"Error processing row {lineNumber}: {ex.Message}"
                                );
                                problemRows[$"Row {lineNumber}"] =
                                    $"Error processing: {ex.Message}";
                            }
                        }

                        // Save any remaining items
                        if (proteinBatch.Any())
                        {
                            try
                            {
                                context.Proteins.AddRange(proteinBatch);
                                context.SaveChanges();
                                Console.WriteLine(
                                    $"Saved final batch of {proteinBatch.Count} proteins"
                                );
                                proteinBatch.Clear();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(
                                    $"Error saving final protein batch: {ex.Message}"
                                );
                                proteinBatch.Clear();
                            }
                        }

                        if (interactionBatch.Any())
                        {
                            try
                            {
                                context.ProteinInteractions.AddRange(interactionBatch);
                                context.SaveChanges();
                                Console.WriteLine(
                                    $"Saved final batch of {interactionBatch.Count} interactions"
                                );
                                interactionsAdded += interactionBatch.Count;
                                interactionBatch.Clear();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(
                                    $"Error saving final interaction batch: {ex.Message}"
                                );
                                interactionBatch.Clear();
                            }
                        }
                    }

                    context.EnableForeignKeyConstraints();

                    Console.WriteLine("\n========== SEEDING SUMMARY ==========");
                    Console.WriteLine($"Total CSV rows processed: {totalRowsProcessed}");
                    Console.WriteLine($"Rows with valid data: {rowsWithValidData}");
                    Console.WriteLine($"Small molecules identified: {smallMoleculeCount}");
                    Console.WriteLine($"Interactions attempted: {interactionsAttempted}");
                    Console.WriteLine($"Interactions successfully added: {interactionsAdded}");

                    try
                    {
                        int totalProteins = context.Proteins.Count();
                        int totalInteractions = context.ProteinInteractions.Count();

                        Console.WriteLine($"Final database counts:");
                        Console.WriteLine($"  - Total proteins: {totalProteins}");
                        Console.WriteLine($"  - Total interactions: {totalInteractions}");

                        // Print sample proteins to verify
                        var uniprotProteins = context
                            .Proteins.Where(p => p.ProteinType == "Protein")
                            .Take(5)
                            .ToList();
                        var smallMolecules = context
                            .Proteins.Where(p => p.ProteinType == "Small Molecule Mediator")
                            .Take(5)
                            .ToList();

                        Console.WriteLine("\nSample Uniprot proteins:");
                        foreach (var protein in uniprotProteins)
                        {
                            Console.WriteLine(
                                $"  {protein.UniprotIdCasNumber} - {protein.ShortName} - {protein.ProteinType}"
                            );
                        }

                        Console.WriteLine("\nSample small molecules:");
                        foreach (var molecule in smallMolecules)
                        {
                            Console.WriteLine(
                                $"  {molecule.UniprotIdCasNumber} - {molecule.ShortName} - {molecule.ProteinType}"
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error getting final counts: {ex.Message}");
                    }

                    if (problemRows.Any())
                    {
                        Console.WriteLine($"\nEncountered {problemRows.Count} problem rows:");
                        foreach (var problem in problemRows.Take(20))
                        {
                            Console.WriteLine($"  - {problem.Key}: {problem.Value}");
                        }
                        if (problemRows.Count > 20)
                        {
                            Console.WriteLine($"  - ... and {problemRows.Count - 20} more issues");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during processing: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    try
                    {
                        context.EnableForeignKeyConstraints();
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception in seed method: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("Seed method completed");
        }

        // Improved CSV parsing function that handles empty fields and whitespace better
        private static string[] ParseCsvRow(string line)
        {
            if (string.IsNullOrEmpty(line))
                return new string[0];

            List<string> fields = new List<string>();
            bool inQuote = false;
            StringBuilder currentField = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuote = !inQuote;
                }
                else if (c == ',' && !inQuote)
                {
                    // End of field
                    fields.Add(currentField.ToString().Trim());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }

            // Add the last field
            fields.Add(currentField.ToString().Trim());

            return fields.ToArray();
        }

        // Helper method to determine protein level ID based on T/F flags in the CSV
        private static string DetermineProteinLevel(
            bool isLevel1,
            bool isLevel2,
            bool isLevel3,
            bool isLevel4
        )
        {
            if (isLevel1)
                return "level1";
            if (isLevel2)
                return "level2";
            if (isLevel3)
                return "level3";
            if (isLevel4)
                return "level4";
            return "level4"; // Default to level4 if no level is specified
        }

        // Helper method to seed protein levels
        private static void SeedProteinLevels(ProteinInteractionDbContext context)
        {
            // Check if protein levels already exist
            if (context.ProteinLevels.Any())
            {
                return;
            }

            var levels = new List<Cytonet.Data.ProteinInteraction.ProteinLevel>
            {
                new Cytonet.Data.ProteinInteraction.ProteinLevel
                {
                    ProteinLevelId = "level1",
                    Category = "Mediator",
                },
                new Cytonet.Data.ProteinInteraction.ProteinLevel
                {
                    ProteinLevelId = "level2",
                    Category = "Receptor",
                },
                new Cytonet.Data.ProteinInteraction.ProteinLevel
                {
                    ProteinLevelId = "level3",
                    Category = "Receptor Subunit",
                },
                new Cytonet.Data.ProteinInteraction.ProteinLevel
                {
                    ProteinLevelId = "level4",
                    Category = "Interactor",
                },
            };

            try
            {
                context.ProteinLevels.AddRange(levels);
                context.SaveChanges();
                Console.WriteLine("Protein levels seeded successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding protein levels: {ex.Message}");
            }
        }

        // ... other methods ...

        /// <summary>
        /// Seed TissueDistribution data from CSV
        /// </summary>
        public static void SeedTissueDistributionData(TissueDistributionDbContext context)
        {
            Console.WriteLine("Starting tissue distribution database seed...");

            string projectDirectory = Path.GetFullPath(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../")
            );
            string filePath = Path.Combine(projectDirectory, "SeedData", "TableC.csv");
            Console.WriteLine($"Attempting to load from: {filePath}");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                TrimOptions = TrimOptions.Trim,
                Delimiter = ",",
                Quote = '"',
                IgnoreBlankLines = true,
                MissingFieldFound = null,
                HeaderValidated = null,
            };

            var tissueDistributions =
                new List<CytoNET.Data.TissueDistribution.TissueDistribution>();
            var tissueOrgans =
                new Dictionary<string, CytoNET.Data.TissueDistribution.TissueOrgan>();
            var tissueDistributionTissueOrgans =
                new List<CytoNET.Data.TissueDistribution.TissueDistributionTissueOrgan>();

            // Track existing UniprotIds to avoid duplicates
            var existingUniprotIds = new HashSet<string>(
                context.TissueDistributions.AsNoTracking().Select(td => td.UniprotId)
            );

            // Track composite keys to prevent duplicates
            var existingTdToRelations = new HashSet<string>();

            SeedProteinLevels(context);

            using (var reader = new StreamReader(filePath))
            {
                reader.ReadLine(); // Skip header

                string? line;
                int lineNumber = 2;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    var fields = line.Split(',');
                    int numberOfFields = fields.Length;

                    string uniprotIdField = fields[0].Trim();

                    List<string> uniprotIds;
                    if (uniprotIdField.StartsWith('"') && uniprotIdField.EndsWith('"'))
                    {
                        // Remove quotes and split
                        uniprotIds = uniprotIdField
                            .Trim('"')
                            .Split(',')
                            .Select(id => id.Trim())
                            .ToList();
                    }
                    else
                    {
                        // Single UniprotId
                        uniprotIds = new List<string> { uniprotIdField };
                    }

                    var casNumber = fields.Length > 1 ? fields[1].Trim() : string.Empty;

                    bool isLevel1 =
                        fields.Length > 2
                        && fields[2].Equals("T", StringComparison.OrdinalIgnoreCase);
                    bool isLevel2 =
                        fields.Length > 3
                        && fields[3].Equals("T", StringComparison.OrdinalIgnoreCase);
                    bool isLevel3 =
                        fields.Length > 4
                        && fields[4].Equals("T", StringComparison.OrdinalIgnoreCase);
                    bool isLevel4 =
                        fields.Length > 5
                        && fields[5].Equals("T", StringComparison.OrdinalIgnoreCase);

                    // Determine which Protein Level it is
                    string proteinLevelId = DetermineProteinLevel(
                        isLevel1,
                        isLevel2,
                        isLevel3,
                        isLevel4
                    );

                    var tissueOrganNames = new List<string>();

                    // Process tissue organ columns (columns 7-26)
                    for (int i = 7; i < fields.Length && i <= 26; i++)
                    {
                        var tissueOrganName = fields[i].Trim();

                        if (!string.IsNullOrWhiteSpace(tissueOrganName))
                        {
                            // Remove any quotes and capitalize the tissue organ name
                            tissueOrganName = tissueOrganName.Replace("\"", "");
                            tissueOrganName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                                tissueOrganName.ToLower()
                            );
                            tissueOrganNames.Add(tissueOrganName);
                        }
                    }

                    if (tissueOrganNames.Count == 0)
                    {
                        continue;
                    }

                    foreach (var uniprotId in uniprotIds)
                    {
                        if (string.IsNullOrWhiteSpace(uniprotId))
                        {
                            continue;
                        }

                        if (existingUniprotIds.Contains(uniprotId))
                        {
                            continue;
                        }

                        existingUniprotIds.Add(uniprotId);

                        var tissueDistribution =
                            new CytoNET.Data.TissueDistribution.TissueDistribution
                            {
                                UniprotId = uniprotId,
                                CasNumber = casNumber,
                                ProteinLevelId = proteinLevelId,
                            };

                        tissueDistributions.Add(tissueDistribution);

                        foreach (var tissueOrganName in tissueOrganNames)
                        {
                            // Check if this tissue organ has a bold marker (*)
                            var isBold = tissueOrganName.EndsWith("*");
                            var cleanName = isBold ? tissueOrganName.TrimEnd('*') : tissueOrganName;

                            // Create or retrieve tissue organ
                            CytoNET.Data.TissueDistribution.TissueOrgan? tissueOrgan = null;
                            var organsWithSameName = tissueOrgans
                                .Where(to => to.Value.Name == cleanName)
                                .ToList();

                            // Look for existing tissue organ with the same name and bold status
                            foreach (var entry in organsWithSameName)
                            {
                                if (entry.Value.IsBold == isBold)
                                {
                                    tissueOrgan = entry.Value;
                                    break;
                                }
                            }

                            // If no matching tissue organ found, create a new one
                            if (tissueOrgan == null)
                            {
                                var id = Guid.NewGuid().ToString();
                                tissueOrgan = new CytoNET.Data.TissueDistribution.TissueOrgan
                                {
                                    Id = id,
                                    Name = cleanName,
                                    IsBold = isBold,
                                };
                                tissueOrgans.Add(id, tissueOrgan);
                            }

                            string compositeKey = $"{uniprotId}_{tissueOrgan.Id}";

                            // Skip if this relationship already exists
                            if (existingTdToRelations.Contains(compositeKey))
                            {
                                continue;
                            }

                            existingTdToRelations.Add(compositeKey);

                            tissueDistributionTissueOrgans.Add(
                                new CytoNET.Data.TissueDistribution.TissueDistributionTissueOrgan
                                {
                                    TissueDistributionId = uniprotId,
                                    TissueOrganId = tissueOrgan.Id,
                                }
                            );
                        }
                    }
                }
            }

            try
            {
                if (tissueDistributions.Any())
                {
                    context.TissueDistributions.AddRange(tissueDistributions);
                    context.SaveChanges();
                    Console.WriteLine($"Added {tissueDistributions.Count} tissue distributions");
                }

                if (tissueOrgans.Any())
                {
                    context.TissueOrgans.AddRange(tissueOrgans.Values);
                    context.SaveChanges();
                    Console.WriteLine($"Added {tissueOrgans.Count} tissue organs");
                }

                if (tissueDistributionTissueOrgans.Any())
                {
                    const int batchSize = 1000;
                    int totalAdded = 0;

                    for (int i = 0; i < tissueDistributionTissueOrgans.Count; i += batchSize)
                    {
                        try
                        {
                            var batch = tissueDistributionTissueOrgans
                                .Skip(i)
                                .Take(batchSize)
                                .ToList();
                            context.TissueDistributionTissueOrgans.AddRange(batch);
                            context.SaveChanges();
                            totalAdded += batch.Count;
                            Console.WriteLine(
                                $"Added batch of {batch.Count} tissue distribution-organ relationships"
                            );
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(
                                $"Error adding batch starting at index {i}: {ex.Message}"
                            );
                        }

                        context.ChangeTracker.Clear();
                    }

                    Console.WriteLine(
                        $"Total tissue distribution-organ relationships added: {totalAdded}"
                    );
                }

                Console.WriteLine(
                    $"Total new tissue distributions added: {tissueDistributions.Count}"
                );
                Console.WriteLine($"Total new tissue organs added: {tissueOrgans.Count}");
                Console.WriteLine(
                    $"Total new tissue distribution-organ relationships added: {tissueDistributionTissueOrgans.Count}"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding tissue distribution data: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static void SeedProteinLevels(TissueDistributionDbContext context)
        {
            if (context.ProteinLevels.Any())
            {
                return;
            }

            var levels = new List<CytoNET.Data.TissueDistribution.ProteinLevel>
            {
                new CytoNET.Data.TissueDistribution.ProteinLevel
                {
                    ProteinLevelId = "level1",
                    Category = "Mediator",
                },
                new CytoNET.Data.TissueDistribution.ProteinLevel
                {
                    ProteinLevelId = "level2",
                    Category = "Receptor",
                },
                new CytoNET.Data.TissueDistribution.ProteinLevel
                {
                    ProteinLevelId = "level3",
                    Category = "Receptor Subunit",
                },
                new CytoNET.Data.TissueDistribution.ProteinLevel
                {
                    ProteinLevelId = "level4",
                    Category = "Interactor",
                },
            };

            try
            {
                context.ProteinLevels.AddRange(levels);
                context.SaveChanges();
                Console.WriteLine("Protein levels seeded successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding protein levels: {ex.Message}");
            }
        }
    }
}
