#!/bin/bash

set -e

tools=$(mktemp -d)
trap "rm -r '$tools' /tmp/saga-build-pwd $PWD/Build.Developement.Mono.proj $PWD/Tools/Saga.Tools.Mono.sln; touch $PWD/Binary/.gitkeep" EXIT

sed '/B71F02D6-CCF7-4CCC-A4B2-785A06791A32/d' Tools/Saga.Tools.sln >Tools/Saga.Tools.Mono.sln
sed 's:Tools\/Saga\.Tools.sln:Tools/Saga.Tools.Mono.sln:g' Build.Developement.proj >Build.Developement.Mono.proj

cat >$tools/CD <<'EOCD'
#!/bin/bash
echo "$@" >/tmp/saga-build-pwd
EOCD

cat >$tools/COPY <<'EOCD'
#!/bin/bash

cd $(</tmp/saga-build-pwd)

args=()
lastArg=
for a in "$@"
do
  if [ "x$a" != "x/Y" -a "x$a" != "x/B" ]
  then
    a=${a//.pdb/.mdb}
    args+=("$a")
    lastArg="$a"
  fi
done
args[${#args[@]}-1]="${lastArg%/*}" # strip dest file/mask from output 'Dir/*.dll'
echo -e "\e[32;1mcp ${args[@]}\e[0m"
cp -f ${args[@]}
EOCD


chmod +x $tools/*

export PATH="$tools:$PATH"
export MSBuildStartupDirectory=$PWD/Binary
xbuild Build.Developement.Mono.proj /p:TargetFrameworkVersion=v4.5
