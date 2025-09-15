// This function should and may be changed for different queries
// currently this works with a format like
/*
 * {
 *  UniprotID: "P08581",
    Sites: [
        { Site: "K962", Description: "Ubiquitination" },
        { Site: "S100", Description: "Phosphorylation" },
        { Site: "S1236", Description: "Phosphorylation" },
 * }
 */

export function parseData(data) {
  const nodes = [];
  const links = [];
  //? If we want to display all the products on the center node, we can use variable
  const products = data.Sites.filter((d) => d.Products.length > 1)
    .map((p) => p.Products)
    .flat();
  // Center node
  const centerX = 300;
  const centerY = 200;
  nodes.push({
    id: data.Protein.UniprotId,
    x: centerX,
    y: centerY,
    isCenter: true,
    name: data.Protein.ShortName,
    background: "tyr-kinase.svg",
    isProteinModification: true,
    site: data.Protein.Site,
    modificationType: data.Protein.Modification?.Type,
    modificationEffect: "TO BE DONE",
    phosphoSitePlusLink: data.Protein.PhosphoSitePlusLink,
    antibodies: data.Protein.Product?.Code,
    modificationEffect: data.Protein.Modification?.Description,
    modificationProducts: data.Protein.Products,
  });

  // Site nodes
  data.Sites.forEach((site, index) => {
    nodes.push({
      id: site.Description,
      name: "",
      x: 0, // Will be calculated during rendering
      y: 0, // Will be calculated during rendering
      isCenter: false,
      description: site.Description,
      NumReports: site.NumReports,
      ModificationId: site.ModificationId,
      color: site.Description.slice(-1) === "-" ? "Red" : site.Description.slice(-1) === "+" ? "Green" : "Grey",
      shape: site.Modification.Shape,
      site: site.Site,
      modificationType: site.Modification.Type,
      modificationEffect: "TO BE DONE",
      phosphoSitePlusLink: data.Protein?.PhosphoSitePlusLink,
      phosphoNetLink: data.Protein.PhosphoNetLink,
      antibodies: site.Product?.Code,
      modificationEffect: site.ModificationDescription,
      modificationProducts: site.Products,
    });
    // Links
    links.push({
      source: data.Protein.UniprotId,
      target: site.Description,
      color: site.Description.slice(-1) === "-" ? "Red" : site.Description.slice(-1) === "+" ? "Green" : "Grey",
      isProteinModification: true,
    });
  });
  return {nodes, links};
}
