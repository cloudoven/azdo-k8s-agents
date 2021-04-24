
docker build -t moimhossain/octolamp-agent-controller:v1.0.0-beta -f ./AgentController/Dockerfile ./AgentController/

kubectl -n octolamp-system create secret docker-registry dockerhub --docker-server=https://index.docker.io/v1/ --docker-username=moimhossain --docker-password=$DockerPassword --docker-email=moimhossain@gmail.com