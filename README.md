# _Saigai Solves!_ 

Saigai Solves is an exploration/puzzle game that focuses on educating players about natural disasters.

![](Assets/Art/MENU_SCREEN.png)

## Installing

Saigai Solves is available for macOS, Windows, Linux, and webGL. The game is triggered to be rebuilt after commit to the remote repository through GitHub Actions. Visit the [releases page](https://github.com/saigai-studios/saigai-solves/releases) to download and play the game for your preferred computer system.

## Details

Saigai Solves's overarching goal is to educate players about natural disasters. This includes:

- How to prepare for a natural disaster (if possible)
- What to do when a natural disaster occurs
- How to recover from a natural disaster

To convey the information about natural disasters, Saigai Solves allows the user to traverse an overworld map and select one of multiple specially marked locations. Selecting a location will provide the user with a unique minigame tied to the location's context. Completing the minigame will reward the user with a _Saigai Card_, a cool collectible that provides informational tidbits about natural disasters and how to combat them.

Players can quickly recall natural disaster information from their collected Saigai Cards on the "gallery" screen, which is accessible from the game's start screen.

### Disasters 


Each world focuses on one particular disaster. Before entering a world, players have the ability to select which disaster to play through. The following is our roadmap for disasters support:

Disaster | Status |
--- | --- |
Earthquake | In-progress
Fire | Planned

## Contributing

Before working on the project for an extended period of time, be sure to do the following:

> __Note:__ All commands are ran from this README's directory in your local filesystem.

1. Pull the latest changes from the remote repository (either using `git` command-line tool or GitHub Desktop):
```
git pull
```

2. Rebuild the Rust dynamic library:
```
cd Rust; cargo run; cd ..
```

3. Open the project in Unity (either using `Unity` command-line tool or Unity Editor).
```
Unity -projectPath . 
```

## Team

Kelly Beaudrot, Joseph Benigno, Thet Ei, Gianna Giliberto, Thomas Le, Yi Min, Chase Ruskin  
