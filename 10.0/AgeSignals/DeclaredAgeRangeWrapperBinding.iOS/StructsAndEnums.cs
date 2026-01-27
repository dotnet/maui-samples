using ObjCRuntime;

namespace DeclaredAgeRangeWrapper;

[Native]
public enum MyAgeRangeResponseType : long
{
    Sharing = 0,
    DeclinedSharing = 1
}

[Native]
public enum MyAgeRangeDeclaration : long
{
    SelfDeclared = 0,
    GuardianDeclared = 1,
    Unknown = 2
}
