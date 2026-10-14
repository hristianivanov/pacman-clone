# Pacman Clone

![Pacman Clone](Assets/packman-clone-banner.png)

A 2D Pac-Man clone built with **Unity 6** and **C#**. This is my first Unity game, made during my studies in Computer Graphic Design at Technical University of Sofia, Plovdiv Branch.

## Features

- Grid-based Pac-Man movement with smooth direction changes
- Animated sprites for Pac-Man and the ghosts
- Ghost enemies that chase the player
- Screen-edge passages (tunnels) that teleport you to the other side
- Game manager that handles score, lives, and round resets

## Screenshots

<!-- TODO: add 1-2 gameplay screenshots, e.g. -->
<!-- ![Gameplay](Assets/screenshots/gameplay.png) -->

## Controls

| Key | Action |
| --- | ------ |
| Arrow keys / WASD | Move Pac-Man |

<!-- TODO: adjust to match your actual input setup -->

## Getting Started

### Requirements

- [Unity Hub](https://unity.com/download)
- Unity 6 (the exact version is in `ProjectSettings/ProjectVersion.txt`)

### Run the project

1. Clone the repository:
```bash
   git clone https://github.com/hristianivanov/pacman-clone.git
```
2. Open **Unity Hub** → **Add** → select the cloned folder.
3. Open the project with the matching Unity 6 version.
4. Open the main scene from `Assets/Scenes` and press **Play**.

## What I Learned

- Working with the Unity editor, scenes, prefabs, and the 2D physics system
- Structuring a small game with separate scripts for movement, characters, and game state
- Frame-based sprite animation
