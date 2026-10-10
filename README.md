# app-dotnet-function

C# .NET 10 Isolated Worker Azure Function for the **pjmcore** workload platform.

## Repository Purpose

This repository contains the application source code, unit tests, and CI/CD workflows
that build and deploy the Function App across development, testing, and production environments.

## Endpoints

| Route | Purpose |
|---|---|
| `/api/health` | Health check validating storage connectivity |
| `/api/version` | Returns the deployed Git commit SHA |
