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

export function parseProteinInteractionData(data, showStimulatory, showInhibitory, showUndefined) {
  const nodes = [];
  const links = [];
  // Center node
  const centerX = 300;
  const centerY = 200;
  var background = determineBackground(data);
  nodes.push({
    id: data.UniprotID,
    name: data.shortName ? data.shortName : "",
    x: centerX,
    y: centerY,
    isCenter: true,
    background,
    description: data.isInitiatingProtein ? "Protein" : "Small Molecule",
  });
  // Site nodes
  data.Sites.forEach((site, index) => {
    // This is so in case is the asscioating protein has a initiating protein
    if (!showStimulatory && site.InteractionEdge === "+") {
      return;
    }
    if (!showInhibitory && site.InteractionEdge === "-") {
      return;
    }
    if (!showUndefined && site.InteractionEdge == 0) {
      return;
    }
    var nodeId = data.isInitiatingProtein
      ? site.InitiatingProteinId === data.UniprotID
        ? site.AssociatingProteinId
        : site.InitiatingProteinId
      : site.AssociatingProteinId === data.UniprotID
      ? site.InitiatingProteinId
      : site.AssociatingProteinId;

    var pointTowardsMainProtein = !(site.InitiatingProteinId === nodeId);

    site.UniprotID = nodeId;

    site.iconType = data.isInitiatingProtein
      ? site.InitiatingProteinId === data.UniprotID
        ? site.AssociatingProteinIcon
        : site.InitiatingProteinIcon
      : site.AssociatingProteinId === data.UniprotID
      ? site.InitiatingProteinIcon
      : site.AssociatingProteinIcon;

    var background = determineBackground(site);
    nodes.push({
      id: nodeId,
      name: site.AssociatingProteinId === nodeId ? site.AssociatingProtein.ShortName : site.InitiatingProtein.ShortName,
      x: 0, // Will be calculated during rendering
      y: 0, // Will be calculated during rendering
      isCenter: false,
      description: site.EffectOfInteraction,
      background: background,
      isProteinInteraction: true,
    });

    // TODO: this checks if initiating protein is small mol, arrows alway point away. DOUBLE CHECK IF THIS IS CORRECT FOR ALL CASES
    if (/^\d/.test(site.InitiatingProteinId)) {
      links.push({
        source: data.UniprotID,
        target: nodeId,
        color: "Green",
        pointTowardsMainProtein,
      });
    } else {
      // Links
      if (
        !links.some(
          (link) =>
            (link.source === data.UniprotID && link.target === nodeId) ||
            (link.source === nodeId && link.target === data.UniprotID)
        )
      ) {
        links.push({
          source: data.UniprotID,
          target: nodeId,
          color: site.InteractionEdge == 0 ? "Grey" : site.InteractionEdge === "+" ? "Green" : "Red",
          pointTowardsMainProtein,
        });
      }
    }
  });
  return {nodes, links};
}

function determineBackground(node) {
  // check if small molecule (starts with number)
  if (!isNaN(node.UniprotID[0])) {
    node.iconType = "Small Molecule Extracellular Mediator";
  }
  switch (node.iconType) {
    case "Regulatory":
      return "regulatory.svg";
    case "Unclassified":
      return "unclassified.svg";
    case "Protein Extracellular Mediator":
      return "protein-extracellular-mediator.svg";
    case "Extracellular Protein Mediator":
      return "protein-extracellular-mediator.svg";
    case "Extracellular protein mediator":
      return "protein-extracellular-mediator.svg";
    case "Extracellular Protein Mediator":
      return "protein-extracellular-mediator.svg";
    case "Small Molecule Extracellular Mediator":
      return "small-molecule-extracellular-mediator.svg";
    case "Metabolic":
      return "metabolic.svg";
    case "Structural":
      return "structural.svg";
    case "Ser/Thr Kinase":
      return "ser-thr-kinase.svg";
    case "Adaptor/scaffold":
      return "scaffold.svg";
    case "Transcription":
      return "transcription.svg";
    case "Phosphatase":
      return "phosphatase.svg";
    case "Tyr Kinase":
      return "tyr-kinase.svg";
    case "":
      return "unclassified.svg";
  }
}
