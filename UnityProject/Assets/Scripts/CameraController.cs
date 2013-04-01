using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	private float camZoom = 2f ;
	public Transform lookTarget = null ;
	private float rotateSpeed = 5f ;
	private float yPos = 0f ;
	
	public void MyRotate( float xGap , float yGap )
	{
		xGap = Mathf.Clamp( ( xGap / ( Screen.width * 0.5f ) ) , -rotateSpeed , rotateSpeed ) ;
		lookTarget.Rotate( Vector3.up * xGap ) ;
		
		yPos = Mathf.Clamp( ( yGap / ( Screen.height * 5f ) ) + yPos , -2f , 4f ) ;
		SetCamera() ;
	}
	
	void SetCamera()
	{
		Vector3 direction = lookTarget.forward * 2f ;
		direction.y = yPos ;
		Vector3 RayPositoin = lookTarget.transform.position ;
		Vector3 RayDirection = direction ;
		RayDirection.Normalize() ;
		Ray ray = new Ray( RayPositoin , RayDirection ) ;
		
		this.transform.position = ray.GetPoint( camZoom ) ;
		this.transform.LookAt( lookTarget ) ;
	}
	
	public void SetCameraZoom( float newZoom )
	{
		camZoom = newZoom ;
		SetCamera() ;
	}
	
	public float GetCameraZoom(){return camZoom ;}
}