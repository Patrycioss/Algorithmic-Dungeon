
# Some information :smiley:
This project was made for a course during my first year at University of Applied Sciences Saxion in Enschede for the algorithms course. 
The code is based on the GXPEngine, an engine made by Saxion.

###### (The imagery in this ReadME is all recorded by me)

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


## Nodegraph Agents
The nodegraph agents make Morc walk through the dungeon.

The first version only allows Morc to walk to a node that's connected to his node.

![ShittyAgent](https://user-images.githubusercontent.com/72610925/175547107-9401cf5f-1739-40be-ac7c-1b7a139503af.gif)

The second version allows Morc to walk to any node he wants but he doesn't pathfind, he justs chooses a random node to go to each time that isn't the previous one.

![RandomAgent](https://user-images.githubusercontent.com/72610925/175547256-2bffacf1-fca3-4776-8961-188afd63c563.gif)

The third version handles Morcs movement by getting the path from a pathfinder...


## Pathfinders
For pathfinding I implemented a couple of algorithms.

The first one I implemented was DFS (depth first search) an algorithm that finds the shortest path by checking out all possible paths. Because of this it is really slow and doesn't work well with the second version of the nodegraph so for this one I switched to the first version. 

![DFS path](https://user-images.githubusercontent.com/72610925/175549205-c2203a69-c882-4a3c-8b9f-accf16277a30.gif)
![DFS pathfinding](https://user-images.githubusercontent.com/72610925/175549215-dbda743e-d076-4e97-85c6-3cb124e51f8e.gif)

The second one is BFS (breadth first search) this algorithm is faster and stops checking when it reaches the end node which DFS doesn't. It searches by searching in an expanding circle around it until it finds the end node.

![BFS path](https://user-images.githubusercontent.com/72610925/175550647-5c73918b-f0a0-45a2-a1bc-0d30d4df2c89.gif)
![BFS pathfinding](https://user-images.githubusercontent.com/72610925/175550653-80dff6a6-5af0-4e3c-897c-c67295b69172.gif)

The previous algorithms search for the shortest path in regards to the amount of nodes not the distance.
This next algorithm does however, Dijkstra is an algorithm that generates a path based on the total physical distance from the start node to the end node.

![Dijkstra path](https://user-images.githubusercontent.com/72610925/175552225-0bf48834-7d98-413c-b9f5-436c11ec45b9.gif)
![Dijkstra pathfinding](https://user-images.githubusercontent.com/72610925/175552236-efb59334-2f8d-46ec-8c9a-5d12234620df.gif)

The last and best algorithm is A* this is an algorithm that works like Dijkstra but instead of only taking the shortest route so far into account it also takes the distance to the end node in account which means a lot less nodes are checked in the process.

![Astar path](https://user-images.githubusercontent.com/72610925/175553407-9fc896a9-3627-4777-a2cb-86c7dd3d1520.gif)
![Astar pathfinding](https://user-images.githubusercontent.com/72610925/175553430-b47a6705-493d-4fe8-ba58-97ff5b1c1cfc.gif)




