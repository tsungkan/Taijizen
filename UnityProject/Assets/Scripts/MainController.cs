using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {
	private bool bQuit = false ;
	private bool bPause = false ;
	private float myFrameTime = 1f / 30f ;
	
	AnimatorController AniCtr ;
	private float StartNormalizedTime = 0.0f ;
	private float EndNormalizedTime = 1.0f ;
	
	public GUITexture TimerBarBackground ;
	public GUITexture TimerBarPoint ;
	public GUITexture PlayPauseBoutton ;
	private float timerBarWidth = Screen.width * 0.7f ;
	private Vector2 TimerBarPos = new Vector2( Screen.width * 0.15f , 10f ) ;

	private bool MouseClick = false ;
	private bool MouseRelease = false ;
	private bool bDragTimerBar = false ;
	private Vector3 MouseDownPosition = Vector3.zero ;
	private float nowNormalizedTime = 0.0f ;
	
	void Awake () {
		AniCtr = this.gameObject.AddComponent<AnimatorController>() ;
		AniCtr.SetAnimator( this.GetComponent<Animator>() ) ;
		SetAnimationDescription() ;
		AniCtr.StartPlay() ;
		SetupMyUI() ;		
	}
	
	void SetupMyUI()
	{
		TimerBarBackground.pixelInset = new Rect( TimerBarPos.x , TimerBarPos.y , timerBarWidth , 20 ) ;
		TimerBarPoint.pixelInset = new Rect( TimerBarPos.x - ( TimerBarPoint.pixelInset.width * 0.5f ) , TimerBarPos.y ,10f , 20 ) ;
	}
	
	void Start()
	{
		StartCoroutine( CheckUserCtr() ) ;
		StartCoroutine( TimerBarController() ) ;
	}
	
	IEnumerator CheckUserCtr()
	{
		while( !bQuit )
		{
			if( MouseClick )
			{
				MouseClick = false ;
				if( TimerBarPoint.HitTest( Input.mousePosition ) )
				{
					bDragTimerBar = true ;
					bPause = AniCtr.GetPauseState() ;
					if( !bPause )
						AniCtr.SetPause( true ) ;
					MouseDownPosition = Input.mousePosition ;
					nowNormalizedTime = AniCtr.GetNormalizedTime() ;
				}
				else if( PlayPauseBoutton.HitTest( Input.mousePosition ) )
				{
					AniCtr.SetPause( !AniCtr.GetPauseState() ) ;
				}
			}
			
			if( MouseRelease )
			{
				MouseRelease = false ;
				if( bDragTimerBar )
				{
					bDragTimerBar = false ;
					if( !bPause )
						AniCtr.SetPause( false ) ;
				}
			}			
			yield return new WaitForSeconds( myFrameTime ) ;
		}
	}
	
	IEnumerator TimerBarController()
	{
		while( !bQuit )
		{
			if( bDragTimerBar )
			{
				float xGap = Input.mousePosition.x - MouseDownPosition.x ;
				float newNormalizedTime = nowNormalizedTime + ( xGap / timerBarWidth ) ;
				newNormalizedTime = Mathf.Clamp( newNormalizedTime , StartNormalizedTime , EndNormalizedTime ) ;
				AniCtr.SetNormalizedTime( newNormalizedTime ) ;
			}
			
			float PlaySliderValue = TimerBarPos.x - ( TimerBarPoint.pixelInset.width * 0.5f ) + timerBarWidth * AniCtr.GetNormalizedTime() ;
			TimerBarPoint.pixelInset = new Rect( PlaySliderValue , TimerBarPos.y , 10f , 20f ) ;

			yield return new WaitForSeconds( myFrameTime ) ;
		}
	}

	void SetAnimationDescription()
	{
		//Fake animation description
		//set 0 , 120-1100
		AniCtr.AddAnimation( 120 , 770 ) ;
		//set 1 , 120-264
		AniCtr.AddAnimation( 120 , 264 ) ;
		//set 2 , 265-332
		AniCtr.AddAnimation( 265 , 332 ) ;
		//set 3 , 333-449
		AniCtr.AddAnimation( 333 , 449 ) ;
		//set 4 , 450-588
		AniCtr.AddAnimation( 450 , 588 ) ;
		//set 5 , 589-770
		AniCtr.AddAnimation( 589 , 770 ) ;
	}
	
	void FixedUpdate()
	{
		if( Input.GetMouseButtonDown( 0 ) )
			MouseClick = true ;
		if( Input.GetMouseButtonUp( 0 ) )
			MouseRelease = true ;
	}
	
    void OnGUI() {
 		//debug information
		
		GUI.Label( new Rect( 5f , 5f , Screen.width , 20f ) , "NormalizedTime : " + AniCtr.GetNormalizedTime() ) ;
		GUI.Label( new Rect( 5f , 25f , Screen.width , 20f ) , "MouseClick : " + MouseClick ) ;
		GUI.Label( new Rect( 5f , 45f , Screen.width , 20f ) , "MouseRelease : " + MouseRelease ) ;
    }
	
}
