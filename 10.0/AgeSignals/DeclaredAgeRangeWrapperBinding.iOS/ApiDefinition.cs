using Foundation;
using ObjCRuntime;
using System;
using UIKit;

namespace DeclaredAgeRangeWrapper;

// @interface DeclaredAgeRangeBridge : NSObject
[BaseType(typeof(NSObject), Name = "_TtC23DeclaredAgeRangeWrapper22DeclaredAgeRangeBridge")]
interface DeclaredAgeRangeBridge
{
    	// +(void)requestAgeRangeWithAgeGates:(NSInteger)threshold1 :(NSNumber * _Nullable)threshold2 :(NSNumber * _Nullable)threshold3 in:(UIViewController * _Nonnull)viewController completion:(void (^ _Nonnull)(MyAgeRangeResponse * _Nullable, NSError * _Nullable))completion;
	[Static]
	[Export("requestAgeRangeWithAgeGates:::in:completion:")]
	void RequestAgeRange(nint threshold1, [NullAllowed] NSNumber threshold2, [NullAllowed] NSNumber threshold3, UIViewController viewController, Action<MyAgeRangeResponse, NSError> completion);
}

// @interface MyAgeRange : NSObject
[BaseType(typeof(NSObject), Name = "_TtC23DeclaredAgeRangeWrapper10MyAgeRange")]
[DisableDefaultCtor]
interface MyAgeRange
{
    // @property (readonly, nonatomic, strong) NSNumber * _Nullable lowerBound;
    [NullAllowed, Export("lowerBound", ArgumentSemantic.Strong)]
    NSNumber LowerBound { get; }

    // @property (readonly, nonatomic, strong) NSNumber * _Nullable upperBound;
    [NullAllowed, Export("upperBound", ArgumentSemantic.Strong)]
    NSNumber UpperBound { get; }

    // @property (readonly, nonatomic) enum MyAgeRangeDeclaration declaration;
    [Export("declaration")]
    MyAgeRangeDeclaration Declaration { get; }
}

// @interface MyAgeRangeResponse : NSObject
[BaseType(typeof(NSObject), Name = "_TtC23DeclaredAgeRangeWrapper18MyAgeRangeResponse")]
[DisableDefaultCtor]
interface MyAgeRangeResponse
{
    // @property (readonly, nonatomic) enum MyAgeRangeResponseType type;
    [Export("type")]
    MyAgeRangeResponseType Type { get; }

    // @property (readonly, nonatomic, strong) MyAgeRange * _Nullable range;
    [NullAllowed, Export("range", ArgumentSemantic.Strong)]
    MyAgeRange Range { get; }
}
