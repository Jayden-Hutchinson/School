export function drawMap(
  svg,
  container,
  links,
  nodes,
  currentPage = 0,
  itemsPerPage = 10,
  showStimulatory,
  showInhibitory,
  showUndefined,
  showUniProtId
) {
  if (typeof showStimulatory === undefined) showStimulatory = true;
  if (typeof showInhibitory === undefined) showInhibitory = true;
  if (typeof showUndefined === undefined) showUndefined = true;
  if (typeof showUniProtId === undefined) showUniProtId = true;
  svg.selectAll("*").remove();
  const linesGroup = svg.append("g").attr("class", "lines-group");
  const nodesGroup = svg.append("g").attr("class", "nodes-group");
  const labelsGroup = svg.append("g").attr("class", "labels-group");

  const centerX = 300;
  const centerY = 200;
  const radius = 150;

  const centerNode = nodes.find((node) => node.isCenter);
  const siteNodes = nodes.filter((node) => !node.isCenter);
  const totalPages = Math.ceil(siteNodes.length / itemsPerPage);

  // Get current nodes
  const startIdx = currentPage * itemsPerPage;
  const endIdx = Math.min(startIdx + itemsPerPage, siteNodes.length);
  const currentPageNodes = siteNodes.slice(startIdx, endIdx);

  document.getElementById("page-info").textContent = `Page ${currentPage + 1} of ${totalPages}`;

  document.getElementById("prev-button").disabled = currentPage <= 0;
  document.getElementById("next-button").disabled = currentPage >= totalPages - 1;

  let visibleNodes = [centerNode, ...currentPageNodes];
  // Place site nodes
  currentPageNodes.forEach((node, index) => {
    const angle = (index / currentPageNodes.length) * 2 * Math.PI;
    node.x = centerX + radius * Math.cos(angle);
    node.y = centerY + radius * Math.sin(angle);
  });
  if (!showInhibitory) {
    visibleNodes = visibleNodes.filter((node) => node.color !== "Red");
  }
  if (!showStimulatory) {
    visibleNodes = visibleNodes.filter((node) => node.color !== "Green");
  }
  if (!showUndefined) {
    visibleNodes = visibleNodes.filter((node) => node.color !== "Grey");
  }

  // Filter links to only show connections to visible nodes
  let visibleLinks = links.filter(
    (link) => visibleNodes.some((n) => n.id === link.source) && visibleNodes.some((n) => n.id === link.target)
  );
  if (!showUndefined) {
    visibleLinks = visibleLinks.filter((link) => link.color !== "Grey");
  }
  // Calculate the direction of each link - inward or outward from center
  visibleLinks.forEach((link) => {
    const sourceNode = visibleNodes.find((n) => n.id === link.source);
    const targetNode = visibleNodes.find((n) => n.id === link.target);
    // Adjust path for arrow rendering
    link.adjustedCoords = {
      x1: sourceNode.x,
      y1: sourceNode.y,
      x2: targetNode.x,
      y2: targetNode.y,
    };
  });

  // arrow pointAwayFromMainProtein Red
  if (showInhibitory) {
    svg
      .append("defs")
      .selectAll("marker")
      .data(["awayArrowRed"])
      .enter()
      .append("marker")
      .attr("id", (d) => d)
      .attr("viewBox", "0 -5 10 10")
      .attr("refX", 90)
      .attr("refY", 0)
      .attr("markerWidth", 6)
      .attr("markerHeight", 6)
      .attr("orient", "auto")
      .append("path")
      .attr("fill", "#ff0000")
      .attr("d", "M10,-5L0,0L10,5");
  }

  // arrow pointTowardsMainProtein Red
  if (showInhibitory) {
    svg
      .append("defs")
      .selectAll("marker")
      .data(["towardArrowRed"])
      .enter()
      .append("marker")
      .attr("id", (d) => d)
      .attr("viewBox", "0 -5 10 10")
      .attr("refX", -80)
      .attr("refY", 0)
      .attr("markerWidth", 6)
      .attr("markerHeight", 6)
      .attr("orient", "auto")
      .append("path")
      .attr("fill", "#ff0000")
      .attr("d", "M0,-5L10,0L0,5");
  }

  // arrow pointAwayFromMainProtein Green
  if (showStimulatory) {
    svg
      .append("defs")
      .selectAll("marker")
      .data(["awayArrowGreen"])
      .data(visibleNodes)
      .enter()
      .append("marker")
      .attr("id", (d) => "awayArrowGreen")
      .attr("viewBox", "0 -5 10 10")
      .attr("refX", 90)
      .attr("refY", 0)
      .attr("markerWidth", 6)
      .attr("markerHeight", 6)
      .attr("orient", "auto")
      .append("path")
      .attr("fill", "#39B54A")
      .attr("d", "M10,-5L0,0L10,5");
  }

  // arrow pointTowardsMainProtein Green
  if (showStimulatory) {
    svg
      .append("defs")
      .selectAll("marker")
      .data(["towardArrowGreen"])
      .data(visibleNodes)
      .enter()
      .append("marker")
      .attr("id", (d) => "towardArrowGreen")
      .attr("viewBox", "0 -5 10 10")
      .attr("refX", -80)
      .attr("refY", 0)
      .attr("markerWidth", 6)
      .attr("markerHeight", 6)
      .attr("orient", "auto")
      .append("path")
      .attr("fill", "#39B54A")
      .attr("d", "M0,-5L10,0L0,5");
  }

  // point towardsGrey
  if (showUndefined) {
    svg
      .append("defs")
      .selectAll("marker")
      .data(["towardArrowGrey"])
      .data(visibleNodes)
      .enter()
      .append("marker")
      .attr("id", (d) => "towardArrowGrey")
      .attr("viewBox", "0 -5 10 10")
      .attr("refX", -80) // Adjusted position to align correctly
      .attr("refY", 0)
      .attr("markerWidth", 6)
      .attr("markerHeight", 6)
      .attr("orient", "auto")
      .append("path")
      .attr("d", "M0,-5L10,0L0,5")
      .attr("fill", "#8CB4DA");
  }
  // point awayGrey
  if (showUndefined) {
    svg
      .append("defs")
      .selectAll("marker")
      .data(["awayArrowGrey"])
      .data(visibleNodes)
      .enter()
      .append("marker")
      .attr("id", (d) => "awayArrowGrey")
      .attr("viewBox", "0 -5 10 10")
      .attr("refX", 90)
      .attr("refY", 0)
      .attr("markerWidth", 6)
      .attr("markerHeight", 6)
      .attr("orient", "auto")
      .append("path")
      .attr("fill", "#8CB4DA")
      .attr("d", "M10,-5L0,0L10,5");
  }

  // Lines
  linesGroup
    .selectAll(".link")
    .data(visibleLinks)
    .enter()
    .append("line")
    .attr("class", "link")
    .each(function (d) {
      if (d.color === "Green" && !showStimulatory) {
        return;
      }
      if (d.color === "Red" && !showInhibitory) {
        return;
      }
      const selection = d3.select(this);
      if (d.color) {
        selection
          .attr("stroke", getColor(d.color))
          .attr(
            "marker-end",
            d.pointTowardsMainProtein
              ? ""
              : d.color === "Grey"
              ? "url(#awayArrowGrey)"
              : d.color === "Green"
              ? "url(#awayArrowGreen)"
              : "url(#awayArrowRed)"
          )
          .attr(
            "marker-start",
            !d.pointTowardsMainProtein
              ? ""
              : d.color === "Grey"
              ? "url(#towardArrowGrey)"
              : d.color === "Green"
              ? "url(#towardArrowGreen)"
              : "url(#towardArrowRed)"
          );
      }
    })
    .attr("stroke-width", 1.5)
    .attr("x1", (d) => d.adjustedCoords.x1)
    .attr("y1", (d) => d.adjustedCoords.y1)
    .attr("x2", (d) => d.adjustedCoords.x2)
    .attr("y2", (d) => d.adjustedCoords.y2);

  // Nodes
  nodesGroup
    .selectAll(".node")
    .data(visibleNodes)
    .enter()
    .each(function (d) {
      const selection = d3.select(this);
      getShape(selection, d);
    });

  labelsGroup
    .selectAll(".label-name")
    .data(visibleNodes)
    .enter()
    .append("text")
    .attr("class", "label label-name")
    .attr("x", (d) => d.x)
    .attr("y", (d) => d.y - (d.name?.length > 0 ? 5 : -3)) // Positioned above center (max: i changed if no name then go to the middle for protein modifications)
    .attr("text-anchor", "middle")
    .attr("font-size", (d) => (d.isCenter ? "16px" : "14px"))
    .attr("font-weight", "bold")
    .style("fill", "#ffffff")
    .style("overflow", "visible")
    .text((d) => {
      return d.name?.length > 0 ? d.name : d.longName?.length < 1 || !d.longName ? d.id : d.longName;
    });

  // Second text element for ID
  labelsGroup
    .selectAll(".label-id")
    .data(visibleNodes)
    .enter()
    .append("text")
    .attr("class", "label label-id")
    .attr("x", (d) => d.x)
    .attr("y", (d) => d.y + (d.id.length > 0 ? 15 : 0)) // Positioned below center
    .attr("text-anchor", "middle")
    .attr("font-size", (d) => (d.isCenter ? "14px" : "12px")) // Slightly smaller
    .attr("font-weight", "bold")
    .style("fill", "#ffffff")
    .style("overflow", "visible")
    .text((d) => {
      if (!showUniProtId) {
        return "";
      }
      return d.name?.length > 0 ? d.id : d.longName?.length > 1 ? d.id : "";
    });

  if (nodes[0].isProteinModification) {
    setupProteinModificationNodeClickHandlers(svg, container);
  } else {
    setupProteinInteractionsNodeEventHandlers(svg, container);
  }
  return svg;
}

