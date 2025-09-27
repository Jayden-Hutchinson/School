const { ApiServer } = require("./modules/api_server");

class App {
  start() {
    this.server = new ApiServer();
  }
}

new App().start();
