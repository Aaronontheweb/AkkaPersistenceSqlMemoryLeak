@echo off

:: Run docker-compose using the file in the /dependencies subdirectory
docker-compose -f "%~dp0dependencies\docker-compose.yml" up
