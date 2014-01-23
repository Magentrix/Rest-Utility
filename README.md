Rest-Utility
============

## C# Wrapper for Accessing the Magentrix Platform

This C# wrapper is used for accessing Magentrix platform APIs. With it you can query, insert, update, and delete objects from the platform. 

The RESTUtility class contains everything you need to access the Magentrix platform. 

The sample.cs (along with the User.cs) is an example that shows you to successfully connect with the Magentrix platform.

## Overview

### Authenticate
To Authenticate, create a new REST object and then call the login method

```csharp

//Create new instance of REST class
REST api = new REST(@"https://yourcompany.magentrix.com");

//Perform the login.
var loginResult = api.Login(@"username@example.com", "password");

```

### Query
To Query an Entity, you must supply the type and the query. It will return a QueryResult object.

```csharp
QueryResult results = Query<T>(string query);
```

### Delete

To delete a record from the Magentrix platform, use this method. 

When deleting an Entity, you must provide the record id of the Entity you want to delete and a boolean to signify if this delete should be permanent. Deleting will return a DeleteResult object.

```csharp
DeleteResult result = Delete(string id, bool permanent);
```

### Create

To create a record on the Magentrix platform, use this method. 

When creating an Entity, you must supply the type. It returns a SaveResult object

```csharp
SaveResult result = Create<T>(T model); // T is the model type
```

### Update

To update a record on the Magentrix platform, use this method. 

When updating a Entity, you must supply the Entity type. It will return a SaveResult object

```csharp
SaveResult result = Update<T>(T model);
```

# Examples

### Authenticating with your Magentrix organization

Authenticating with the platform is very simple. In order to authenticate, you provide the organization URL, the username, and the password. you can check the Login method's result to see if the authentication was successful or not. 

```csharp
REST api = new REST(@"https://yourcompany.magentrix.com");

var loginResult = api.Login(@"username@yourcompany.com", "password");

if(loginResult.IsSuccess)
{
  // perform your desired actions here, such as querying users, creating accounts, editing contacts, etc.  
}

```

### Performing Actions on User

In order to query a list of Users, you must create define a User class with the necessary fields you want to retrieve

```csharp
public class User
{
  public string Id { get; set; }
  public string Firstname { get; set; }
  public string Lastname { get; set; }
  public string Name { get; set; }
  public string Username { get; set; }
  public string Email { get; set; }
  public bool? IsActive { get; set; }
  public DateTime? LastLoginDate { get; set; }
}
```

### Querying a list of Users

In order to query a list of active Users, you must do the following:

```csharp
QueryResult<User> users = api.Query<User>("FROM User WHERE IsActive = true");

if(users.Count > 0)
{
  // perform logic
}
```

Passing a query to the api uses the following format:

```csharp
"FROM [entityname] WHERE [conditions] {ORDER BY [fieldname] ASC|DESC}{SELECT [field1], [field2],...}"
```

The ORDER BY and SELECT clauses are optional. The following examples demonstrate various ways you are able to construct a query string:
```csharp
"FROM User WHERE IsActive = true"

"FROM User WHERE IsActive = true ORDER BY Name ASC"

"FROM User WHERE IsActive = true SELECT Id, Name, Email"

"FROM User WHERE IsActive = true ORDER BY Email DESC SELECT Id, Name, Email"
```
### Create a User

In order to create a User, you must do the following:

```csharp
SaveResult result = api.Create<User>(new User { Firstname = "Test", Lastname = "Testerson" });
```

### Editing a User

In order to edit a user, you must do the following:

```csharp
string name = "Test Testerson";

QueryResult<User> users = api.Query<User>("FROM User WHERE Name = " + name);

if(users.Count > 0)
{
	// change the user's last name
	User user = users[0];
	user.Lastname = user.Lastname + " modified";

	// update the user
	SaveResult result = api.Update<User>(user);

}
```

### Deleting a Contact

In order to delete a user, you must do the following:

```csharp
string name = "Test Testerson";

QueryResult<Contact> contacts = api.Query<Contact>("FROM User WHERE Name = " + name);

if(contacts.Count > 0)
{
	Contact contact = contacts[0];
	
	// delete the contact
	DeleteResult result = api.Delete(contact.Id);	

}
```

### Bringing it all together

We will create create a connection, create a user, and edit the user

```csharp
REST api = new REST(@"https://yourcompany.magentrix.com");

var loginResult = api.Login(@"username@yourcompany.com", "password");

if(loginResult.IsSuccess)
{
	User user = new User();
	user.Firstname = "Test";
	user.Lastname = "Macpherson";
	user.Email = "test.macpherson@yourcompany.com";
	
	SaveResult saveResult = api.Create<User>(user);
	
	User loadedUser;
	string email = "test.macpherson@yourcompany.com";
		
	QueryResult<User> users = api.Query<User>("FROM User WHERE Email = " + email);
	
	if(users.Count > 0)
	{
		loadedUser = users[0];		
		loadedUser.Firstname = "Jim";		
		SaveResult updateResult = api.Update<User>(user);
	}
}
```