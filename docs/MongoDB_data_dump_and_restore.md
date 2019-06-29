# 

mongodump --host=localhost --db=spark --authenticationDatabase admin  --username=root --password=CosmicTopSecret --archive=r4.archive.gz --gzip


mongorestore --host=localhost --db=spark --authenticationDatabase admin --username=root --password=CosmicTopSecret --drop --archive=/data/db_dump/r4.archive.gz --gzip