export function initMap(
  svg,
  container,
  links,
  nodes,
  itemsPerPage = 10,
  showInhibitory,
  showUndefined,
  showUniProtId,
  showBlackBackground
) {
  let currentPage = 0;
  const siteNodes = nodes.filter((node) => !node.isCenter);
  const totalPages = Math.ceil(siteNodes.length / itemsPerPage);

  // Initial render
  drawMap(
    svg,
    container,
    links,
    nodes,
    currentPage,
    itemsPerPage,
    showInhibitory,
    showUndefined,
    showUniProtId,
    showBlackBackground
  );

  document.getElementById("prev-button").addEventListener("click", () => {
    if (currentPage > 0) {
      currentPage--;
      drawMap(
        svg,
        container,
        links,
        nodes,
        currentPage,
        itemsPerPage,
        showInhibitory,
        showUndefined,
        showUniProtId,
        showBlackBackground
      );
    }
  });

  document.getElementById("next-button").addEventListener("click", () => {
    if (currentPage < totalPages - 1) {
      currentPage++;
      drawMap(
        svg,
        container,
        links,
        nodes,
        currentPage,
        itemsPerPage,
        showInhibitory,
        showUndefined,
        showUniProtId,
        showBlackBackground
      );
    }
  });
}

function getColor(color) {
  let colorCode = "#654FA1";
  switch (color) {
    case "Red":
      colorCode = "#ff0000";
      break;
    case "Green":
      colorCode = "#39B54A";
      break;
    case "Grey":
      colorCode = "#8CB4DA";
      break;
  }
  return colorCode;
}

