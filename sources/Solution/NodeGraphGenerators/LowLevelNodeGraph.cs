using System;
using System.Drawing;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;

namespace Saxion.CMGT.Algorithms.sources.Solution.NodeGraphGenerators;

internal class LowLevelNodeGraph : NodeGraph
{
	private readonly Dungeon dungeon;

	public LowLevelNodeGraph(Dungeon pDungeon) : base((int)(pDungeon.size.Width * pDungeon.scale), (int)(pDungeon.size.Height * pDungeon.scale), (int)pDungeon.scale/3) => dungeon = pDungeon;

	protected override void Generate()
	{
		//Add nodes to all tiles in the rooms
		foreach (Room room in dungeon.rooms)
		{
			Console.WriteLine($"TL: {room.topLeft},TR: {room.topRight}, BL: {room.bottomLeft}, BR: {room.bottomRight}");
			
			for (int i = room.topLeft.X + 1; i < room.topRight.X; i++)
			{
				for (int j = room.topRight.Y + 1; j < room.bottomLeft.Y; j++)
				{
					Node node = new(GetPointCenter(new Point(i,j)));
					nodes.Add(node);
					room.node ??= node;
				}
			}
		}
		
		//Add nodes to doors
		foreach (Door door in dungeon.doors) nodes.Add(new Node(GetDoorCenter(door), Node.OwnerType.Door));

		//Fixed so this is unnecessary
		
		// check for overlapping nodes, just in case
		 // for (int i = nodes.Count-1; i >= 0; i--)
		 // {
		 // 	for (int j = i -1; j >= 0; j--)
		 // 	{
		 // 		if (nodes[i].location.X != nodes[j].location.X || nodes[i].location.Y != nodes[j].location.Y) continue;
		 //
		 // 		//if they're overlapping, remove the second one
		 // 		nodes.RemoveAt(j);
		 // 		Console.WriteLine($"Removed overlapping node at with id {nodes[i].id} at {nodes[i].location}");
		 // 	}
		 // }

		int dScale = (int)dungeon.scale;

		//Add connections when nodes are next to each other
		for (int i = nodes.Count-1; i >= 0; i--)
		{
			Node nodeA = nodes[i];
			
			for (int j = i - 1; j >= 0; j--)
			{
				Node nodeB = nodes[j];
				
				for (int targetX = -1; targetX <= 1; targetX++)
				{
					for (int targetY = -1; targetY <= 1; targetY++)
					{
						if (nodeA.location.X + targetX * dScale == nodeB.location.X && nodeA.location.Y + targetY * dScale == nodeB.location.Y)
						{
							AddConnection(nodeA,nodeB);
						}
					}
				}
				
				
				// //Left
				// if (nodeA.location.X - dScale == nodeB.location.X && nodeA.location.Y == nodeB.location.Y)
				// {
				// 	AddConnection(nodeA,nodeB);
				// 	continue;
				// }
				// //Right
				// if (nodeA.location.X + dScale == nodeB.location.X && nodeA.location.Y == nodeB.location.Y)
				// {
				// 	AddConnection(nodeA,nodeB);
				// 	continue;
				// }
				// //Top
				// if (nodeA.location.X == nodeB.location.X && nodeA.location.Y - dScale == nodeB.location.Y)
				// {
				// 	AddConnection(nodeA,nodeB);
				// 	continue;
				// }
				// //Bottom
				// if (nodeA.location.X == nodeB.location.X && nodeA.location.Y + dScale == nodeB.location.Y)
				// {
				// 	AddConnection(nodeA,nodeB);
				// 	continue;
				// }
				//
				// //NW
				// if (nodeA.location.X == nodeB.location.X - dScale && nodeA.location.Y == nodeB.location.Y - dScale)
				// {
				// 	AddConnection(nodeA,nodeB);
				// 	continue;
				// }
				//
				// //NE
				// if (nodeA.location.X == nodeB.location.X + dScale && nodeA.location.Y == nodeB.location.Y - dScale)
				// {
				// 	AddConnection(nodeA,nodeB);
				// 	continue;
				// }
				//
				// //SW
				// if (nodeA.location.X == nodeB.location.X - dScale && nodeA.location.Y == nodeB.location.Y + dScale)
				// {
				// 	AddConnection(nodeA,nodeB);
				// 	continue;
				// }
				//
				// //SE
				// if (nodeA.location.X == nodeB.location.X + dScale && nodeA.location.Y == nodeB.location.Y + dScale)
				// {
				// 	AddConnection(nodeA,nodeB);
				// 	continue;
				// }
				
			}
		}
	}

	private Point GetDoorCenter(Door pDoor) => GetPointCenter(pDoor.location);

	private Point GetPointCenter(Point pLocation)
	{
		float centerX = (pLocation.X + 0.5f) * dungeon.scale;
		float centerY = (pLocation.Y + 0.5f) * dungeon.scale;
		return new Point((int)centerX, (int)centerY);
	}
}