using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

//holder of person details
public class Contact
{
	public string FirstName ;
	public string LastName ;
	public byte[] Photo ;
	public Texture2D PhotoTexture ;
	public List<string> Phones 	  = new List<string>();
	public List<string> PhoneType = new List<string>();//mobile,home,work,...
	public List<string> Emailes   = new List<string>();
	public List<string> Connections = new List<string>();//for android only. example(google,whatsup,...)
	//To Do
	/*public string Company ;
	public List<string> URls 	= new List<string>();
	public List<string> Address = new List<string>();
	public String		Birthday ;
	public String 		Note ;
	public List<string> SocialProfiles = new List<string>();
	public List<string> Data = new List<string>();*/

}
//holder for all contacts information of your mobile
//use this class to acces all mobile contacts information
//this is the core of this asset, you have to know only how this class work
// first call int count = Contacts.GetContactsCount();
// then loop throught all of them and acces data you like name name phones
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
	
#if UNITY_IPHONE
	public static void LoadContactList()
	{
#if UNITY_EDITOR_OSX
		test();
		return;
#else
		GameObject helper = new GameObject ();
		GameObject.DontDestroyOnLoad( helper);
		helper.name = "ContactsListMessageReceiver";
		helper.AddComponent<MssageReceiver> ();
		_LoadContactList ();
#endif

	}
	public static void OnInitializeDone()
	{
		MyPhoneNumber = GetMyPhoneNumber();
		SimSerialNumber = GetSimSerialNumber();
		NetworkOperator = GetNetworkOperator();
		NetworkCountryIso = GetNetworkCountryIso();
		
		int count = GetContactsCount();
		for( int i = 0 ; i < count ; i++ )
		{
			Contact c = new Contact();
			c.FirstName = GetContactName(i);
			int phonesCount = GetContactNumberCount(i);
			for(int p = 0 ; p < phonesCount ; p++ )
			{
				string phone = GetContactNumber(i,p);
				//if( !c.Phones.Contains(phone))
					c.Phones.Add( phone );
				string phoneType = GetContactNumberType(i,p);
				//if( !c.PhoneType.Contains(phoneType))
					c.PhoneType.Add( phoneType ); 
			}
			int emailsCount = GetEmailsCount(i);
			for(int e = 0 ; e < emailsCount ; e++ )
			{
				string email = GetEmail(i,e);
				if( !c.Emailes.Contains(email))
					c.Emailes.Add( email );
			}
			string photo = GetContactPhoto(i);
			if( !string.IsNullOrEmpty( photo ) )
				c.Photo = System.Convert.FromBase64String( photo );
			if( c.Photo != null )
			{
				c.PhotoTexture = new Texture2D(4,4);
				c.PhotoTexture.LoadImage(c.Photo);
			}
			ContactsList.Add(c);
		}
	}
	[DllImport("__Internal")]
	private static extern void _LoadContactList();
	[DllImport("__Internal")]
	private static extern string GetMyPhoneNumber();
	[DllImport("__Internal")]
	private static extern string GetSimSerialNumber();
	[DllImport("__Internal")]
	private static extern string GetNetworkOperator();
	[DllImport("__Internal")]
	private static extern string GetNetworkCountryIso();
	[DllImport("__Internal")]
	private static extern int GetContactsCount();
	[DllImport("__Internal")]
	private static extern string GetContactId( int id );
	[DllImport("__Internal")]
	private static extern string GetContactName( int id );
	[DllImport("__Internal")]
	private static extern int GetContactNumberCount(int id);
	[DllImport("__Internal")]
	private static extern string GetContactNumber( int id , int phoneId);
	[DllImport("__Internal")]
	private static extern string GetContactNumberType( int id , int phoneId);
	[DllImport("__Internal")]
	private static extern int GetEmailsCount(int id);
	[DllImport("__Internal")]
	private static extern string GetEmail( int id , int emailId);
	[DllImport("__Internal")]
	private static extern string GetContactPhoto( int id );