function getShape(selection, d) {
  let color = getColor(d.color);
  if (d.background) {
    selection
      .append("image")
      .attr("class", "node")
      .attr("href", `/images/${d.background}`) // or xlink:href for older SVG versions
      .attr("x", d.x - (d.isCenter ? 40 : 40))
      .attr("y", d.y - (d.isCenter ? 40 : 40))
      .attr("width", d.isCenter ? 80 : 80)
      .attr("height", d.isCenter ? 80 : 80)
      .style("cursor", "pointer");
    return;
  }
  if (d.shape === "Rectangle") {
    selection
      .append("rect")
      .attr("class", "node")
      .attr("fill", color)
      .attr("x", d.x - (d.isCenter ? 50 : 30))
      .attr("y", d.y - (d.isCenter ? 20 : 10))
      .attr("width", d.isCenter ? 100 : 60)
      .attr("height", d.isCenter ? 40 : 20)
      .attr("rx", 3) // slightly rounded corners
      .style("cursor", "pointer");
  } else {
    selection
      .append("ellipse")
      .attr("class", "node")
      .attr("fill", color)
      .attr("cx", d.x)
      .attr("cy", d.y)
      .attr("rx", d.isCenter ? 40 : 30)
      .attr("ry", d.isCenter ? 16 : 10)
      .style("cursor", "pointer");
  }
}
export function setupProteinModificationNodeClickHandlers(svg, containerElement) {
  let infoPanel = document.getElementById("node-info-panel");
  if (!infoPanel) {
    infoPanel = document.createElement("div");
    infoPanel.id = "node-info-panel";
    infoPanel.className = "hidden text-white rounded-lg shadow-lg p-4 absolute -right-40 top-4 w-64";
    containerElement.appendChild(infoPanel);
  }

  svg.selectAll(".node").on("click", function (event, d) {
    event.stopPropagation();
    displayNodeInfo(d, infoPanel);

    infoPanel.classList.remove("hidden");

    // Highlight the selected node
    svg
      .selectAll(".node")
      .attr("stroke", (n) => (n.id === d.id ? "#fff" : "#fff"))
      .attr("stroke-width", (n) => (n.id === d.id ? 1.5 : 0));
  });

  // Close panel when clicking outside
  document.addEventListener("click", function (event) {
    const isClickInside = infoPanel.contains(event.target);
    const isClickOnNode = event.target.classList.contains("node");

    if (!isClickInside && !isClickOnNode && !infoPanel.classList.contains("hidden")) {
      infoPanel.classList.add("hidden");

      // svg.selectAll(".node").attr("stroke", "#fff").attr("stroke-width", 0);
    }
  });

  return infoPanel;
}

