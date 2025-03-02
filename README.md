
# GeoGame Backend API Documentation

This is the backend server for the **GeoGame** application built with **ASP.NET Core**. It provides various API endpoints for user management, game data, city traversal, and friendship features. Below are the details of the available APIs.

## Table of Contents

1. [API Endpoints](#api-endpoints)
    1. [Register](#1-register) - Registers the user with the game and provides a unique ID
    3. [Login](#2-login) - user can log in to the game with this api
    4. [GetUser](#3-getuser) - metadata of the user can be fetched using this - details like friends, score, which city to display next can be found in this object
    5. [GetLocation](#4-getlocation) - metadata of the city can be fetched from here
    6. [UpdateUser](#5-updateuser) - User object can be updated using this api , updating the score or next city
    7. [MoveToNextCity](#6-movetonextcity) - this moves the user to next city question
    8. [ResetCityList](#7-resetcitylist) - This api helps to reset the game
    9. [CreateFriendRequest](#8-createfriendrequest) - This generates a link which can be used by other user to join the game
    10. [AcceptFriendRequest](#9-acceptfriendrequest) - Once this link is clicked, both the users will be added to each others list.

## Tech Stack

* ASP .NET
* MongoDB
---


