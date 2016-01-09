using UnityEngine;
using System.Collections;

//helper class
//it listen for messages comming from plugin
public class MssageReceiver : MonoBehaviour {

	void OnInitializeDone( string message )
	{
		Contacts.OnInitializeDone ();
	}

	void OnInitializeFail( string message )
	{
		Debug.LogError( "OnInitializeFail : " + message );
		Contacts.OnInitializeFail (message);
	}

	void Log( string message )
	{
		Debug.Log( "internal log : " + message );
	}

	void Error( string error )
	{
		Debug.LogError( "internal error : " + error );
	}

	void OnContactReady( string id )
	{

		debug( "OnContactReady: " + id);

		if( string.IsNullOrEmpty( id ))
			return;

		int index = int.Parse( id);
		Contacts.GetContact( index );
	}

	/*void OnContactReady( string data )
	{
		return;
		//debug( data );
		byte[] bytes = System.Text.Encoding.UTF8.GetBytes( data );
		Contact c = new Contact();
		c.FromBytes( bytes );
		Contacts.ContactsList.Add( c );
	}*/

	void debug( string message )
	{
		//Debug.Log( message );
	}
}
