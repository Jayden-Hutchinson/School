# Docker Setup and Run Instructions

## Prerequisites

- Ensure Docker is installed on your machine. You can download it from [Docker's official website](https://www.docker.com/get-started).

## Setup

1. **Run Docker Desktop**

   Make sure this is running or else docker commands won't work.

2. **Build the Docker Image**

   ```sh
   docker build -t cytonet-image .
   ```

## Running the Docker Container

3. **Run the Container**

   ```sh
   docker run --rm -p 8080:8080 --name cytonet-container cytonet-image
   ```

4. **Access the Application**
   
   Open your web browser and navigate to `http://localhost:8080`.

## Stopping the Docker Container

5. **Stop the Container**

   ```sh
   docker stop cytonet-container
   ```

6. **Remove the Container**
   ```sh
   docker rm cytonet-container
   ```

## Additional Commands

- **View Running Containers**

  ```sh
  docker ps
  ```

- **View Container Logs**

  ```sh
  docker logs cytonet-container
  ```

- **Access the Container Shell**
  
   ```sh
  docker exec -it cytonet-container /bin/bash
  ```

For more detailed information, refer to the [Docker documentation](https://docs.docker.com/).
