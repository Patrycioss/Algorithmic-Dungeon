﻿using System;
using System.Collections.Generic;
using System.Linq;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;
using Saxion.CMGT.Algorithms.sources.Assignment.PathFinding;

namespace Saxion.CMGT.Algorithms.sources.Solution.PathFinders;

internal class RecursivePathFinder : PathFinder
{
	private Dictionary<Node, Node> childParents;
	private List<List<Node>> paths;

	private Node start;
	private Node end;
	
	public RecursivePathFinder(NodeGraph pGraph) : base(pGraph) {}

	protected override List<Node> Generate(Node pFrom, Node pTo)
	{
		start = pFrom;
		end = pTo;
		
		paths = new List<List<Node>>();
		childParents = new Dictionary<Node, Node> {{start, end}};
		
		CheckConnections(start, new[]{$"{start.id}"});

		// foreach (Node node in childParents.Keys)
		// {
		// 	Console.WriteLine($"Child: {node.id}, Parent{childParents[node].id}");
		// }
		
		return GetShortestPath();
	}



	private void CheckConnections(Node from, string[] prevNodes)
	{
		Console.WriteLine($"CheckingNode: {from.id}");
		
		foreach (Node connection in from.connections)
		{
			if (prevNodes.Contains(connection.id)) continue;

			if (connection.Equals(end))
			{
				if (childParents.ContainsKey(end)) childParents[end] = from;
				else childParents.Add(end,from);
				
				Console.WriteLine($"Make at: {from.id}");

				// foreach (Node node in childParents.Keys)
				// {
				// 	Console.WriteLine($"Child: {node.id}, Parent{childParents[node].id}");
				// }
				
				paths.Add(MakePath(from));
				continue;
			}

			if (childParents.ContainsKey(connection)) childParents[connection] = from;
			else childParents.Add(connection,from);

			string[] ids = new string[prevNodes.Length + 1];
			for (int i = 0; i < prevNodes.Length; i++) ids[i] = prevNodes[i];
			ids[ids.Length - 1] = connection.id;
			CheckConnections(connection,ids);
		} 
	}

	private List<Node> MakePath(Node pathStart)
	{
		List<Node> path = new List<Node>{end};

		GetParent(pathStart);
		
		void GetParent(Node node)
		{
			if (childParents[node] == start)
			{
				path.Add(node);
				path.Add(start);
			}
			else if (childParents.ContainsKey(node))
			{
				path.Add(node);	
				GetParent(childParents[node]);
			}
		}
		return path;
	}

	private List<Node> GetShortestPath()
	{
		List<Node> shortestPath = null;

		foreach (List<Node> path in paths)
		{
			if (shortestPath == null || path.Count < shortestPath.Count)
			{
				shortestPath = path;
			}
		}
		
		return shortestPath;
	}
}