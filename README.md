# UnityTerrainTextureToMesh
Paint textures on Terrains, then use this tool to generate coherent meshes on selected textures.

## Steps:
1) Add this to your project
2) Create an Empty object and attach this script, MeshRenderer and MeshFilter components to it.
3) Select the Terrain, material, Texture numbers(0-indexed) from the terrain and type in the generated asset's name if you want.
4) Once done, run the game!

The Asset will be saved in the "Assets/MyStuff/Models" directory. It will also be assigned to your empty object.


## Why I made this?
- I'd much rather paint roads on complex terrains and generate a mesh on it, than modelling something of that complexity.
- There wasn't any tool that did what I wanted.

If you liked it, consider starring ⭐ the repo as it helps make this tool discoverable for other devs.
