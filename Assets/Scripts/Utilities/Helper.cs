using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA{

	public class Helper : MonoBehaviour {
		[Range(-1,1)]
		public float vertical;
		[Range(-1,1)]
		public float horizontal;

		public string[] oh_attacks;
		public string[] th_attacks;

		public bool playAnim;
		public bool twoHanded;
		public bool enableRootMotion;
		public bool useItem;
		public bool interacting;
		public bool lockOn;

		Animator anim;

		// Use this for initialization
		void Start () {
			anim = GetComponent<Animator> ();
		}
		
		// Update is called once per frame
		void Update () {

			enableRootMotion = !anim.GetBool ("canMove");
			anim.applyRootMotion = enableRootMotion;

			interacting = anim.GetBool ("interacting");

			if (!lockOn) {
				horizontal = 0;
				vertical = Mathf.Clamp01 (vertical);
			}
			anim.SetBool ("lockOn", lockOn);

			if (enableRootMotion)
				return;

			if (useItem) {
				anim.CrossFade ("use_item", 0.2f);
				useItem = false;
			}

			if (interacting) {
				playAnim = false;
				vertical = Mathf.Clamp (vertical, 0, 0.5f);
			}

			anim.SetBool ("two_handed", twoHanded);
			if (playAnim) {
				string targetAnim;

				if (!twoHanded) {
					int r = Random.Range (0, oh_attacks.Length);
					targetAnim = oh_attacks [r];
				} else {
					int r = Random.Range (0, th_attacks.Length);
					targetAnim = th_attacks [r];
				}
				if (vertical > 0.5F) {
					targetAnim = ("oh_attack_3");
				}
				vertical = 0;
				anim.CrossFade (targetAnim, 0.2f);
				//anim.SetBool ("canMove", false);
				//enableRootMotion = true;
				playAnim = false;
			}
			anim.SetFloat ("vertical", vertical);
			anim.SetFloat ("horizontal", horizontal);
		}
	}
}