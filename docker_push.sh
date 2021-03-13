echo "$1" > ~/my_password.txt
cat ~/my_password.txt | docker login -u "$2" --password-stdin
rm ~/my_password.txt
docker-compose push
