# Saga Revised

Saga Revised is an emulator for Ragnarok Online 2 : The Gate of the World.

## How to compile
You need to install [.NET Framework 3.5](http://www.microsoft.com/en-us/download/details.aspx?id=21) in order to be able to build the emulator. Then launch `build.bat` and it's done.

## Setting up MySQL

### Installation
Download [MySQL Installer](http://dev.mysql.com/downloads/windows/installer/) and install at least MySQL Server and MySQL Workbench. Then start MySQL Workbench and check in `startup / shutdown` if the MySQL Server is running.

### Create the database
In MySQL Workbench, create a new schema called "saga", open it and run all SQL scripts in the directory `Database\Mysql` of the emulator. This will added each of them as a table in your schema.

## Setting up the emulator

### Saga.Map
Launch it. You are asked to create a world server, choose ID 1, give a max player number and a code. Set the rates, type 1 to load every plugins, finally say no 2 times and then yes for the last one. Type this database setting for example :
```
database name: saga
database username: root
database password: root_password
database port: 3306
database host: localhost
```
Close it.

### Saga.Authentication
Launch it and type 1 to load the plugin. Say no to the 2 next questions and yes for the last one. Use the same settings as above :
```
database name: saga
database username: root
database password: root_password
database port: 3306
database host: localhost
```
Close it.

### Saga.Gateway
Launch it and say no to everything. Close it.

### Adding the world server
Open `Saga.Map.config`, go to the line which looks like `<Saga.Manager.NetworkSettings world="1" playerlimit="50" proof="2C6CFC5F906506F46HHKIU679Y6J8Y7K">` and copy the proof somewhere. Open MySQL Workbench, open a new query tab and execute the command :
```
INSERT INTO `list_worlds` (`Id`,`Name`,`Proof`) VALUES (1,'world_name',proof);
```
replacing `world_name` with whatever you want and `proof` with the same proof found in `Saga.Map.config`.
Now you're done, you can launch the server.

## Play
* Type `account -create username password male` in `Saga.Authentication` to create an account.
* Launch the game with a batch file using the command : `"System\RagII.exe" ServerIP=127.0.0.1 ServerPort=64000`

## Some tips
* Some time the `build.bat` doesn't work, check the path to `MSBuild.exe` and/or try with administrator privileges.
* If you are stuck at the login screen for no reason, restart the server and before trying to connect into the game type `host -connect` in the Saga.Gateway console. You don't have to do that each time you want to connect, just one time after launching the server.

## Full Client
We use the latest [DiviniaRO2 full client](https://mega.co.nz/#!yZhlkB5S!j6zia8kE_uLZ65WaJavDS-nVvq7-vyDgtGfRIbcmm9E).

## Credits
* [Julien Grave](https://github.com/darkin47)
* [kalel60](https://www.assembla.com/profile/kalel60) & [sebda](https://www.assembla.com/profile/Sebda) (SIN dev.)
* Zenzija & phr34k (SagaRO2 dev.)
* [All Contributors](https://github.com/Darkin47/SagaRevised/graphs/contributors)

## License
The Creative Common License (CC BY-NC-SA 3.0). Please see [License File](https://github.com/Darkin47/SagaRevised/blob/master/LICENSE) for more information.
