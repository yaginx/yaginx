# yaginx

Replacement for nginx's HTTP reverse proxy, but more power feature than nginx.

## Features(功能)

* 平替Nginx Http & Https的反向代理
* 反向代理规则UI可视化管理, 降低管理复杂度
* Docker容器管理, 简化Docker的操作管理
* 证书管理, 自动申请及续期证书
* 多节点负载均衡支持
* 可视化监控

## deploy

```bash
mkdir -p /data/yaginx
docker pull cnhub.feinian.net/yaginx/yaginx:latest
docker rm yaginx -f || true
docker run -d -it --name yaginx -p 80:8080 -p 443:8443 \
-v /data/yaginx:/app_data -v /run/docker.sock:/var/run/docker.sock --link redis:redis cnhub.feinian.net/yaginx/yaginx:latest
```

## ReverseProxy Config

### Routes

```json

```
