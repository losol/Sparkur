# 

mongodump --uri=mongodb://root:CosmicTopSecret@localhost:27017/spark?authSource=admin --archive=r4.archive.gz --gzip


mongorestore --uri=mongodb://root:CosmicTopSecret@localhost:27017/spark?authSource=admin --drop --archive=/data/db_dump/r4.archive.gz --gzip
