using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {
	private bool bStart = false ;
	private bool bPlay = false ;
	private bool bPause = false ;
	private bool bQuit = false ;
	
	AnimatorController aniCtr ;
	

	private bool mouseClick = false ;
	private bool mouseRelease = false ;
	private Vector3 mouseDownPosition = Vector3.zero ;
	
	public GUITexture timerBarBackground ;
	public GUITexture timerBarPoint ;
	public GUITexture playPauseBoutton ;
	private bool bDragTimerBar = false ;
	private float oldNormalizedTime = 0.0f ;
	private float timerBarWidth = Screen.width * 0.7f ;
	private Vector2 timerBarPos = new Vector2( Screen.width * 0.15f , 10f ) ;
	
	public GUITexture speedBarBackground ;
	public GUITexture speedBarPoint ;
	private bool bDragSpeedBar = false ;
	private float oldAnimationSpeed = 1.0f ;
	private float speedBarHeight = 80f ;

	public GUITexture zoomBarBackground ;
	public GUITexture zoomBarPoint ;
	private bool bDragZoomBar = false ;
	private float oldZoomValue = 60f ;
	private float zoomBarHeight = 80f ;
	
	public GUITexture startEndFrameBarBackground ;
	public GUITexture startFrameBarPoint ;	
	public GUITexture endFrameBarPoint ;
	private bool bDragStartFrameBar = false ;
	private bool bDragEndFrameBar = false ;
	private float oldStartEndFrameNormalized = 0.0f ;
	private float startNormalizedTime = 0.0f ;
	private float endNormalizedTime = 1.0f ;
	
	CameraController camCtr ;
	private bool bRotateCam = false ;
	/*
	enum MouseState
	{
		None , DragTimerBar , DragSpeedBar , RotateCam 
	}
	*/
	void Awake ()
	{
		camCtr = Camera.main.GetComponent<CameraController>() ;
		
		aniCtr = this.gameObject.AddComponent<AnimatorController>() ;
		aniCtr.SetAnimator( this.GetComponent<Animator>() ) ;
		SetAnimationDescription() ;
		SetupMyUI() ;
		bStart = aniCtr.StartPlay() ;
		bPlay = aniCtr.GetPlayState() ;
	}
	
	float barPointLongSide = 20f ;
	float barPointShortSide = 10f ;
	
	void SetupMyUI()
	{
		timerBarBackground.pixelInset = new Rect( timerBarPos.x , timerBarPos.y , timerBarWidth , barPointLongSide ) ;
		float playSliderValue = CalculateBarPointPos( timerBarPos.x , barPointShortSide , aniCtr.GetNormalizedTime() , startNormalizedTime , endNormalizedTime , timerBarWidth ) ;
		timerBarPoint.pixelInset = new Rect( playSliderValue , timerBarPos.y , barPointShortSide , barPointLongSide ) ;
		
		playPauseBoutton.pixelInset = new Rect( timerBarPos.x - 40f , timerBarPos.y , barPointLongSide , barPointLongSide ) ;
		
		speedBarBackground.pixelInset = new Rect( Screen.width - 50f , 10f , 20f , speedBarHeight ) ;
		float speedSliderValue = CalculateBarPointPos( speedBarBackground.pixelInset.y , barPointShortSide , aniCtr.GetAnimationSpeed() , 0.5f , 2f , speedBarHeight ) ;
		speedBarPoint.pixelInset = new Rect( Screen.width - 50f , speedSliderValue , barPointLongSide , barPointShortSide ) ;//15-75
		
		zoomBarBackground.pixelInset = new Rect( Screen.width - 50f , 150f , 20f , zoomBarHeight ) ;
		float zoomSliderValue = CalculateBarPointPos( zoomBarBackground.pixelInset.y , barPointShortSide , camCtr.GetCameraZoom() , 60f , 100f , zoomBarHeight ) ;
		zoomBarPoint.pixelInset =  new Rect( Screen.width - 50f , zoomSliderValue , barPointLongSide , 10f ) ;//15-75

		startEndFrameBarBackground.pixelInset = new Rect( timerBarPos.x , Screen.height - timerBarPos.y , timerBarWidth , 20f ) ;		
		float startFrameSliderValue = CalculateBarPointPos( timerBarPos.x , barPointLongSide , startNormalizedTime , 0f , 1f , timerBarWidth ) ;
		startFrameBarPoint.pixelInset = new Rect( startFrameSliderValue , Screen.height - timerBarPos.y , barPointShortSide , barPointLongSide ) ;
		float endFrameSliderValue = CalculateBarPointPos( timerBarPos.x , barPointLongSide , endNormalizedTime , 0f , 1f , timerBarWidth ) ;
		endFrameBarPoint.pixelInset = new Rect( endFrameSliderValue , Screen.height - timerBarPos.y , barPointShortSide , barPointLongSide ) ;
	}
	
	void CheckUserCtr()
	{
		if( mouseClick )
		{
			mouseClick = false ;
			mouseDownPosition = Input.mousePosition ;
			if( playPauseBoutton.HitTest( Input.mousePosition ) )
			{
				if( bPlay )
					aniCtr.SetPause( !aniCtr.GetPauseState() ) ;
				else
					aniCtr.SetStart( startNormalizedTime ) ;
				bPlay = aniCtr.GetPlayState() ;
			}
			else if( timerBarPoint.HitTest( Input.mousePosition ) )
			{
				bDragTimerBar = true ;
				bPause = aniCtr.GetPauseState() ;
				if( !bPause )
					aniCtr.SetPause( true ) ;
				
				oldNormalizedTime = aniCtr.GetNormalizedTime() ;
			}
			else if( speedBarPoint.HitTest( Input.mousePosition ) )
			{
				bDragSpeedBar = true ;
				oldAnimationSpeed = aniCtr.GetAnimationSpeed() ;
			}
			else if( zoomBarPoint.HitTest( Input.mousePosition ) )
			{
				bDragZoomBar = true ;
				oldZoomValue = camCtr.GetCameraZoom() ;
			}
			else if( startFrameBarPoint.HitTest( Input.mousePosition ) )
			{
				bDragStartFrameBar = true ;
				NewStartEndFrameNormalized = oldStartEndFrameNormalized = startNormalizedTime ;
				oldNormalizedTime = aniCtr.GetNormalizedTime() ;
				bPause = aniCtr.GetPauseState() ;
				if( !bPause )
					aniCtr.SetPause( true ) ;
			}
			else if( endFrameBarPoint.HitTest( Input.mousePosition ) )
			{
				bDragEndFrameBar = true ;
				NewStartEndFrameNormalized = oldStartEndFrameNormalized = endNormalizedTime ;
				oldNormalizedTime = aniCtr.GetNormalizedTime() ;
				bPause = aniCtr.GetPauseState() ;
				if( !bPause )
					aniCtr.SetPause( true ) ;
			}
			else
			{
				bRotateCam = true ;
			}
		}
		
		if( mouseRelease )
		{
			mouseRelease = false ;
			if( bDragTimerBar )
			{
				if( !bPause )
					aniCtr.SetPause( false ) ;
				bDragTimerBar = false ;
			}
			else if( bDragStartFrameBar )
			{
				bDragStartFrameBar = false ;
				startNormalizedTime = NewStartEndFrameNormalized ;
				if( !bPause )
					aniCtr.SetPause( false ) ;
			}
			else if( bDragEndFrameBar )
			{
				bDragEndFrameBar = false ;
				endNormalizedTime = NewStartEndFrameNormalized ;
				if( !bPause )
					aniCtr.SetPause( false ) ;
			}
			bDragSpeedBar = bRotateCam = bDragZoomBar = false ;
		}
	}
	
	void TimerBar()
	{
		if( bDragTimerBar )
		{
			float xGap = Input.mousePosition.x - mouseDownPosition.x ;
			float newNormalizedTime = CalculateBarPointValue( oldNormalizedTime , xGap , timerBarWidth , startNormalizedTime , endNormalizedTime ) ;
			aniCtr.SetNormalizedTime( newNormalizedTime ) ;
		}
		else if( bDragStartFrameBar )
		{
			if( oldNormalizedTime < NewStartEndFrameNormalized )
			{
				oldNormalizedTime = NewStartEndFrameNormalized ;
				aniCtr.SetNormalizedTime( oldNormalizedTime ) ;
			}
		}
		else if( bDragEndFrameBar )
		{
			if( oldNormalizedTime > NewStartEndFrameNormalized )
			{
				oldNormalizedTime = NewStartEndFrameNormalized ;
				aniCtr.SetNormalizedTime( oldNormalizedTime ) ;
			}
		}
		float playSliderValue = CalculateBarPointPos( timerBarPos.x , barPointShortSide , aniCtr.GetNormalizedTime() , startNormalizedTime , endNormalizedTime , timerBarWidth ) ;
		timerBarPoint.pixelInset = new Rect( playSliderValue , timerBarPos.y , barPointShortSide , barPointLongSide ) ;
		
		if( !bPlay )
			return ;
		
		if( aniCtr.GetNormalizedTime() > endNormalizedTime )
		{
			aniCtr.SetFinish() ;
			bPlay = aniCtr.GetPlayState() ;
		}
	}
	
	void SpeedBar()
	{
		if( !bDragSpeedBar )
			return ;
		
		float yGap = Input.mousePosition.y - mouseDownPosition.y ;
		float newSpeed = CalculateBarPointValue( oldAnimationSpeed , yGap , speedBarHeight , 0.5f , 2f ) ;
		aniCtr.SetAnimationSpeed( newSpeed ) ;
		
		float speedSliderValue = CalculateBarPointPos( speedBarBackground.pixelInset.y , barPointShortSide , aniCtr.GetAnimationSpeed() , 0.5f , 2f , speedBarHeight ) ;
		speedBarPoint.pixelInset = new Rect( Screen.width - 50f , speedSliderValue , barPointLongSide , barPointShortSide ) ;//15-75
	}
	
	void ZoomBar()
	{
		if( !bDragZoomBar )
			return ;
		
		float yGap = Input.mousePosition.y - mouseDownPosition.y ;
		float newZoom = CalculateBarPointValue( oldZoomValue , yGap , speedBarHeight , 60f , 100f ) ;
		camCtr.SetCameraZoom( newZoom ) ;
		
		float zoomSliderValue = CalculateBarPointPos( zoomBarBackground.pixelInset.y , barPointShortSide , camCtr.GetCameraZoom() , 60f , 100f , zoomBarHeight ) ;
		zoomBarPoint.pixelInset =  new Rect( Screen.width - 50f , zoomSliderValue , barPointLongSide , 10f ) ;//15-75
	}
	
	void CameraRotate()
	{
		if( !bRotateCam )
			return ;
		
		float xGap = Input.mousePosition.x - mouseDownPosition.x ;
		float yGap = Input.mousePosition.y - mouseDownPosition.y ;
		camCtr.MyRotate( xGap , yGap ) ;
	}
	
	float NewStartEndFrameNormalized = 0.0f ;
	void StartEndFrameBar()
	{
		if( bDragStartFrameBar )
		{
			float xGap = Input.mousePosition.x - mouseDownPosition.x ;
			NewStartEndFrameNormalized = CalculateBarPointValue( oldStartEndFrameNormalized , xGap , timerBarWidth , 0f , endNormalizedTime - 0.05f ) ;
			
			float startFrameSliderValue = CalculateBarPointPos( timerBarPos.x , barPointLongSide , NewStartEndFrameNormalized , 0f , 1f , timerBarWidth ) ;
			startFrameBarPoint.pixelInset = new Rect( startFrameSliderValue , Screen.height - timerBarPos.y , barPointShortSide , barPointLongSide ) ;
		}
		else if( bDragEndFrameBar )
		{
			float xGap = Input.mousePosition.x - mouseDownPosition.x ;
			NewStartEndFrameNormalized = CalculateBarPointValue( oldStartEndFrameNormalized , xGap , timerBarWidth , startNormalizedTime + 0.05f , 1f ) ;
			
			float endFrameSliderValue = CalculateBarPointPos( timerBarPos.x , barPointLongSide , NewStartEndFrameNormalized , 0f , 1f , timerBarWidth ) ;
			endFrameBarPoint.pixelInset = new Rect( endFrameSliderValue , Screen.height - timerBarPos.y , barPointShortSide , barPointLongSide ) ;
		}
	}
	
 	void SetStrokes( int StrokesIndex )
	{
		bPause = aniCtr.GetPauseState() ;
		if( !bPause )
			aniCtr.SetPause( true ) ;
		AnimatorController.AnimationFrame StrokesFrame = aniCtr.GetAnimationFrame( StrokesIndex ) ;
		startNormalizedTime = aniCtr.CalculateNormalizedTime( StrokesFrame.StartFrame ) ;
		endNormalizedTime = aniCtr.CalculateNormalizedTime( StrokesFrame.EndFrame ) ;
		
		float startFrameSliderValue = CalculateBarPointPos( timerBarPos.x , barPointLongSide , startNormalizedTime , 0f , 1f , timerBarWidth ) ;
		startFrameBarPoint.pixelInset = new Rect( startFrameSliderValue , Screen.height - timerBarPos.y , barPointShortSide , barPointLongSide ) ;
		float endFrameSliderValue = CalculateBarPointPos( timerBarPos.x , barPointLongSide , endNormalizedTime , 0f , 1f , timerBarWidth ) ;
		endFrameBarPoint.pixelInset = new Rect( endFrameSliderValue , Screen.height - timerBarPos.y , barPointShortSide , barPointLongSide ) ;
		
		aniCtr.SetNormalizedTime( startNormalizedTime ) ;
		if( !bPause )
			aniCtr.SetPause( false ) ;
	}
	
	void SetAnimationDescription()
	{		
		//Fake animation description
		//set 0 , 120-1100
		aniCtr.AddAnimation( "Set 0" , 120 , 770 ) ;
		//set 1 , 120-264
		aniCtr.AddAnimation( "Set 1" , 120 , 264 ) ;
		//set 2 , 265-332
		aniCtr.AddAnimation( "Set 2" , 265 , 332 ) ;
		//set 3 , 333-449
		aniCtr.AddAnimation( "Set 3" , 333 , 449 ) ;
		//set 4 , 450-588
		aniCtr.AddAnimation( "Set 4" , 450 , 588 ) ;
		//set 5 , 589-770
		aniCtr.AddAnimation( "Set 5" , 589 , 770 ) ;		
	}
	
	private float CalculateBarPointPos( float barStartPos , float pointLength , float pointValue , float startValue , float endValue , float barLength )
	{
		float pointStartPos = barStartPos - pointLength * 0.5f ;
		float pointpercent = ( pointValue - startValue ) / ( endValue - startValue ) ;
		float pointPos = pointStartPos + pointpercent * barLength ;
		return pointPos ;
	}
	
	private float CalculateBarPointValue( float oldValue , float gap , float barLength , float startValue , float endValue )
	{
		float percent = gap / barLength ;
		float newValue = oldValue + ( percent * ( endValue - startValue ) ) ;
		newValue = Mathf.Clamp( newValue , startValue , endValue ) ;
		return newValue ;
	}
	
	void Update()
	{
		if( !bStart && bQuit )
			return ;
		
		if( Input.GetMouseButtonDown( 0 ) )
			mouseClick = true ;
		if( Input.GetMouseButtonUp( 0 ) )
			mouseRelease = true ;
		
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
		GUI.Label( new Rect( 5f , 5f , Screen.width , 20f ) , "Animation Timeline Bar : Bule" ) ;
		GUI.Label( new Rect( 5f , 25f , Screen.width , 20f ) , "Speed Bar : Red" ) ;
		GUI.Label( new Rect( 5f , 45f , Screen.width , 20f ) , "Zoom Bar : Green" ) ;
		GUI.Label( new Rect( 5f , 65f , Screen.width , 20f ) , "Start & End Frame Bar : Yellow Black" ) ;
		//GUI.Label( new Rect( 5f , 65f , Screen.width , 20f ) , "bRotateCam : " + bRotateCam ) ;
    }
}