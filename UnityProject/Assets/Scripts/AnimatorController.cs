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
		animator.speed = AnimationSpeed ;
	}
	
	public bool StartPlay()
	{
		if( animations.Count <= 0 )
			return false ;
		
		StartCoroutine( AnimationController() ) ;
		return true ;
	}
	
	public void AddAnimation( string AnimationName , int StartFrame , int EndFrame )
	{
		AnimationFrame NewAni = new AnimationFrame() ;
		NewAni.AnimationName = AnimationName ;
		NewAni.StartFrame = StartFrame ;
		NewAni.EndFrame = EndFrame ;
		animations.Add( NewAni ) ;
	}
	
	private float AnimationTimer = 0f ;
	private float AnimationSpeed = 1f ;
	IEnumerator AnimationController()
	{
		bPlay = true ;
		animator.ForceStateNormalizedTime( AnimationTimer ) ;
		float animationTotalTime = ( animations[0].EndFrame - animations[0].StartFrame ) / 30 ;
		float frameNormalizedTime = 1 / animationTotalTime ;

		while( !bQuit )
		{
			if( !bPause )
			{
				AnimationTimer += ( myFrameTime * frameNormalizedTime * AnimationSpeed ) ;				
				animator.ForceStateNormalizedTime( AnimationTimer ) ;
			}
			yield return new WaitForSeconds( myFrameTime ) ;
		}
	}
	
	public void SetFinish()
	{
		bPlay = false ;
		SetPause( true ) ;
	}

	public void SetStart( float StartNormalizedTime )
	{
		bPlay = true ;
		AnimationTimer = StartNormalizedTime ;
		animator.ForceStateNormalizedTime( AnimationTimer ) ;
		SetPause( false ) ;
	}

	public bool GetPlayState(){return bPlay;}
	public bool GetPauseState(){return bPause;}
	public void SetPause( bool NewPause )
	{
		bPause = NewPause ;
		//need set speed 1 to blend animation
		if( !bPause )
		{
			animator.speed = AnimationSpeed ;
		}
		else
		{
			animator.speed = 0f ;
		}
	}
	
	public float GetNormalizedTime(){return AnimationTimer;}
	public void SetNormalizedTime( float NewNormalizedTime )
	{
		AnimationTimer = NewNormalizedTime ;
		animator.ForceStateNormalizedTime( AnimationTimer ) ;
	}
	
	public float GetAnimationSpeed(){return AnimationSpeed;}
	public void SetAnimationSpeed( float NewSpeed )
	{
		AnimationSpeed = NewSpeed ;
		if( !bPause )
			animator.speed = AnimationSpeed ;
	}
	
	public float CalculateNormalizedTime( float FrameIndex )
	{
		return ( ( FrameIndex - animations[0].StartFrame ) / ( animations[0].EndFrame - animations[0].StartFrame ) ) ;
	}
	
	public AnimationFrame GetAnimationFrame( int index )
	{
		return animations[index] ;
	}
}