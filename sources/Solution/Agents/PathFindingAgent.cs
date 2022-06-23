using System;
using System.Collections.Generic;
using System.Threading;
using Saxion.CMGT.Algorithms.GXPEngine.Utils;
using Saxion.CMGT.Algorithms.sources.Assignment.Agent;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;
using Saxion.CMGT.Algorithms.sources.Assignment.PathFinding;
using Saxion.CMGT.Algorithms.sources.Solution.PathFinders;

namespace Saxion.CMGT.Algorithms.sources.Solution.Agents;
internal sealed class PathFindingAgent : NodeGraphAgent
{
	private Node currentTarget;
	private float currentSpeed;
	private bool isMoving;
	private readonly PathFinder pathFinder;
	private Queue<Node> nodeQueue;

	public PathFindingAgent(NodeGraph pNodeGraph, PathFinder pPathFinder, bool debugging = false) : base(pNodeGraph, debugging)
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
			List<Node> path = null;
			// path = pathFinder.InternalGenerate(currentTarget, pNode, true);
			
			if (AlgorithmsAssignment.DO_WONKY_STEP_BY_STEP)
			{
				Thread thread = new Thread(() => path = pathFinder.InternalGenerate(currentTarget, pNode));
				thread.Start();
			}
			else path = pathFinder.InternalGenerate(currentTarget, pNode);
			

			if (path == null || path.Count == 0) return;
			if (path[0] != currentTarget) path.Reverse();
			
			nodeQueue = new Queue<Node>(path);
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

			if (debugMode) Console.WriteLine($"Moving towards node: {currentTarget}");
		}
	}
}