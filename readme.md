# uttt - Ultimate Tic Tac Toe

A simple playable Ultimate Tic Tac Toe browser game.

Todo list
- [x] Rename project
- [x] Provide an insightful project description
- [ ] Document softwareSystem context C4 Model
- [ ] Define most important ADRs


Links:
- [Structurizr Lite Local Runtime](http://localhost:8080/workspace/diagrams)
- [Excalidraw](https://excalidraw.com/)


## Required tools
1. [Structurizr Lite](https://docs.structurizr.com/) - C4 Model documentation
1. [adr-tools](https://github.com/npryce/adr-tools) - Architecture Decision Record tooling
1. [Excalidraw](https://excalidraw.com/) - Whiteboarding tool

## Recommended Tools
1. [Podman](https://podman.io/) - Container management / runtime

## Podman setup
```
podman build -t structurizr-adr .
podman run -it --rm -p 8080:8080 -v c:\Users\gabe_\code\starter-project\doc:/usr/local/structurizr structurizr-adr
```

## adr-tools setup
ADR tools is incorporated into the structurizr image so adr commands should be issued inside the container.
```
podman ps
podman exec -it <container_id> bash
/usr/local/structurizr$ adr list
```