using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	private Vector3 camPos = Vector3.zero ;
	private float camDis = 0.0f ;
	public Transform LookTarget = null ;
	
	public void readyRotate()
	{
		camPos = this.transform.position ;
		camDis = Vector3.Distance( camPos , LookTarget.position ) ;
	}

	public void myRotate( float xGap , float yGap )
	{
		//if( xGap > ( Screen.width * 0.5f ) )
		//Debug.Log( "xGap : " + xGap + "\nyGap : " + yGap ) ;
		
		yGap = Mathf.Clamp( yGap , -0.02f , 0.02f ) ;
		camDis -= yGap ;
		camDis = Mathf.Clamp( camDis , 2f , 5f ) ;
		
		xGap = Mathf.Clamp( xGap , -0.5f , 0.5f ) ;
		LookTarget.Rotate( Vector3.up * xGap ) ;
		
		
		Vector3 Direction = LookTarget.forward * camDis ;
		Direction.y = 1.0f ;
		
		this.transform.position = Direction ;
		this.transform.LookAt( LookTarget ) ;
	}
}
