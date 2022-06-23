using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;

namespace Saxion.CMGT.Algorithms.sources.Solution.NodeGraphGenerators;

internal class HighLevelNodeGraph : NodeGraph
{
	private readonly Dungeon dungeon;
	private readonly List<Room> rooms;
	private readonly List<Door> doors;

	private Dictionary<Door, Node> doorNodes;

	public HighLevelNodeGraph(Dungeon pDungeon) : base((int)(pDungeon.size.Width * pDungeon.scale), (int)(pDungeon.size.Height * pDungeon.scale), (int)pDungeon.scale/3)
	{
		dungeon = pDungeon;

		rooms = dungeon.rooms;
		doors = dungeon.doors;

		doorNodes = new Dictionary<Door, Node>();
	}

	protected override void Generate()
	{
		if (debugMode)
		{
			Console.WriteLine($"-------------------------");
			Console.WriteLine($"Doing doors");
			Console.WriteLine($"-------------------------");
		}
		
		foreach (Door door in doors)
		{
			Node node = new(GetDoorCenter(door), Node.OwnerType.Door);
			nodes.Add(node);
			
			if (!doorNodes.ContainsKey(door)) doorNodes.Add(door,node);

			if (debugMode) Console.WriteLine($"Generated node {node} at {node.location}");
		}

		
		if (debugMode)
		{
			Console.WriteLine($"-------------------------");
			Console.WriteLine($"Doing rooms and connections");
			Console.WriteLine($"-------------------------");
		}
		
		foreach (Room room in rooms)
		{
			Node roomNode = new(GetRoomCenter(room));
			nodes.Add(roomNode);
			room.node ??= roomNode;
			
			if (debugMode) Console.WriteLine($"Generated node {roomNode} at {roomNode.location}");


			foreach (Door door in room.doors)
			{
				AddConnection(roomNode, doorNodes[door]);
				if (debugMode) Console.WriteLine($"Made connection between node {roomNode} and node {doorNodes[door]}");
			}
		}
	}

	private Point GetRoomCenter(Room pRoom)
	{
		float centerX = (pRoom.area.Left + pRoom.area.Right) / 2.0f * dungeon.scale;
		float centerY = (pRoom.area.Top + pRoom.area.Bottom) / 2.0f * dungeon.scale;
		return new Point((int)centerX, (int)centerY);
	}
	
	private Point GetDoorCenter(Door pDoor) => GetPointCenter(pDoor.location);

	private Point GetPointCenter(Point pLocation)
	{
		float centerX = (pLocation.X + 0.5f) * dungeon.scale;
		float centerY = (pLocation.Y + 0.5f) * dungeon.scale;
		return new Point((int)centerX, (int)centerY);
	}
}