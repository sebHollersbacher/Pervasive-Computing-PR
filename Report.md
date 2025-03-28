# Report - Implementation of 3D Drawing and Shaping in VR

| Name                    | Matriculation Number | Semester | Course-id |
| ----------------------- | -------------------- | -------- | --------- |
| Sebastian Hollersbacher | 12015864             | WS-2024  | 367.032   |

Project Code and Milestone videos can be seen on the `GitHub Repository`[^GitHub].

The source files are located in `Assets/Scripts`.



## Goal

The goal of the project was to create a drawing and modelling application for a VR-Headset. The application should work similar as a standard 2D graphical editors but in a 3D space where the user can also move around. The following features have been implemented:

- Drawing and Erasing of 3D-Lines
- Creating basic Shapes
- Changed Color/Size for creating Shapes/Lines
- Selecting Objects and Manipulating them
- Modelling Cubes



## Setup of Unity

For the project, the Unity-3D project template is used with the Unity-Editor version `2022.3.56f1`.

Starting the project, the first part is to include the `Oculus XR Plugin`, and from the Asset Store `Meta XR All-in-One SDK`[^Meta XR All-in-One SDK] using the Unity-Package-Manager. Using these 2 packages is mostly used combined with an Oculus Quest, and also the best documented way.

After the packages have been imported, the Platform of the project has to be switched to `Android` inside the Build Settings.

To enable the Stereoscopic view of the VR-Headset, the Camera of the Scene has to be replaced using the `OVRCameraRig`prefab. In the `OVRManager ` script of the `OVRCameraRig` the Target Device `Oculus Quest 2` has to be selected for the Oculus Quest 1 to work.

What is left to do, to be able to `Build and Run` the project, is to fix all "Outstanding Issues" and apply "Recommended Items" in `Edit > Project Settings > Meta XR > Android Settings`.

To also see the controller movement, `OculusTouchForQuestAndRiftS ` have been added to the `ControllerAnchors` of the `OVRCameraRig`.



## Drawing Lines

Drawing Lines has been implemented by using a defined 'Brush' object on the controller and creating a Mesh when the brush is moved and the Interaction-Button (Right Trigger) of the controller is pressed. The Mesh is updated if the Brush moved a certain distance while the trigger is pressed, by adding a new segment to the line. The segments are generated by creating cylinders manually, adding the vertices and triangles to the already existing Mesh. This way of implementing the drawing has been chosen, as the Mesh is extended step by step which results in a good performance and also decent looking lines. It also enables for an easier implementation of the erasing.



## Erasing Lines

The erasing of lines works by creating a Unity-`BoxCollider` for each segment (cylinder) of a line. If a predefined 'Eraser', attached to the controller, enters the collider, the segment gets deleted, by removing the vertices and triangles from the Mesh. In this case, it is crucial to keep track of the indices of the segments to delete the right one. With the implementation of drawing lines, removing parts of the mesh offers an easy solution with a result that feels good to use.



## Changing Color/Size/Mode

### Creating an interactable Canvas

For changing color/size/mode, a Canvas has to be created for the user to interact with. The prefab `OculusInteractionSampleRayCanvas2` is used as a base, where the `CanvasCylinder` script has been exchanged with the `CanvasRect` script as the canvas will be a flat plane (additionally the `PointableCanvasMesh` has to be re-added to the canvas).

To also interact with the canvas, interactors have to be defined for the controllers (in this case `RayInteractor`). To achieve this, the `RayExample` Scene of the `Meta XR Interaction SDK Examples` has been used as a reference, where the `OVRInteraction` and the `Pointable Canvas Module` have been copied into the project. Now only the `OVRCameraRig` has to be set inside the `OVRCameraRigRef` script inside the `OVRInteraction`.

The canvas was then bound to the Menu-Button (Left-Trigger), so the menu can be hidden/revealed.

### UI Elements

For changing modes, buttons for each mode have been added to canvas, which change the mode when pressed and also indicate which mode is active.

Changing size is done by using a horizontal scrollbar changing the radius of the lines.

The color change is done the same as the size, but instead of a scrollbar, the `Flexible Color Picker` [^Flexible Color Picker] from the Unity Assets Store has been used, which can be seen below. This Asset offers a good API for selecting a color on a canvas (requirement in this case), while also being customizable.

<img src="D:\OneDrive\Studium\PR Pervasive Computing\Report-Images\Color-Picker.png" alt="Colour-Picker" style="zoom:45%;" />

With the 2 fields, the user can choose the Hue (left field), Saturation and Value (right field). Using HSV is the most intuitive way for users to choose a color, as the most important part (Hue) is what is primarily used.



## Creating of Shapes

For creating shapes a new mode is introduced using a new Menu, where the user can select the desired shape out of the 6 possible ones (Line, Plane, Cube, Sphere, Cylinder, Pyramid).

