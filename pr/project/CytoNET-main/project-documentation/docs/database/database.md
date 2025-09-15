# Database and Seeding Documentation

## Database Structure Overview

The system currently uses 5 separate SQLite databases, each corresponding to a different domain area:

1. **Small Molecule Database** - Table A
2. **Protein Database** - Table B
3. **Tissue Distribution Database** - Table C
4. **Protein Modifications Database** - Table D
5. **Protein Interactions Database** - Table E

> **⚠️ Design Note:** The current implementation using 5 separate databases is a design flaw resulting from a miscommunication. This structure makes it difficult to perform inner joins between related data. Future developers should consider refactoring this into a single database. If refactoring is not chosen, developers will need to inject multiple database contexts into repositories and abstract this complexity in the view models.

## Database Seeding Process

The application includes a seeding function that allows for easy data updates through CSV files. To update the data in the databases, follow these steps:

### Step 1: Prepare CSV Files

Create or update the following CSV files with your data:

- **TableA.csv** - Small Molecule Info
- **TableB.csv** - Protein Info
- **TableC.csv** - Tissue Distribution
- **TableD.csv** - Protein Modifications
- **TableE.csv** - Protein Interactions

> **Important:** The file names must exactly match as specified above, as the seeding function relies on these specific names.

### Step 2: Place CSV Files in the Project

Drag and drop the updated CSV files into /SeedData folder and replace old files.

### Step 3: Run the Application

Restart the server using:

```bash
dotnet run
```

The seeding function will automatically detect and process the CSV files, updating the databases with the new data.

## Database Migration Commands

If you need to update the database schema, use the following Entity Framework Core migration commands:

### Option 1: Using the Shell Script (Recommended)

Execute the provided shell script to run all migrations:

```bash
chmod +x Scripts/run-migrations.sh
./Scripts/run-migrations.sh
```

### Option 2: Manual Migration Commands

You can also run the migrations manually for each database context:

#### For SmallMoleculeDbContext (Table A)

```bash
dotnet ef migrations add InitialSmallMoleculeCreate --context SmallMoleculeDbContext
dotnet ef database update --context SmallMoleculeDbContext
```

#### For ProteinDbContext (Table B)

```bash
dotnet ef migrations add InitialProteinCreate --context ProteinDbContext
dotnet ef database update --context ProteinDbContext
```

#### For TissueDistributionDbContext (Table C)

```bash
dotnet ef migrations add InitialTissueDistributionCreate --context TissueDistributionDbContext
dotnet ef database update --context TissueDistributionDbContext
```

#### For ProteinModificationDbContext (Table D)

```bash
dotnet ef migrations add InitialProteinModificationCreate --context ProteinModificationDbContext
dotnet ef database update --context ProteinModificationDbContext
```

#### For ProteinInteractionDbContext (Table E)

```bash
dotnet ef migrations add InitialProteinInteractionCreate --context ProteinInteractionDbContext
dotnet ef database update --context ProteinInteractionDbContext
```

## Working with Multiple Databases

Until a potential refactoring occurs, here are some guidelines for working with the multiple database structure:

### Repository Pattern Implementation

Repositories should abstract the complexity of working with multiple databases:

```csharp
public class ComplexRepository : IComplexRepository
{
    private readonly SmallMoleculeDbContext _smallMoleculeContext;
    private readonly ProteinDbContext _proteinContext;
    // Other contexts as needed

    public ComplexRepository(
        SmallMoleculeDbContext smallMoleculeContext,
        ProteinDbContext proteinContext,
        /* Other contexts */)
    {
        _smallMoleculeContext = smallMoleculeContext;
        _proteinContext = proteinContext;
        // Initialize other contexts
    }

    // Methods that combine data from multiple contexts
    public async Task<ComplexViewModel> GetComplexDataAsync(string identifier)
    {
        var smallMoleculeData = await _smallMoleculeContext.SmallMolecules
            .FirstOrDefaultAsync(sm => sm.CasNumber == identifier);

        var proteinData = await _proteinContext.Proteins
            .Where(p => p.RelatedIdentifier == identifier)
            .ToListAsync();

        // Combine results into view model
        return new ComplexViewModel
        {
            SmallMoleculeInfo = smallMoleculeData,
            RelatedProteins = proteinData,
            // Other properties
        };
    }
}
```

### View Model Abstraction

Create comprehensive view models that combine data from multiple sources:

```csharp
public class ComplexViewModel
{
    // Properties from SmallMoleculeDb
    public SmallMoleculeInfo SmallMoleculeInfo { get; set; }

    // Properties from ProteinDb
    public IEnumerable<ProteinInfo> RelatedProteins { get; set; }

    // Properties from TissueDistributionDb
    public IEnumerable<TissueDistribution> TissueDistributions { get; set; }

    // Properties from other databases
    // ...

    // Computed properties that require data from multiple sources
    public bool HasProteinInteractions => RelatedProteins?.Any() == true;
    // ...
}
```

## Conclusion

This documentation outlines the current database structure, seeding process, and migration commands. The multi-database design presents challenges for data relationships, but these can be addressed through careful repository and view model design until a refactoring to a single database can be implemented.
