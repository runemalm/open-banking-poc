# open-banking-poc

A proof-of-concept demonstrating bank integration with a simple drop-in JS client.

## Purpose

Part of [my portfolio](https://davidrunemalm.com)

## Key Features

- ✅ Scalable logically separated monolith.
- ✅ Easily add more bank integrations.
- ✅ Plug-and-play client for web applications.

## Website Integration Demo

The demo will be available online soon.

Please come back later.

## Backend

The backend is written using C# and .NET8.

## Frontend

The frontend is a JavaScript client SDK built using React and ready to drop-in for easy website integration.

## Add Bank Integrations

You can add a new bank integration by simply subclassing the StateMachineBase class.

Example: [SeSeb01.cs](https://github.com/runemalm/open-banking-poc/blob/master/Backend/src/Sessions/Infrastructure/Integrations/Se/Seb/SeSeb01.cs)
