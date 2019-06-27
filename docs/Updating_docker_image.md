# Update docker image

docker build -t sparkur .
docker tag sparkur losolio/sparkur
docker login
docker push losolio/sparkur