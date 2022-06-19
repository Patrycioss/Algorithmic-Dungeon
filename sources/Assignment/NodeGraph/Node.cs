using System.Collections.Generic;
using System.Drawing;

namespace Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;

public class Node
{
	public readonly List<Node> connections = new();
	public readonly Point location;
	public readonly string id;
	private static int lastId = 0;

	public float distanceToStart;
	public float distanceToEnd;
	public float fValue;
	public bool visited;


	public Node(Point pLocation)
	{
		location = pLocation;
		id = ""+lastId++;
		System.Console.WriteLine(id);

		Reset();
	}

	public void Reset()
	{
		distanceToEnd = float.MaxValue;
		distanceToStart = float.MaxValue;
		fValue = float.MaxValue;
		visited = false;
	}
	
	public override string ToString()
	{
		return id;
	}
}