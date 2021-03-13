echo "${{ secrets.DOCKER_ACCESS_TOKEN }}" > ~/my_password.txt
cat ~/my_password.txt | docker login -u " ${{ secrets.DOCKER_ID }} " --password-stdin
rm ~/my_password.txt
docker-compose push
