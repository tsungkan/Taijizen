using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	private float camFOV = 60f ;
	public Transform lookTarget = null ;
	private float rotateSpeed = 1f ;
	private float yPos = 1f ;
	
	public void MyRotate( float xGap , float yGap )
	{
		yPos = Mathf.Clamp( ( yGap / ( Screen.height * 5f ) ) + yPos , -1f , 3f ) ;
		xGap = Mathf.Clamp( xGap , -rotateSpeed , rotateSpeed ) ;
		lookTarget.Rotate( Vector3.up * xGap ) ;
		
		Vector3 direction = lookTarget.forward * 2f ;
		direction.y = yPos ;
		
		this.transform.position = direction ;
		this.transform.LookAt( lookTarget ) ;
	}
	
	public void SetCameraZoom( float newZoom )
	{
		camFOV = newZoom ;
		this.camera.fieldOfView = camFOV ;
		
	}
	
	public float GetCameraZoom(){return camFOV ;}
	
}
