using System.Collections.Generic;
using System.Drawing;

namespace Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;

public class Node
{
	public readonly List<Node> connections = new();
	public readonly Point location;
	public readonly string id;
	public OwnerType ownerType;
	private static int lastId;

	public Node(Point pLocation, OwnerType pOwnerType = OwnerType.Room)
	{
		ownerType = pOwnerType;
		location = pLocation;
		id = ""+lastId++;
		// System.Console.WriteLine(id);
	}
	public override string ToString() => id;

	public enum OwnerType
	{
		Door,
		Room
	};
}