using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;



public class PhoneContact
{
	public string Number ;
	public string Type ;
}

public class EmailContact
{
	public string Address;
	public string Type ;
}

//holder of person details
public class Contact
{
	public string Id ;
	public string Name ;
	public byte[] 	 Photo;
	public Texture2D PhotoTexture ;
	public bool		 PhotoIsLoaded = false;
	public List<PhoneContact> Phones = new List<PhoneContact>();
	public List<EmailContact> Emails = new List<EmailContact>();
	
	public List<string> Connections = new List<string>();//for android only. example(google,whatsup,...)
	
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
			c.Name = GetContactName(i);
			int phonesCount = GetContactNumberCount(i);
			for(int p = 0 ; p < phonesCount ; p++ )
			{
				PhoneContact pc = new PhoneContact();
				pc.Number = GetContactNumber(i,p);
				pc.Type = GetContactNumberType(i,p);

				c.Phones.Add( pc);
			}
			int emailsCount = GetEmailsCount(i);
			for(int e = 0 ; e < emailsCount ; e++ )
			{
				EmailContact ec = new EmailContact();
				ec.Address = GetEmail(i,e);

				c.Emails.Add( ec );
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
	public static void LoadAsyncContactPhoto( int id )
	{
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
	public static String GetEmailAdress( int id , int emailId )
	{
		String text = ojc.CallStatic<String>("GetEmailAdress" , id , emailId );
		return text ;
	}
	public static String GetEmailType( int id , int emailId )
	{
			String text = ojc.CallStatic<String>("GetEmailType" , id , emailId );
			return text ;
	}
	public static void LoadAsyncContactPhoto( int id )
	{
		Contact c = ContactsList[id];
		if( c.PhotoIsLoaded )
			return;
		activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
		{
			String photo = ojc.CallStatic<String>("GetContactPhoto" , id);
			c.PhotoIsLoaded = true ;
			if( string.IsNullOrEmpty(photo))
			{
				return;
			}
			c.Photo = System.Convert.FromBase64String( photo ) ;
		}));
		
		
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

	static AndroidJavaObject activity;
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

		try
		{

			ojc = new AndroidJavaClass("com.aliessmael.contactslist.ContactList");
			
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
			//jo.Call("launchAndroidActivity");
			float total = Time.realtimeSinceStartup ;
			float t = Time.realtimeSinceStartup ;
			
			/*activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
			{
				
			}));*/

			ojc.CallStatic("LoadInformation" , activity , false, true,true,true);
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
				c.Name = GetContactName(i);
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
				{
					string number = GetContactNumber(i,p);
					string type = GetContactNumberType(i,p);
					c.Phones.Add( new PhoneContact(){ Number=number, Type=type});
				}
				
			}
			t = Time.realtimeSinceStartup - t ;
			Debug.Log("Load Phones tooks          " + t.ToString("0.00") + " sec");

			t = Time.realtimeSinceStartup ;
			for( int i = 0 ; i < count ; i++ )
			{
				Contact c = ContactsList[i];
				int emailsCount = GetEmailsCount(i);
				for(int e = 0 ; e < emailsCount ; e++ )
				{
					string address = GetEmailAdress(i,e);
					string type = GetEmailType(i,e);
					c.Emails.Add( new EmailContact(){ Address=address, Type = type});
				}
			}
			t = Time.realtimeSinceStartup - t ;
			Debug.Log("Load Emailes tooks          " + t.ToString("0.00") + " sec");

			/*t = Time.realtimeSinceStartup ;
			for( int i = 0 ; i < count ; i++ )
			{
				Contact c = ContactsList[i];
				GetContactPhoto(i);

			}
			t = Time.realtimeSinceStartup - t ;
			Debug.Log("Load Photos tooks          " + t.ToString("0.00") + " sec");*/

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

		}
		catch( System.Exception e )
		{
			Debug.LogException( e );
		}
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
	public static void LoadAsyncContactPhoto( int id )
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
			
			c.Name = "Test name " + i;
			for( int p = 0 ; p < 3 ; p++ )
			{
				c.Phones.Add( new PhoneContact{ Number = "+34235334244!", Type = "Type Test"});
			}
			ContactsList.Add( c );
		}
	}
}
