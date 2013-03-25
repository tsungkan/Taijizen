using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatorController : MonoBehaviour {
	public class AnimationFrame
	{
		public int StartFrame ;
		public int EndFrame ;
	}
	
	private bool bPlay = false ;
	private bool bQuit = false ;
	private bool bPause = false ;
	float myFrameTime = 1f / 30f ;
	
	private Animator animator ;
	List<AnimationFrame> animations = new List<AnimationFrame>() ;
	
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
	}
	
	public void StartPlay()
	{
		/*Check animation assign to animations
		foreach( AnimationFrame AF in animations )
		{
			Debug.Log( AF.StartFrame + "\n" + AF.EndFrame ) ;
		}
		*/
		StartCoroutine( AnimationController() ) ;
	}
	
	public void AddAnimation( int StartFrame , int EndFrame )
	{
		AnimationFrame NewAni = new AnimationFrame() ;
		NewAni.StartFrame = StartFrame ;
		NewAni.EndFrame = EndFrame ;
		animations.Add( NewAni ) ;
	}
	
	float AnimationTimer = 0f ;
	IEnumerator AnimationController (){
		bPlay = true ;
		animator.ForceStateNormalizedTime( AnimationTimer ) ;
		float animationTotalTime = ( animations[0].EndFrame - animations[0].StartFrame ) / 30 ;
		float frameNormalizedTime = 1 / animationTotalTime ;

		while( !bQuit && bPlay )
		{
			if( !bPause )
			{
				AnimationTimer += ( myFrameTime * frameNormalizedTime ) ;
				
				if( AnimationTimer >= 1.0f )
				{
					SetFinish() ;
				}
				animator.ForceStateNormalizedTime( AnimationTimer ) ;
			}
			yield return new WaitForSeconds( myFrameTime ) ;
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
		StartPlay() ;
	}
	*/
	public void SetPause( bool NewPause ){
		bPause = NewPause ;
		//need set speed 1 to blend animation
		if( !bPause )	animator.speed = 1f ;
		else			animator.speed = 0f ;
	}
	
	public bool GetPauseState(){return bPause;}
	public float GetNormalizedTime(){return AnimationTimer;}
	public void SetNormalizedTime( float NewNormalizedTime )
	{
		AnimationTimer = NewNormalizedTime ;
		animator.ForceStateNormalizedTime( AnimationTimer ) ;
	}

}
