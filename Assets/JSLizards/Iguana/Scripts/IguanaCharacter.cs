﻿using UnityEngine;
using System.Collections;

public class IguanaCharacter : SpecialMoment, IMover {
	Animator iguanaAnimator;
	private bool moving = false,
				 attacking = false;

	void Awake () {
		specialIconMaterial = (Material) Resources.Load ("SpecialIconMaterial/TimeMaterial");
	}

	void Start () {
		base.RegisterAndScale ();
		iguanaAnimator = GetComponent<Animator> ();
		EventManager.RegisterEvent ("Start", startMoving);
		EventManager.RegisterEvent ("Move", getMoving);
		EventManager.RegisterEvent ("Stop", stopMoving);
		EventManager.RegisterGameObject (gameObject, multiplyOnAttack);
	}
		

	protected override void Update () {
		base.Update ();
		if (moving)
			Move (1f, 0f);
		else
			Move (0f, 0f);
		if (Random.Range (1, 60) == 30) 
			Attack ();
	}

	private IEnumerator Attacking () {
		yield return new WaitForSeconds (3f);
		attacking = false;
		HideSpecial ();
	}

	public void Attack() {
		ShowSpecial ();
		attacking = true;
		iguanaAnimator.SetTrigger("Attack");
		StartCoroutine (Attacking ());
	}
	
	public void Hit(){
		iguanaAnimator.SetTrigger("Hit");
	}

	public void multiplyOnAttack () {
		if (attacking) {
			ScoreManager.AddSpecial ();
			SpecialIsInvoked ();
		}
	}
	
	/*public void Eat(){
		iguanaAnimator.SetTrigger("Eat");
	}*/

	/*public void Death(){
		iguanaAnimator.SetTrigger("Death");
	}*/

	/*public void Rebirth(){
		iguanaAnimator.SetTrigger("Rebirth");
	}*/

	public void Move(float v,float h){
		iguanaAnimator.SetFloat ("Forward", v);
		iguanaAnimator.SetFloat ("Turn", h);
	}

	public void startMoving () {
		getMoving ();
	}
	
	public void getMoving () {
		moving = true;
	}

	public void stopMoving () {
		moving = false;
	}
}
