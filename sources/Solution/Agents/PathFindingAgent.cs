using System;
using System.Collections.Generic;
using Saxion.CMGT.Algorithms.GXPEngine.Utils;
using Saxion.CMGT.Algorithms.sources.Assignment.Agent;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;
using Saxion.CMGT.Algorithms.sources.Assignment.PathFinding;

namespace Saxion.CMGT.Algorithms.sources.Solution.Agents;
internal sealed class PathFindingAgent : NodeGraphAgent
{
	private Node currentTarget;
	private float currentSpeed;
	private bool isMoving;
	private readonly PathFinder pathFinder;
	private Queue<Node> nodeQueue;

	public PathFindingAgent(NodeGraph pNodeGraph, PathFinder pPathFinder) : base(pNodeGraph)
	{
		pathFinder = pPathFinder;
		isMoving = false;
		
		SetOrigin(width / 2.0f, height / 2.0f);

		//position ourselves on a random node
		if (pNodeGraph.nodes.Count > 0)
		{
			currentTarget = pNodeGraph.nodes[Utils.Random(0,pNodeGraph.nodes.Count)];;
			JumpToNode(currentTarget);
		}

		//listen to node clicks
		pNodeGraph.onNodeLeftClicked += OnNodeClickHandler;

		currentSpeed = 10;
	}
	
	private void OnNodeClickHandler(Node pNode)
	{
		if (!isMoving)
		{
			List<Node> path = pathFinder.InternalGenerate(currentTarget, pNode);
			path.Reverse();

			if (path.Contains(currentTarget)) path.Remove(currentTarget);
			
			nodeQueue = new Queue<Node>(path);

			Console.WriteLine(nodeQueue.Count);

			if (nodeQueue.Count == 0)
			{
				Console.WriteLine("No path!");
				return;
			}
			
			currentTarget = nodeQueue.Dequeue();
			isMoving = true;
		}
	}

	protected override void Update()
	{
		//Speed controls
		if (Input.GetKeyDown(Key.PLUS)) currentSpeed += 0.1f;
		else if (Input.GetKeyDown(Key.MINUS)) currentSpeed -= 0.1f;
		
		if (nodeQueue == null || currentTarget == null) return;

		if (MoveTowardsNode(currentTarget, currentSpeed))
		{
			if (nodeQueue.Count == 0)
			{
				isMoving = false;
			}
			else currentTarget = nodeQueue.Dequeue();
		}
	}
}