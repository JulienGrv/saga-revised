# Saga Revised

Saga Revised is an emulator for Ragnarok Online 2: The Gate of the World.

> Only Windows platforms are supported but you can also run the emulator under Unix using Mono.

## Table of Contents

1. [Getting Started](#getting-started)
1. [Frequently Asked Questions](#faq)
1. [About](#about)

## Getting Started

### Building Sources

You need to have [.NET Framework 3.5](http://www.microsoft.com/en-us/download/details.aspx?id=21) installed in order to be able to build the emulator. Then simply run the script `build.bat` in the project root directory and wait until everything is done.

### Setting up the SQL database

#### Installation

Download [MySQL Installer](http://dev.mysql.com/downloads/windows/installer/) and install at least MySQL Server and MySQL Workbench. Then start MySQL Workbench and check in `startup / shutdown` if the MySQL Server is running.

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

### I can't build the emulator

Sometimes the `build.bat` script doesn't work *out-of-the-box*. Check that the path to `MSBuild.exe` is correct by editing the script and/or try to run the script with administrator privileges.

### Where can I get a client of the game?

You can download the latest [DiviniaRO2 full client](https://mega.co.nz/#!yZhlkB5S!j6zia8kE_uLZ65WaJavDS-nVvq7-vyDgtGfRIbcmm9E).

### How do I create a player account?

Type `account -create <username> <password> male` in the `Saga.Authentication` console to create a male account by replacing `<username>` and `<password>` with whatever you want (use female to create a female account).

### How do I connect to the server with my game client?

You need to run the game executable with some options. For example, you can use a batch script with the command: `"System\RagII.exe" ServerIP=127.0.0.1 ServerPort=64000`.

### I can't login to my player account

If you can't login to your player account, restart the server and before trying to login to your player account, type `host -connect` in the `Saga.Gateway` console. You don't have to do that each time you want to login to a player account, just once after launching the server.

## About

### Authors

* phr34k, Zenzija, and all original SagaRevised contributors
* [kalel60](https://www.assembla.com/profile/kalel60), [Sebda](https://www.assembla.com/profile/Sebda), and all [SIN](https://app.assembla.com/spaces/stilleinnorden) contributors
* **Darkin** - *Developer* - [JulienGrv](https://github.com/JulienGrv)
* [All Contributors](https://github.com/JulienGrv/saga-revised/contributors)

### License

This project is licensed under the Creative Commons Public License (CC BY-NC-SA 4.0) - see the [LICENSE](https://github.com/JulienGrv/saga-revised/blob/master/LICENSE) for details.

**[Back to top](#table-of-contents)**
