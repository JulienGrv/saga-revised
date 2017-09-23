Preparation: Download and compile lua for mono like this:

sudo apt install lua5.1 liblua5.1-dev mono-devel
mkdir -p ~/devel
cd ~/devel
git clone https://github.com/stevedonovan/MonoLuaInterface.git
./configure
make
sudo ./install /usr/local/bin
cd MonoLuaInterface/src



To build Saga Revised under Linux you must have Mono installed. Recent Linux distros does not ship with framework 3.5, but it's possible to build Saga Revised with framework 4.5.
To to this, install mono-full package and create a symboic link 4.5 -> 3.5 like this:


sudo apt install mono-complete

sudo mv /usr/lib/mono/3.5 /usr/lib/mono/3.5~
sudo ln -s 4.5 /usr/lib/mono/3.5


Use build.sh to build Saga Revised.
Copy all the files from ~/devel/MonoLuaInterface/bin to the server Binary directory replacing existing files.

Start and configure as described in README.md 


Notes:

Saga.Tools.Tasks currently doesn't build under Mono.

