using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;




//holder for all contacts information of your mobile
//use this class to acces all mobile contacts information
//this is the core of this asset, you have to know only how this class work
// first call LoadContactList();
// then loop throught all of ContactsList and acces data you like name name phones
// Contact c = Contacts.ContactsList[i];
// that is it, so easy, have fun
public class Contacts{

	//looks likee these four fields down doesnot working for all mobiles
	public static String 		MyPhoneNumber ;
	public static String 		SimSerialNumber ;
	public static String 		NetworkOperator ;
	public static String 		NetworkCountryIso ;
	//
	public static List<Contact> ContactsList = new List<Contact>();


	#if UNITY_ANDROID
	static AndroidJavaObject activity;
	static AndroidJavaClass ojc = null ;
	#elif UNITY_IOS
	[DllImport("__Internal")]
	private static extern void loadIOSContacts();
	[DllImport("__Internal")]
	private static extern string getContact( int index );
	#endif


	static System.Action<string> onFailed;
	static System.Action onDone;
	public static void LoadContactList( )
	{
		LoadContactList( null , null);
	}

	public static void LoadContactList( System.Action _onDone, System.Action<string> _onFailed )
	{
		Debug.Log ( "LoadContactList at " + Time.realtimeSinceStartup );
		onFailed = _onFailed;
		onDone = _onDone;

		GameObject helper = new GameObject ();
		GameObject.DontDestroyOnLoad( helper);
		helper.name = "ContactsListMessageReceiver";
		helper.AddComponent<MssageReceiver> ();

		#if UNITY_ANDROID
		ojc = new AndroidJavaClass("com.aliessmael.contactslist.ContactList");
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
		ojc.CallStatic("LoadInformation" , activity , true, true,true,true);
		#elif UNITY_IOS
		loadIOSContacts();
		#endif
	}

	public static void GetContact( int index )
	{
		byte[] data = null;

		#if UNITY_ANDROID
		data = ojc.CallStatic<byte[]>("getContact" , index);
		#elif UNITY_IOS
		string str = getContact( index);
		data = System.Convert.FromBase64String( str );
		#endif
		Contact c 	= new Contact();
		debug( "Data length for " + index + " is " + data.Length );
		c.FromBytes( data );
		Contacts.ContactsList.Add( c );

	}

	public static void OnInitializeDone()
	{
		Debug.Log ( "done at " + Time.realtimeSinceStartup );
		if (onDone != null) 
		{
			onDone();
		}
	}

	public static void OnInitializeFail( string message )
	{
		Debug.Log ( "fail at " + Time.realtimeSinceStartup );
		if (onFailed != null) 
		{
			onFailed( message );
		}
	}

	static void debug( string message)
	{
		//Debug.Log ( message );
	}

}