The selected shape is then created by pressing the Interaction-Button. For creating the Plane and Cube, the Unity-Primitive type `Cube` is used, for the Sphere the type `Sphere` and for the Line and Cylinder, the type `Cylinder` is used. The pyramid is manually created by defining the vertices and triangles to draw it.

After creating the shape, the user can scale it my moving the controller away from the initial creation point.



## Selection of Objects

The selection process of an object is done by first creating a transparent cube, similar to creating a cube in the "Creating of Shapes" feature, using a dedicated Selection Button (Right Grip Button). To make the cube transparent, a Costume Shader had to be defined. Here it was also important to add the Shader to the list of `Always Included Shaders` inside the `Graphics` tab of the `Project Settings`.

After the cube is created, all intersecting objects are added to a list of selected elements, and are given an outline, using the `Quick Outline`[^Quick Outline] package. The intersection between objects and the selection cube is calculated by using the Unity Colliders.

Using the Deselect Button (Right Primary Button), the selected objects are removed from the selected-objects-list and their outlines are removed.

The way the selection works should imitate how selection works in 2D. To simplify the process, objects can not be partially selected but only as a whole. Also to not restrict the view of the user, the selection cube has been chosen to be transparent, as for the outline, the selected package offers an easy way to highlight 3D objects.



## Manipulation of Selections

Using and manipulating the selected objects can be done, by selecting a mode on the "Selection" tab on the menu canvas. The following modes can be chosen:

- Delete: Deletes all selected objects
- Move: Moves the objects like the controller moves, when the Interaction Button is pressed
- Rotate: Rotates each objects individually like the controller when the Interaction Button is pressed
- Scale: Scales the objects in the direction where the controller is moved
- Align Rotation: Shows the alignment object seen below, where the user can select a fixed rotation for the objects (top of the object will face where the selected arrow is pointing)
- Align Position: Shows the alignment object and when an arrow is pressed, all objects move in that direction to the furthest object (only works if 2 or more objects are selected).

<img src="D:\OneDrive\Studium\PR Pervasive Computing\Report-Images\Alignment.png" alt="Alignment" style="zoom:50%;" />

The moving, rotating, and scaling is done by using the Unity internal coordinate system as it already provides those basic transformations and the differences has to only be mapped from the controller to the objects.





## Modelling Objects

The modelling of objects is done by using the API of the Unity-package `ProBuilder` as it provides all the required features and is part of Unity itself. This package allows the developer to create objects and manipulate them by moving the objects vertices/edges/faces, create new edges between vertices, subdivide edges/faces and more. But this only works inside the Unity Editor, so the API has to be used to enable these features also in runtime.

The first part is to change the creation of objects to create them with the `ProBuilderMesh`. This has currently only been done with creation of the cube. When a new cube is created the vertices and edges have to be made selectable. This is achieved by creating a new `Vertex` and `Edge` class containing the internal vertex/edge of the `ProBuilderMesh` and `GameObject` (Sphere/Cube) which can then be selected/moved. The selection is done by tracking the vertices/edges in a list by adding them when they collide with a predefined selector object.

When moving the vertices/edges, by holding down the Interaction Button and moving the controller inside the shaping mode, the internal vertices have to be moved and also the `GameObjects` representing the vertices/edges have to be updated. Here it is also important to know, that each corner is represented by multiple vertices (1 for each face where the corner is used).

To better shape an object it is important to be able to create new vertices/edges and not be restricted by the initial ones. For this feature, the `ProBuilder` API provided functions that could be used. After creating new vertices/edges, their visual representation had to be regenerated, as it does not include the newly created ones.



## Coordinate System

The coordinate system has been made by creating vertical planes along the X and Z-Axis with a transparent repeating box image with an additional plane and image on the bottom of the vertical planes for markers. The floor is done using a horizontal plane and a repeating texture, which can be seen below. Both the texture for the floor and the sides indicate one unit of the Unity integrated coordinate system, which translates to 1 meter in reality. By using try and error, this textures managed to deliver the size and position of the user and controller while also not overwhelming the user with too much information.

![Floor](D:\OneDrive\Studium\PR Pervasive Computing\Report-Images\Floor.png)

Additionally, transparent bars can be added, using a toggle button on the menu canvas, going from the sides/floor to the currently selected tool on the controller to better identify the current position.

<div style="page-break-after: always; break-after: page;"></div>

[^GitHub]: https://github.com/sebHollersbacher/Pervasive-Computing-PR
[^Meta XR All-in-One SDK]: https://assetstore.unity.com/packages/tools/integration/meta-xr-all-in-one-sdk-269657
[^Flexible Color Picker]: https://assetstore.unity.com/packages/tools/gui/flexible-color-picker-150497
[^Quick Outline]: https://assetstore.unity.com/packages/tools/particles-effects/quick-outline-115488

