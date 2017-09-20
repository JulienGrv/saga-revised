To build Saga Revised under Linux you must have Mono installed. Recent Linux distros does not ship with framework 3.5, but it's possible to build Saga Revised with framework 4.5.
To to this, install mono-full package and create a symboic link 4.5 -> 3.5 like this:


sudo apt install mono-full

sudo mv /usr/lib/mono/3.5 /usr/lib/mono/3.5~
ln -s 4.5 /usr/lib/mono/3.5


Next use build.sh to build Saga Revised.
After build binaries will be placed into Binary directory.

Notes:

Saga.Tools.Tasks currently doesn't build under Mono.