using System;
using System.Collections.Generic;

namespace Common.AI.Fsm
{
    class ConcreteFsmState : FsmState
    {
        private readonly string name;
        private readonly Fsm owner;

        private readonly Dictionary<string, FsmState> transitionMap;
        private readonly List<FsmAction> actionList;

        public ConcreteFsmState(string name, Fsm owner)
        {
            this.name = name;
            this.owner = owner;

            this.transitionMap = new Dictionary<string, FsmState>();
            this.actionList = new List<FsmAction>();
        }

        public string GetName()
        {
            return name;
        }

        public void AddTransition(string eventId, FsmState destinationState)
        {
            if(transitionMap.ContainsKey(eventId))
            {
                UnityEngine.Debug.LogError(string.Format("The state {0} already contains a transition for event {1}.", this.name, eventId));
                return;
            }

            transitionMap[eventId] = destinationState;
        }

        public FsmState GetTransition(string eventId)
        {
            if(transitionMap.ContainsKey(eventId))
            {
                return transitionMap[eventId];
            }

            return null;
        }

        public void AddAction(FsmAction action)
        {
            if (actionList.Contains(action))
            {
                UnityEngine.Debug.LogError("The state already contains the specified action");
                return;
            }

            if (action.GetOwner() != this)
            {
                UnityEngine.Debug.LogError("The owner of the action should be this state.");
                return;
            }

            actionList.Add(action);
        }

        public IEnumerable<FsmAction> GetActions()
        {
            return actionList;
        }

        public void SendEvent(string eventId)
        {
            this.owner.SendEvent(eventId);
        }

    }
}