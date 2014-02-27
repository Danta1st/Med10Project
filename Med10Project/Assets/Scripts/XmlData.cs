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

	private string _userID;

	private GA_Submitter GA_SubmitterScript;

	void Start()
	{
		GA_SubmitterScript = GameObject.Find("GA_Submitter").GetComponent<GA_Submitter>();

		_userID = GA_SubmitterScript.GetUserID();
		Debug.Log(_userID);

		filepath = Application.dataPath + @"/Data/" + _userID + ".xml";

		if(File.Exists (filepath))
		{
			xmlDoc = new XmlDocument();
			xmlDoc.Load(filepath);
			elmRoot = xmlDoc.DocumentElement;

			Debug.Log("file exists");
		}
		else
		{
			xmlDoc = new XmlDocument();

			XmlNode docNode = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
			xmlDoc.AppendChild(docNode);
		
			elmRoot = xmlDoc.CreateElement(_userID);
			xmlDoc.AppendChild(elmRoot);

			Debug.Log("file created");
		}

		CheckSessionIDinXML();
		elmSession = xmlDoc.CreateElement("session");
		elmSession.SetAttribute("id", _currentSessionID.ToString());
		elmRoot.AppendChild(elmSession);
		xmlDoc.Save(filepath);
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