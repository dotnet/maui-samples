---
name: .NET MAUI - ScaleAndRotate
description: The .NET MAUI Scale and Rotation properties allow a program to scale and rotate .NET MAUI visual elements. In addition, the ScaleX and...
page_type: sample
languages:
- csharp
products:
- dotnet-maui
urlFragment: scaleandrotate
---
# ScaleAndRotate

The .NET MAUI *Scale* and *Rotation* properties allow a program to scale and rotate .NET MAUI visual elements. In addition, the *ScaleX* and *ScaleY* properties perform scaling on the X and Y axes, and the *RotationX* and *RotationY* properties perform 3D-like rotations around the X and Y axes.

ScaleAndRotate lets you interactively experiment with these properties using a *Slider*. All these transforms are affected by the *AnchorX* and *AnchorY* properties, which establish a pivot point relative to the transformed view that remains in place when the transform is applied.
*Stepper* views let you set the *AnchorX* and *AnchorY* properties to values ranging from -1 to 2, in increments of 0.5.

![ScaleAndRotate application screenshot](Screenshots/scale-and-rotate.png "ScaleAndRotate application screenshot")

