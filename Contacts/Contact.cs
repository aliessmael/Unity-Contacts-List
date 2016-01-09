using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	
	public List<PhoneContact> Phones = new List<PhoneContact>();
	public List<EmailContact> Emails = new List<EmailContact>();
	
	public List<string> Connections = new List<string>();//for android only. example(google,whatsup,...)

	public Texture2D PhotoTexture ;
	
	public void FromBytes( byte[] bytes )
	{
	
		System.IO.BinaryReader reader = new System.IO.BinaryReader( new System.IO.MemoryStream( bytes ));
		
		Id = readString( reader );
		Name = readString( reader );
		
		short size = reader.ReadInt16();
		log( "Photo size == " + size );
		if( size > 0 )
		{
			byte[] photo = reader.ReadBytes( (int)size);
			PhotoTexture = new Texture2D(2,2);
			PhotoTexture.LoadImage( photo );
		}
		
		size = reader.ReadInt16();
		log( "Phones size == " + size );
		if( size > 0 )
		{
			for( int i = 0 ; i < size ; i++ )
			{
				PhoneContact pc = new PhoneContact();
				pc.Number = readString( reader );
				pc.Type = readString( reader );
				Phones.Add( pc );
			}
		}
		
		size = reader.ReadInt16();
		log( "Emails size == " + size );
		if( size > 0 )
		{
			for( int i = 0 ; i < size ; i++ )
			{
				EmailContact ec = new EmailContact();
				ec.Address = readString( reader );
				ec.Type = readString( reader );
				Emails.Add( ec );
			}
		}
		
		size = reader.ReadInt16();
		log( "Connections size == " + size );
		if( size > 0 )
		{
			for( int i = 0 ; i < size ; i++ )
			{
				string connection = readString( reader );
				Connections.Add( connection );
			}
		}
		
	}
	
	string readString( System.IO.BinaryReader reader )
	{
		string res = "";
		short size = reader.ReadInt16();
		log( "read string of size " + size );
		if( size == 0 )
			return res;
		
		byte[] data = reader.ReadBytes( size);
		res = System.Text.Encoding.UTF8.GetString( data );

		log( "string " + res + " is " + res);
	
		return res;
		
	}

	void log( string message )
	{
		//Debug.Log( message );
	}
}
