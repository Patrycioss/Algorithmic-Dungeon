using System.Collections.Generic;
using System.Drawing;
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

	private Queue<Node> pathQueue;

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

		currentSpeed = 0.5f;
	}
	

	private void OnNodeClickHandler(Node pNode)
	{
		if (!isMoving)
		{
			List<Node> path = pathFinder.InternalGenerate(currentTarget, pNode);
			path.Reverse();
			pathQueue = new Queue<Node>(path);
			currentTarget = pathQueue.Dequeue();
		}
	}

	protected override void Update()
	{
		//Speed controls
		if (Input.GetKeyDown(Key.PLUS)) currentSpeed += 0.1f;
		else if (Input.GetKeyDown(Key.MINUS)) currentSpeed -= 0.1f;

		if (pathQueue == null || pathQueue.Count == 0 || currentTarget == null) return;

		isMoving = MoveTowardsNode(currentTarget, currentSpeed);
		
		if (isMoving)
		{
			if (pathQueue.Count == 1) pathFinder.graphics.Clear(Color.Transparent);
			else currentTarget = pathQueue.Dequeue();
		}
	}
}