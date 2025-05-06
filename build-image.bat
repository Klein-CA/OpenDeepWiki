@echo off
echo ===========================================
echo Building KoalaWiki Docker Images
echo ===========================================

echo Building backend image...
docker build -t crpi-j9ha7sxwhatgtvj4.cn-shenzhen.personal.cr.aliyuncs.com/koala-ai/koala-wiki -f src/KoalaWiki/Dockerfile .
if %ERRORLEVEL% NEQ 0 (
    echo Error building backend image!
    exit /b %ERRORLEVEL%
)

echo Building frontend image...
cd web
docker build -t crpi-j9ha7sxwhatgtvj4.cn-shenzhen.personal.cr.aliyuncs.com/koala-ai/koala-wiki-web -f Dockerfile .
if %ERRORLEVEL% NEQ 0 (
    echo Error building frontend image!
    exit /b %ERRORLEVEL%
)

echo ===========================================
echo Images built successfully!
echo ===========================================
echo You can now push the images or run them with docker-compose up
echo ===========================================