# Part09 Deployment
多阶段 Dockerfile（参考 eShop/官方模板）。
```bash
docker build -t campus-part09 -f src/Part09_Deployment/Dockerfile src/Part09_Deployment
docker run --rm -p 8089:8080 campus-part09
```
