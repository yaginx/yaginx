# yaginx

Replacement for nginx's HTTP reverse proxy, but more power feature than nginx.

## Features(功能)

* 平替Nginx Http & Https的反向代理
* 反向代理规则UI可视化管理, 降低管理复杂度
* Docker容器管理, 简化Docker的操作管理
* 证书管理, 自动申请及续期证书
* 多节点负载均衡支持
* 可视化监控

## 转发的问题
目前是使用Docker部署的, 使用的是docker的nat网络, 最终收到的ip都是docker的ip

## deploy

```bash
mkdir -p /data/pgdb_data
docker run -d --name pgdb -p 5432:5432 \
    -e POSTGRES_PASSWORD=123456\
    -e TZ=Asia/Shanghai \
    -e PGDATA=/var/lib/postgresql/data/pgdata \
    -v /data/pgdb_data:/var/lib/postgresql/data \
    hub.feinian.net/library/postgres:16.0
    
mkdir -p /data/redis_data
docker run -d --name redis -p 127.0.0.1:6379:6379 -e TZ=Asia/Shanghai \
    -v /data/redis_data:/data \
    redis:7.2.3 redis-server --save 60 1 --loglevel warning 

mkdir -p /data/yaginx
docker pull cnhub.feinian.net/yaginx/yaginx:latest
docker rm yaginx -f || true
docker run -d -it --name yaginx -p 8080:8080 -p 8443:8443 \
-v /data/yaginx:/app_data -v /run/docker.sock:/var/run/docker.sock --link redis:redis hub.feinian.net/yaginx/yaginx:latest
```

## ReverseProxy Config

### Routes

```json

```

## 数据库创建
```bash
create role yaginx with login encrypted password '123456' connection limit -1;
create database yaginx with owner yaginx encoding='UTF8';
alter database yaginx set timezone to 'Asia/Shanghai';
\c yaginx
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "ltree";
CREATE EXTENSION IF NOT EXISTS "pg_stat_statements"; 
```
