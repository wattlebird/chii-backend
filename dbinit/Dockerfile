FROM postgres:13
COPY db_init.sql /docker-entrypoint-initdb.d

EXPOSE 5432
CMD ["postgres"]