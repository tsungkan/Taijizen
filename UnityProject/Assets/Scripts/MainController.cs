using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {
	AnimatorController AniCtr ;
	
	void Start () {
		AniCtr = this.gameObject.AddComponent<AnimatorController>() ;
		AniCtr.SetAnimator( this.GetComponent<Animator>() ) ;
	}
	
	void Update () {
		if( Input.GetKeyDown( KeyCode.P ) )
		{
			AniCtr.SetPause( !AniCtr.GetPauseState() ) ;
		}
	}
}
