using System;
using System.Linq;
using UnityEngine;
using Common.Events;

namespace Common.Inputs
{
    public abstract class KeyCommand
    {
        protected virtual KeyCode KeyCode { get; }
        protected virtual KeyCode[] Modifiers { get; }  = { };
        protected virtual Func<KeyCode, bool> InputChecker { get; }
        protected virtual Func<KeyCode, bool> ModifierChecker { get; }

        public virtual string Event { get; }

        public virtual bool InputCheck()
        {
            return InputChecker(KeyCode) && Modifiers.All((x) => ModifierChecker(x));
        }

        public virtual void Send()
        {
            this.PostCommand(this.Event);
        }
    }
}