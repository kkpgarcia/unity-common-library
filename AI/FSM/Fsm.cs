using System;
using System.Collections.Generic;

using UnityEngine;

namespace Common.AI.Fsm
{
	public class Fsm
	{
		private readonly string name;

		private FsmState currentState;
		private readonly Dictionary<string, FsmState> stateMap;

		private Dictionary<string, FsmState> globalTransitionMap;

		public Fsm (string name)
		{
			this.name = name;
			currentState = null;
			stateMap = new Dictionary<string, FsmState> ();
		}

		public string Name {
			get {
				return name;
			}
		}

		public FsmState AddState (string name)
		{
			if (stateMap.ContainsKey (name))
				Debug.LogWarning ("The FSM already contains: " + name);

			FsmState newState = new ConcreteFsmState (name, this);
			stateMap [name] = newState;
			return newState;
		}

		private delegate void StateActionProcessor (FsmAction action);

		private void ProcessStateactions (FsmState state, StateActionProcessor actionProcessor)
		{
			FsmState currentStateOnInvoke = this.currentState;

			IEnumerable<FsmAction> actions = state.GetActions ();

			foreach (FsmAction action in actions) {
				actionProcessor (action);

				if (this.currentState != currentStateOnInvoke) {
					break;
				}
			}
		}

		public void Start (string stateName)
		{
			if (!stateMap.ContainsKey (stateName))
				Debug.LogWarning ("FSM doesn't contain a state: " + stateName);

			ChangeToState (stateMap [stateName]);
		}

		private void ChangeToState (FsmState state)
		{
			if (this.currentState != null) {
				ExitState (this.currentState);
			}

			this.currentState = state;
			EnterState (this.currentState);
		}

		public void EnterState (FsmState state)
		{
			ProcessStateactions (state, delegate (FsmAction action) {
				action.OnEnter ();
			});
		}

		private void ExitState (FsmState state)
		{
			FsmState currentStateOnInvoke = this.currentState;

			ProcessStateactions (state, delegate (FsmAction action) {
				action.OnExit ();
				if (this.currentState != currentStateOnInvoke) {
					throw new Exception ("State cannot be changed on exit of the specified state.");
				}
			});
		}

		public void Update ()
		{
			if (this.currentState == null) {
				return;
			}

			ProcessStateactions (this.currentState, delegate (FsmAction action) {
				action.OnUpdate ();
			});
		}

		public void FixedUpdate ()
		{
			if (this.currentState == null) {
				return;
			}

			ProcessStateactions (this.currentState, delegate (FsmAction action) {
				action.OnFixedUpdate ();
			});
		}

		public FsmState GetCurrentState ()
		{
			return this.currentState;
		}

		public void SendEvent (string eventId)
		{
			if (string.IsNullOrEmpty (eventId))
				Debug.LogWarning ("The specified event ID cannot be empty.");

			if (currentState == null) {
				Debug.LogWarning (string.Format ("Fsm {0} doesn't have a current state.", this.name));
				return;
			}

			FsmState transitionState = ResolveTransition (eventId);

			if (transitionState == null) {
				Debug.LogWarning (string.Format ("The current state {0} has no transition for event {1}", this.currentState.GetName (), eventId));
			} else {
				ChangeToState (transitionState);
			}
		}

		private FsmState ResolveTransition (string eventId)
		{
			FsmState transitionState = this.currentState.GetTransition (eventId);

			if (transitionState == null) {
				if (this.globalTransitionMap != null && this.globalTransitionMap.ContainsKey (eventId)) {
					return this.globalTransitionMap [eventId];
				}
			} else {
				return transitionState;
			}

			return null;
		}

		public void AddGlobalTransition (string eventId, FsmState destinationState)
		{
			if (this.globalTransitionMap == null) {
				this.globalTransitionMap = new Dictionary<string, FsmState> ();
			}

			if (!this.globalTransitionMap.ContainsKey (eventId))
				this.globalTransitionMap [eventId] = destinationState;
		}
	}
}