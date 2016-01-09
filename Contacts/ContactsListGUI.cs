using UnityEngine;
using System.Collections;

//this is test class, it use Contacts class to access your mobile contacts and draw it on screen 
//it is just viewer , you may create your own GUI for that
public class ContactsListGUI : MonoBehaviour {
	
	public Font font ;
	public Texture2D even_contactTexture;
	public Texture2D odd_contactTexture;
	public Texture2D contact_faceTexture;
	GUIStyle style ;
	GUIStyle   	evenContactStyle ;
	GUIStyle   	oddContactStyle ;
	GUIStyle   	contactFaceStyle ;
	GUIStyle   	nonStyle2 ;
	Vector2 size ;
	float   dragTime ;
	float   dragSpeed ;

	string failString;
	// Use this for initialization
	void Start () {
		
		Contacts.LoadContactList( onDone, onLoadFailed );



	
		style = new GUIStyle();
		style.font = font ;
		style.fontSize = (int)(size.y / 2) ;


		size.x = Screen.width ;
		size.y = Screen.height / 6 ;

		evenContactStyle = new GUIStyle();
		evenContactStyle.normal.background = even_contactTexture ;
		//evenContactStyle.fixedWidth= size.x ;
		evenContactStyle.fixedHeight = size.y ;
		evenContactStyle.clipping = TextClipping.Clip ;
		evenContactStyle.alignment = TextAnchor.UpperLeft ;
		evenContactStyle.imagePosition = ImagePosition.ImageLeft ;
		evenContactStyle.fontSize = (int)(size.y /2 );
		//evenLogStyle.wordWrap = true;
		
		oddContactStyle = new GUIStyle();
		oddContactStyle.normal.background = odd_contactTexture;
		//oddContactStyle.fixedWidth= size.x ;
		oddContactStyle.fixedHeight = size.y ;
		oddContactStyle.clipping = TextClipping.Clip ;
		oddContactStyle.alignment = TextAnchor.UpperLeft ;
		oddContactStyle.imagePosition = ImagePosition.ImageLeft ;
		oddContactStyle.fontSize = (int)(size.y /2 );

		contactFaceStyle = new GUIStyle();
		contactFaceStyle.fixedHeight = size.y ;
		contactFaceStyle.fixedWidth  = size.y ;
		//contactFaceStyle.normal.background = contact_faceTexture;

		nonStyle2 = new GUIStyle();
		nonStyle2.clipping = TextClipping.Clip;
		nonStyle2.border = new RectOffset(0,0,0,0);
		nonStyle2.normal.background = null ;
		nonStyle2.fontSize = 1;
		nonStyle2.alignment = TextAnchor.MiddleCenter ;
	}


	Vector2 downPos ;
	Vector2 getDownPos()
	{
		if( Application.platform == RuntimePlatform.Android || 
		   Application.platform == RuntimePlatform.IPhonePlayer )
		{
			
			if( Input.touches.Length == 1 &&  Input.touches[0].phase == TouchPhase.Began )
			{
				dragSpeed = 0;
				downPos = Input.touches[0].position ;
				return  downPos ;
			}
		}
		else 
		{
			if( Input.GetMouseButtonDown(0) )
			{
				dragSpeed = 0;
				downPos.x = Input.mousePosition.x ;
				downPos.y = Input.mousePosition.y ;
				return  downPos ;
			}
		}
		
		return Vector2.zero ;
	}
	Vector2 mousePosition ;
	Vector2 getDrag()
	{
		
		if( Application.platform == RuntimePlatform.Android || 
		   Application.platform == RuntimePlatform.IPhonePlayer )
		{
			if( Input.touches.Length != 1 )
			{
				return Vector2.zero ;
			}
			return Input.touches[0].position - downPos ;
		}
		else 
		{
			if( Input.GetMouseButton(0) )
			{
				mousePosition = Input.mousePosition ;
				return mousePosition - downPos ;
			}
			else 
			{
				return Vector2.zero;
			}
		}
	}

	int calculateStartIndex()
	{
		int totalCout = Contacts.ContactsList.Count ;
		int totalVisibleCount = 6 ;
		int startIndex = (int)(contactsScrollPos.y /size.y)  ;
		if( startIndex < 0 ) startIndex = 0 ;

		if( totalCout < totalVisibleCount ) startIndex = 0 ;
		else if( startIndex > totalCout - totalVisibleCount )
			startIndex = totalCout - totalVisibleCount ;

		return startIndex ;
	}
	Vector2 contactsScrollPos ;

