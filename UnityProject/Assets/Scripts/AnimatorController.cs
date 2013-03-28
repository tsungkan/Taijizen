using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatorController : MonoBehaviour {
	public class AnimationFrame
	{
		public string AnimationName ;
		public int StartFrame ;
		public int EndFrame ;
	}
	
	private bool bPlay = false ;
	private bool bQuit = false ;
	private bool bPause = false ;
	float myFrameTime = 1f / 60f ;
	
	private Animator animator ;
	List<AnimationFrame> animations = new List<AnimationFrame>() ;
	
	public void SetAnimator( Animator animatorObj )
	{
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
		animator.speed = animationSpeed ;
	}
	
	public bool StartPlay()
	{
		if( animations.Count <= 0 )
			return false ;
		
		StartCoroutine( AnimationController() ) ;
		return true ;
	}
	
	public void AddAnimation( string animationName , int startFrame , int endFrame )
	{
		AnimationFrame newAni = new AnimationFrame() ;
		newAni.AnimationName = animationName ;
		newAni.StartFrame = startFrame ;
		newAni.EndFrame = endFrame ;
		animations.Add( newAni ) ;
	}
	
	private float animationTimer = 0f ;
	private float animationSpeed = 1f ;
	IEnumerator AnimationController()
	{
		bPlay = true ;
		animator.ForceStateNormalizedTime( animationTimer ) ;
		float animationTotalTime = ( animations[0].EndFrame - animations[0].StartFrame ) / 30 ;
		float frameNormalizedTime = 1 / animationTotalTime ;

		while( !bQuit )
		{
			if( !bPause )
			{
				animationTimer += ( myFrameTime * frameNormalizedTime * animationSpeed ) ;				
				animator.ForceStateNormalizedTime( animationTimer ) ;
			}
			yield return new WaitForSeconds( myFrameTime ) ;
		}
	}
	
	public void SetFinish()
	{
		bPlay = false ;
		SetPause( true ) ;
	}

	public void SetStart( float startNormalizedTime )
	{
		bPlay = true ;
		animationTimer = startNormalizedTime ;
		animator.ForceStateNormalizedTime( animationTimer ) ;
		SetPause( false ) ;
	}

	public bool GetPlayState(){return bPlay;}
	public bool GetPauseState(){return bPause;}
	public void SetPause( bool newPause )
	{
		bPause = newPause ;
		//need set speed 1 to blend animation
		if( !bPause )
		{
			animator.speed = animationSpeed ;
		}
		else
		{
			animator.speed = 0f ;
		}
	}
	
	public float GetNormalizedTime(){return animationTimer;}
	public void SetNormalizedTime( float newNormalizedTime )
	{
		animationTimer = newNormalizedTime ;
		animator.ForceStateNormalizedTime( animationTimer ) ;
	}
	
	public float GetAnimationSpeed(){return animationSpeed;}
	public void SetAnimationSpeed( float newSpeed )
	{
		animationSpeed = newSpeed ;
		if( !bPause )
			animator.speed = animationSpeed ;
	}
	
	public float CalculateNormalizedTime( float frameIndex )
	{
		return ( ( frameIndex - animations[0].StartFrame ) / ( animations[0].EndFrame - animations[0].StartFrame ) ) ;
	}
	
	public AnimationFrame GetAnimationFrame( int index )
	{
		return animations[index] ;
	}
}