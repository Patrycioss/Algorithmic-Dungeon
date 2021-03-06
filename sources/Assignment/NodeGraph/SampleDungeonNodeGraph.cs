using System.Diagnostics;
using System.Drawing;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;

namespace Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;

/**
 * An example of a dungeon nodegraph implementation.
 * 
 * This implementation places only three nodes and only works with the SampleDungeon.
 * Your implementation has to do better :).
 * 
 * It is recommended to subclass this class instead of NodeGraph so that you already 
 * have access to the helper methods such as getRoomCenter etc.
 * 
 * TODO:
 * - Create a subclass of this class, and override the generate method, see the generate method below for an example.
 */
internal class SampleDungeonNodeGraph : NodeGraph
{
	protected readonly Dungeon.Dungeon dungeon;

	public SampleDungeonNodeGraph(Dungeon.Dungeon pDungeon) : base((int)(pDungeon.size.Width * pDungeon.scale), (int)(pDungeon.size.Height * pDungeon.scale), (int)pDungeon.scale/3)
	{
		Debug.Assert(pDungeon != null, "Please pass in a dungeon.");

		dungeon = pDungeon;
	}

	protected override void Generate ()
	{
		//Generate nodes, in this sample node graph we just add to nodes manually
		//of course in a REAL nodeGraph (read:yours), node placement should 
		//be based on the rooms in the dungeon

		//We assume (bad programming practice 1-o-1) there are two rooms in the given dungeon.
		//The getRoomCenter is a convenience method to calculate the screen space center of a room
		nodes.Add(new Node(GetRoomCenter(dungeon.rooms[0])));
		nodes.Add(new Node(GetRoomCenter(dungeon.rooms[1])));
		//The getDoorCenter is a convenience method to calculate the screen space center of a door
		nodes.Add(new Node(GetDoorCenter(dungeon.doors[0])));

		//create a connection between the two rooms and the door...
		AddConnection(nodes[0], nodes[2]);
		AddConnection(nodes[1], nodes[2]);
	}

	/**
	 * A helper method for your convenience so you don't have to meddle with coordinate transformations.
	 * @return the location of the center of the given room you can use for your nodes in this class
	 */
	protected Point GetRoomCenter(Room pRoom)
	{
		float centerX = ((pRoom.area.Left + pRoom.area.Right) / 2.0f) * dungeon.scale;
		float centerY = ((pRoom.area.Top + pRoom.area.Bottom) / 2.0f) * dungeon.scale;
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