using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Saxion.CMGT.Algorithms.sources.Assignment.Dungeon;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;
using Saxion.CMGT.Algorithms.sources.Assignment.PathFinding;

namespace Saxion.CMGT.Algorithms.sources.Solution.PathFinders;

internal class RecursivePathFinder : PathFinder
{
	private List<Node> shortestPath;
	private Node start;
	private Node end;
	
	public RecursivePathFinder(NodeGraph pGraph, Dungeon pDungeon, bool debugging) : base(pGraph, pDungeon, debugging) {}

	protected override List<Node> Generate(Node pFrom, Node pTo)
	{
		if (pFrom == pTo) return null;
		if (pFrom.connections.Contains(pTo)) return new List<Node>{pTo, pFrom};

		start = pFrom;
		end = pTo;
		shortestPath = null;
		
		CheckConnections(start, new List<Node>{start});

		if (debugMode && shortestPath != null) Console.WriteLine($"ShortestPath found with length: {shortestPath.Count}");
		return shortestPath;
	}
	
	private void CheckConnections(Node from, List<Node> prevNodes)
	{
		if (debugMode) Console.WriteLine($"CheckingNode: {from.id}");

		if (AlgorithmsAssignment.DO_WONKY_STEP_BY_STEP)
		{
			DrawNode(from,Brushes.Red);
			Thread.Sleep(10);
		}
		
		foreach (Node connection in from.connections)
		{
			if (prevNodes.Contains(connection)) continue;

			if (connection.Equals(end))
			{
				List<Node> path = new List<Node>(prevNodes) {end};
				
				//Debug
				if (debugMode)
				{
					Console.WriteLine($"----");
					Console.WriteLine($"New path found with count: {path.Count}");
					Console.WriteLine($"----");
				}
				
				if (shortestPath == null || path.Count < shortestPath.Count) shortestPath = path;
				
				if (debugMode) Console.WriteLine($"Shortest path till now!");

				continue;
			}
			
			List<Node> currentPath = new List<Node>(prevNodes) {connection};
			CheckConnections(connection, currentPath);
		} 
	}
}