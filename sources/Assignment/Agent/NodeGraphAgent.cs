using System.Diagnostics;
using Saxion.CMGT.Algorithms.GXPEngine;
using Saxion.CMGT.Algorithms.GXPEngine.Utils;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph;
using Saxion.CMGT.Algorithms.sources.Util;

namespace Saxion.CMGT.Algorithms.sources.Assignment.Agent;

/**
 * NodeGraphAgent provides a starting point for your own agents that would like to navigate the nodegraph.
 * It provides convenience methods such as moveTowardsNode & jumpToNode.
 * 
 * Create a subclass of this class, override Update and call these methods as required for your specific assignment.
 * See SampleNodeGraphAgent for an example.
 */
internal abstract class NodeGraphAgent : AnimationSprite
{
	protected const int REGULAR_SPEED = 1;
	protected const int FAST_TRAVEL_SPEED = 10;
	protected const int SPEED_UP_KEY = Key.LEFT_CTRL;
	protected bool debugMode;

	protected NodeGraphAgent(NodeGraph.NodeGraph pNodeGraph, bool debugging = false) : base("assets/orc.png", 4, 2, 7)
	{
		debugMode = debugging;
		Debug.Assert(pNodeGraph != null, "Please pass in a node graph.");

		SetOrigin(width / 2, height / 2);
		System.Console.WriteLine(this.GetType().Name + " created.");
	}

	//override in subclass to implement any functionality
	protected abstract void Update();

	/////////////////////////////////////////////////////////////////////////////////////////
	///	Movement helper methods

	/**
 * Moves towards the given node with either REGULAR_SPEED or FAST_TRAVEL_SPEED 
 * based on whether the RIGHT_CTRL key is pressed.
 */
	protected virtual bool MoveTowardsNode(Node pTarget, float pSpeed = 0)
	{
		float speed = pSpeed;
		
		

		//standard vector math as you had during the Physics course
		Vec2 targetPosition = new(pTarget.location.X, pTarget.location.Y);
		Vec2 currentPosition = new(x, y);
		Vec2 delta = targetPosition.Sub(currentPosition);

		if (delta.Length() < speed)
		{
			JumpToNode(pTarget);
			return true;
		}
		
		//increase our current frame based on time passed and current speed
		SetFrame((int)(speed * (Time.time / 100f)) % frameCount);

		Vec2 velocity = delta.Normalize().Scale(speed);
		x += velocity.x;
		y += velocity.y;

		scaleX = velocity.x >= 0 ? 1 : -1;

		return false;
	}

	/**
	 * Jumps towards the given node immediately
	 */
	protected virtual void JumpToNode(Node pNode)
	{
		x = pNode.location.X;
		y = pNode.location.Y;
	}

}