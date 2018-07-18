﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA{
	public class StateManagerNoCurve : MonoBehaviour {

		[Header("Init")]
		public GameObject activeModel;

		[Header("Inputs")]
		public float vertical;
		public float horizontal;
		public float moveAmount;
		public Vector3 moveDir;
        public bool rt, rb, lt, lb;
        public bool a, b, x, y;
        public bool rollInput;

        [Header("Stats")]
		public float moveSpeed = 2;
		public float runSpeed = 3.5f;
		public float rotateSpeed = 5;
		public float toGround = 0.5f;
        public float rollSpeed = 1;

		[Header("States")]
		public bool onGround;
		public bool run;
		public bool lockOn;
        public bool inAction;
        public bool canMove;
        public bool isTwoHanded;

        [Header("Other")]
        public EnemyTarget lockOnTarget;
        public Transform lockOnTransform;


        [HideInInspector]
		public Animator anim;
		[HideInInspector]
		public Rigidbody rigid;
        [HideInInspector]
        public AnimatorHookNoCurve a_hook;
		[HideInInspector]
		public float delta;

		[HideInInspector]
		public LayerMask ignoreLayers;

        float _actionDelay;

		public void Init(){
			SetupAnimator();
			rigid = GetComponent<Rigidbody> ();
            rigid.angularDrag = Mathf.Infinity;
            rigid.drag = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            a_hook = activeModel.AddComponent<AnimatorHookNoCurve>();
            a_hook.Init(this);
        
			gameObject.layer = 9;
			ignoreLayers = ~(1 << 10);
			anim.SetBool ("onGround", true);
		}

		void SetupAnimator(){
			if (activeModel == null) {

				anim = GetComponentInChildren<Animator> ();
				if (anim == null) {

					Debug.LogError ("No active model found");
				} else {

					activeModel = anim.gameObject;
				}
			}

			if (anim == null) {

				anim = activeModel.GetComponent<Animator> ();
			}
		}

		public void FixedTick(float d){
			delta = d;

            DetectAction();

            if (inAction)
            {
                anim.applyRootMotion = true;
                _actionDelay += delta;
                if (_actionDelay > 0.3f)
                {
                    inAction = false;
                    _actionDelay = 0;
                }
                else
                {
                    return;
                }
            }

            canMove = anim.GetBool("canMove");

            if (!canMove) return;

            a_hook.rm_Multiplier = 1;
            HandleRolls();
            
            anim.applyRootMotion = false;

            rigid.drag = (moveAmount > 0 || !onGround) ? 0 : 4;

			float targetSpeed = moveSpeed;
			if (run) {
				targetSpeed = runSpeed;
			}

			if (onGround) {
				rigid.velocity = moveDir * (targetSpeed * moveAmount);
			}

			if (run) {
				lockOn = false;
			}

			Vector3 targetDir = (!lockOn) ? moveDir
                : (lockOnTransform != null) ?
                    (lockOnTransform.position - transform.position)
                    : moveDir;
            targetDir.y = 0;
			if (targetDir == Vector3.zero) {
				targetDir = transform.forward;
			}
			Quaternion tr = Quaternion.LookRotation (targetDir);
			Quaternion targetRotation = Quaternion.Slerp (transform.rotation, tr, delta * moveAmount * rotateSpeed);
			transform.rotation = targetRotation;

            anim.SetBool("lockOn", lockOn);

            if (!lockOn)
                HandleMovementAnimations();
            else
                HandleLockOnAnimations(moveDir);
		}

        public void DetectAction()
        {
            if (!canMove) return;
            if(!rb && !rt && !lt && !lb)
            {
                return;
            }
            string targetAnim = null;
            if (rb) targetAnim = "oh_attack_1";
            if (rt) targetAnim = "oh_attack_2";
            if (lb) targetAnim = "oh_attack_3";
            if (lt) targetAnim = "th_attack_1";

            canMove = false;
            inAction = true;
            anim.CrossFade(targetAnim, 0.2f);
            //rigid.velocity = Vector3.zero;
        }

        public void HandleRolls()
        {
            if (!rollInput) return;

            float v = vertical;
            float h = horizontal;

            // Correct way, blending animations, animations MUST be good
            /*if (!lockOn)
            {
                v = (moveAmount > 0.3f) ? 1 : 0;
                h = 0;
            }
            else
            {
                if (Mathf.Abs(v) < 0.3f) v = 0;
                if (Mathf.Abs(h) < 0.3f) h = 0;
            }*/
            // Hack if animations are shit
            if (v != 0)
            {
                v = (moveAmount > 0.3f) ? 1 : 0;
                h = 0;
                if (moveDir == Vector3.zero) moveDir = transform.forward;
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = targetRot;
            }

            a_hook.rm_Multiplier = rollSpeed;

            anim.SetFloat("vertical", v);
            anim.SetFloat("horizontal", h);

            canMove = false;
            inAction = true;
            anim.CrossFade("Rolls", 0.2f);
        }

		public void Tick(float d){
			delta = d;
			onGround = OnGround ();
			anim.SetBool ("onGround", onGround);
		}

		void HandleMovementAnimations(){
			anim.SetBool ("run", run);
			anim.SetFloat ("vertical", moveAmount, 0.4f, delta);

		}

        void HandleLockOnAnimations(Vector3 moveDir)
        {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
            float h = relativeDir.x;
            float v = relativeDir.z;

            anim.SetFloat("vertical", v, 0.2f, delta);
            anim.SetFloat("horizontal", h, 0.2f, delta);
        }

		public bool OnGround(){
			bool r = false;

			Vector3 origin = transform.position + (Vector3.up * toGround);
			Vector3 dir = -Vector3.up;
			float dis = toGround + 0.3f;	
			RaycastHit hit;
			Debug.DrawRay (origin, dir * dis);
			if (Physics.Raycast (origin, dir, out hit, dis, ignoreLayers)) {
				r = true;
				Vector3 targetPositon = hit.point;
				transform.position = targetPositon;
			}
			return r;
		}

        public void HandleTwoHanded()
        {
            anim.SetBool("two_handed", isTwoHanded);
        }
    }
}