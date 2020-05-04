using System;
using System.Collections.Generic;

namespace Common.AI.Fsm
{
    public interface FsmState
    {
        string GetName();

        void AddTransition(string eventId, FsmState destinationState);

        FsmState GetTransition(string eventId);

        void AddAction(FsmAction action);

        IEnumerable<FsmAction> GetActions();

        void SendEvent(string eventId);        
    }
}