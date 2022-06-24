
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


## Nodegraph Generators
The nodegraphs are generated to provide a path for "Morc the Orc" to explore the dungeon. 

The first version of this graph only makes a single node in the center of each room and one at the doors. These nodes are then connected appropriately. This version doesn't work with the third version of the dungeon generator so we use the second version. 

![HighLevelNodeGraph](https://user-images.githubusercontent.com/72610925/175546079-7594f637-2950-4b2e-8713-1ae653c1d23d.png)

The second version makes a node at every tile Morc should be able to walk. These nodes are then connected appropriately diagonally, horizontally and vertically. 

![LowLevelNodeGraph](https://user-images.githubusercontent.com/72610925/175546248-45a78ee0-c273-4998-a858-db3b2258e8b0.png)



