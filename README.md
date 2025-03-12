# Voxel Engine

## Overview
This is an engine for generating and rendering a landscape comprised of chunks of voxels according to the user's specifications.

## Instructions / Controls
On the WorldGen menu, adjust any of the sliders to your liking and click 'Generate!' to generate a world. Press ESC at any time to repoen the menu and generate a new world.  
(at the time of this release, the world's seed will always be randomly generated.)

WASD - Movement  
Space - Jump  
Left Mouse - Destroy Block  
Right Mouse - Place Block  
Mouse Scroll - Pick Block  
ESC - Open/close WorldGen menu  

## Progress
So far, I've completed most of the fundamental voxel generation and management. Given a seed and some dimensions, the engine will procedurally generate a landscape from the seed, and drop the user into the newly generated world. From there, the user can move around and place or destroy individual voxels within the world.

## Future Plans
I have two major objectives moving forward.
The first is to further optimize my mesh generation (using a greedy mesh algorithm or something similar) to reduce the number of triangles the engine needs to generate and improve performance when creating large worlds.
My other goal is to implement more tools for the user to customize the world, both in terms of parameters for initial generation, and tools for world editing post-generation. Scope pending, I want to look into how I can have different voxels render or behave differently even if they're in the same chunk, which would open the door for many more customization options.
