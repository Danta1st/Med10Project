using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class XmlData : MonoBehaviour
{
	//public GameObject CubeObject;

	private int _currentSessionID = 0;
	private int _currentTargetID = 0;
	private bool _hasPassed = false;
	private int _currentAngle = 0;
	private float _currentDistance = 0;
	private float _currentXPosition = 0;
	private float _currentYPosition = 0;
	private float _currentZPosition = 0;
	private float _currentReactionTime = 0;
	private string filepath;
	private XmlDocument xmlDoc;
	private XmlElement elmRoot;
	private XmlElement elmSession;
	private XmlElement elmTargetInstance;
	private string _userID;
	private GA_Submitter GA_SubmitterScript;

	void Start ()
	{
		GA_SubmitterScript = GameObject.Find ("GA_Submitter").GetComponent<GA_Submitter> ();
		_userID = GA_SubmitterScript.GetUserID ();

		//XML file path dependant on current UserID
		filepath = Application.dataPath + @"/Data/" + _userID + ".xml";

		if (File.Exists (filepath))
		{
			LoadExistingXMLFile ();
		} 

		else 
		{
			CreateNewXMLFile ();
		}

		CreateNewSession ();
	}

	private void LoadExistingXMLFile ()
	{
		//Use existing file with current UserID
		xmlDoc = new XmlDocument ();
		xmlDoc.Load (filepath);
		elmRoot = xmlDoc.DocumentElement;
	}

	private void CreateNewXMLFile ()
	{
		//Create new XML file if one with current UserID does not exist
		xmlDoc = new XmlDocument ();
		XmlNode docNode = xmlDoc.CreateXmlDeclaration ("1.0", "UTF-8", null);
		xmlDoc.AppendChild (docNode);
		elmRoot = xmlDoc.CreateElement (_userID);
		xmlDoc.AppendChild (elmRoot);
	}

	private void CreateNewSession ()
	{
		CheckSessionID ();
		//Create new session
		elmSession = xmlDoc.CreateElement ("session");
		elmSession.SetAttribute ("id", _currentSessionID.ToString ());
		elmRoot.AppendChild (elmSession);
		xmlDoc.Save (filepath);
	}

	private void CheckSessionID ()
	{
		_currentSessionID = PlayerPrefs.GetInt (_userID);
	}

	public void WriteTargetDataToXml ()
	{
		//Write all info to current session in current XML file for one specific target
		if (File.Exists (filepath)) 
		{
			elmTargetInstance = xmlDoc.CreateElement ("targetinstance");
			elmTargetInstance.SetAttribute ("id", _currentTargetID.ToString ());
			_currentTargetID++;
			elmSession.AppendChild (elmTargetInstance);
				
			XmlElement elmPassed = xmlDoc.CreateElement ("passed");
			elmPassed.InnerText = _hasPassed.ToString ();
			elmTargetInstance.AppendChild (elmPassed);

			if (_hasPassed) 
			{
				XmlElement elmReactionTime = xmlDoc.CreateElement ("reactiontime");
				elmReactionTime.InnerText = _currentReactionTime.ToString ();
				elmTargetInstance.AppendChild (elmReactionTime);
			}

			XmlElement elmAngle = xmlDoc.CreateElement ("angle");
			elmAngle.InnerText = _currentAngle.ToString ();
			elmTargetInstance.AppendChild (elmAngle);

			XmlElement elmDistance = xmlDoc.CreateElement ("distance");
			elmDistance.InnerText = _currentDistance.ToString ();
			elmTargetInstance.AppendChild (elmDistance);

						
			XmlElement elmPosition = xmlDoc.CreateElement ("position");
			elmTargetInstance.AppendChild (elmPosition);
						
			XmlElement elmXPosition = xmlDoc.CreateElement ("x");
			elmXPosition.InnerText = _currentXPosition.ToString ();
			elmPosition.AppendChild (elmXPosition);

			XmlElement elmYPosition = xmlDoc.CreateElement ("y");
			elmYPosition.InnerText = _currentYPosition.ToString ();
			elmPosition.AppendChild (elmYPosition);

			XmlElement elmZPosition = xmlDoc.CreateElement ("z");
			elmZPosition.InnerText = _currentZPosition.ToString ();
			elmPosition.AppendChild (elmZPosition);
			
			xmlDoc.Save (filepath);
		}
	}
	
	public void SetPassed (bool passed)
	{
		_hasPassed = passed;
	}

	public void SetReactionTime (float time)
	{
		_currentReactionTime = time;
	}

	public void SetAngle (int angle)
	{
		_currentAngle = angle;
	}
	
	public void SetDistance (float distance)
	{
		_currentDistance = distance;
	}
	
	public void SetPosition (Vector3 position)
	{
		_currentXPosition = position.x;
		_currentYPosition = position.y;
		_currentZPosition = position.z;
	}
}