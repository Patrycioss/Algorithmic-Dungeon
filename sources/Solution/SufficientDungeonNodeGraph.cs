using System.Collections.Generic;
using System.Drawing;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;

namespace Saxion.CMGT.Algorithms.sources.Solution;

class SufficientDungeonNodeGraph : NodeGraph
{
	private Dungeon dungeon;
	private List<Room> rooms;
	private List<Door> doors;

	private Dictionary<Door, Node> doorNodes;

	public SufficientDungeonNodeGraph(Dungeon pDungeon) : base((int)(pDungeon.size.Width * pDungeon.scale), (int)(pDungeon.size.Height * pDungeon.scale), (int)pDungeon.scale/3)
	{
		dungeon = pDungeon;

		rooms = dungeon.rooms;
		doors = dungeon.doors;

		doorNodes = new Dictionary<Door, Node>();
	}

	protected override void ActualGenerate()
	{
		foreach (Door door in doors)
		{
			Node node = new Node(GetDoorCenter(door));
			nodes.Add(node);
			
			doorNodes.Add(door,node);
		}

		foreach (Room room in rooms)
		{
			Node roomNode = new Node(GetRoomCenter(room));
			nodes.Add(roomNode);
			
			foreach (Door door in room.doors)
			{
				AddConnection(roomNode,doorNodes[door]);
			}
		}

	}
	
	protected Point GetRoomCenter(Room pRoom)
	{
		float centerX = (pRoom.area.Left + pRoom.area.Right) / 2.0f * dungeon.scale;
		float centerY = (pRoom.area.Top + pRoom.area.Bottom) / 2.0f * dungeon.scale;
		return new Point((int)centerX, (int)centerY);
	}

	/**
	 * A helper method for your convenience so you don't have to meddle with coordinate transformations.
	 * @return the location of the center of the given door you can use for your nodes in this class
	 */
	protected Point GetDoorCenter(Door pDoor)
	{
		return GetPointCenter(pDoor.location);
	}

	/**
	 * A helper method for your convenience so you don't have to meddle with coordinate transformations.
	 * @return the location of the center of the given point you can use for your nodes in this class
	 */
	protected Point GetPointCenter(Point pLocation)
	{
		float centerX = (pLocation.X + 0.5f) * dungeon.scale;
		float centerY = (pLocation.Y + 0.5f) * dungeon.scale;
		return new Point((int)centerX, (int)centerY);
	}

}