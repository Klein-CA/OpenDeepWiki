#!/bin/bash

echo "==========================================="
echo "Building KoalaWiki Docker Images"
echo "==========================================="

echo "Building backend image..."
docker build -t crpi-j9ha7sxwhatgtvj4.cn-shenzhen.personal.cr.aliyuncs.com/koala-ai/koala-wiki -f src/KoalaWiki/Dockerfile .
if [ $? -ne 0 ]; then
    echo "Error building backend image!"
    exit 1
fi

echo "Building frontend image..."
pushd web > /dev/null
docker build -t crpi-j9ha7sxwhatgtvj4.cn-shenzhen.personal.cr.aliyuncs.com/koala-ai/koala-wiki-web -f Dockerfile .
if [ $? -ne 0 ]; then
    echo "Error building frontend image!"
    exit 1
fi
popd > /dev/null

echo "==========================================="
echo "Images built successfully!"
echo "==========================================="
echo "You can now push the images or run them with docker-compose up"
echo "==========================================="