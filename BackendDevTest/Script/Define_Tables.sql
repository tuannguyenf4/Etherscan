create table blocks(
   blockID INT(20) NOT NULL AUTO_INCREMENT,
   blockNumber INT(20) NOT NULL,
   hash VARCHAR(66) NOT NULL,
   parentHash VARCHAR(66) NOT NULL,
   miner VARCHAR(42) NOT NULL,
   blockReward DECIMAL(50,0) NOT NULL,
   gasLimit DECIMAL(50,0) NOT NULL,
   gasUsed DECIMAL(50,0) NOT NULL,
   PRIMARY KEY ( blockID )
);

create table transactions(
   transactionID INT(20) NOT NULL AUTO_INCREMENT,
   blockID INT(20) NOT NULL,
   hash VARCHAR(66) NOT NULL,
   `from` VARCHAR(42) NOT NULL,
   `to` VARCHAR(42) NOT NULL,
   value  VARCHAR(42) NOT NULL,
   gas DECIMAL(50,0) NOT NULL,
   gasPrice DECIMAL(50,0) NOT NULL,
   transactionIndex INT(20) NOT NULL,
   PRIMARY KEY ( transactionID ),
   FOREIGN KEY (blockID) REFERENCES blocks(blockID)
);
