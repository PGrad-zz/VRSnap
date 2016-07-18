using UnityEngine;
using System.Collections;

public class IguanaCharacter : MonoBehaviour, Mover {
	Animator iguanaAnimator;
	private bool moving = false,
				 attacking = false;

	void Start () {
		iguanaAnimator = GetComponent<Animator> ();
		EventManager.RegisterEvent ("Move", getMoving);
		EventManager.RegisterEvent ("Stop", stopMoving);
		EventManager.RegisterGameobject (gameObject, multiplyOnAttack);
	}

	void Update () {
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
	}

	public void Attack(){
		attacking = true;
		iguanaAnimator.SetTrigger("Attack");
		StartCoroutine (Attacking ());
	}
	
	public void Hit(){
		iguanaAnimator.SetTrigger("Hit");
	}

	public void multiplyOnAttack () {
		if (attacking)
			ScoreManager.MultiplyScore (2);
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

	public void getMoving () {
		moving = true;
	}

	public void stopMoving () {
		moving = false;
	}
}
