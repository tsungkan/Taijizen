using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	private Vector3 camPos = Vector3.zero ;
	private float camDis = 2.0f ;
	public Transform LookTarget = null ;
	
	public void ReadyRotate()
	{
		camPos = this.transform.position ;
	}

	public void MyRotate( float xGap )
	{
		xGap = Mathf.Clamp( xGap , -0.5f , 0.5f ) ;
		LookTarget.Rotate( Vector3.up * xGap ) ;
		
		Vector3 Direction = LookTarget.forward * camDis ;
		Direction.y = 1.0f ;
		
		this.transform.position = Direction ;
		this.transform.LookAt( LookTarget ) ;
	}
	
	public void SetCameraZoom( float newZoom )
	{
		camDis = newZoom ;
		Vector3 Direction = LookTarget.forward * camDis ;
		Direction.y = 1.0f ;
		
		this.transform.position = Direction ;
	}
	
	public float GetCameraZoom(){ return camDis ;}
}
