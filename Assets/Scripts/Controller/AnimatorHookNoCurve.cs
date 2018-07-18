using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class AnimatorHookNoCurve : MonoBehaviour
    {
        Animator anim;
        StateManagerNoCurve states;
        public float rm_Multiplier;

        public void Init(StateManagerNoCurve st)
        {
            states = st;
            anim = st.anim;
        }

        void OnAnimatorMove()
        {
            if (states.canMove) return;

            states.rigid.drag = 0;

            if (rm_Multiplier == 0) rm_Multiplier = 1;

            Vector3 delta = anim.deltaPosition;
            delta.y = 0;
            Vector3 v = (delta * rm_Multiplier) / states.delta;
            states.rigid.velocity = v;
        }
    }
}