﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace Assets.Scripts
{
	public class NPCConnectedPatrol : MonoBehaviour
	{
		//Determines if the object waits
		[SerializeField]
		bool _patrolWaiting;

		//The time the object waits if it does wait
		[SerializeField]
		float _totalWaitTime = 3f;

		//The chance of changing direction
		[SerializeField]
		float _switchProbability = 0.2f;

		//other variables
		NavMeshAgent _navMeshAgent;
		ConnectedWaypoint _currentWaypoint;
		ConnectedWaypoint _previousWaypoint;

		bool _travelling;
		bool _waiting;
		float _waitTimer;
		int _waypointsVisited;

		// Use this for initialization
		void Start()
		{
			_navMeshAgent = this.GetComponent<NavMeshAgent>();

			if (_navMeshAgent == null)
			{
				Debug.LogError("The nav mesh agent component is not attached to " + gameObject.name);
			}
			else
			{
				if(_currentWaypoint == null)
				{
					GameObject[] allWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");

					if(allWaypoints.Length > 0)
					{
						while (_currentWaypoint == null)
						{
							int random = UnityEngine.Random.Range(0, allWaypoints.Length);
							ConnectedWaypoint startingWaypoint = allWaypoints[random].GetComponent<ConnectedWaypoint>();

							if(startingWaypoint != null)
							{
								_currentWaypoint = startingWaypoint;
							}
						}
					}
					else
					{
						Debug.LogError("Failed to find any waypoints for use in the scene");
					}
				}

				SetDestination();
			}
		}

		// Update is called once per frame
		void Update()
		{
			if(_travelling && _navMeshAgent.remainingDistance <= 1.0f)
			{
				_travelling = false;
				_waypointsVisited++;

				if(_patrolWaiting)
				{
					_waiting = true;
					_waitTimer = 0f;
				}
				else
				{
					SetDestination();
				}
			}

			if(_waiting)
			{
				_waitTimer += Time.deltaTime;
				if (_waitTimer >= _totalWaitTime)
				{
					_waiting = false;

					SetDestination();
				}
			}
		}

		private void SetDestination()
		{
			if(_waypointsVisited > 0)
			{
				ConnectedWaypoint nextWaypoint = _currentWaypoint.NextWaypoint(_previousWaypoint);
				_previousWaypoint = _currentWaypoint;
				_currentWaypoint = nextWaypoint;
			}

			Vector3 targetVector = _currentWaypoint.transform.position;
			_navMeshAgent.SetDestination(targetVector);
			_travelling = true;
		}
	}
}
