#!/bin/bash

# Run docker-compose using the file in the /dependencies subdirectory
docker-compose -f "$(dirname "$0")/dependencies/docker-compose.yml" up
