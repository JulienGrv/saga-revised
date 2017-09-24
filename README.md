# Saga Revised

> Saga Revised is a server emulator for Ragnarok Online 2: The Gate of the World.

Saga Revised is the new and improved Saga.
The new system is less hardcoded based and more on input data.

The data that we provide is official data that we mainly extract throughout a gigantic database of packetlogs.
The numbers of players on the official servers have been decreased and so are our packetlogs so the data isn't entirely trustworthy to be compatible with latest calculations.

## Table of Contents

1. [Getting Started](#getting-started)
    1. [Sources Compilation](#sources-compilation)
    1. [Setting up the SQL database](#setting-up-the-sql-database)
    1. [Setting up the emulator](#setting-up-the-emulator)
1. [Frequently Asked Questions](#faq)
1. [Known Issues](#known-issues)
1. [About](#about)

## Getting Started

### Sources Compilation

#### Windows Instructions

You need to have [.NET Framework 3.5](http://www.microsoft.com/en-us/download/details.aspx?id=21) installed in order to be able to build the emulator.
Then simply run the script `build.bat` in the project root directory and wait until everything is done.

#### Linux Instructions

To build Saga Revised under Linux you must have [Mono](http://www.mono-project.com/) installed.
Recent Linux distributions do not ship with .NET framework 3.5, but it's possible to build Saga Revised with .NET framework 4.5.

To do this, install `mono-complete` package and create a symbolic link 4.5 -> 3.5 like this:

```bash
sudo apt install mono-complete
sudo mv /usr/lib/mono/3.5 /usr/lib/mono/3.5~
sudo ln -s 4.5 /usr/lib/mono/3.5
```

Then, run `build.sh` to build Saga Revised.

Now, download and compile lua for Mono like this:

```bash
sudo apt install lua5.1 liblua5.1-dev mono-devel
mkdir -p ~/devel
cd ~/devel
git clone https://github.com/stevedonovan/MonoLuaInterface.git
cd MonoLuaInterface/src
./configure
make
sudo ./install /usr/local/bin
```

Copy all the files from `~/devel/MonoLuaInterface/bin` to the server Binary directory replacing existing files.

> Saga.Tools.Tasks currently doesn't build under Mono.

### Setting up the SQL database

#### Installation

Download [MySQL Installer](http://dev.mysql.com/downloads/windows/installer/) and install at least MySQL Server and MySQL Workbench. Then start MySQL Workbench and check in `startup / shutdown` if the MySQL Server is running.

> MySQL <5.1 is not supported.

#### Database Creation

In MySQL Workbench, create a new schema (database) called "saga", open it and run all SQL scripts in the directory `Database\Mysql` of the emulator. This will create every tables in the schema.

### Setting up the emulator

#### Saga.Map

Run it. You will be asked to create a world server, choose ID 1, give a max player number and a passcode. Set the rates, type 1 to load every plugins, finally say no 2 times and then yes for the last question. Enter your database settings like in this example:

```bash
database name: saga
database username: root
database password: <root_password>
database port: 3306
database host: localhost
```

Close it.

#### Saga.Authentication

Run it and type 1 to load the plugin. Say no to the 2 next questions and yes for the last one. Use the same database settings as above:

```bash
database name: saga
database username: root
database password: <root_password>
database port: 3306
database host: localhost
```

Close it.

#### Saga.Gateway

Run it and say no to everything. Close it.

#### Adding the world server in the database

Open `Saga.Map.config`, go to the line which looks like `<Saga.Manager.NetworkSettings world="1" playerlimit="50" proof="2C6CFC5F906506F46HHKIU679Y6J8Y7K">` and copy the proof somewhere. Open MySQL Workbench, open a new query tab and execute the following command:

```sql
INSERT INTO `list_worlds` (`Id`, `Name`, `Proof`)
VALUES (1, '<world_name>', '<proof>');
```

replacing `<world_name>` with whatever you want and `<proof>` with the same proof found in `Saga.Map.config`.
Now you're done, you can launch the server.

## FAQ

### I can't build the emulator under Windows

Sometimes the `build.bat` script doesn't work *out-of-the-box*. Check that the path to `MSBuild.exe` is correct by editing the script and/or try to run the script with administrator privileges.

### Where can I get a client of the game?

You can download the latest [DiviniaRO2 full client](https://mega.co.nz/#!yZhlkB5S!j6zia8kE_uLZ65WaJavDS-nVvq7-vyDgtGfRIbcmm9E).

### How do I create a player account?

Type `account -create <username> <password> male` in the `Saga.Authentication` console to create a male account by replacing `<username>` and `<password>` with whatever you want (use female to create a female account).

### How do I connect to the server with my game client?

You need to run the game executable with some options. For example, you can use a batch script with the command: `"System\RagII.exe" ServerIP=127.0.0.1 ServerPort=64000`.

### I can't login to my player account

If you can't login to your player account, restart the server and before trying to login to your player account, type `host -connect` in the `Saga.Gateway` console. You don't have to do that each time you want to login to a player account, just once after launching the server.

## Known Issues

No lua command to add a new weapon to your inventory. We do have all the packets and know how to use them however.

There is a targeting problem when casting emoticons on other people. We have to debug where this goes wrong.
To debug the server, run the server from the binary folder, in Visual Studio -> Tools -> Attach to process -> Saga.Map.Exe.
Set a break point on the function where it does the offensive attack, and use the intermediate window to check which expression
is evaluated as false.

AI is still a bit dumb. We need to figure out a way to read out all the height information for closed rooms with multiple height planes in order to fix this.
This will take a long amount of time so bear with the dumb ai for now.

If the computer is extremely stressed loading the lua quest files ad-hoc can cause problems.
Lua reads the files ASCII encoding so if you save it with unicode encoding this causes issues also.
A fix could be to preload all quest files and add a reload command.

Mysql <5.1 is not supported. It will never be supported either. However both Mysql 6.x and 5.1 are supported.

The server has never been tested under a stressful environment and Mono. Be aware to expect bugs in that corner or even uncompatibility.

## About

### Authors

* phr34k, Zenzija, and all original SagaRevised contributors
* [kalel60](https://www.assembla.com/profile/kalel60), [Sebda](https://www.assembla.com/profile/Sebda), and all [SIN](https://app.assembla.com/spaces/stilleinnorden) contributors
* **Darkin** - *Developer* - [JulienGrv](https://github.com/JulienGrv)
* [All Contributors](https://github.com/JulienGrv/saga-revised/contributors)

### License

This project is licensed under the Creative Commons Public License (CC BY-NC-SA 4.0) - see the [LICENSE](https://github.com/JulienGrv/saga-revised/blob/master/LICENSE) for details.

**[Back to top](#table-of-contents)**
