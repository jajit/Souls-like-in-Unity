using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class ActionManager : MonoBehaviour
    {
        public List<Action> actionSlots = new List<Action>();

        StateManager states;

        public void Init(StateManager st)
        {
            states = st;
            UpdateActionsOneHanded();
        }

        public void UpdateActionsOneHanded()
        {
            EmptyAllSlots();
            Weapon w = states.inventoryManager.currentWeapon;
            for (int i = 0; i < w.actions.Count; i++)
            {
                Action a = GetActionEntry(w.actions[i].input);
                a.targetAnimation = w.actions[i].targetAnimation;
            }
        }

        public void UpdateActionsTwoHanded()
        {
            EmptyAllSlots();
            Weapon w = states.inventoryManager.currentWeapon;
            for (int i = 0; i < w.thActions.Count; i++)
            {
                Action a = GetActionEntry(w.thActions[i].input);
                a.targetAnimation = w.thActions[i].targetAnimation;
            }
        }

        void EmptyAllSlots()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = GetActionEntry((ActionInput)i);
                a.targetAnimation = null;
            }
        }

        ActionManager()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = new Action();
                a.input = (ActionInput)i;
                actionSlots.Add(a);
            }
        }

        public Action GetActionSlot(StateManager st)
        {
            ActionInput a_input = GetActionInput(st);
            return GetActionEntry(a_input);
        }

        Action GetActionEntry(ActionInput inp)
        {
            for ( int i = 0; i< actionSlots.Count; i++)
            {
                if (actionSlots[i].input == inp)
                    return actionSlots[i];
            }
            return null;
        }

        public ActionInput GetActionInput(StateManager st)
        {
            if (st.rb) return ActionInput.rb;
            if (st.lb) return ActionInput.lb;
            if (st.rt) return ActionInput.rt;
            if (st.lt) return ActionInput.lt;

            return ActionInput.rb;
        }
    }

    public enum ActionInput
    {
       rb,lb,rt,lt
    }

    [System.Serializable]
    public class Action
    {
        public ActionInput input;
        public string targetAnimation;
    }
}
