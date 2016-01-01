using UnityEngine;
using System.Collections;

//helper class
//it listen message comming from iphone plugin to inform us that all information are ready to use
public class MssageReceiver : MonoBehaviour {

	void OnInitializeDone(string message )
	{
		Contacts.OnInitializeDone ();
	}

	void Log( string message )
	{
		Debug.Log( "internal log : " + message );
	}

	void Error( string error )
	{
		Debug.LogError( "internal error : " + error );
	}
}
