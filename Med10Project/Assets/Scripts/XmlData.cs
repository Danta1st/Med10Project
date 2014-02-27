using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
	private float _currentSpawnTime = 0;
	private float _currentCompleteTime = 0;
	private float _currentReactionTime = 0;

	private string filepath;
	private XmlDocument xmlDoc;
	private XmlElement elmRoot;
	private XmlElement elmSession;
	private XmlElement elmSessionID;


	void Start()
	{
		//Writes session ID information to the XML on startup - should probably do this OnUserLogIn
		filepath = Application.dataPath + @"/Data/Lars.xml";

		xmlDoc = new XmlDocument();
		xmlDoc.Load(filepath);
		
		elmRoot = xmlDoc.DocumentElement;

		CheckSessionIDinXML();
		elmSession = xmlDoc.CreateElement("session");
		elmSession.SetAttribute("id", _currentSessionID.ToString());
		elmRoot.AppendChild(elmSession);
	}

	void Update()
	{
		//x = CubeObject.transform.rotation.eulerAngles.x.ToString();
		if(Input.GetKeyDown(KeyCode.Mouse0))
		{
			WriteTargetDataToXml();
		}
	}

	private void CheckSessionIDinXML()
	{
		if(File.Exists (filepath))
		{
			XmlNodeList sessionList = xmlDoc.GetElementsByTagName("session");
			_currentSessionID = sessionList.Count;
		}
	}

	public void WriteTargetDataToXml()
	{
		if(File.Exists (filepath))
		{
					XmlElement elmTargetInstance = xmlDoc.CreateElement("targetinstance");
							elmTargetInstance.SetAttribute("id", _currentTargetID.ToString());
							_currentTargetID++;
							elmSession.AppendChild(elmTargetInstance);

						XmlElement elmPassed = xmlDoc.CreateElement("passed");
							elmPassed.InnerText = _hasPassed.ToString();
							elmTargetInstance.AppendChild(elmPassed);

						XmlElement elmAngle = xmlDoc.CreateElement("angle");
							elmAngle.InnerText = _currentAngle.ToString();
							elmTargetInstance.AppendChild(elmAngle);

						XmlElement elmDistance = xmlDoc.CreateElement("distance");
							elmDistance.InnerText = _currentDistance.ToString();
							elmTargetInstance.AppendChild(elmDistance);

						XmlElement elmSpawnTime = xmlDoc.CreateElement("spawntime");
							elmSpawnTime.InnerText = _currentSpawnTime.ToString();
							elmTargetInstance.AppendChild(elmSpawnTime);

						XmlElement elmCompleteTime = xmlDoc.CreateElement("completetime");
							elmCompleteTime.InnerText = _currentCompleteTime.ToString();
							elmTargetInstance.AppendChild(elmCompleteTime);

						XmlElement elmReactionTime = xmlDoc.CreateElement("reactiontime");
							_currentReactionTime = _currentCompleteTime - _currentSpawnTime;
							elmReactionTime.InnerText = _currentReactionTime.ToString();
							elmTargetInstance.AppendChild(elmReactionTime);
			
			xmlDoc.Save(filepath);
		}
	}
}