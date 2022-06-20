using System.Collections.Generic;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;
using Saxion.CMGT.Algorithms.sources.Assignment.PathFinding;

namespace Saxion.CMGT.Algorithms.sources.Solution.PathFinders;

internal class RecursivePathFinder : PathFinder
{
	private List<Node> shortestPath;
	private Node start;
	private Node end;
	
	public RecursivePathFinder(NodeGraph pGraph, Dungeon pDungeon) : base(pGraph, pDungeon) {}

	protected override List<Node> Generate(Node pFrom, Node pTo)
	{
		if (pFrom == pTo) return null;
		if (pFrom.connections.Contains(pTo)) return new List<Node>{pTo, pFrom};

		start = pFrom;
		end = pTo;
		shortestPath = null;
		
		CheckConnections(start, new List<Node>{start});

		return shortestPath;
	}
	
	private void CheckConnections(Node from, List<Node> prevNodes)
	{
		// Console.WriteLine($"CheckingNode: {from.id}");
		foreach (Node connection in from.connections)
		{
			if (prevNodes.Contains(connection)) continue;

			if (connection.Equals(end))
			{
				List<Node> path = new List<Node>(prevNodes) {end};
				if (shortestPath == null || path.Count < shortestPath.Count) shortestPath = path;
				continue;
			}
			
			List<Node> currentPath = new List<Node>(prevNodes) {connection};
			CheckConnections(connection, currentPath);
		} 
	}
}