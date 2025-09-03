const goButton = "go-button";

class ButtonManager {
  goButton = document.getElementById(goButton);
  colourButtons = [];
}

const buttonManager = new ButtonManager();
console.log(buttonManager.goButton);
