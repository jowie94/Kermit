# The Kermit programming language
![Kermit](doc/img/kermit_keyboard.gif)
## What is Kermit?
Kermit is an interpreted, object oriented, dynamically typed, statically typed, generic and extensible language. It is being developed by me at HP Inc. as my bachelor's thesis for the FIB.

The main objective of the language is to assist developers in debugging and mocking tasks by executing instructions inside the target application.

It consists in a parser and an interpreter, one DLL for each component. The language is based in C (and derived), JavaScript and Python. It inherits constructions from these languages.

## Roadmap

The roadmap is divided in tasks which are listed in the issues sections with the `TODO` tag. The milestones are:
- [x] Implement a basic interpreter
- [x] Support external (_native_) functions
- [x] Support external (_native_) objects
- [x] Correct code and solve all remaining TODOs

## How to build Kermit
### Prerequisites
- Git
- Visual Studio 2015 (any edition will work)
- C# support installed
- NuGet

### Instructions
First download the repo using your favorite program or CMD:
```bash
cd %SOMEDEVDIR%
git clone https://github.com/jowie94/Kermit
```
Go to the Kermit directory and open in VS 2015 the file `src\Kermit\Kermit.sln`.

Be sure that the folder `tools\Antlr3` exists and that contains all the Antlr files. They are needed to compile the project.

Once everything is checked, you can build the project by tapping CTRL+B or F5 to launch it. Once everything is compiled, you will be presented with a terminal where you can test the language.

## Code examples
### Function calling
```javascript
SomeFunction(param1, "string")
```
### While loop
```javascript
i = 0
while (i < 10)
{
	Write(i+2)
	i+=1
}
```
### Do.. while loop
### For Loop
```javascript
for (i = 0; i < 10; i += 1)
{
	Write(i+2)
}
```
### Function declaration
```javascript
function asdfg(param1, param2) {
	Write(param1)
	return param1 + param2
}
```

## How-to implement native functions
// TODO
