using System;
using System.Drawing;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;

namespace Saxion.CMGT.Algorithms.sources.Solution.NodeGraphGenerators;

internal class ExcellentDungeonNodeGraph : NodeGraph
{
	private readonly Dungeon dungeon;

	public ExcellentDungeonNodeGraph(Dungeon pDungeon) : base((int)(pDungeon.size.Width * pDungeon.scale), (int)(pDungeon.size.Height * pDungeon.scale), (int)pDungeon.scale/3)
	{
		dungeon = pDungeon;
	}

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
		foreach (Door door in dungeon.doors)
		{
			Node node = new(GetDoorCenter(door));
			nodes.Add(node);
		}
		
		
		//check for overlapping nodes, just in case
		for (int i = 0; i < nodes.Count; i++)
		{
			for (int j = i + 1; j < nodes.Count; j++)
			{
				if (nodes[i].location.X != nodes[j].location.X || nodes[i].location.Y != nodes[j].location.Y) continue;

				//if they're overlapping, remove the second one
				nodes.RemoveAt(j);
				Console.WriteLine($"Removed overlapping node at with id {nodes[i].id} at {nodes[i].location}");
				j--;
			}
		}

		int o = (int)dungeon.scale;

		//Add connections when nodes are next to each other
		for (int i = nodes.Count-1; i >= 0; i--)
		{
			Node nodeA = nodes[i];
			
			for (int j = i - 1; j >= 0; j--)
			{
				Node nodeB = nodes[j];
				
				//Left
				if (nodeA.location.X - o == nodeB.location.X && nodeA.location.Y == nodeB.location.Y)
				{
					AddConnection(nodeA,nodeB);
					continue;
				}
				//Right
				if (nodeA.location.X + o == nodeB.location.X && nodeA.location.Y == nodeB.location.Y)
				{
					AddConnection(nodeA,nodeB);
					continue;
				}
				//Top
				if (nodeA.location.X == nodeB.location.X && nodeA.location.Y - o == nodeB.location.Y)
				{
					AddConnection(nodeA,nodeB);
					continue;
				}
				//Bottom
				if (nodeA.location.X == nodeB.location.X && nodeA.location.Y + o == nodeB.location.Y)
				{
					AddConnection(nodeA,nodeB);
					continue;
				}
				
				//NW
				if (nodeA.location.X == nodeB.location.X - o && nodeA.location.Y == nodeB.location.Y - o)
				{
					AddConnection(nodeA,nodeB);
					continue;
				}
				
				//NE
				if (nodeA.location.X == nodeB.location.X + o && nodeA.location.Y == nodeB.location.Y - o)
				{
					AddConnection(nodeA,nodeB);
					continue;
				}
				
				//SW
				if (nodeA.location.X == nodeB.location.X - o && nodeA.location.Y == nodeB.location.Y + o)
				{
					AddConnection(nodeA,nodeB);
					continue;
				}
				
				//SE
				if (nodeA.location.X == nodeB.location.X + o && nodeA.location.Y == nodeB.location.Y + o)
				{
					AddConnection(nodeA,nodeB);
					continue;
				}
				
			}
		}
	}

	private Point GetDoorCenter(Door pDoor)
	{
		return GetPointCenter(pDoor.location);
	}

	private Point GetPointCenter(Point pLocation)
	{
		float centerX = (pLocation.X + 0.5f) * dungeon.scale;
		float centerY = (pLocation.Y + 0.5f) * dungeon.scale;
		return new Point((int)centerX, (int)centerY);
	}
}