#!/bin/bash

if command -v cursor; then
  COMMAND=cursor
elif command -v code-server; then
  COMMND=code-server
fi
echo "COMMAND=$COMMAND"

BASEDIR=$(dirname "$0")
EXTENSIONS_DIR=$BASEDIR/extensions
echo EXTENSIONS_DIR=$EXTENSIONS_DIR
if [[ "$COMMAND" ]] then
  echo "Install extensions"
  for extension in $EXTENSIONS_DIR/* ; do
    $COMMAND --install-extension $extension
  done
fi
