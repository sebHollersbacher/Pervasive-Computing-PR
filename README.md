# Pervasive-Computing-PR

| **Name**                 | **Matriculation Number** | **Semester** | **Course-id** |
| ------------------------ | ------------------------ | ------------ | ------------- |
| Sebastian  Hollersbacher | 12015864                 | WS-2024      | 367.032       |

 

The goal of the project is to create a drawing and modelling application for a VR-Headset. The application should work similar as a standard 2D graphical editors but in a 3D space where the user can also move around.



## Features

The following features should be available in the application.

- Drawing lines using the controller
- Erasing lines
- Change colours
- Changing line-size
- Creating basic shapes
  - Lines
  - Planes
  - Cube
  - Sphere
  - Cylinder
  - Pyramid
- Select areas
- Areas that are selected should be able to be:
  - Deleted
  - Moved
  - Rotated
  - Scaled
  - Aligned (left/middle/right/top/bottom)
- Modelling shapes
  - Moving sides/edges/corners
- Showing 3D-Coordinate system



 

## Implementation

The implementation of drawing should be straightforward. Using the controller, the user creates small objects (like cubes or spheres, depending on what works best) along the controller position. What may lead to problems is the number of objects created, which may lead to performance problems. 

Erasing drawings is then done by removing these objects.

Changing colour/size can be simply done by changing the attribute of the created objects.

Creating basic shapes can be done by using the already available shapes in Unity. 

When implementing the modelling of shapes, it can be done by either using a Unity built-in function or by manually drawing/adjusting the smaller parts of the shape individually (e.g. sides of a cube)

Selecting areas will be implemented by the user choosing 2 points using the controller position, creating an invisible cube area, and collecting all elements inside this cube to be manipulated.

Manipulating on a selected area should then happen in different modes, where the modes can be realized by using the transformation of Unity.

 

 

## Resources

For the project, the following is needed:

- VR-Headset (in this case Oculus Quest 1)
- Unity Editor

 

# Milestones

| Implementation of  basic drawing           | 22.11     |
| ------------------------------------------ | --------- |
| Drawing  additions (Erasing, Colour, Size) | 29.11     |
| Implementation of creating  shapes         | 13.12     |
| Area  Selection and Manipulation           | 3.1.2025  |
| Modelling shapes                           | 31.1.2025 |
| Refinement  & Bugfixes                     | 14.2.2025 |

 