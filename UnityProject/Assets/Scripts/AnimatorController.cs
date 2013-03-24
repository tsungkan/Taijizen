using UnityEngine;
using System.Collections;

public class AnimatorController : MonoBehaviour {
	private bool bPlay = false ;
	private bool bQuit = false ;
	private bool bPause = false ;
	
	private Animator animator ;
	private int FirstState = Animator.StringToHash("Base Layer.Wave") ;
	private int LastState = Animator.StringToHash("Base Layer.WalkBack") ;
	
	public void SetAnimator( Animator animatorObj ){
		animator = animatorObj ;
		if( animator == null )
		{
			animator = this.GetComponent<Animator>() ;
		}
		if( animator == null )
		{
			Debug.LogError( "animator is null !" ) ;
			return ;
		}
		//need set speed 1 to blend animation
		animator.speed = 1f ;
		StartCoroutine( AnimationController() ) ;
	}
	
	IEnumerator AnimationController (){
		bPlay = true ;
		float AnimationTimer = 0f ;
		animator.SetFloat( "fTimer" , AnimationTimer ) ;
		animator.ForceStateNormalizedTime( AnimationTimer ) ;

		while( !bQuit && bPlay )
		{
			if( !bPause )
			{
				AnimationTimer += 0.01f ;
				animator.SetFloat( "fTimer" , AnimationTimer ) ;
				
				if( AnimationTimer > 1.0f )
				{
					if( animator.GetCurrentAnimatorStateInfo(0).nameHash != LastState )
						AnimationTimer -= 1.0f ;
					else
						SetFinish() ;
				}
				animator.ForceStateNormalizedTime( AnimationTimer ) ;
			}
			yield return new WaitForSeconds( 0.01f ) ;
		}
	}
	
	private void SetFinish()
	{
		bPlay = false ;
		SetPause( true ) ;
		//StartCoroutine( WaitReplay() ) ;
	}
	/*
	IEnumerator WaitReplay (){
		while( bPause )
		{
			yield return null ;
		}
		StartCoroutine( AnimationController() ) ;
	}
	*/
	public void SetPause( bool NewPause ){
		bPause = NewPause ;
		//need set speed 1 to blend animation
		if( !bPause )	animator.speed = 1f ;
		else			animator.speed = 0f ;
	}
	
	public bool GetPauseState(){return bPause;}

}
