# Camera-based object tracking for VR experiences

**Project Objective:** Tracking a ball in 3D physical space and reproducing it within a VR headset, such that ball games can be played.

Immersivity of Virtual Reality experiences is often enhanced when users are able to interact with objects within a virtual environment. 

Typically the mode of interaction restricts users to handling non-existent objects, creating a sense of incompleteness whereby the user's tactile sense is not stimulated. 

This project introduces a complete system, such that a physical object is tracked using a webcam and reproduced in the same precise location within a virtual environment, allowing users to manipulate the object whilst in virtual reality.

The system I have created consists of two components that communicate via UDP:

- A python 2.7.16 application that utilises [OpenCV](https://opencv.org/) to handle webcam data and produce an output of positional information of an untethered object (a ball!)
- A C# virtual reality application that takes the positional data of said object and renders it into the same position relative to the user, such that it can be interacted with (Unity3D implementation)

## Demo Videos

Handling the ball 🎾

![Ball Demo](https://media.giphy.com/media/w53V104XFiCCjfoaR7/giphy-downsized-large.gif)

Bowling experience 🎳

![Bowling Demo](https://media.giphy.com/media/WoanwYbF20QDUPGmPu/giphy.gif)


## Created by:

[George Lowe](https://github.com/georgelowe)

Contact me here:

<p align="left">
  <a href="https://www.linkedin.com/in/george-lowe/"> 
    <img alt="My LinkedIn" src="https://img.shields.io/badge/-LinkedIn-0072b1?style=flat&logo=Linkedin&logoColor=white" />
  </a>
  <a href="https://twitter.com/gloweio"> 
    <img alt="My Twitter" src="https://img.shields.io/badge/-Twitter-00acee?style=flat&logo=Twitter&logoColor=white" />
  </a>
</p>


This project was initially produced as part of my Dissertation project whilst undertaking my BSc in Computer Science.
