using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {
	private bool bStart = false ;
	private bool bPlay = false ;
	private bool bPause = false ;
	private bool bQuit = false ;
	
	AnimatorController AniCtr ;
	

	private bool MouseClick = false ;
	private bool MouseRelease = false ;
	private Vector3 MouseDownPosition = Vector3.zero ;
	
	public GUITexture TimerBarBackground ;
	public GUITexture TimerBarPoint ;
	public GUITexture PlayPauseBoutton ;
	private bool bDragTimerBar = false ;
	private float nowNormalizedTime = 0.0f ;
	private float timerBarWidth = Screen.width * 0.7f ;
	private Vector2 TimerBarPos = new Vector2( Screen.width * 0.15f , 10f ) ;
	
	public GUITexture SpeedBarBackground ;
	public GUITexture SpeedBarPoint ;
	private bool bDragSpeedBar = false ;
	private float OldAnimationSpeed = 1.0f ;
	private float speedBarHeight = 80f ;

	public GUITexture ZoomBarBackground ;
	public GUITexture ZoomBarPoint ;
	private bool bDragZoomBar = false ;
	private float OldZoomValue = 2.0f ;
	private float zoomBarHeight = 80f ;	
	
	public GUITexture StartEndFrameBarBackground ;
	public GUITexture StartFrameBarPoint ;	
	public GUITexture EndFrameBarPoint ;
	private bool bDragStartFrameBar = false ;
	private bool bDragEndFrameBar = false ;
	private float OldStartEndFrameNormalized = 0.0f ;
	private float StartNormalizedTime = 0.0f ;
	private float EndNormalizedTime = 1.0f ;
	
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
		SetupMyUI() ;
		bStart = AniCtr.StartPlay() ;
		bPlay = AniCtr.GetPlayState() ;
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

		StartEndFrameBarBackground.pixelInset = new Rect( TimerBarPos.x , Screen.height - TimerBarPos.y , timerBarWidth , 20f ) ;		
		float StartFrameSliderValue = TimerBarPos.x - ( TimerBarPoint.pixelInset.width * 0.5f ) + timerBarWidth * StartNormalizedTime ;
		StartFrameBarPoint.pixelInset = new Rect( StartFrameSliderValue , Screen.height - TimerBarPos.y ,10f , 20f ) ;		
		float EndFrameSliderValue = TimerBarPos.x - ( TimerBarPoint.pixelInset.width * 0.5f ) + timerBarWidth * EndNormalizedTime ;
		EndFrameBarPoint.pixelInset = new Rect( EndFrameSliderValue , Screen.height - TimerBarPos.y ,10f , 20f ) ;
	}
	
	void CheckUserCtr()
	{
		if( MouseClick )
		{
			MouseClick = false ;
			MouseDownPosition = Input.mousePosition ;
			if( PlayPauseBoutton.HitTest( Input.mousePosition ) )
			{
				if( bPlay )
					AniCtr.SetPause( !AniCtr.GetPauseState() ) ;
				else
					AniCtr.SetStart( StartNormalizedTime ) ;
				bPlay = AniCtr.GetPlayState() ;
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
			else if( StartFrameBarPoint.HitTest( Input.mousePosition ) )
			{
				bDragStartFrameBar = true ;
				NewStartEndFrameNormalized = OldStartEndFrameNormalized = StartNormalizedTime ;
				nowNormalizedTime = AniCtr.GetNormalizedTime() ;
				bPause = AniCtr.GetPauseState() ;
				if( !bPause )
					AniCtr.SetPause( true ) ;
			}
			else if( EndFrameBarPoint.HitTest( Input.mousePosition ) )
			{
				bDragEndFrameBar = true ;
				NewStartEndFrameNormalized = OldStartEndFrameNormalized = EndNormalizedTime ;
				nowNormalizedTime = AniCtr.GetNormalizedTime() ;
				bPause = AniCtr.GetPauseState() ;
				if( !bPause )
					AniCtr.SetPause( true ) ;
			}
			else
			{
				bRotateCam = true ;
			}
		}
		
		if( MouseRelease )
		{
			MouseRelease = false ;
			if( bDragTimerBar )
			{
				if( !bPause )
					AniCtr.SetPause( false ) ;
				bDragTimerBar = false ;
			}
			else if( bDragStartFrameBar )
			{
				bDragStartFrameBar = false ;
				StartNormalizedTime = NewStartEndFrameNormalized ;
				if( !bPause )
					AniCtr.SetPause( false ) ;
			}
			else if( bDragEndFrameBar )
			{
				bDragEndFrameBar = false ;
				EndNormalizedTime = NewStartEndFrameNormalized ;
				if( !bPause )
					AniCtr.SetPause( false ) ;
			}
			bDragSpeedBar = bRotateCam = bDragZoomBar = false ;
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
		else if( bDragStartFrameBar )
		{
			if( nowNormalizedTime < NewStartEndFrameNormalized )
			{
				nowNormalizedTime = NewStartEndFrameNormalized ;
				AniCtr.SetNormalizedTime( nowNormalizedTime ) ;
			}
			//nowNormalizedTime = Mathf.Clamp( nowNormalizedTime , NewStartEndFrameNormalized , EndNormalizedTime ) ;
			//AniCtr.SetNormalizedTime( nowNormalizedTime ) ;
		}
		else if( bDragEndFrameBar )
		{
			if( nowNormalizedTime > NewStartEndFrameNormalized )
			{
				nowNormalizedTime = NewStartEndFrameNormalized ;
				AniCtr.SetNormalizedTime( nowNormalizedTime ) ;
			}
			//nowNormalizedTime = Mathf.Clamp( nowNormalizedTime , StartNormalizedTime , NewStartEndFrameNormalized ) ;
			//AniCtr.SetNormalizedTime( nowNormalizedTime ) ;
		}
		float PlaySliderValue = TimerBarPos.x - ( TimerBarPoint.pixelInset.width * 0.5f ) + timerBarWidth * AniCtr.GetNormalizedTime() ;
		TimerBarPoint.pixelInset = new Rect( PlaySliderValue , TimerBarPos.y , 10f , 20f ) ;
		
		if( !bPlay )
			return ;
		
		if( AniCtr.GetNormalizedTime() > EndNormalizedTime )
		{
			AniCtr.SetFinish() ;
			bPlay = AniCtr.GetPlayState() ;
		}
	}

	void SpeedBar()
	{
		if( !bDragSpeedBar )
			return ;
		
		float yGap = Input.mousePosition.y - MouseDownPosition.y ;
		float newSpeed = OldAnimationSpeed + ( yGap / speedBarHeight * ( 2f - 0.5f ) ) ;
		newSpeed = Mathf.Clamp( newSpeed , 0.5f , 2f ) ;
		AniCtr.SetAnimationSpeed( newSpeed ) ;
		float SpeedSliderValue = speedBarHeight * ( AniCtr.GetAnimationSpeed() - 0.5f ) / ( 2f - 0.5f ) + SpeedBarBackground.pixelInset.y - SpeedBarPoint.pixelInset.height ;
		SpeedBarPoint.pixelInset = new Rect( Screen.width - 50f , SpeedSliderValue , 20f , 10f ) ;
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
	
	float NewStartEndFrameNormalized = 0.0f ;
	void StartEndFrameBar()
	{
		if( bDragStartFrameBar )
		{
			float xGap = Input.mousePosition.x - MouseDownPosition.x ;
			NewStartEndFrameNormalized = OldStartEndFrameNormalized + ( xGap / timerBarWidth ) ;
			NewStartEndFrameNormalized = Mathf.Clamp( NewStartEndFrameNormalized , 0f , EndNormalizedTime - 0.05f ) ;
			float StartFrameSliderValue = TimerBarPos.x - ( TimerBarPoint.pixelInset.width * 0.5f ) + timerBarWidth * NewStartEndFrameNormalized ;
			StartFrameBarPoint.pixelInset = new Rect( StartFrameSliderValue , Screen.height - TimerBarPos.y ,10f , 20f ) ;		
		}
		else if( bDragEndFrameBar )
		{
			float xGap = Input.mousePosition.x - MouseDownPosition.x ;
			NewStartEndFrameNormalized = OldStartEndFrameNormalized + ( xGap / timerBarWidth ) ;
			NewStartEndFrameNormalized = Mathf.Clamp( NewStartEndFrameNormalized , StartNormalizedTime + 0.05f , 1f ) ;
			float EndFrameSliderValue = TimerBarPos.x - ( TimerBarPoint.pixelInset.width * 0.5f ) + timerBarWidth * NewStartEndFrameNormalized ;
			EndFrameBarPoint.pixelInset = new Rect( EndFrameSliderValue , Screen.height - TimerBarPos.y ,10f , 20f ) ;
		}
	}
	
 	void SetStrokes( int StrokesIndex )
	{
		bPause = AniCtr.GetPauseState() ;
		if( !bPause )
			AniCtr.SetPause( true ) ;
		AnimatorController.AnimationFrame StrokesFrame = AniCtr.GetAnimationFrame( StrokesIndex ) ;
		StartNormalizedTime = AniCtr.CalculateNormalizedTime( StrokesFrame.StartFrame ) ;
		EndNormalizedTime = AniCtr.CalculateNormalizedTime( StrokesFrame.EndFrame ) ;
		
		float StartFrameSliderValue = TimerBarPos.x - ( TimerBarPoint.pixelInset.width * 0.5f ) + timerBarWidth * StartNormalizedTime ;
		StartFrameBarPoint.pixelInset = new Rect( StartFrameSliderValue , Screen.height - TimerBarPos.y ,10f , 20f ) ;		
		float EndFrameSliderValue = TimerBarPos.x - ( TimerBarPoint.pixelInset.width * 0.5f ) + timerBarWidth * EndNormalizedTime ;
		EndFrameBarPoint.pixelInset = new Rect( EndFrameSliderValue , Screen.height - TimerBarPos.y ,10f , 20f ) ;
		
		AniCtr.SetNormalizedTime( StartNormalizedTime ) ;
		if( !bPause )
			AniCtr.SetPause( false ) ;
	}
	
	void SetAnimationDescription()
	{
		
		//Fake animation description
		//set 0 , 120-1100
		AniCtr.AddAnimation( "Set 0" , 120 , 770 ) ;
		//set 1 , 120-264
		AniCtr.AddAnimation( "Set 1" , 120 , 264 ) ;
		//set 2 , 265-332
		AniCtr.AddAnimation( "Set 2" , 265 , 332 ) ;
		//set 3 , 333-449
		AniCtr.AddAnimation( "Set 3" , 333 , 449 ) ;
		//set 4 , 450-588
		AniCtr.AddAnimation( "Set 4" , 450 , 588 ) ;
		//set 5 , 589-770
		AniCtr.AddAnimation( "Set 5" , 589 , 770 ) ;
		
	}
	
	void Update()
	{
		if( !bStart && bQuit )
			return ;
		
		if( Input.GetMouseButtonDown( 0 ) )
			MouseClick = true ;
		if( Input.GetMouseButtonUp( 0 ) )
			MouseRelease = true ;
		
		if( Input.GetKeyDown( KeyCode.Alpha1 ) )
			SetStrokes( 1 ) ;
		else if( Input.GetKeyDown( KeyCode.Alpha2 ) )
			SetStrokes( 2 ) ;
		else if( Input.GetKeyDown( KeyCode.Alpha3 ) )
			SetStrokes( 3 ) ;
		else if( Input.GetKeyDown( KeyCode.Alpha4 ) )
			SetStrokes( 4 ) ;
		else if( Input.GetKeyDown( KeyCode.Alpha5 ) )
			SetStrokes( 5 ) ;
		
		CheckUserCtr() ;
		TimerBar() ;
		SpeedBar() ;
		CameraRotate() ;
		ZoomBar() ;
		StartEndFrameBar() ;
	}
	
	void OnGUI()
	{
 		//debug information
		GUI.Label( new Rect( 5f , 5f , Screen.width , 20f ) , "Animation Bar : Bule" ) ;
		GUI.Label( new Rect( 5f , 25f , Screen.width , 20f ) , "Speed Bar : Red" ) ;
		GUI.Label( new Rect( 5f , 45f , Screen.width , 20f ) , "Zoom Bar : Green" ) ;
		GUI.Label( new Rect( 5f , 65f , Screen.width , 20f ) , "Start Frame Bar : Yellow Black" ) ;
		//GUI.Label( new Rect( 5f , 65f , Screen.width , 20f ) , "bRotateCam : " + bRotateCam ) ;
    }
}