//
//  Contacts.m
//  ContactsList
//
//  Created by Apple on 8/4/14.
//  Copyright (c) 2014 DreamMakers. All rights reserved.
//

#import "Contacts.h"
#import <AddressBook/AddressBook.h>
@implementation Contacts


@end

NSString* CreateNSString (const char* string)
{
	if (string)
		return [NSString stringWithUTF8String: string];
	else
		return [NSString stringWithUTF8String: ""];
}

// Helper method to create C string copy
char* MakeStringCopy (const char* string)
{
	if (string == NULL)
		return NULL;
	
	char* res = (char*)malloc(strlen(string) + 1);
	strcpy(res, string);
	return res;
}

@interface ContactItem :NSObject
{
@public ABRecordRef person;
@public NSString *firstName ;
@public NSString *lastName;
@public ABMultiValueRef phoneNumbersRef;
@public NSMutableArray *phoneNumber;
@public NSMutableArray *phoneNumberType;
@public bool  emailsInitialized ;
@public NSMutableArray *emails;
@public bool  imageInitialized ;
@public char* image;
};
@end
@implementation ContactItem


@end
extern "C" {
    CFIndex nPeople;
    CFArrayRef allPeople;
    NSMutableArray* contactItems;
    ABAddressBookRef addressBook;
    
    void checkForRelease()
    {
        if( addressBook )
        {
            for( int i = 0 ; i < contactItems.count ; i ++ )
            {
                ContactItem* c = [contactItems objectAtIndex:i];
                if( c->imageInitialized == false )
                    return;
                if( c->emailsInitialized == false )
                    return;
            }
            CFRelease(addressBook);
            CFRelease(allPeople);
            addressBook = NULL;
        }
    }
    
    void listContacts()
    {
        //ABRecordRef source = ABAddressBookCopyDefaultSource(addressBook);
        allPeople = ABAddressBookCopyArrayOfAllPeopleInSourceWithSortOrdering(addressBook, nil, kABPersonSortByFirstName);
        nPeople = ABAddressBookGetPersonCount( addressBook );
        // NSLog( @"error %@" , *error );
        NSLog( @"cont %ld" , nPeople );
        contactItems = [NSMutableArray new];
        for ( int i = 0; i < nPeople; i++ )
        {
            ContactItem* c = [[ContactItem alloc]init];
            c->person = CFArrayGetValueAtIndex( allPeople, i );
			if( c->person == nil)
			{
				NSLog( @"contact %ld is empty" , i );
				continue;
			}
            c->firstName = (__bridge NSString *)(ABRecordCopyValue(c->person,kABPersonFirstNameProperty));
            c->lastName = (__bridge NSString *)(ABRecordCopyValue(c->person, kABPersonLastNameProperty));
            
            c->phoneNumbersRef = ABRecordCopyValue(c->person, kABPersonPhoneProperty);
            int phonesCount = ABMultiValueGetCount(c->phoneNumbersRef);
            c->phoneNumber = [NSMutableArray new];
			c->phoneNumberType = [NSMutableArray new];
            for (CFIndex i = 0; i < phonesCount ;i++) {
                NSString *phoneNumber = (__bridge NSString *) ABMultiValueCopyValueAtIndex(c->phoneNumbersRef, i);
                [c->phoneNumber addObject:phoneNumber];
                
                CFStringRef locLabel = ABMultiValueCopyLabelAtIndex(c->phoneNumbersRef, i);
				 NSString* phoneLabel = (__bridge NSString*) ABAddressBookCopyLocalizedLabel(locLabel);
				 [c->phoneNumberType addObject:phoneLabel];
            }
            
            c->imageInitialized = false ;
            c->emailsInitialized = false ;
            
            
            [contactItems addObject:c];
           
        }
        UnitySendMessage("ContactsListMessageReceiver", "OnInitializeDone","");
        
        checkForRelease();
    }
    
    
    
    void _LoadContactList()
    {
        ABAuthorizationStatus status = ABAddressBookGetAuthorizationStatus();
        if (status == kABAuthorizationStatusDenied) {
            // if you got here, user had previously denied/revoked permission for your
            // app to access the contacts, and all you can do is handle this gracefully,
            // perhaps telling the user that they have to go to settings to grant access
            // to contacts
            NSLog(@"permissin issu");
            [[[UIAlertView alloc] initWithTitle:nil message:@"This app requires access to your contacts to function properly. Please visit to the \"Privacy\" section in the iPhone Settings app." delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil] show];
            return;
        }
        CFErrorRef *error = NULL;
        addressBook = ABAddressBookCreateWithOptions(NULL, error );
        if (error) {
            NSLog(@"error: %@", CFBridgingRelease(error));
            if (addressBook) CFRelease(addressBook);
            return;
        }
        if (status == kABAuthorizationStatusNotDetermined) {
            
            // present the user the UI that requests permission to contacts ...
            
            ABAddressBookRequestAccessWithCompletion(addressBook, ^(bool granted, CFErrorRef error) {
                if (granted) {
                    // if they gave you permission, then just carry on
                    
                    listContacts();
                } else {
                    // however, if they didn't give you permission, handle it gracefully, for example...
                    
                    dispatch_async(dispatch_get_main_queue(), ^{
                        // BTW, this is not on the main thread, so dispatch UI updates back to the main queue
                        
                        [[[UIAlertView alloc] initWithTitle:nil message:@"This app requires access to your contacts to function properly. Please visit to the \"Privacy\" section in the iPhone Settings app." delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil] show];
                    });
                }
                
                //CFRelease(addressBook);
            });
        }
        else if( status == kABAuthorizationStatusAuthorized )
        {
            listContacts();
        }
        
    }
    
    char* GetMyPhoneNumber()
    {
        return MakeStringCopy("");
    }
    char* GetSimSerialNumber()
    {
        return MakeStringCopy("");
    }
	
	char* GetNetworkOperator()
    {
        return MakeStringCopy("");
    }
	
	char* GetNetworkCountryIso()
    {
        return MakeStringCopy("");
    }
	
	int GetContactsCount()
    {
        int count = (int)nPeople;
        NSLog(@"------user ask for conatcts count , count is %i" , count);
        return count;
    }
	
	char* GetContactId( int id )
    {
        return MakeStringCopy("");
    }
	
	char* GetContactName( int index )
    {
        ContactItem* c = [contactItems objectAtIndex:index];
        NSString* f = (c->firstName == NULL)?[[NSString alloc]init]:c->firstName ;
        NSString* s = (c->lastName == NULL)?[[NSString alloc]init]:c->lastName ;
        NSString* name = [NSString stringWithFormat:@"%@ %@",f,s];
        return MakeStringCopy([name UTF8String]);
    }
	
    int GetContactNumberCount( int index )
    {
        ContactItem* c = [contactItems objectAtIndex:index];
        return c->phoneNumber.count;
    }
	char* GetContactNumber( int index , int phoneId )
    {
        ContactItem* c = [contactItems objectAtIndex:index];
        if( c->phoneNumber == nil || c->phoneNumber.count == 0 )
            return MakeStringCopy([@"empty" UTF8String]);
        NSString* text = [c->phoneNumber objectAtIndex:phoneId];
        return MakeStringCopy([text UTF8String]);
    }
	char* GetContactNumberType( int index , int phoneId )
	{
		ContactItem* c = [contactItems objectAtIndex:index];
        if( c->phoneNumberType == nil || c->phoneNumberType.count == 0 )
            return MakeStringCopy([@"empty" UTF8String]);
        NSString* text = [c->phoneNumberType objectAtIndex:phoneId];
        return MakeStringCopy([text UTF8String]);
	}
    int GetEmailsCount( int index )
    {
        ContactItem* c = [contactItems objectAtIndex:index];
        if(c->emailsInitialized == false)
        {
            c->emails = [NSMutableArray new];
            ABMultiValueRef emails = ABRecordCopyValue(c->person, kABPersonEmailProperty);
            for (CFIndex j=0; j < ABMultiValueGetCount(emails); j++) {
                NSString* email = (__bridge NSString*)ABMultiValueCopyValueAtIndex(emails, j);
                [c->emails addObject:email];
                //[email release];
            }
            CFRelease(emails);
            c->emailsInitialized =true;
            checkForRelease();
        }
        return c->emails.count;
    }
	char* GetEmail( int index , int emailId )
    {
        ContactItem* c = [contactItems objectAtIndex:index];
        if( c->emails == nil || c->emails.count == 0 )
            return MakeStringCopy([@"empty" UTF8String]);
        NSString* text = [c->emails objectAtIndex:emailId];
        return MakeStringCopy([text UTF8String]);
    }
	
	char* GetContactPhoto( int index )
    {
        ContactItem* c = [contactItems objectAtIndex:index];
        if( c->imageInitialized == false )
        {
            UIImage *img = nil;
            c->image = nil;
            if (c->person != nil && ABPersonHasImageData(c->person)) {
                if ( &ABPersonCopyImageDataWithFormat != nil ) {
                    NSData * data = (__bridge NSData *)ABPersonCopyImageDataWithFormat(c->person, kABPersonImageFormatThumbnail);
                    NSString *im = [data base64Encoding] ;
                    const char* imc = [ im UTF8String ];
                    c->image = MakeStringCopy(imc) ;
                    // iOS >= 4.1
                    // img= [UIImage imageWithData:(__bridge NSData *)ABPersonCopyImageDataWithFormat(c->person, kABPersonImageFormatThumbnail)];
                } else {
                    NSData * data = (__bridge NSData *)ABPersonCopyImageData(c->person);
                    NSString *im = [data base64Encoding] ;
                    const char* imc = [ im UTF8String ];
                    c->image = MakeStringCopy(imc);
                    // iOS < 4.1
                    //img= [UIImage imageWithData:(__bridge NSData *)ABPersonCopyImageData(c->person)];
                }
                //CFRelease( img );
                
            } else {
                img= nil;
                c->image = nil;
            }
            c->imageInitialized = true;
            checkForRelease();
        }
        if(c->image == nil)
            return nil;
        char* im = c->image ;
        
        return  im ;
    }
}












