using System.Collections.Generic;
using System.Drawing;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;

namespace Saxion.CMGT.Algorithms.sources.Solution.NodeGraphGenerators;

internal class GoodDungeonNodeGraph : NodeGraph
{
	private readonly Dungeon dungeon;
	private readonly List<Room> rooms;
	private readonly List<Door> doors;

	private Dictionary<Door, Node> doorNodes;

	public GoodDungeonNodeGraph(Dungeon pDungeon) : base((int)(pDungeon.size.Width * pDungeon.scale), (int)(pDungeon.size.Height * pDungeon.scale), (int)pDungeon.scale/3)
	{
		dungeon = pDungeon;
		rooms = dungeon.rooms;
		doors = dungeon.doors;
		doorNodes = new Dictionary<Door, Node>();
	}

	protected override void Generate()
	{
		foreach (Door door in doors)
		{
			Node node = new(GetDoorCenter(door));
			nodes.Add(node);
			
			doorNodes.Add(door,node);
		}

		foreach (Room room in rooms)
		{
			Node roomNode = new(GetRoomCenter(room));
			nodes.Add(roomNode);
			
			foreach (Door door in room.doors)
			{
				AddConnection(roomNode,doorNodes[door]);
			}
		}
	}

	private Point GetRoomCenter(Room pRoom)
	{
		float centerX = (pRoom.area.Left + pRoom.area.Right) / 2.0f * dungeon.scale;
		float centerY = (pRoom.area.Top + pRoom.area.Bottom) / 2.0f * dungeon.scale;
		return new Point((int)centerX, (int)centerY);
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