
# Some information :smiley:
This project was made for a course during my first year at University of Applied Sciences Saxion in Enschede for the algorithms course. 
The code is based on the GXPEngine, an engine made by Saxion.


This project features multiple versions of dungeon generators, nodegraph generators, nodegraph agents and pathfinders.

## Dungeon Generators
The dungeon gets generated through binary space partitioning, rooms get split up at random points until the rooms are too small to split up any further.

In the first version of the generator the doors get generated alongside the rooms making a door between the two split rooms each time:

![SufficientDungeon](https://user-images.githubusercontent.com/72610925/175539435-fdc90ec0-7321-4d3c-b292-fa11b5524a82.gif)

In the second version of the generator the biggest and smallest rooms are removed, the door get generated after the rooms are done generating:

![BetterDungeon](https://user-images.githubusercontent.com/72610925/175540458-85679101-9ecb-4308-9b3a-2690b32f5651.gif)

In the third and last version of the generator the rooms get shrinked down making the doors into hallways:

![ExcellentDungeon](https://user-images.githubusercontent.com/72610925/175541078-da080a73-c234-45cd-a22f-b248de1c89d7.gif)


