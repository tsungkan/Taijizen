using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {
	private bool bQuit = false ;
	private bool bPause = false ;
	private float myFrameTime = 1f / 60f ;
	
	AnimatorController AniCtr ;
	private float StartNormalizedTime = 0.0f ;
	private float EndNormalizedTime = 1.0f ;
	
	public GUITexture TimerBarBackground ;
	public GUITexture TimerBarPoint ;
	public GUITexture PlayPauseBoutton ;
	public GUITexture SpeedBarBackground ;
	public GUITexture SpeedBarPoint ;
	public GUITexture ZoomBarBackground ;
	public GUITexture ZoomBarPoint ;
	private float timerBarWidth = Screen.width * 0.7f ;
	private Vector2 TimerBarPos = new Vector2( Screen.width * 0.15f , 10f ) ;
	private float speedBarHeight = 80f ;
	private float zoomBarHeight = 80f ;
	

	private bool MouseClick = false ;
	private bool MouseRelease = false ;
	private Vector3 MouseDownPosition = Vector3.zero ;
	
	private bool bDragTimerBar = false ;
	private float nowNormalizedTime = 0.0f ;
	private bool bDragSpeedBar = false ;
	private float OldAnimationSpeed = 1.0f ;
	private bool bDragZoomBar = false ;
	private float OldZoomValue = 2.0f ;
	
	
	CameraController CamCtr ;
	private bool bRotateCam = false ;
	/*
	enum MouseState
	{
		None , DragTimerBar , DragSpeedBar , RotateCam 
	}
	*/
	void Awake ()
	{
		CamCtr = Camera.main.GetComponent<CameraController>() ;
		
		AniCtr = this.gameObject.AddComponent<AnimatorController>() ;
		AniCtr.SetAnimator( this.GetComponent<Animator>() ) ;
		SetAnimationDescription() ;
		AniCtr.StartPlay() ;
		SetupMyUI() ;
	}
	
	void SetupMyUI()
	{
		TimerBarBackground.pixelInset = new Rect( TimerBarPos.x , TimerBarPos.y , timerBarWidth , 20f ) ;
		TimerBarPoint.pixelInset = new Rect( TimerBarPos.x - ( TimerBarPoint.pixelInset.width * 0.5f ) , TimerBarPos.y ,10f , 20f ) ;
		PlayPauseBoutton.pixelInset = new Rect( TimerBarPos.x - 40f , TimerBarPos.y ,20f , 20f ) ;
		
		SpeedBarBackground.pixelInset = new Rect( Screen.width - 50f , 10f , 20f , speedBarHeight ) ;
		SpeedBarPoint.pixelInset = new Rect( Screen.width - 50f , speedBarHeight * ( AniCtr.GetAnimationSpeed() - 0.5f ) / ( 2f - 0.5f ) + SpeedBarBackground.pixelInset.y , 20f , 10f ) ;//15-75
		
		ZoomBarBackground.pixelInset = new Rect( Screen.width - 50f , 150f , 20f , zoomBarHeight ) ;
		ZoomBarPoint.pixelInset =  new Rect( Screen.width - 50f , zoomBarHeight * ( CamCtr.GetCameraZoom() - 2f ) / ( 5f - 2f ) + ZoomBarBackground.pixelInset.y , 20f , 10f ) ;//15-75
	}
	
	void CheckUserCtr()
	{
		if( MouseClick )
		{
			MouseClick = false ;
			MouseDownPosition = Input.mousePosition ;
			if( PlayPauseBoutton.HitTest( Input.mousePosition ) )
			{
				AniCtr.SetPause( !AniCtr.GetPauseState() ) ;
			}
			else if( TimerBarPoint.HitTest( Input.mousePosition ) )
			{
				bDragTimerBar = true ;
				bPause = AniCtr.GetPauseState() ;
				if( !bPause )
					AniCtr.SetPause( true ) ;
				
				nowNormalizedTime = AniCtr.GetNormalizedTime() ;
			}
			else if( SpeedBarPoint.HitTest( Input.mousePosition ) )
			{
				bDragSpeedBar = true ;
				OldAnimationSpeed = AniCtr.GetAnimationSpeed() ;
			}
			else if( ZoomBarPoint.HitTest( Input.mousePosition ) )
			{
				bDragZoomBar = true ;
				OldZoomValue = CamCtr.GetCameraZoom() ;
			}
			else 
			{
				bRotateCam = true ;
				CamCtr.ReadyRotate() ;
			}
		}
		
		if( MouseRelease )
		{
			MouseRelease = false ;
			if( bDragTimerBar )
			{
				if( !bPause )
					AniCtr.SetPause( false ) ;
			}
			bDragTimerBar =	bDragSpeedBar = bRotateCam = bDragZoomBar = false ;
		}
	}
	
	void TimerBar()
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
	}

	void SpeedBar()
	{
		if( !bDragSpeedBar )
			return ;
		
		float yGap = Input.mousePosition.y - MouseDownPosition.y ;
		float newSpeed = OldAnimationSpeed + ( yGap / speedBarHeight * ( 2f - 0.5f ) ) ;
		newSpeed = Mathf.Clamp( newSpeed , 0.5f , 2f ) ;
		AniCtr.SetAnimationSpeed( newSpeed ) ;
		float SpeedSliderValue = speedBarHeight * ( AniCtr.GetAnimationSpeed() - 0.5f ) / ( 2f - 0.5f ) + SpeedBarBackground.pixelInset.y  - SpeedBarPoint.pixelInset.height ;
		SpeedBarPoint.pixelInset = new Rect( Screen.width - 50f , speedBarHeight * ( AniCtr.GetAnimationSpeed() - 0.5f ) / 1.5f + 10f , 20f , 10f ) ;
	}
	
	void ZoomBar()
	{
		if( !bDragZoomBar )
			return ;
		
		float yGap = Input.mousePosition.y - MouseDownPosition.y ;
		float newZoom = OldZoomValue + ( yGap / speedBarHeight * ( 5f - 2f ) ) ;
		newZoom = Mathf.Clamp( newZoom , 2f , 5f ) ;
		CamCtr.SetCameraZoom( newZoom ) ;
		float ZoomSliderValue = zoomBarHeight * ( CamCtr.GetCameraZoom() - 2f ) / ( 5f - 2f ) + ZoomBarBackground.pixelInset.y - ZoomBarPoint.pixelInset.height ;
		ZoomBarPoint.pixelInset =  new Rect( Screen.width - 50f , ZoomSliderValue , 20f , 10f ) ;//15-75
	}
	
	void CameraRotate()
	{
		if( !bRotateCam )
			return ;
		
		float xGap = Input.mousePosition.x - MouseDownPosition.x ;
		CamCtr.MyRotate( xGap ) ;
	}
	
	void SetAnimationDescription()
	{
		//Fake animation description
		//set 0 , 120-1100
		AniCtr.AddAnimation( "set 0" , 120 , 770 ) ;
		//set 1 , 120-264
		AniCtr.AddAnimation( "set 1" , 120 , 264 ) ;
		//set 2 , 265-332
		AniCtr.AddAnimation( "set 2" , 265 , 332 ) ;
		//set 3 , 333-449
		AniCtr.AddAnimation( "set 3" , 333 , 449 ) ;
		//set 4 , 450-588
		AniCtr.AddAnimation( "set 4" , 450 , 588 ) ;
		//set 5 , 589-770
		AniCtr.AddAnimation( "set 5" , 589 , 770 ) ;
	}
	
	void Update()
	{
		if( Input.GetMouseButtonDown( 0 ) )
			MouseClick = true ;
		if( Input.GetMouseButtonUp( 0 ) )
			MouseRelease = true ;
		
		CheckUserCtr() ;
		TimerBar() ;
		SpeedBar() ;
		CameraRotate() ;
		ZoomBar() ;
	}
	
 	void Start()
	{
	}
	
	void OnGUI()
	{
 		//debug information
		
		GUI.Label( new Rect( 5f , 5f , Screen.width , 20f ) , "Animation Bar : Bule" ) ;
		GUI.Label( new Rect( 5f , 25f , Screen.width , 20f ) , "Speed Bar : Red" ) ;
		GUI.Label( new Rect( 5f , 45f , Screen.width , 20f ) , "Zoom Bar : Green" ) ;
		//GUI.Label( new Rect( 5f , 65f , Screen.width , 20f ) , "bRotateCam : " + bRotateCam ) ;
    }
}