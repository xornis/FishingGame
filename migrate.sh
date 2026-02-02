#!/bin/bash

cd "D:/Programs/Unity/Hub/Projects/GitHub/FishingGame"

# Create directories
mkdir -p Assets/Project/{Scripts/{Gameplay,UI,Utils},Data/{Tiles,Upgrades,Generation},Prefabs/{Tiles,UI},Art/{Sprites/{Tiles,UI},Audio/SFX},Scenes}

# Copy Scripts
echo "Copying Gameplay scripts..."
cp Assets/Scripts/{AudioManager,CameraController,FishingInteraction,InputController,IslandManager,PlayerMovement,RunController,SaveSystem,Tile,TileView}.cs* Assets/Project/Scripts/Gameplay/
cp Assets/Scripts/SO/"Tile Inheritors"/TileView/*.cs* Assets/Project/Scripts/Gameplay/

echo "Copying UI scripts..."
cp Assets/Scripts/UI/*.cs* Assets/Project/Scripts/UI/

echo "Copying Utils..."
cp Assets/Scripts/SO/GameEvents.cs* Assets/Project/Scripts/Utils/

echo "Copying Data..."
cp Assets/Scripts/SO/{TileData,TileGenerationRules,TileStepSounds}.cs* Assets/Project/Data/
cp Assets/Scripts/SO/UpgradeTemplate.cs* Assets/Project/Data/Upgrades/
cp -r Assets/Scripts/SO/"Tile Inheritors"/TileData/*.cs* Assets/Project/Data/Tiles/

echo "Copying Asse...
cp -r Assets/Prefabs/* Assets/Project/Prefabs/
cp -r Assets/Sprites/* Assets/Project/Art/Sprites/
cp -r Assets/SFX/* Assets/Project/Art/Audio/SFX/
cp -r "Assets/Scriptable Objects"/* Assets/Project/Data/

echo "Done!"
