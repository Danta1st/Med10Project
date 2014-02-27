using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class xmlTest : MonoBehaviour {


	private string userID = "Danny";
	private int sessionID = 0;
	private string filepath;
	private XmlDocument xmlDoc;

	void Start()
	{
		//XML file path dependant on current UserID
		filepath = Application.dataPath + @"/Data/" + userID + ".xml";


		//Check if file exists and create file if it does not
		if (File.Exists(filepath)) 
			LoadXmlDoc();
		else 
			CreateXmlDoc();
	}

	#region Class Methods
	public void CreateXmlDoc()
	{
		//Create new XML file if one with current UserID does not exist
		xmlDoc = new XmlDocument();
		//Create xml declaration
		XmlNode docNode = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
		xmlDoc.AppendChild(docNode);
		//Create Root element
		XmlElement elmRoot = xmlDoc.CreateElement(userID);
		xmlDoc.AppendChild(elmRoot);
		//Save file
		xmlDoc.Save(filepath);

		CheckSession();
	}

	public void LoadXmlDoc()
	{
		xmlDoc = new XmlDocument();
		xmlDoc.Load(filepath);

		CheckSession();
//		AddAngle(2,50,false);
//		AddDistance(2,2.5f,false);
//		AddReactionTime(4,0.453f,true);
	}
	
	private void CheckSession()
	{
		if(xmlDoc.SelectSingleNode(""+userID+"/session_"+sessionID) == null)
			CreateSession();
	}

	public void CreateSession()
	{
		var newSessionElement = xmlDoc.CreateElement("session_"+sessionID);
		var x = xmlDoc.GetElementsByTagName(userID)[0];
		x.AppendChild(newSessionElement);
		//Append child succes element
		var newSuccesElement = xmlDoc.CreateElement("succes");
		newSessionElement.AppendChild(newSuccesElement);
		//Append child failed element
		var newFailedElement = xmlDoc.CreateElement("failed");
		newSessionElement.AppendChild(newFailedElement);
		//Save the file
		xmlDoc.Save(filepath);
	}

	private void CheckTarget(int ID, bool succes)
	{
		if(succes == true)
		{
			if(xmlDoc.SelectSingleNode(""+userID+"/session_"+sessionID+"/succes/target_"+ID) == null)
				CreateTarget(ID, true);
		}
		else
		{
			if(xmlDoc.SelectSingleNode(""+userID+"/session_"+sessionID+"/failed/target_"+ID) == null)
				CreateTarget(ID, false);
		}
	}

	public void CreateTarget(int ID, bool succes)
	{
		var newTarget = xmlDoc.CreateElement("target_"+ID);

		XmlNode x;
		if(succes == true)
			x = xmlDoc.SelectSingleNode(""+userID+"/session_"+sessionID+"/succes");
		else
			x = xmlDoc.SelectSingleNode(""+userID+"/session_"+sessionID+"/failed");

		x.AppendChild(newTarget);

		//Create empty elements
		var newAngle = xmlDoc.CreateElement("angle");
		var newDistance = xmlDoc.CreateElement("distance");
		var newReactionTime = xmlDoc.CreateElement("reactiontime");
		//Append empty elements to the target instance
		newTarget.AppendChild(newAngle);
		newTarget.AppendChild(newDistance);
		newTarget.AppendChild(newReactionTime);
		//Save the file
		xmlDoc.Save(filepath);
	}

	public void AddAngle(int ID, int angle, bool succes)
	{
		CheckTarget(ID, succes);
		//Was it a hit or miss? 
		XmlNode x;
		if(succes == true)
		{
			x = xmlDoc.SelectSingleNode(""+userID+"/session_"+sessionID+"/succes/target_"+ID+"/angle");
		}
		else
		{
			x = xmlDoc.SelectSingleNode(""+userID+"/session_"+sessionID+"/failed/target_"+ID+"/angle");
		}

		//Did we already log something here? if yes then overwrite. Safety measure.
		if(x.InnerText == null)
		{
			var newText = xmlDoc.CreateTextNode(""+angle);
			x.AppendChild(newText);
		}
		else
			x.InnerText = ""+angle;

		xmlDoc.Save(filepath);
	}

	public void AddDistance(int ID, float distance, bool succes)
	{
		CheckTarget(ID, succes);
		//Was it a hit or miss? 
		XmlNode x;
		if(succes == true)
		{
			x = xmlDoc.SelectSingleNode(""+userID+"/session_"+sessionID+"/succes/target_"+ID+"/distance");
		}
		else
		{
			x = xmlDoc.SelectSingleNode(""+userID+"/session_"+sessionID+"/failed/target_"+ID+"/distance");
		}
		
		//Did we already log something here? if yes then overwrite. Safety measure.
		if(x.InnerText == null)
		{
			var newText = xmlDoc.CreateTextNode(""+distance);
			x.AppendChild(newText);
		}
		else
			x.InnerText = ""+distance;
		
		xmlDoc.Save(filepath);
	}

	public void AddReactionTime(int ID, float reactiontime, bool succes)
	{
		CheckTarget(ID, succes);
		//Was it a hit or miss? 
		XmlNode x;
		if(succes == true)
		{
			x = xmlDoc.SelectSingleNode(""+userID+"/session_"+sessionID+"/succes/target_"+ID+"/reactiontime");
		}
		else
		{
			x = xmlDoc.SelectSingleNode(""+userID+"/session_"+sessionID+"/failed/target_"+ID+"/reactiontime");
		}
		
		//Did we already log something here? if yes then overwrite. Safety measure.
		if(x.InnerText == null)
		{
			var newText = xmlDoc.CreateTextNode(""+reactiontime);
			x.AppendChild(newText);
		}
		else
			x.InnerText = ""+reactiontime;
		
		xmlDoc.Save(filepath);
	}
	#endregion 
}
