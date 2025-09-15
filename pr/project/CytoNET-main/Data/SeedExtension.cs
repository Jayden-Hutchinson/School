// using Microsoft.AspNetCore.Builder;
// using Microsoft.Extensions.DependencyInjection;
// using CytoNET.Data.SmallMolecule;
// using CytoNET.Data.Protein;

// namespace CytoNET.Data.Extensions
// {
//     public static class ApplicationBuilderExtensions
//     {
//         public static IApplicationBuilder SeedAllDatabases(this IApplicationBuilder app)
//         {
//             using (var scope = app.ApplicationServices.CreateScope())
//             {
//                 // Seed SmallMolecule database
//                 var smallMoleculeContext = scope.ServiceProvider.GetRequiredService<SmallMoleculeDbContext>();
//                 DatabaseSeeder.SeedSmallMoleculeData(smallMoleculeContext);
//                 // DatabaseSeeder.SeedProteaseData(smallMoleculeContext);

//                 // Seed Protein database
//                 // var proteinContext = scope.ServiceProvider.GetRequiredService<ProteinDbContext>();
//                 // DatabaseSeeder.SeedProteinData(proteinContext);
//             }

//             return app;
//         }
//     }
// }