function displayNodeInfo(nodeData, infoPanel) {
  let products;
  if (nodeData.products?.length > 0) {
    let htmlProds = nodeData.products.map(
      (prod) =>
        `<li class="text-white"><a target="_blank" rel="noopener noreferrer" href="${prod.Link}">${prod.Name}</a></li>`
    );
    products = htmlProds.join(" ");
  }

  let proteinModificationProducts;
  if (nodeData.modificationProducts?.length > 0) {
    let htmlProds = nodeData.modificationProducts.map(
      (prod) =>
        `<li class="text-white"><a target="_blank" rel="noopener noreferrer" href="${prod.Link}">${prod.Name}</a></li>`
    );
    proteinModificationProducts = htmlProds.join(" ");
  }
  if (nodeData.isProteinInteraction) {
    infoPanel.innerHTML = `
        <h3 class="text-lg font-bold text-[#FF7E00]">${nodeData.name}</h3>
        <div class="mt-2">
        <ul>
          ${
            nodeData.stringDbLink
              ? `<li><a target="_blank" rel="noopener noreferrer" href="${nodeData.stringDbLink}" class="text-white">Find it in STRING-db</a></li>`
              : ""
          }
          ${
            nodeData.uniprotLink
              ? `<li><a target="_blank" rel="noopener noreferrer" href="${nodeData.uniprotLink}" class="text-white">Find it in UNIPROT</a></li>`
              : ""
          }
          ${
            nodeData.phosphoNetLink
              ? `<li><a target="_blank" rel="noopener noreferrer" href="${nodeData.phosphoNetLink}" class="text-white">Find it in PhosphoNET</a></li>`
              : ""
          }
          ${
            nodeData.transcriptoNetLink
              ? `<li><a target="_blank" rel="noopener noreferrer" href="${nodeData.transcriptoNetLink}" class="text-white">Find it in TranscriptoNET</a></li>`
              : ""
          }
          ${
            nodeData.kinaseNetLink
              ? `<li><a target="_blank" rel="noopener noreferrer" href="${nodeData.kinaseNetLink}" class="text-white">Find it in KinaseNET</a></li>`
              : ""
          }
          ${
            nodeData.oncoNetLink
              ? `<li><a target="_blank" rel="noopener noreferrer" href="${nodeData.oncoNetLink}" class="text-white">Find it in OncoNET</a></li>`
              : ""
          }
          ${
            nodeData.drugProNetLink
              ? `<li><a target="_blank" rel="noopener noreferrer" href="${nodeData.drugProNetLink}" class="text-white">Find it in DrugProNET</a></li>`
              : ""
          }
          ${
            nodeData.phosphoSitePlusLink
              ? `<li><a target="_blank" rel="noopener noreferrer" href="${nodeData.phosphoSitePlusLink}" class="text-white">Find it in PhosphoSitePlus</a></li>`
              : ""
          }
          </ul>
        </div>
         <div class="mt-2">
          <a target="_blank" rel="noopener noreferrer" href=/ProteinModification/Result/${
            nodeData.id
          } class="text-[#38B747]">Show major post-translational modifications</a>
        </div>
          <a target="_blank" rel="noopener noreferrer" href=/TissueDistribution/Result/${
            nodeData.id
          } class="text-[#38B747]">Show main tissue/cell expression</a>
        </div>
        ${
          products
            ? `
         <div class="mt-2">
           <p class="text-[#38B747]">Affinity-purified rabbit polyclonal antibodies available from Kinexus:</p>
        </div>
        <div class="mt-2">
          <ul>
          ${products ? products : `<li class="text-white">None</li>`}
          </ul>
        </div>`
            : ""
        }
      `;
    return;
  } else if (nodeData.isSmallMolecule) {
    infoPanel.innerHTML = `
    <h3 class="text-lg font-bold text-[#FF7E00]">${nodeData.longName}</h3>
    <div class="mt-2">
    <ul>
      ${
        nodeData.casLink
          ? `<li><a target="_blank" rel="noopener noreferrer" href="${nodeData.casLink}" class="text-white">Find it in CAS</a></li>`
          : ""
      }
      ${
        nodeData.pubChemLink
          ? `<li><a target="_blank" rel="noopener noreferrer" href="${nodeData.pubChemLink}" class="text-white">Find it in PubChem</a></li>`
          : ""
      }
      ${
        nodeData.chemSpiderLink
          ? `<li><a target="_blank" rel="noopener noreferrer" href="${nodeData.chemSpiderLink}" class="text-white">Find it in ChemSpider</a></li>`
          : ""
      }
      ${
        nodeData.chemBlLink
          ? `<li><a target="_blank" rel="noopener noreferrer" href="${nodeData.chemBlLink}" class="text-white">Find it in ChEMBL</a></li>`
          : ""
      }
      </ul>
    </div>
    <div class="mt-2">
      <a target="_blank" rel="noopener noreferrer" href=/TissueDistribution/Result/${
        nodeData.id
      } class="text-white mt-4">Show main tissue/cell expression of enzyme for biosynthesis of Mediator <br> <span class="font-bold text-[#FF7E00]">${
      nodeData.shortName
    }</span></a>
    </div>
    ${
      products
        ? `
     <div class="mt-2">
       <p class="text-[#38B747]">Affinity-purified rabbit polyclonal antibodies available from Kinexus:</p>
    </div>
    <div class="mt-2">
      <ul>
      ${products ? products : `<li class="text-white">None</li>`}
      </ul>
    </div>`
        : ""
    }
  `;
  } else {
    infoPanel.innerHTML = `
        <div class="flex justify-between">
          <h3 class="text-xl font-bold text-[#FF7E00]">${nodeData.name ? nodeData.name : nodeData.id}</h3>
          <button id="close-info-panel" class="text-[#FF7E00] text-xl font-bold">✕</button>
        </div>
        
        ${
          nodeData.site
            ? `<div class="mt-2">
          <p class="text-[#38B747]">Modification Site</p>
          <p class="text-white">${nodeData.site}</p>
        </div>
        
        <div class="mt-2">
          <p class="text-[#38B747]">Modification Type</p>
          <p class="text-white">${nodeData.modificationType || "Unknown"}</p>
        </div>`
            : ""
        }
        
        ${
          nodeData.modificationEffect
            ? `
        <div class="mt-2">
          <p class="text-[#38B747]">Modification Effect</p>
          <p class="text-white">${nodeData.modificationEffect}</p>
        </div>
        `
            : ""
        }
        
        ${
          nodeData.NumReports
            ? `
        <div class="mt-2">
          <p class="text-[#38B747]">Number of Mass Reports</p>
          <p class="text-white">≥${nodeData.NumReports}</p>
        </div>
        `
            : ""
        }
        <ul>
        ${
          nodeData.phosphoSitePlusLink
            ? `<li><a target="_blank" rel="noopener noreferrer" href="${nodeData.phosphoSitePlusLink}" class="text-[#38B747]">PhosphoSitePlus Link</a></li>`
            : ""
        }
        ${
          nodeData.phosphoNetLink
            ? `<li><a target="_blank" rel="noopener noreferrer" href="${nodeData.phosphoNetLink}" class="text-[#38B747]">PhosphoNET Link</a></li>`
            : ""
        }
        </ul>
        ${
          proteinModificationProducts
            ? `
        <div class="mt-2">
        <p class="text-[#38B747]">Phosphosite-specific antibodies available from Kinexus:</p>
          <ul>
            ${proteinModificationProducts ? proteinModificationProducts : `<li class="text-white">None</li>`}
          </ul>
        </div>`
            : ""
        }
      `;
  }

  setTimeout(() => {
    const closeButton = document.getElementById("close-info-panel");
    if (closeButton) {
      closeButton.addEventListener("click", function (e) {
        e.stopPropagation();
        infoPanel.classList.add("hidden");
      });
    }
  }, 0);
}

