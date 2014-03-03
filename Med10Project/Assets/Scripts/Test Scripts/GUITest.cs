using UnityEngine;
using System.Collections;

public class GUITest : MonoBehaviour {

	private int optimalHeight = 1200;
	private int optimalWidth = 720;
	private Rect someRect = new Rect(200, 200, 400, 300);
	public GameObject unityLogo;

	float scaledHeight;
	float scaledWidth;

	void Update()
	{
		scaledHeight = getScaleHeight();
		scaledWidth = getScaleWidth();

		if(Input.GetKeyDown(KeyCode.T))
		{
			scaleGUITexture(unityLogo.guiTexture);
		}
	}

	private float getScaleHeight()
	{
		return (float) Screen.height/ (float) optimalHeight;
	}

	private float getScaleWidth()
	{
		return (float) Screen.width/ (float) optimalWidth;
	}

	private Rect scaleRectangle(Rect rect)
	{
		float scaleHeight = getScaleHeight();
		float scaleWidth = getScaleWidth();

		float x = scaleWidth * rect.x;
		float y = scaleHeight * rect.y;
		float width = scaleWidth * rect.width;
		float height = scaleHeight * rect.height;

		return new Rect(x, y, width, height);
	}
	
	private void scaleGUITexture(GUITexture guiTexture)
	{
		guiTexture.transform.position = Vector3.zero;
		guiTexture.transform.localScale = Vector3.zero;

		Debug.Log(guiTexture.pixelInset);
		Rect scaledRect = scaleRectangle(guiTexture.pixelInset);
		Debug.Log(scaledRect);

		guiTexture.pixelInset = scaledRect;
	}

	private void scaleObject(GameObject guiObject)
	{
		float scaleHeight = getScaleHeight();
		float scaleWidth = getScaleWidth();

		float xScale = guiObject.transform.localScale.x * scaleWidth;
		float yScale = guiObject.transform.localScale.y * scaleHeight;
		float zScale = guiObject.transform.localScale.z;
		Vector3 newScale = new Vector3(xScale,yScale,zScale);

		float xPos = guiObject.transform.localPosition.x * scaleWidth;
		float yPos = guiObject.transform.localPosition.y * scaleHeight;
		float zPos = guiObject.transform.localPosition.z;
		Vector3 newPos = new Vector3(xPos, yPos, zPos);

		guiObject.transform.localScale = newScale;
		guiObject.transform.localPosition = newPos;
	}
}
