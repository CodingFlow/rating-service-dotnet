FROM redis:8.4
# COPY redis.conf /usr/local/etc/redis/redis.conf
CMD [ "redis-server" ]