export function setupProteinInteractionsNodeEventHandlers(svg, containerElement) {
  let infoPanel = document.getElementById("node-info-panel");
  if (!infoPanel) {
    infoPanel = document.createElement("div");
    infoPanel.id = "node-info-panel";
    infoPanel.className = "hidden text-white rounded-lg shadow-lg p-4 absolute -right-40 top-4 w-64";
    containerElement.appendChild(infoPanel);
  }

  svg.selectAll(".node").on("mouseover", function (event, d) {
    event.stopPropagation();
    displayNodeInfo(d, infoPanel);
    infoPanel.classList.remove("hidden");

    svg
      .selectAll(".node")
      .attr("stroke", (n) => (n.id === d.id ? "#fff" : "#fff"))
      .attr("stroke-width", (n) => (n.id === d.id ? 1.5 : 0));
  });

  svg.selectAll(".node").on("click", function (event, d) {
    event.stopPropagation();

    window.open(`/ProteinInteraction/Result/${d.id}`);
  });

  document.addEventListener("click", function (event) {
    const isClickInside = infoPanel.contains(event.target);
    const isClickOnNode = event.target.classList.contains("node");

    if (!isClickInside && !isClickOnNode && !infoPanel.classList.contains("hidden")) {
      infoPanel.classList.add("hidden");

      svg.selectAll(".node").attr("stroke", "#fff").attr("stroke-width", 0);
    }
  });
}
