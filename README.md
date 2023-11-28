# Identity

This comprehensive solution encompasses four distinct projects, each of which incorporates various authentication and authorization methods. These projects have been meticulously developed to cater to different scenarios, utilizing role-based, claim-based, combination of role and claim, and permission-based approaches. By implementing these diverse methods, this solution ensures robust security measures and provides flexibility in managing user access and privileges. In order to develop these projects, [Microsoft identity platform] has been used.

If you find this project useful, please give it a star. Thanks! ⭐

### SimpleRoleBase Project
In this project, the required roles are defined and then each user is assigned a role. After authentication, users can access resources if they have the relevant role.

### SimpleClaimBase Project
In this project, the required claims are defined static and then one or more claim assign to each user. After authentication, users can access resources if they have the relevant claim.

### MixClaimRole Project
In this project, after statically defining claims and defining system roles, one or more claims must be assigned to each role. Then roles are given to users. Finally, if the user with the role has the required claim, he will get access to the desired resource.

### PermissionBase Project
This project is similar to the mode of combining role and claim in terms of operation; With the difference that there is no need to define the claims statically. In this method, the full name of each API is considered as a claim. If a user has the necessary claim, he will get access to the desired resource.

## Getting Started
- Download this Repository (and modify as needed)

To get started based on this repository, you need to get a copy locally. You have three options: **fork**, **clone**, or **download**.

1- If you intend to experiment with the project or use it as a foundation for an application, it is recommended to **download** the repository, unblock the zip file, and extract it into a new folder.

2- **Fork** this repository only if your intention is to submit a pull request or if you desire to maintain a personal copy of the repository in your GitHub account.

3- If you are a contributor with commit access, it is advisable to **clone** this repository.

## Technologies and Important Libraries
- [ASP.Net 6]
- [ASP.NET Core Identity]
- [Result]

## Contributing

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
This project is licensed with the [MIT license].


[Microsoft identity platform]: <https://learn.microsoft.com/en-us/entra/identity-platform/>
[ASP.Net 6]: <https://github.com/dotnet/aspnetcore>
[ASP.NET Core Identity]:  <https://github.com/dotnet/aspnetcore/tree/main/src/Identity>
[Result]: <https://github.com/ardalis/Result>
[MIT license]: <https://github.com/yapma/identity/blob/main/LICENSE>
