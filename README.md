# ByteDB

## SYNTAX
Every query has to end with ';' <br/>
#### If any of the syntax is incorrect the ERROR Packet will be sent else OKAY Packet is sent

### INSERT INTO
  EX. ```INSERT INTO "TABLE_NAME";``` Empty row <br/>
  EX. ```INSERT INTO "TABLE_NAME" IF CONTAINS (COLUMN1 = VALUE1, COLUMN2 = VALUE2, COLUMN3 = VALUE3);``` Empty row if contains values <br/>
  EX. ```INSERT INTO "TABLE_NAME" (COLUMN1, COLUMN2, COLUMN3) WITH VALUES (VALUE1, VALUE2, VALUE3);``` Row with all not referenced columns empty <br/>
  EX. ```INSERT INTO "TABLE_NAME" (COLUMN1, COLUMN2, COLUMN3) WITH VALUES (VALUE1, VALUE2, VALUE3) IF CONTAINS (COLUMN1 = VALUE1, COLUMN2 = VALUE2, COLUMN3 = VALUE3);``` Row with all not referenced columns empty if contains values <br/>

