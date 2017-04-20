-- ---
-- Collation - SQL_Latin1_General_CP1251_CS_AS
-- ---

-- ---
-- Table 'Responses'
-- 
-- ---

DROP TABLE IF EXISTS Responses;
		
CREATE TABLE Responses (
  id INTEGER IDENTITY(1, 1) ,
  content NVARCHAR(1024),
  intentId INTEGER,
  PRIMARY KEY (id)
);

-- ---
-- Table 'Intents'
-- 
-- ---

DROP TABLE IF EXISTS Intents;
		
CREATE TABLE Intents (
  id INTEGER IDENTITY(1, 1),
  content NVARCHAR(512) NOT NULL,
  PRIMARY KEY (id)
);

DROP TABLE IF EXISTS Photos

CREATE TABLE Photos(
id INTEGER IDENTITY(1,1),
photoLink nvarchar(1024) not null, 
descrip nvarchar(1024),
primary key(id)
);
-- ---
-- Foreign Keys 
-- ---

ALTER TABLE Responses ADD FOREIGN KEY (intentId) REFERENCES Intents (id);

-- ---
-- Insert Data
-- ---
 INSERT INTO Intents (content) VALUES
 ('Привітаня'),
 ('Універ')
 INSERT INTO Responses (content,intentId) VALUES
 ('Доброго дня!', 1),
 ('Привіт!', 1),
 ('бурса єбана', 2)
 

 

 select * from Responses 
 select * from Intents