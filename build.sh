#!/bin/sh -eux

usage(){
  printf "usage: .\build.sh [build|run|clean]"
}

build(){
  docker build -t $1 ./gherkin/
}

run(){
  build $1
  docker run -it -v /home/vagrant/src:/data/gherkin $1 $2
}

clean(){
  docker image rm -f $1
  docker image prune -f
}

IMAGE_NAME='erraticmotion/gherkin:dev'
CONTAINER_NAME='em-gherkin'

# main
if [ $# -eq 0 ]; then
  usage
else
  case "$1" in
    "build")
	    build $IMAGE_NAME
	    ;;
    "run")
      run $IMAGE_NAME $CONTAINER_NAME
      ;;
    "clean")
      clean $IMAGE_NAME
      ;;
    *)
      usage
      ;;
  esac
fi