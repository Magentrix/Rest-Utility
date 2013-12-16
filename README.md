Rest-Utility
============

## C# Wrapper for Accessing a Magentrix Portal

This C# wrapper is used for accessing a Magentrix Portal from an external source. With it you can query, insert, update, and delete objects from the portal. 

The RESTUtility class contains everything you need to access the Magentrix Portal. 

The sample.cs (along with the Account.cs) is an example that hows you to successfully connect with the Magentrix Portal.

## Overview

### Authenticate


### Query
To Query an object, you must supply the type and the query. It will return a QueryResult objetc.

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

REST api = new REST(@"https://example.magentrix.com", @"username@example.com", "password");

if(api.Login(true).Success)
{
  // perform your desired actions here  
}

```

### Performing actions on Account

In order to query a list of Accounts, you must create an Account object with the necessary information you need

```csharp

public class Account
{
  public string Id { get; set; }
  public string Name { get; set; }
  public DateTime? CreatedOn { get; set; }
}

```

### Querying a list of Accounts

In order to actually query the list of Accounts, you must do the following

```csharp

QueryResult<Account> accounts = api.Query<Account>("FROM Account WHERE CreatedOn < DateTime.Now.Date");

if(accounts.Count > 0)
{
  // perform logic
}

```

### Create an Account

In order to create an Account, you must do the following

```csharp

SaveResult result = api.Create<Account>(new Account { Name = "Test Account",  });


```

### Editing an Account

```csharp

QueryResult<Account> accounts = api.Query<Account>("FROM Account WHERE CreatedOn < DateTime.Now.Date");

if(accounts.Count > 0)
{
  foreach (Account account in account)
  {
    // change the account name
    account.Name = account.Name + " modified";
    
    // update the account
    SaveResult result = api.Update<Account>(account);
  }
}

```

### Deleting an Account

```csharp

QueryResult<Account> accounts = api.Query<Account>("FROM Account WHERE CreatedOn < DateTime.Now.Date");

if(accounts.Count > 0)
{
  foreach (Account account in account)
  {
    // delete the account and make it permanent
    DeleteResult result = api.Delete(account.Id, true);
  }
}

```