	Vector2 oldContactsDrag ;
	void OnGUI()
	{
		if (!string.IsNullOrEmpty (failString)) 
		{
			GUILayout.Label( "failed : " + failString );
			if( GUILayout.Button("Retry"))
			{
				Contacts.LoadContactList( onDone, onLoadFailed );
				failString = null;
			}
		}
		//return;
		getDownPos();

		Vector2 drag = getDrag(); 
		if( (drag.x != 0) && (downPos != Vector2.zero)  )
		{
			//contactsScrollPos.x -= (drag.x - oldContactsDrag.x) ;
		}
		if( (drag.y != 0) && (downPos != Vector2.zero)  )
		{
			contactsScrollPos.y += (drag.y - oldContactsDrag.y) ;
			dragTime += Time.deltaTime ;
			//dragSpeed = drag.y / dragTime ;
		}


		else if( dragSpeed > 1f){
			dragTime = 0 ;
			contactsScrollPos.y += dragSpeed * Time.deltaTime ;
			dragSpeed -= (dragSpeed*0.01f);
		}
		oldContactsDrag = drag;

		//GUILayout.Label("My Phone Number = " + Contacts.MyPhoneNumber );
		/*GUILayout.Label("simSerialNumber " + Contacts.SimSerialNumber );
		GUILayout.Label("networkOperator " + Contacts.NetworkOperator );
		GUILayout.Label("networkCountryIso " + Contacts.NetworkCountryIso );
		GUILayout.Label("Contacts Count are " + Contacts.ContactsList.Count );*/
		contactsScrollPos = GUILayout.BeginScrollView( contactsScrollPos );

		int startIndex = calculateStartIndex();
		int endIndex = startIndex + 7 ;
		if( endIndex > Contacts.ContactsList.Count  )
			endIndex = Contacts.ContactsList.Count  ;

		int beforeHeight = (int)(startIndex*size.y) ;
		if( beforeHeight > 0 )
		{
			//fill invisible gap befor scroller to make proper scroller pos
			GUILayout.BeginHorizontal(  GUILayout.Width(size.x*2) , GUILayout.Height( beforeHeight ) );
			GUILayout.Box(" " ,nonStyle2 );
			GUILayout.EndHorizontal();
		}
		else 
		{
			GUILayout.BeginHorizontal(  GUILayout.Width(size.x*2) , GUILayout.Height( 1f ) );
			GUILayout.Box(" " , nonStyle2 );
			GUILayout.EndHorizontal();
		}

		for( int i = startIndex ; i < endIndex ; i++ )
		{
			if( i%2 == 0 )
				GUILayout.BeginHorizontal( evenContactStyle ,GUILayout.Width( size.x) , GUILayout.Height( size.y ));
			else
				GUILayout.BeginHorizontal( oddContactStyle ,GUILayout.Width( size.x) , GUILayout.Height( size.y ));
			GUILayout.Label( i.ToString() , style );
			Contact c = Contacts.ContactsList[i];

			if( c.PhotoTexture != null )
			{
				GUILayout.Box( new GUIContent(c.PhotoTexture) , GUILayout.Width(size.y), GUILayout.Height(size.y));
			}
			else 
			{
				GUILayout.Box( new GUIContent(contactFaceStyle.normal.background) , GUILayout.Width(size.y), GUILayout.Height(size.y));
			}

			string text = "Native Id : " + c.Id + "\n";
			text += "Name : " + c.Name + "\n";
			for(int p = 0 ; p < c.Phones.Count ; p++)
			{
				text +=  "Number : " + c.Phones[p].Number + " , Type " + c.Phones[p].Type  + " \n";
			}
			for(int e = 0 ; e < c.Emails.Count ; e++)
				text +=  "Email  : " + c.Emails[e].Address + " : Type " + c.Emails[e].Type + "\n"; 
			for(int e = 0 ; e < c.Connections.Count ; e++)
				text += "Connection : " + c.Connections[e] + "\n";
			text += "------------------";
			GUILayout.Label(  text , style , GUILayout.Width(size.x - size.y - 40 ) );
			GUILayout.Space(50);
			GUILayout.EndHorizontal();
		}
		int afterHeight = (int)((Contacts.ContactsList.Count - ( startIndex + 6 ))*size.y) ;
		if( afterHeight > 0 )
		{
			//fill invisible gap after scroller to make proper scroller pos
			GUILayout.BeginHorizontal(  GUILayout.Width(size.x*2) , GUILayout.Height(afterHeight ) );
			GUILayout.Box(" ",nonStyle2);
			GUILayout.EndHorizontal();
		}
		else
		{
			GUILayout.BeginHorizontal(  GUILayout.Width(size.x*2) , GUILayout.Height(1f) );
			GUILayout.Box(" ",nonStyle2);
			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();
	}


	void onLoadFailed( string reason )
	{
		failString = reason;
	}

	void onDone()
	{
		failString = null;
	}
}
