# open-banking-poc

A open banking proof-of-concept for integrating with banks using PSD2 directive and webscraping.

## Purpose

This project was created partly for [my portfolio of projects](https://davidrunemalm.com).

## Key Features

- ✅ Simple monolithic architecture.
- ✅ Prepared for microservices.
- ✅ One developer per bank integration.
- ✅ Javascript SDK for website integration.

## Demo

The demo will be available online soon.

Please come back later.

## Architecture

TODO: ...

## Frontend

The frontend is a JavaScript client SDK built using React and ready to drop-in for easy website integration.

## Backend

The backend is written using C# and .NET8.

## Bank Integration Example

You can add a new bank integration by simply subclassing the StateMachineBase class.

Example: [SeSeb01.cs](https://github.com/runemalm/open-banking-poc/blob/master/Backend/src/Sessions/Infrastructure/Integrations/Se/Seb/SeSeb01.cs)
