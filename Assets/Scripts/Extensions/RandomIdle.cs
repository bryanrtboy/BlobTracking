//Plays a random animation when the "Idle" state is called;

using UnityEngine;
using System.Collections;

public class RandomIdle : StateMachineBehaviour
{

	public GameObject particle;
	public float radius;
	public float power;
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{

		int rand = Random.Range (0, animator.runtimeAnimatorController.animationClips.Length + 1);

		animator.SetInteger ("IdleIndex", rand);
		Debug.Log ("Playing the " + animator.runtimeAnimatorController.animationClips [rand].name + " clip, number is " + rand.ToString ());
	}
	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{

		if (animator.GetBool ("ContinueIndefinitely")) {
			
			int rand = Random.Range (0, animator.runtimeAnimatorController.animationClips.Length + 1);

			Debug.Log ("Picking a new random number - " + rand.ToString ()); 
			animator.SetInteger ("IdleIndex", rand);
		}
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}


