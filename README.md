Rest-Utility
============

## C# Wrapper for Accessing a Magentrix Portal

This C# wrapper is used for accessing a Magentrix Portal from an external source. With it you can query, insert, update, and delete objects from the portal. 

The RESTUtility class contains everything you need to access the Magentrix Portal. 

The sample.cs (along with the User.cs) is an example that shows you to successfully connect with the Magentrix Portal.

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
To Query an object, you must supply the type and the query. It will return a QueryResult object.

```csharp
QueryResult results = Query<T>(string query);
```

### Delete
When deleting an object, you must provide the id of the object you want to delete and a boolean to signify if this delete should be permanent. Deleting will return a DeleteResult object.

```csharp
DeleteResult result = Delete(string id, bool permanent);
```

### Create
When creating an object, you must supply the type. It returns a SaveResult object

```csharp
SaveResult result = Create<T>(T model); // T is the model type
```

### Update
When updating a model, you must supply the model type. It will return a SaveResult object

```csharp
SaveResult result = Update<T>(T model);
```

# Examples

### Connecting to a Magentrix Portal

Connecting to the portal is very simple. In order to authenticate, you provide the end destination, the username, and the password. Once the Login method returns true, you are able to perform actions. 

```csharp

REST api = new REST(@"https://yourcompany.magentrix.com");

var loginResult = api.Login(@"username@yourcompany.com", "password");


if(loginResult.IsSuccess)
{
  // perform your desired actions here, such as querying users, creating accounts, editing contacts, etc  
}

```

### Performing actions on User

In order to query a list of Users, you must create a User object with the necessary information you need

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
"FROM [entityname] WHERE [conditions] {ORDER BY [fieldname] ASC|DESC}{SELECT [field1, field2,...}"
```

The ORDER BY and SELECT values are optional. For example, all the following queries are valid:
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

### Deleting a User

In order to delete a user, you must do the following:

```csharp
string name = "Test Testerson";

QueryResult<User> users = api.Query<User>("FROM User WHERE Name = " + name);

if(users.Count > 0)
{
	User user = users[0];
	
	// delete the user and make it permanent
	DeleteResult result = api.Delete(user.Id, true);	

}
```

### Bringing it all together

We will create create a connection, create a user, edit the user, and then delete the user

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
	
	string firstName = "Jim";
	
	QueryResult<User> toBeDeleted = api.Query<User>("FROM User WHERE Firstname = " + firstName + " SELECT Id");
	
	if(toBeDeleted.Count > 0)
	{
		User u = toBeDeleted[0];
		
		DeleteResult deleteResult = api.Delete(u.Id, true);
	}	
}
```