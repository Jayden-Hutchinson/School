#!/bin/bash

# Array of context names
contexts=(
    "ProteinDbContext"
    "ProteinModificationDbContext"
    "ProteinInteractionDbContext"
    "SmallMoleculeDbContext"
    "TissueDistributionDbContext"
)

timestamp=$(date +%F%l%P | tr ' ' '-')

# Loop through each context and run the migrations
for context in "${contexts[@]}"
do
    echo "Processing $context..."
    
    # Create migration
    migration_name="${timestamp}${context%DbContext}Create"
    echo "Creating migration: $migration_name"
    dotnet ef migrations add "$migration_name" --context "$context"
    
    # Update database
    echo "Updating database for $context"
    dotnet ef database update --context "$context"
    
    echo "Completed $context"
    echo "-----------------------------------"
done

echo "All migrations and updates completed successfully!"