# A C# solution to the Sales Tax Problem

## Overview
The project is written in C# 7.0 and it was created in a Windows 10 operating system using Visual Studio 2017 RC. 
The solution provided conisists in three projects:

1. **Salestax**: a simple Console App (*.NET Core*) that prints out results after processing hardcoded input
2. **Salestax.Core**: a Class Library (*.NET Standard*) with the core logic
3. **Salestax.Tests**: a xUnit Test Project (*.NET Core*) that verifies the accuracy of core project logics

The Console App is able to parse input strings (using a Regex) in the following format:
> \<quantity\> imported? \<description\> at \<price\>

- \<quantity\> must be an integer
- "imported" is an optional string
- \<description\> is the description of the product
- \<price\> must be a decimal

The result is the receipt with computed amount of each product followed by the sales taxes and the total amount:
```
1 book: 12.49
1 music CD: 16.49
1 chocolate bar: 0.85
Sales Taxes: 1.50
Total: 29.83
```

## Dev box Setup
The solution was written using `Visual Studio 2017 RC`. 
- You have to download the `1.0.0-preview4-004175 .NET Core SDK Installer` from the [.NET Core command-line (CLI) tools site](https://github.com/dotnet/cli)
- You have to download the `Community` edition of [Visual Studio](https://www.visualstudio.com/vs/visual-studio-2017-rc/) in order to modify and run the code.
    - Please ensure you check the `.NET Core and Docker Preview` when you install the product

### 1) Compile the code
#### Clean & Build from Visual Studio
- open Visual Studio
- from the top menu click on `File -> Open -> Project/solution`
- select the `SalesTax.sln` file
- from the top menu click on `Build -> Rebuild Solution` (or simply use the keyboard shortcut `F6`)

#### Clean & Build from Command Prompt
If you love the "*Command Prompt*" you can compile the entire solution from command line:
- open a terminal in solution folder
- "*dotnet **restore***"
- "*dotnet **build***"

### 2) Run the code
#### Run from Visual Studio
- from the top menu click on `Debug -> Start Debugging` (or simply use the keyboard shortcut `F5`)

#### Run from Command Prompt
- open a terminal in **Salestax** project folder (where `Program.cs` file is located)
- "*dotnet **run***"

### 3) Test the code
Every test inside the project **Salestax.Tests** has comments to describe what's going on.
* `// Arrange` -> setup test environment (input, variables, ...)
* `// Act` -> execute the operation you want to test
* `// Assert` -> do all the checks to establish the test result.

#### Test from Visual Studio
- from the top menu click on `Test -> Windows -> Test Explorer`
- run all tests from the Test Explorer Tab

#### Test from Command Prompt
- open a terminal in **Salestax.Tests** project folder (where all test classes are located)
- "*dotnet **test***"