#elif UNITY_ANDROID
	static AndroidJavaClass ojc = null ;

	static String GetMyPhoneNumber()
	{
		String myNumber = ojc.CallStatic<String>("GetMyPhoneNumber" );
		return myNumber ;
	}
	static String GetSimSerialNumber()
	{
		String str = ojc.CallStatic<String>("GetSimSerialNumber" );
		return str ;
	}
	static String GetNetworkOperator()
	{
		String str = ojc.CallStatic<String>("GetNetworkOperator" );
		return str ;
	}
	static String GetNetworkCountryIso()
	{
		String str = ojc.CallStatic<String>("GetNetworkCountryIso" );
		return str ;
	}
	static int GetContactsCount()
	{
		int count = ojc.CallStatic<int>("GetContactsCount" );
		return count ;
	}

	public static String GetContactId( int id )
	{
		String text = ojc.CallStatic<String>("GetContactId" , id);
		return text ;
	}
	public static String GetContactName( int id )
	{
		String text = ojc.CallStatic<String>("GetContactName" , id);
		return text ;
	}
	public static int GetContactNumberCount( int id )
	{
		int value = ojc.CallStatic<int>("GetContactNumberCount" , id);
		return value ;
	}
	public static String GetContactNumber( int id , int phoneId )
	{
		String text = ojc.CallStatic<String>("GetContactNumber" , id , phoneId );
		return text ;
	}
	public static String GetContactNumberType( int id , int phoneId )
	{
		String text = ojc.CallStatic<String>("GetContactNumberType" , id , phoneId );
		return text ;
	}

	public static int GetEmailsCount( int id )
	{
		int value = ojc.CallStatic<int>("GetEmailsCount" , id);
		return value ;
	}
	public static String GetEmail( int id , int emailId )
	{
		String text = ojc.CallStatic<String>("GetEmail" , id , emailId );
		return text ;
	}
	public static byte[] GetContactPhoto( int id )
	{
		String photo = ojc.CallStatic<String>("GetContactPhoto" , id);
		if( string.IsNullOrEmpty(photo))
			return null ;
		return System.Convert.FromBase64String( photo ) ;
	}
	public static int GetContactConnectionsCount( int id )
	{
		int value = ojc.CallStatic<int>("GetContactConnectionsCount" , id);
		return value ;
	}
	public static String GetContactConnection( int id , int connectionId)
	{
		String value = ojc.CallStatic<String>("GetContactConnection" , id,connectionId );
		return value ;
	}

	public static void LoadContactList()
	{
#if UNITY_EDITOR
		test();
		return;
#else
		GameObject helper = new GameObject ();
		GameObject.DontDestroyOnLoad( helper);
		helper.name = "ContactsListMessageReceiver";
		helper.AddComponent<MssageReceiver> ();

		ojc = new AndroidJavaClass("com.cloudsoft.contactslist.ContactList");

		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
		//jo.Call("launchAndroidActivity");
		float total = Time.realtimeSinceStartup ;
		float t = Time.realtimeSinceStartup ;
		ojc.CallStatic("LoadInformation" , jo );
		t = Time.realtimeSinceStartup - t ;
		Debug.Log("LoadInformation tooks          " + t.ToString("0.00") + " sec");
		MyPhoneNumber = GetMyPhoneNumber();
		SimSerialNumber = GetSimSerialNumber();
		NetworkOperator = GetNetworkOperator();
		NetworkCountryIso = GetNetworkCountryIso();

		int count = GetContactsCount();
		Debug.Log("GetContactsCount give " + count + " contacts");
		t = Time.realtimeSinceStartup ;
		for( int i = 0 ; i < count ; i++ )
		{
			Contact c = new Contact();
			c.FirstName = GetContactName(i);
			ContactsList.Add(c);
		}
		t = Time.realtimeSinceStartup - t ;
		Debug.Log("Load Names tooks          " + t.ToString("0.00") + " sec");

		t = Time.realtimeSinceStartup ;
		for( int i = 0 ; i < count ; i++ )
		{
			Contact c = ContactsList[i];
			int phonesCount = GetContactNumberCount(i);
			for(int p = 0 ; p < phonesCount ; p++ )
				c.Phones.Add( GetContactNumber(i,p));
			for(int p = 0 ; p < phonesCount ; p++ )
				c.PhoneType.Add( GetContactNumberType(i,p));
		}
		t = Time.realtimeSinceStartup - t ;
		Debug.Log("Load Phones tooks          " + t.ToString("0.00") + " sec");

		t = Time.realtimeSinceStartup ;
		for( int i = 0 ; i < count ; i++ )
		{
			Contact c = ContactsList[i];
			int emailsCount = GetEmailsCount(i);
			for(int e = 0 ; e < emailsCount ; e++ )
				c.Emailes.Add( GetEmail(i,e));
		}
		t = Time.realtimeSinceStartup - t ;
		Debug.Log("Load Emailes tooks          " + t.ToString("0.00") + " sec");

		t = Time.realtimeSinceStartup ;
		for( int i = 0 ; i < count ; i++ )
		{
			Contact c = ContactsList[i];
			c.Photo 	= GetContactPhoto(i);
			if( c.Photo != null )
			{
				c.PhotoTexture = new Texture2D(4,4);
				c.PhotoTexture.LoadImage(c.Photo);
			}
		}
		t = Time.realtimeSinceStartup - t ;
		Debug.Log("Load Photos tooks          " + t.ToString("0.00") + " sec");

		t = Time.realtimeSinceStartup ;
		for( int i = 0 ; i < count ; i++ )
		{
			Contact c = ContactsList[i];
			int connectionsCount = GetContactConnectionsCount(i);
			for(int j = 0 ; j < connectionsCount ; j++ )
				c.Connections.Add( GetContactConnection(i,j));
		}
		t = Time.realtimeSinceStartup - t ;
		Debug.Log("Load Types tooks          " + t.ToString("0.00") + " sec");

		total = Time.realtimeSinceStartup - total ;
		Debug.Log("Total load tooks          : " + total.ToString("0.00") + " sec");
		//To improve load time dont fill data you dont need, for example remove this block
		//int emailsCount = GetEmailsCount(i);
		//for(int e = 0 ; e < emailsCount ; e++ )
		//	c.Emailes.Add( GetEmail(i,e));
		// if you dont care about mailes or
		// c.Type = GetContactType( i ); if you dont care about type 
		// or load them once it is needed
	
		#endif
	}
	public static void OnInitializeDone()
	{
	}
#else
	public static void LoadContactList()
	{
		test();
	}
	public static void OnInitializeDone()
	{
	}
#endif
	static void test()
	{
		MyPhoneNumber = "3493943 my phone number";
		ContactsList.Clear();
		for( int i = 0 ; i < 300 ; i++ )
		{
			Contact c = new Contact();
			c.FirstName = "Test name " + i ;
			c.LastName = "Last Name" ;
			for( int p = 0 ; p < 3 ; p++ )
			{
				c.Phones.Add("+34235334244!");
				c.PhoneType.Add("Type Test");
			}
			ContactsList.Add( c );
		}
	}
}
